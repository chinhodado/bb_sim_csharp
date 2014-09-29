using System;
using System.Collections.Generic;

namespace bb_sim {
    class Card {

        const double NEW_DEBUFF_LOW_LIMIT_FACTOR = 0.4;

        public readonly string name;
        string fullName;
        int dbId; // the id in game (used in famDatabase)
        public readonly int id;   // id for this simulator, not the id in game
        public readonly bool isMounted;
        bool isWarlord;
        string imageLink;

        readonly Stats stats;
        public readonly Stats originalStats;
        public Status status;
        Affliction affliction;
        public bool isDead;
        public double bcAddedProb; // added probability for bloodclash

        public double lastBattleDamageTaken;
        public double lastBattleDamageDealt;
        public bool justMissed;
        public bool justEvaded;

        public readonly Player player;
        public readonly int formationColumn; // 0 to 4
        readonly FormationRow formationRow; // 1, 2 or 3
        public readonly int procIndex;

        List<Skill> skills;
        public readonly Skill autoAttack;

        private readonly List<Skill> openingSkills = new List<Skill>();
        private readonly List<Skill> activeSkills = new List<Skill>();
        private readonly List<Skill> protectSkills = new List<Skill>();
        readonly List<Skill> defenseSkills = new List<Skill>(); // does not contain survive skills
        readonly List<Skill> ondeathSkills = new List<Skill>(); // first is buff, second is inherent
        private readonly Skill surviveSkill;
    
        public Card(int dbId, Player player, int nth, List<Skill> skills) {
            var cardData = FamiliarDatabase.famDatabase[dbId];
            name = cardData.name;
            fullName = cardData.fullName;
            this.dbId = dbId;
            id = player.id * 100 + nth; // 100-109, 200-209
            isMounted = cardData.isMounted;
            isWarlord = cardData.isWarlord;
            imageLink = cardData.img;

            // the HP will be modified during the battle
            stats = new Stats(cardData.stats[0], cardData.stats[1], cardData.stats[2], cardData.stats[3], cardData.stats[4]);
        
            // this should never be modified
            originalStats = new Stats(cardData.stats[0], cardData.stats[1], cardData.stats[2], cardData.stats[3], cardData.stats[4]);

            status = new Status();
            isDead = false;

            this.player = player; // 1: me, 2: opponent
            formationColumn = nth % 5;
            formationRow = player.formation.getCardRow(formationColumn);
            procIndex = Formation.getProcIndex(formationRow, formationColumn, BattleModel.getInstance().procOrderType);

            this.skills = skills;

            autoAttack = cardData.autoAttack != 0 ? new Skill(cardData.autoAttack) : new Skill(10000);

            foreach (var skill in skills) {
                switch (skill.skillType) {
                    case SkillType.OPENING:
                        openingSkills.Add(skill);
                        break;
                    case SkillType.ACTIVE:
                        activeSkills.Add(skill);
                        break;
                    case SkillType.EVADE:
                    case SkillType.PROTECT:
                        protectSkills.Add(skill);
                        break;
                    case SkillType.DEFENSE:
                        if (skill.skillFunc == SkillFunc.SURVIVE) {
                            surviveSkill = skill;
                        }
                        else {
                            defenseSkills.Add(skill);
                        }
                        break;
                    case SkillType.ACTION_ON_DEATH:
                        ondeathSkills[1] = skill;
                        break;
                }
            }
        }
    
        public Skill getRandomOpeningSkill() {
            return openingSkills.Count == 0 ? null : BBRandom.getRandomListItem(openingSkills);
        }

        public Skill getRandomActiveSkill() {
            return activeSkills.Count == 0 ? null : BBRandom.getRandomListItem(activeSkills);
        }

        /**
         * Note that survive skills will not be returned here
         */
        public Skill getRandomDefenseSkill() {
            return defenseSkills.Count == 0 ? null : BBRandom.getRandomListItem(defenseSkills);
        }

        public Skill getRandomProtectSkill() {
            return protectSkills.Count == 0 ? null : BBRandom.getRandomListItem(protectSkills);
        }

        public Skill getSurviveSkill() {
            return surviveSkill;
        }
    
        public Skill getFirstActiveSkill() {
            return activeSkills[0];
        }
    
