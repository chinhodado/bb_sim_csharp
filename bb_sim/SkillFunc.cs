using System;
using System.Collections.Generic;
using System.Linq;

namespace bb_sim {
    static class SkillLogicFactory {
        public static SkillLogic getSkillLogic(SkillFunc skillFunc) {
            switch (skillFunc) {
                case SkillFunc.BUFF:
                    return new BuffSkillLogic();
                case SkillFunc.DEBUFF:
                case SkillFunc.CASTER_BASED_DEBUFF:
                    return new DebuffSkillLogic();
                case SkillFunc.ONHIT_DEBUFF:
                    return new OnHitDebuffSkillLogic();
                case SkillFunc.DISPELL:
                    return new DispellSkillLogic();
                case SkillFunc.AFFLICTION:
                    return new AfflictionSkillLogic();
                case SkillFunc.ATTACK:
                case SkillFunc.MAGIC:
                case SkillFunc.DEBUFFATTACK:
                case SkillFunc.DEBUFFINDIRECT:
                case SkillFunc.DRAIN_ATTACK:
                case SkillFunc.DRAIN_MAGIC:
                case SkillFunc.CASTER_BASED_DEBUFF_ATTACK:
                case SkillFunc.CASTER_BASED_DEBUFF_MAGIC:
                case SkillFunc.KILL:
                    return new AttackSkillLogic();
                case SkillFunc.PROTECT:
                    return new ProtectSkillLogic();
                case SkillFunc.EVADE:
                    return new EvadeSkillLogic();
                case SkillFunc.PROTECT_COUNTER:
                    return new ProtectCounterSkillLogic();
                case SkillFunc.COUNTER:
                    return new CounterSkillLogic();
                case SkillFunc.COUNTER_DISPELL:
                    return new CounterDispellSkillLogic();
                case SkillFunc.CLEAR_DEBUFF:
                    return new ClearDebuffSkillLogic();
                case SkillFunc.DRAIN:
                    return new DrainSkillLogic();
                case SkillFunc.SURVIVE:
                    return new SurviveSkillLogic();
                case SkillFunc.HEAL:
                    return new HealSkillLogic();
                case SkillFunc.REVIVE:
                    return new ReviveSkillLogic();
                case SkillFunc.TURN_ORDER_CHANGE:
                    return new TurnOrderChangeSkillLogic();
                case SkillFunc.RANDOM:
                    return new RandomSkillLogic();
                default:
                    throw new Exception("Invalid skillFunc or not implemented");
            }
        }
    } 

    abstract class SkillLogic {
        protected readonly BattleModel battleModel;
        protected readonly CardManager cardManager;

        protected SkillLogic() {
            battleModel = BattleModel.getInstance();
            cardManager = CardManager.getInstance();
        }

        public virtual bool willBeExecuted(SkillLogicData data) {
            var deadCond = (data.skill.skillType == SkillType.ACTION_ON_DEATH) ||
                           (!data.executor.isDead && data.skill.skillType != SkillType.ACTION_ON_DEATH);

            bool probCond = true;

            if (data.noProbCheck) {
            }
            else {
                probCond = (BBRandom.get() * 100) <= (data.skill.maxProbability + data.executor.status.skillProbability * 100 + 
                    data.executor.bcAddedProb);
            }

            return (deadCond && 
                data.executor.canAttack() && // if cannot attack -> cannot use skill, so the same. If can attack, true, doesn't matter
                data.executor.canUseSkill() && 
                probCond);
        }

        public virtual void execute(SkillLogicData data) {
            throw new Exception("Implement this");
        }

        protected void clearAllCardsDamagePhaseData() {
            var allCards = cardManager.getAllCurrentMainCards();
            foreach (Card c in allCards) {
                c.clearDamagePhaseData();
            }
        }
    }

    class BuffSkillLogic : SkillLogic {

        public override bool willBeExecuted(SkillLogicData data) {
            List<Card> targets = data.skill.range.getTargets(data.executor);
            return base.willBeExecuted(data) && targets != null && (targets.Count > 0);
        }

