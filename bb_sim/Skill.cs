using System.Collections.Generic;

namespace bb_sim {
    class Skill {
        public readonly int id;
        string name;
        public readonly SkillType skillType;
        public readonly SkillFunc skillFunc;
        public readonly SkillCalcType skillCalcType;
        public readonly double skillFuncArg1;
        public readonly double skillFuncArg2;
        public readonly double skillFuncArg3;
        public readonly double skillFuncArg4;
        public readonly double skillFuncArg5;
        public readonly SkillRange skillRange;
        public readonly int maxProbability;
        public readonly WardType ward;
        string description;
        readonly bool isAutoAttack;

        public readonly BaseRange range;
        readonly SkillLogic logic;

        public Skill (int skillId) {    
            SkillInfo skillData = SkillDatabase.get(skillId);
        
            id = skillId;
            name = skillData.name;
            skillType = skillData.type;
            skillFunc = skillData.func;
            skillCalcType = skillData.calc;
            skillFuncArg1 = skillData.arg1;
            skillFuncArg2 = skillData.arg2;
            skillFuncArg3 = skillData.arg3;
            skillFuncArg4 = skillData.arg4;
            skillFuncArg5 = skillData.arg5;
            skillRange = skillData.range;
            maxProbability = skillData.prob;
            ward = skillData.ward;
            description = skillData.desc;
            isAutoAttack = skillData.isAutoAttack;

            logic = SkillLogicFactory.getSkillLogic(skillFunc);

            bool selectDead = skillFunc == SkillFunc.REVIVE;
            range = RangeFactory.getRange(skillRange, selectDead);
        }

        /**
         * Return true if this is an attack skill
         */
        static bool isAttackSkill(int skillId) {
            var isAttackSkill = false;
            SkillInfo skillInfo = SkillDatabase.get(skillId);

            switch (skillInfo.func) {
                case SkillFunc.ATTACK:
                case SkillFunc.MAGIC:
                case SkillFunc.COUNTER:
                case SkillFunc.PROTECT_COUNTER:
                case SkillFunc.DEBUFFATTACK:
                case SkillFunc.DEBUFFINDIRECT:
                case SkillFunc.CASTER_BASED_DEBUFF_ATTACK:
                case SkillFunc.CASTER_BASED_DEBUFF_MAGIC:
                case SkillFunc.DRAIN_ATTACK:
                case SkillFunc.DRAIN_MAGIC:
                case SkillFunc.KILL:
                    isAttackSkill = true;
                    break;
            }

            return isAttackSkill;
        }

        /**
         * Return true if the skill does not make contact
         */
        public static bool isIndirectSkill(int skillId) {
            var isIndirect = true;
            SkillInfo skillInfo = SkillDatabase.get(skillId);

            switch (skillInfo.func) {
                case SkillFunc.ATTACK:
                case SkillFunc.COUNTER:
                case SkillFunc.PROTECT_COUNTER:
                case SkillFunc.DEBUFFATTACK:
                case SkillFunc.CASTER_BASED_DEBUFF_ATTACK:
                case SkillFunc.DRAIN_ATTACK:
                    isIndirect = false;
                    break;
            }
        
            return isIndirect;
        }

        public static bool isPositionIndependentAttackSkill(int skillId) {
            SkillInfo skillInfo = SkillDatabase.get(skillId);

            // generally, indirect skills are position independent
            // however, kill skills are indirect (do not make contact) but not position independent
            return isIndirectSkill(skillId) && skillInfo.func != SkillFunc.KILL;
        }

        /**
         * Return true if this is an auto attack, and false or undefined if not
         */
        static bool isAutoAttackSkill(int skillId) {
            return SkillDatabase.get(skillId).isAutoAttack;
        }

        /**
         * Return true if this is an attack skill with debuff
         */
        public static bool isDebuffAttackSkill(int skillId) {
            var isDebuffAttack = false;
            SkillInfo skillInfo = SkillDatabase.get(skillId);

            switch (skillInfo.func) {
                case SkillFunc.DEBUFFATTACK:
                case SkillFunc.DEBUFFINDIRECT:
                case SkillFunc.CASTER_BASED_DEBUFF_ATTACK:
                case SkillFunc.CASTER_BASED_DEBUFF_MAGIC:
                    isDebuffAttack = true;
                    break;
            }

            return isDebuffAttack;
        }

        /**
         * Return true if this skill should be available for user to select, or available to be randomly chosen
         */
        public static bool isAvailableForSelect(int skillId) {
            var isAvailable = true;
            SkillInfo skillInfo = SkillDatabase.get(skillId);

            if (skillInfo.isAutoAttack || skillId == 355 || skillId == 452) {
                isAvailable = false;
            }

            return isAvailable;
        }

        /**
         * Return true if the attack skill can be protected/evaded from the calc type
         */
        public static bool canProtectFromCalcType(SkillCalcType type, Skill attackSkill) {
            switch (type) {
                case SkillCalcType.ATK:
                case SkillCalcType.WIS:
                case SkillCalcType.AGI:
                    return attackSkill.skillCalcType == type;
                case SkillCalcType.ATK_WIS:
                    return attackSkill.skillCalcType == SkillCalcType.ATK || attackSkill.skillCalcType == SkillCalcType.WIS;
                case SkillCalcType.ATK_AGI:
                    return attackSkill.skillCalcType == SkillCalcType.ATK || attackSkill.skillCalcType == SkillCalcType.AGI;
                case SkillCalcType.WIS_AGI:
                    return attackSkill.skillCalcType == SkillCalcType.WIS || attackSkill.skillCalcType == SkillCalcType.AGI;
                default:
                    return false;
            }
        }

        /**
         * Return true if the attack skill can be evaded
         */
        public static bool canEvadeFromSkill(Skill attackSkill) {
            return (attackSkill.skillFunc != SkillFunc.COUNTER
                        && attackSkill.skillFunc != SkillFunc.PROTECT_COUNTER
                        && !attackSkill.isAutoAttack);
        }

        public bool isIndirectSkill() {
            return isIndirectSkill(id);
        }

        public bool willBeExecuted(SkillLogicData data) {
            return logic.willBeExecuted(data);
        }

        public void execute(SkillLogicData data) {
            logic.execute(data);
        }

        public List<Card> getTargets(Card executor) {
            return range.getTargets(executor);
        }

    }
}
