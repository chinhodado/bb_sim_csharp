using System;

namespace bb_sim {
    static class AfflictionFactory {
        public static Affliction getAffliction(AfflictionType type) {
            switch (type) {
                case AfflictionType.BLIND:
                    return new BlindAffliction();
                case AfflictionType.DISABLE:
                    return new DisabledAffliction();
                case AfflictionType.FROZEN:
                    return new FrozenAffliction();
                case AfflictionType.PARALYSIS:
                    return new ParalysisAffliction();
                case AfflictionType.POISON:
                    return new PoisonAffliction();
                case AfflictionType.SILENT:
                    return new SilentAffliction();
                default:
                    throw new Exception("Invalid affliction type!");
            }
        }
    }

    class Affliction {
        public readonly AfflictionType type;
        protected bool finished;

        protected Affliction (AfflictionType type) {
            this.type = type;
            finished = false;
        }

        /**
         * Can the familiar do anything (both auto attack and perform skills) with this affliction?
         */
        public virtual bool canAttack() {
            throw new Exception("Implement this");
        }

        /**
         * Can the familiar use skills with this affliction?
         */
        public virtual bool canUseSkill() {
            return canAttack();
        }

        /**
         * Can the familiar miss with this affliction?
         */
        public virtual bool canMiss() {
            return false;
        }

        /**
         * Called when the affliction needs to be updated, like at the end of the fam's turn
         */
        public virtual void update(Card card) {
            throw new Exception ("Implement this");
        }

        /**
         * Called when the fam is affected with another affliction of the same type
         */
        public virtual void add(AfflectOptParam option) {
            // implement this
        }

        public bool isFinished() {
            return finished;
        }

        public void clear() {
            finished = true;
        }

        public AfflictionType getType() {
            return type;
        }
    }

    class PoisonAffliction : Affliction {
        private const int DEFAULT_PERCENT = 5; // default poison is 5% of HP every turn
        private const int MAX_STACK_NUM = 2;   // maximum number that poison can stack
        private const int MAX_DAMAGE = 99999;  // maximum poison damage is 99999 every turn

        public double percent;

        public PoisonAffliction () : base (AfflictionType.POISON) {
            percent = 0;
            finished = false;
        }

        public override bool canAttack() {
            return true;
        }

        public override void update(Card card) {
            double damage = Math.Floor(card.originalStats.hp * percent / 100);
            if (damage > MAX_DAMAGE) {
                damage = MAX_DAMAGE;
            }
            // damage the card
            BattleModel.getInstance().damageToTargetDirectly(card, damage);
        }

        public override void add(AfflectOptParam option) {
            var toAdd = option.percent;
            if (option.percent == 0) {
                toAdd = DEFAULT_PERCENT;
            }
            percent += toAdd;

            // there's a bug in here. Not my fault though
            var maxPercent = percent * MAX_STACK_NUM;
            if (percent > maxPercent) {
                percent = maxPercent;
            }
        }
    }

    class ParalysisAffliction : Affliction {

        public ParalysisAffliction() : base(AfflictionType.PARALYSIS) {
        }

        public override bool canAttack() {
            return isFinished();
        }

        public override void update(Card card) {
            clear();
        }
    }

    class FrozenAffliction : Affliction {

        public FrozenAffliction() : base(AfflictionType.FROZEN) {}

        public override bool canAttack() {
            return isFinished();
        }

        public override void update(Card card) {
            clear();
        }
    }

    class DisabledAffliction : Affliction {

        public DisabledAffliction() : base(AfflictionType.DISABLE) {}

        public override bool canAttack() {
            return isFinished();
        }

        public override void update(Card card) {
            clear();
        }
    }

    class SilentAffliction : Affliction {

        int validTurnNum; // number of turns for silence

        public SilentAffliction() : base(AfflictionType.SILENT) {}

        public override bool canAttack(){
            return true;
        }

        public override bool canUseSkill() {
            return isFinished();
        }

        public override void update(Card card) {
            if (--validTurnNum <= 0) {
                clear();
            }
        }
    
        public override void add(AfflectOptParam option){
            validTurnNum = option.turnNum;
        }
    }

    class BlindAffliction : Affliction {

        double missProb;     // the probability for missing
        int validTurnNum; // number of turns for blind

        public BlindAffliction() : base(AfflictionType.BLIND) {
            missProb = 0;
            finished = false;
            validTurnNum = 0;
        }

        public override bool canAttack(){
            return true;
        }

        public override bool canMiss(){
            return BBRandom.get() <= missProb;
        }

        public override void update(Card card) {
            if (--validTurnNum <= 0) {
                clear();
            }
        }

        public override void add(AfflectOptParam option) {
            validTurnNum = option.turnNum;
            missProb = option.missProb;
        }
    }

    /**
     * A simple struct for afflection's optional parameters
     */
    struct AfflectOptParam {
        public int turnNum;  // for silent and blind
        public double missProb; // for blind
        public double percent;  // for poison
    }
}