        public override void execute(SkillLogicData data) {
            var skill = data.skill;
            var executor = data.executor;
            List<Card> targets = skill.range.getTargets(executor);

            // get a list of things to buff
            List<StatusType> statusToBuff;
            if (skill.skillFuncArg2 != (int) StatusType.ALL_STATUS) {
                statusToBuff = new List<StatusType> {
                    (StatusType)skill.skillFuncArg2
                };

                // for hp shield buff, arg3 is the ceiling 
                if (skill.skillFuncArg3 != 0 && skill.skillFuncArg2 != (double) StatusType.HP_SHIELD) {
                    statusToBuff.Add((StatusType) skill.skillFuncArg3);
                }
            }
            else {
                statusToBuff = new List<StatusType> {StatusType.ATK, StatusType.DEF, StatusType.WIS, StatusType.AGI};
            }

            var basedOnStatType = skill.skillCalcType;
            var baseStat = executor.getStat(basedOnStatType);

            foreach (Card c in targets) {
                foreach (StatusType t in statusToBuff) {
                    var target = c;
                    var statusType = t;

                    double buffAmount;
                    double maxValue = 0;
                    switch (statusType) {
                        case StatusType.ATK:
                        case StatusType.DEF:
                        case StatusType.WIS:
                        case StatusType.AGI:
                            // if the range is "self and adjacent", according to Dena, the base stat will
                            // have to be recalculated
                            if (skill.skillRange == SkillRange.SELF_BOTH_SIDES) {
                                baseStat = executor.getStat(basedOnStatType);
                            }
                            var skillMod = skill.skillFuncArg1;
                            buffAmount = Math.Round(skillMod * baseStat);
                            break;
                        case StatusType.ATTACK_RESISTANCE:
                        case StatusType.MAGIC_RESISTANCE:
                        case StatusType.BREATH_RESISTANCE:
                        case StatusType.SKILL_PROBABILITY:
                        case StatusType.WILL_ATTACK_AGAIN:
                        case StatusType.ACTION_ON_DEATH:
                            buffAmount = skill.skillFuncArg1;
                            break;
                        case StatusType.HP_SHIELD:
                            skillMod = skill.skillFuncArg1;
                            buffAmount = Math.Round(skillMod * baseStat);
                            maxValue = Math.Round(target.getOriginalHP() * skill.skillFuncArg3);
                            break;
                        default :
                            throw new Exception("Wrong status type or not implemented");
                    }

                    target.changeStatus(statusType, buffAmount, false, maxValue);
                }
            }
        }
    }

    class DebuffSkillLogic : SkillLogic {
        public override void execute(SkillLogicData data) {
            var skill = data.skill;
            var executor = data.executor;
           
            List<Card> targets = skill.range.getTargets(executor);
            
            foreach (Card c in targets) {
                battleModel.processDebuff(executor, c, skill);
            }
        }
    }

    abstract class ClearStatusSkillLogic : SkillLogic {
        protected Func<double, bool> condFunc = x => true;

        public override bool willBeExecuted(SkillLogicData data) {
            var targets = getValidTargets(data);
            return base.willBeExecuted(data) && (targets.Count > 0);
        }

        List<Card> getValidTargets(SkillLogicData data) {
            var rangeTargets = data.skill.getTargets(data.executor);
            return rangeTargets.Where(t => t.hasStatus(condFunc)).ToList();
        }

        public override void execute(SkillLogicData data) {
            var targets = getValidTargets(data);

            foreach (Card c in targets) {
                c.clearAllStatus(condFunc);
            }
        }
    }

    class DispellSkillLogic : ClearStatusSkillLogic {
        public DispellSkillLogic() {
            condFunc = x => x > 0;
        }
    }

    class ClearDebuffSkillLogic : ClearStatusSkillLogic {
        public ClearDebuffSkillLogic() {
            condFunc = x => x < 0;
        }
    }

    class AfflictionSkillLogic : SkillLogic {
        public override void execute(SkillLogicData data) {
            var targets = data.skill.range.getTargets(data.executor);

            foreach (Card c in targets) {
                battleModel.processAffliction(data.executor, c, data.skill);
            }
        }
    }

    class AttackSkillLogic : SkillLogic {

        public override bool willBeExecuted(SkillLogicData data) {
            var targets = data.skill.getTargets(data.executor);
            return base.willBeExecuted(data) && targets != null && (targets.Count > 0);
        }

        public override void execute(SkillLogicData data) {
            if (RangeFactory.isEnemyRandomRange(data.skill.skillRange)) {
                executeRandomAttackSkill(data);
            }
            else {
                executeAttackSkillWithRangeTargets(data);
            }
        }