        public Skill getSecondActiveSkill() {
            return activeSkills[1];
        }

        // ondeath skills
        public Skill getBuffOnDeathSkill() {
            return ondeathSkills[0];
        }
        public Skill getInherentOnDeathSkill() {
            return ondeathSkills[1];
        }
        public bool hasOnDeathSkill() {
           return (ondeathSkills[0] != null) || (ondeathSkills[1] != null);
        }

        public void clearBuffOnDeathSkill() {
            ondeathSkills[0] = null;
            status.actionOnDeath = 0;
        }

        public int getPlayerId() {
            return player.id;
        }
    
        public FormationRow getFormationRow() {
            return formationRow;
        }

        public double getStat(SkillCalcType statType) {
            switch (statType) {
                case SkillCalcType.ATK:
                    return getATK();
                case SkillCalcType.DEFAULT:
                case SkillCalcType.WIS:
                    return getWIS();
                case SkillCalcType.AGI:
                    return getAGI();
                default:
                    throw new Exception("Invalid stat type");
            }
        }

        // affliction
        public void setAffliction(AfflictionType type, AfflectOptParam option) {

            if (affliction != null) {
                if (affliction.getType() == type) {
                    affliction.add(option);
                    return;
                }
                else {
                    clearAffliction();
                }
            }
            affliction = AfflictionFactory.getAffliction(type);
            affliction.add(option);
        }
        private void clearAffliction() {
            if (affliction == null) {
                return;
            }
            affliction.clear();
            affliction = null;
        }
        public bool canAttack() {
            return (affliction == null) || affliction.canAttack();
        }
        public bool canUseSkill() {
            return (affliction == null) || affliction.canUseSkill();
        }
        public bool willMiss() {
            return (affliction != null) && affliction.canMiss();
        }

        // return true if an affliction was cleared
        public void updateAffliction() {
            if (affliction == null) {
                return;
            }

            affliction.update(this);
        
            if (affliction != null && affliction.isFinished()) {
                clearAffliction();
            }
        }
    
        public void changeStatus(StatusType statusType, double amount, bool isNewLogic = false, double maxAmount = 0) {
            if (isNewLogic) {
                status.isNewLogic[statusType] = true;
            }

            if (statusType == StatusType.ATK) {
                status.atk += amount;
            }
            else if (statusType == StatusType.DEF) {
                status.def += amount;
            }
            else if (statusType == StatusType.WIS) {
                status.wis += amount;
            }
            else if (statusType == StatusType.AGI) {
                status.agi += amount;
            }
            else if (statusType == StatusType.ATTACK_RESISTANCE) {
                if (status.attackResistance < amount) {
                    status.attackResistance = amount; // do not stack
                }
            }
            else if (statusType == StatusType.MAGIC_RESISTANCE) {
                if (status.magicResistance < amount) {
                    status.magicResistance = amount; // do not stack
                }
            }
            else if (statusType == StatusType.BREATH_RESISTANCE) {
                if (status.breathResistance < amount) {
                    status.breathResistance = amount; // do not stack
                }
            }
            else if (statusType == StatusType.SKILL_PROBABILITY) {
                status.skillProbability += amount;
            }
            else if (statusType == StatusType.WILL_ATTACK_AGAIN) {
                status.willAttackAgain = (int) amount;
            }
            else if (statusType == StatusType.ACTION_ON_DEATH) {
                var skill = new Skill((int) amount);
                ondeathSkills[0] = skill;
                status.actionOnDeath = (int) amount;
            }
            else if (statusType == StatusType.HP_SHIELD) {
                status.hpShield += amount;
                if (maxAmount != 0 && status.hpShield > maxAmount) {
                    status.hpShield = maxAmount;
                }
            }
            else {
                throw new Exception("Invalid status type");
            }
        }
    