        private void executeRandomAttackSkill (SkillLogicData data) {
            var numTarget = ((EnemyRandomRange)data.skill.range).numTarget;
        
            for (var i = 0; i < numTarget && !data.executor.isDead; i++) {

                var targetCard = cardManager.getValidSingleTarget(battleModel.oppositePlayerMainCards);
    
                if (targetCard == null) {
                    // no valid target, miss a turn, continue to next card
                    return;
                }
            
                // since we get a valid index with every iteration of the loop, there's no need
                // to check if the target is dead here
                processAttackAgainstSingleTarget(data.executor, targetCard, data.skill);
            }
        }

        /**
         * Execute an attack skill that has the targets obtained from its range
         */
        private void executeAttackSkillWithRangeTargets (SkillLogicData data) {
            var skill = data.skill;
            var executor = data.executor;
            List<Card> targets = skill.range.getTargets(executor);
            double scaledRatio = 1;

            if (RangeFactory.isEnemyScaledRange(skill.skillRange)) {
                scaledRatio = RangeFactory.getScaledRatio(skill.skillRange, targets.Count);
            }

            if (skill.isIndirectSkill()) {
                // if the skill is indirect and of range type, it must be AoE, so only one reactive skill can be proc

                // NOTE: the algorithm used here for protection may not be correct, since it makes the 
                // proc rate not really what it should be. For example, if two cards, one can protect (A)
                // and one not (B), are hit by an AoE, B only has 35% chance of being protected, and not 70%,
                // since there's 50% that A will be hit first and therefore unable to protect later on when B
                // is the target (this is based on the assumption that a fam cannot be hit twice in an AoE)

                // shuffle the targets. This serves two purposes. First, we can iterate
                // through the array in a random manner. Second, since the order is not
                // simply left-to-right anymore, it reminds us that this is an AoE skill
                targets.Shuffle();

                // assume only one reactive can be proc during an AoE skill. Is it true?
                var aoeReactiveSkillActivated = false; //<- has any reactive skill proc during this whole AoE?

                // keep track of targets attacked, to make sure a fam can only be attacked once. So if a fam has already been
                // attacked, it cannot protect another fam later on 
                var targetsAttacked = new Dictionary<int, bool>();

                foreach (var targetCard in targets) {
                    // a target can be dead, for example from protecting another fam
                    if (targetCard.isDead) {
                        continue;
                    }

                    var protectSkillActivated = false; //<- has any protect skill activated to protect the current target?

                    // if no reactive skill has been activated at all during this AoE, we can try to
                    // protect this target, otherwise no protect can be activated to protect this target
                    // also, if the target has already been attacked (i.e. it protected another card before), then
                    // don't try to protect it
                    if (!aoeReactiveSkillActivated && !targetsAttacked.ContainsKey(targetCard.id)) {
                        var activated = battleModel.processProtect(executor, targetCard, skill, targetsAttacked, scaledRatio);
                        protectSkillActivated = activated;
                        if (protectSkillActivated) {
                            aoeReactiveSkillActivated = true;
                        }
                    }

                    // if not protected, proceed with the attack as normal
                    // also need to make sure the target is not already attacked
                    if (!protectSkillActivated && !targetsAttacked.ContainsKey(targetCard.id)) {
                        var defenseSkill = targetCard.getRandomDefenseSkill();

                        SkillLogicData defenseData = new SkillLogicData {
                            executor = targetCard,
                            skill = defenseSkill,
                            attacker = executor,
                        };

                        battleModel.processDamagePhase(executor, targetCard, skill, scaledRatio);
                        targetsAttacked[targetCard.id] = true;

                        if (!executor.justMissed && !targetCard.justEvaded && !targetCard.isDead) {
                            if (Skill.isDebuffAttackSkill(skill.id)) {
                                if (BBRandom.get() <= skill.skillFuncArg3) {
                                    battleModel.processDebuff(executor, targetCard, skill);
                                }
                            }
                            else if (skill.skillFunc == SkillFunc.ATTACK || skill.skillFunc == SkillFunc.MAGIC) {
                                battleModel.processAffliction(executor, targetCard, skill);
                            }
                        }

                        // try to proc post-damage skills
                        if (defenseSkill != null && defenseSkill.willBeExecuted(defenseData) && !aoeReactiveSkillActivated) 
                        {
                            defenseSkill.execute(defenseData);
                            aoeReactiveSkillActivated = true; 
                        }
                    }

                    if (skill.skillFunc == SkillFunc.DRAIN_ATTACK || skill.skillFunc == SkillFunc.DRAIN_MAGIC) {
                        processDrainPhase(executor, skill);
                    }

                    clearAllCardsDamagePhaseData();
                }
            }
            else {
                // skill makes contact, must be fork/sweeping etc., so just proceed as normal
                // i.e. multiple protection is possible
            
                for (var i = 0; i < targets.Count && !executor.isDead; i++) {
                    var targetCard = targets[i];

                    // a target can be dead, for example from protecting another fam
                    if (targetCard.isDead) {
                        continue;
                    }

                    processAttackAgainstSingleTarget(data.executor, targetCard, data.skill, scaledRatio);
                }
            }        
        }

        private void processAttackAgainstSingleTarget(Card executor, Card target, Skill skill, double scaledRatio = 0) {
            var activated = battleModel.processProtect(executor, target, skill, null, scaledRatio);

            // if not protected, proceed with the attack as normal
            if (!activated) {
                var defenseSkill = target.getRandomDefenseSkill();
                
                SkillLogicData defenseData = new SkillLogicData {
                    executor = target,
                    skill = defenseSkill,
                    attacker = executor,
                };

                battleModel.processDamagePhase(executor, target, skill, scaledRatio);

                if (!executor.justMissed && !target.justEvaded && !target.isDead) {
                    if (Skill.isDebuffAttackSkill(skill.id)) {
                        if (BBRandom.get() <= skill.skillFuncArg3) {
                            battleModel.processDebuff(executor, target, skill);
                        }
                    }
                    else if (skill.skillFunc == SkillFunc.ATTACK || skill.skillFunc == SkillFunc.MAGIC){
                        battleModel.processAffliction(executor, target, skill);
                    }
                }

                if (defenseSkill != null && defenseSkill.willBeExecuted(defenseData)) {
                    defenseSkill.execute(defenseData);    
                }
            }

            if (skill.skillFunc == SkillFunc.DRAIN_ATTACK || skill.skillFunc == SkillFunc.DRAIN_MAGIC) {
                processDrainPhase(executor, skill);
            }

            clearAllCardsDamagePhaseData();
        }

        // for drain attack
        private void processDrainPhase(Card executor, Skill skill) {
            var healRange = RangeFactory.getRange((SkillRange)skill.skillFuncArg4);
            var initialHealTargets = healRange.getTargets(executor);
            List<Card> healTargets = initialHealTargets.Where(tmpCard => !tmpCard.isFullHealth()).ToList();

            if (healTargets.Count == 0) {
                return;
            }

            var healAmount = Math.Floor((executor.lastBattleDamageDealt * skill.skillFuncArg2) / healTargets.Count);
            foreach (Card c in healTargets) {
                battleModel.damageToTargetDirectly(c, -1 * healAmount);
            }
        }
    }

    class ProtectSkillLogic : SkillLogic {

        public override bool willBeExecuted(SkillLogicData data) {
            var targets = data.skill.getTargets(data.executor);

            // a fam cannot protect itself, unless the skillRange is MYSELF
            if (cardManager.isSameCard(data.targetCard, data.executor) && data.skill.skillRange != SkillRange.MYSELF) {
                return false;
            }

            return base.willBeExecuted(data) && cardManager.isCardInList(data.targetCard, targets);
        }

        public override void execute(SkillLogicData data) {
            executeProtectPhase(data);
        }

        protected void executeProtectPhase(SkillLogicData data) {
            var protector = data.executor;
            var attackSkill = data.attackSkill;

            // first redirect the original attack to the protecting fam
            battleModel.processDamagePhase(data.attacker, protector, attackSkill, data.scaledRatio);

            // note: don't need to check for justEvaded here
            if (!data.attacker.justMissed && !protector.isDead) {
                if (attackSkill.skillFunc == SkillFunc.ATTACK || attackSkill.skillFunc == SkillFunc.MAGIC) {
                    battleModel.processAffliction(data.attacker, protector, attackSkill);
                }
                else if (Skill.isDebuffAttackSkill(attackSkill.id)) {
                    if (BBRandom.get() <= attackSkill.skillFuncArg3) {
                        battleModel.processDebuff(data.attacker, protector, attackSkill);
                    }
                }
            }

            // update the targetsAttacked if necessary
            if (data.targetsAttacked != null) {
                data.targetsAttacked[protector.id] = true;
            }

            // clear the temp stuffs
            clearAllCardsDamagePhaseData();        
        }
    }