        /**
         * Clear all statuses of this card that satisfy the supplied conditional function
         */
        public void clearAllStatus(Func<double, bool> condFunc) {
            if (condFunc(status.atk)) status.atk = 0;
            if (condFunc(status.def)) status.def = 0;
            if (condFunc(status.wis)) status.wis = 0;
            if (condFunc(status.agi)) status.agi = 0;
            if (condFunc(status.attackResistance)) status.attackResistance = 0;
            if (condFunc(status.magicResistance)) status.magicResistance = 0;
            if (condFunc(status.breathResistance)) status.breathResistance = 0;
            if (condFunc(status.skillProbability)) status.skillProbability = 0; 
            if (condFunc(status.actionOnDeath)) status.actionOnDeath = 0;
            if (condFunc(status.hpShield)) status.hpShield = 0;
            if (condFunc(status.willAttackAgain)) status.willAttackAgain = 0;

            if (status.actionOnDeath == 0) {
                clearBuffOnDeathSkill();
            }
        }

        /**
         * Return true if this card has a status that satisfies the supplied conditional function
         */
        public bool hasStatus(Func<double, bool> condFunc) {
            bool hasStatus = condFunc(status.atk) ||
                             condFunc(status.def) ||
                             condFunc(status.wis) ||
                             condFunc(status.agi) ||
                             condFunc(status.attackResistance) ||
                             condFunc(status.magicResistance) ||
                             condFunc(status.breathResistance) ||
                             condFunc(status.skillProbability) ||
                             condFunc(status.actionOnDeath) ||
                             condFunc(status.hpShield) ||
                             condFunc(status.willAttackAgain);

            return hasStatus;
        }

        public double getHP() {
            return stats.hp;
        }
        public double getOriginalHP() {
            return originalStats.hp;
        }
        public void changeHP (double amount) {
            stats.hp += amount;

            if (stats.hp > originalStats.hp) {
                stats.hp = originalStats.hp;
            }

            if (stats.hp <= 0) {
                stats.hp = 0;
                setDead();
            }
        }
        public bool isFullHealth() {
            return stats.hp == originalStats.hp;
        }
        public double getHPRatio() {
            return stats.hp / originalStats.hp;
        }

        private void setDead() {
            isDead = true;
            clearAffliction();
            status = new Status();
        }
        public void revive(double hpRatio) {
            if (!isDead) {
                throw new Exception("You can't revive a card that is not dead!");
            }

            isDead = false;
            status = new Status();
            stats.hp = originalStats.hp * hpRatio;
        }

        public double getATK() {
            var value = stats.atk + status.atk;

            if (value < 0) {
                value = 0;
            }

            value = adjustByNewDebuffLogic(StatusType.ATK, value, originalStats.atk);

            return value;
        }
        public double getDEF() {
            var value = stats.def + status.def;

            if (value < 0) {
                value = 0;
            }

            value = adjustByNewDebuffLogic(StatusType.DEF, value, originalStats.def);

            return value;
        }
        public double getWIS() {
            var value = stats.wis + status.wis;

            if (value < 0) {
                value = 0;
            }

            value = adjustByNewDebuffLogic(StatusType.WIS, value, originalStats.wis);

            return value;
        }
        public double getAGI() {
            var value = stats.agi + status.agi;

            if (value < 0) {
                value = 0;
            }

            value = adjustByNewDebuffLogic(StatusType.AGI, value, originalStats.agi);

            return value;
        }

        private double adjustByNewDebuffLogic(StatusType type, double value, double originalValue) {
            if (status.isNewLogic.ContainsKey(type) && status.isNewLogic[type]) {
                var lowerLimit = originalValue * NEW_DEBUFF_LOW_LIMIT_FACTOR;
                value = (value > lowerLimit) ? value : lowerLimit;
            }
            return value;
        }

        public void clearDamagePhaseData() {
            lastBattleDamageDealt = 0;
            lastBattleDamageTaken = 0;
            justMissed = false;
            justEvaded = false;
        }
    }

    class Stats {
        public double hp;
        public readonly int atk;
        public readonly int def;
        public readonly int wis;
        public readonly int agi;

        public Stats(double hp, int atk, int def, int wis, int agi) {
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.wis = wis;
            this.agi = agi;
        }
    }

    class Status {
        // the amount changed because of buffs or debuffs
        public double atk;
        public double def;
        public double wis;
        public double agi;

        public double attackResistance;
        public double magicResistance;
        public double breathResistance;

        public double skillProbability;

        public int actionOnDeath;
        public double hpShield;

        public int willAttackAgain;

        public readonly Dictionary<StatusType, bool> isNewLogic = new Dictionary<StatusType,bool>();
    }
}