    class EvadeSkillLogic : SkillLogic {
        public override bool willBeExecuted(SkillLogicData data) {
            var targets = data.skill.getTargets(data.executor);

            // a fam cannot protect itself, unless the skillRange is MYSELF
            if (cardManager.isSameCard(data.targetCard, data.executor) && data.skill.skillRange != SkillRange.MYSELF) {
                return false;
            }

            var canEvade = Skill.canProtectFromCalcType((SkillCalcType)data.skill.skillFuncArg2, data.attackSkill)
                        && Skill.canEvadeFromSkill(data.attackSkill);

            return base.willBeExecuted(data) && cardManager.isCardInList(data.targetCard, targets) && canEvade;
        }

        public override void execute(SkillLogicData data) {
            data.executor.justEvaded = true;

            battleModel.processDamagePhase(data.attacker, data.executor, data.attackSkill, data.scaledRatio);

            // update the targetsAttacked if necessary
            if (data.targetsAttacked != null) {
                data.targetsAttacked[data.executor.id] = true;
            }

            // clear the temp stuffs
            clearAllCardsDamagePhaseData();
        }
    }

    class ProtectCounterSkillLogic : ProtectSkillLogic {
        public override void execute(SkillLogicData data) {
            // protect phase
            executeProtectPhase(data);
            var protector = data.executor;

            // counter phase
            if (!protector.isDead && protector.canAttack() && !data.attacker.isDead) {
                battleModel.processDamagePhase(protector, data.attacker, data.skill);
            }
        }
    }

    class CounterSkillLogic : SkillLogic {
        public override void execute(SkillLogicData data) {
            // counter phase
            battleModel.processDamagePhase(data.executor, data.attacker, data.skill);
        }
    }

    class CounterDispellSkillLogic : ProtectSkillLogic {
        readonly Func<double, bool> condFunc = x => x > 0;

        public override bool willBeExecuted(SkillLogicData data) {
            var targets = getValidTargets(data);
            return base.willBeExecuted(data) && (targets.Count > 0);
        }

        List<Card> getValidTargets(SkillLogicData data) {
            // nearly similar to DispellSkillLogic, but careful with the range
            var range = RangeFactory.getRange((SkillRange) data.skill.skillFuncArg3);
            var rangeTargets = range.getTargets(data.executor);

            return rangeTargets.Where(t => t.hasStatus(condFunc)).ToList();
        }

        public override void execute(SkillLogicData data) {
            executeProtectPhase(data);

            if (data.executor.isDead || !data.executor.canUseSkill()) {
                return;
            }

            // now process the dispell
            var targets = getValidTargets(data);

            foreach (Card c in targets) {
                c.clearAllStatus(condFunc);
            }
        }
    }

    class OnHitDebuffSkillLogic : SkillLogic {

        private const int UNINITIALIZED_VALUE = -1234;
        private int executionLeft = UNINITIALIZED_VALUE;

        public override bool willBeExecuted(SkillLogicData data) {
            List<Card> targets = data.skill.getTargets(data.executor);

            // this should be done at construction time instead...
            if (executionLeft == UNINITIALIZED_VALUE) {
                executionLeft = (int) data.skill.skillFuncArg5;
            }

            if (executionLeft == 0) return false;

            var success = base.willBeExecuted(data) && targets != null && (targets.Count > 0);

            if (success) {
                executionLeft--;
                return true;
            } else return false;
        }

        public override void execute(SkillLogicData data) {
            // debuff
            var targets = data.skill.getTargets(data.executor);
            foreach (Card c in targets) {
                battleModel.processDebuff(data.executor, c, data.skill);
            }
        }
    }

    class DrainSkillLogic : SkillLogic {

        public override bool willBeExecuted(SkillLogicData data) {
            var targets = getValidTargets(data);
            return base.willBeExecuted(data) && (targets.Count > 0);
        }

        List<Card> getValidTargets(SkillLogicData data) {
            var rangeTargets = data.skill.getTargets(data.executor);

            return rangeTargets.Where(t => !t.isFullHealth()).ToList();
        }

        public override void execute(SkillLogicData data) {

            var targets = getValidTargets(data);

            // don't worry about length == 0, it would not have gotten into here anyway
            var eachTargetHealAmount = Math.Floor(data.executor.lastBattleDamageTaken / targets.Count);

            foreach (Card c in targets) {
                battleModel.damageToTargetDirectly(c, -1 * eachTargetHealAmount);
            }
        }
    }

    class SurviveSkillLogic : SkillLogic {
        public override bool willBeExecuted(SkillLogicData data) {
            var hpRatio = data.executor.getHPRatio();
            return base.willBeExecuted(data) && (hpRatio > data.skill.skillFuncArg1) && (data.wouldBeDamage >= data.executor.getHP());
        }

        public override void execute(SkillLogicData data) {
            // used to just write a log and do nothing
        }
    }

    class HealSkillLogic : SkillLogic {
        public override bool willBeExecuted(SkillLogicData data) {
            var targets = getValidTargets(data);
            return base.willBeExecuted(data) && (targets.Count > 0);
        }

        private List<Card> getValidTargets(SkillLogicData data) {
            var rangeTargets = data.skill.range.getAllPossibleTargets(data.executor);
            var validTargets = new List<Card>();
            var cond = getCondFunc();

            for (var i = 0; rangeTargets != null && i < rangeTargets.Count; i++) {
                if (cond(rangeTargets[i])) {
                    validTargets.Add(rangeTargets[i]);
                }
            }

            return validTargets;
        }

        private Func<Card, bool> getCondFunc() {
            return card => !card.isFullHealth();
        }

        public override void execute(SkillLogicData data) {
            var targets = data.skill.range.getTargets(data.executor, getCondFunc());

            var baseHealAmount = SkillCalc.getHealAmount(data.executor);

            var multiplier = data.skill.skillFuncArg1;
            var healAmount = Math.Floor(multiplier * baseHealAmount);

            foreach (Card c in targets) {
                // if the heal is not based on wis, recalculate the heal amount
                if (data.skill.skillFuncArg2 == 1) {
                    healAmount = multiplier * c.getOriginalHP();
                }

                battleModel.damageToTargetDirectly(c, -1 * healAmount);
            }     
        }
    }

    class ReviveSkillLogic : SkillLogic {
        public override bool willBeExecuted(SkillLogicData data) {
            var targets = data.skill.range.getAllPossibleTargets(data.executor);
            return base.willBeExecuted(data) && targets != null && (targets.Count > 0);
        }

        public override void execute(SkillLogicData data) {
            var targets = data.skill.getTargets(data.executor);
            var hpRatio = data.skill.skillFuncArg1;

            foreach (Card c in targets) {
                c.revive(hpRatio);
            }
        }
    }

    class TurnOrderChangeSkillLogic : SkillLogic {
        public override bool willBeExecuted(SkillLogicData data) {
            return base.willBeExecuted(data) && !battleModel.turnOrderChanged;
        }

        public override void execute(SkillLogicData data) {
            battleModel.turnOrderChanged = true;
            battleModel.turnOrderBase = (BattleTurnOrderType) data.skill.skillFuncArg1;
            battleModel.turnOrderChangeEffectiveTurns = (int) data.skill.skillFuncArg2;
        }
    }

    class RandomSkillLogic : SkillLogic {
        public override void execute(SkillLogicData data) {
            List<int> randSkillsId = SkillDatabase.get(data.skill.id).randSkills;
            randSkillsId.Shuffle();
            data.noProbCheck = true;

            foreach (int t in randSkillsId) {
                var skill = new Skill(t);
                data.skill = skill;

                if (skill.willBeExecuted(data)) {
                    skill.execute(data);
                    break;
                }
            }
        }
    }

    class SkillLogicData {
        public Card executor;
        public Skill skill;
        public double wouldBeDamage; // the would-be damage, for survive skills
        public double scaledRatio;
        public Card attacker;    // for protect/counter
        public Skill attackSkill; // for protect/counter
        public Card targetCard;  // for protect
        public Dictionary<int, bool> targetsAttacked;  // for protect
        public bool noProbCheck; // for passing the prob check of proccing the skill, like for RandomSkillLogic
    }
}
