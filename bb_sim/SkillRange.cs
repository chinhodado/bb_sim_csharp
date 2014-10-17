using System;
using System.Collections.Generic;
using System.Linq;

namespace bb_sim {
    abstract class RangeFactory {

        static readonly Dictionary<int, int> ENEMY_RANDOM_RANGE_TARGET_NUM = new Dictionary<int, int> {
            {16, 3},
            {17, 6},
            {19, 4},
            {20, 5},
            {23, 2}
        };

        static readonly Dictionary<int, int> ENEMY_NEAR_SCALED_RANGE_TARGET_NUM = new Dictionary<int, int> {
            {312, 2},
            {313, 3},
            {314, 4},
            {315, 5}
        };
    
        static readonly Dictionary<int, int> ENEMY_NEAR_RANGE_TARGET_NUM = new Dictionary<int, int> {
             {5, 1},
             {6, 2},
             {7, 3},
            {32, 4},
            {33, 5}
        };

        public static readonly Dictionary<int, int> FRIEND_RANDOM_RANGE_TARGET_NUM = new Dictionary<int, int> {
            {101, 1},
            {102, 2},
            {103, 3},
            {104, 4},
            {105, 5},
            {106, 6},
            {111, 1},
            {112, 2},
            {113, 3},
            {114, 4},
            {115, 5},
            {116, 6},
            {121, 1},
            {122, 2},
            {123, 3},
            {124, 4},
            {125, 5},
            {126, 6},
            {131, 1},
            {132, 2},
            {133, 3},
            {134, 4},
            {135, 5},
            {136, 6}
        };

        public static readonly Dictionary<int, bool> INCLUDE_SELF = new Dictionary<int, bool> {
            {111, true},
            {112, true},
            {113, true},
            {114, true},
            {115, true},
            {116, true},
            {131, true},
            {132, true},
            {133, true},
            {134, true},
            {135, true},
            {136, true},

            {332, true},
            {333, true},
            {334, true},
            {335, true},
            {336, true}
        };

        static Dictionary<int, bool>  IS_UNIQUE = new Dictionary<int, bool> {
            {121, true},
            {122, true},
            {123, true},
            {124, true},
            {125, true},
            {126, true},
            {131, true},
            {132, true},
            {133, true},
            {134, true},
            {135, true},
            {136, true}
        };

        static readonly Dictionary<int, double[]> ScalePatternParams = new Dictionary<int, double[]> {
            {202, new[] {1.5, 1}},
            {203, new[] {1.75, 1.25, 1}},
            {204, new[] {1.9375, 1.4375, 1.25, 1.13, 1, 1, 1, 1, 1, 1}},
            {208, new[] {1.9375, 1.4375, 1.25, 1.13, 1}},
            {212, new[] {1.0, 1, 1, 1, 1}},
            {213, new[] {1.0, 1, 1, 1, 1}},
            {214, new[] {1.0, 1, 1, 1, 1}},
            {215, new[] {1.0, 1, 1, 1, 1}},
            {234, new[] {1.0, 1, 1, 1, 1}},
            {312, new[] {1.5, 1}},
            {313, new[] {1.75, 1.25, 1}},
            {314, new[] {1.875, 1.375, 1.16, 1}},
            {315, new[] {1.9375, 1.4375, 1.25, 1.13, 1}},
            {322, new[] {1.5, 1}},
            {323, new[] {1.75, 1.25, 1}},
            {324, new[] {1.875, 1.375, 1.16, 1}},
            {325, new[] {1.875, 1.375, 1.16, 1, 1}},
            {326, new[] {1.875, 1.375, 1.16, 1, 1, 1}},
            {332, new[] {1.5, 1}},
            {333, new[] {1.75, 1.25, 1}},
            {334, new[] {1.875, 1.375, 1.16, 1}},
            {335, new[] {1.9375, 1.4375, 1.25, 1.13, 1}},
            {336, new[] {1.9375, 1.4375, 1.25, 1.13, 1, 1}}
        };
    
        public static BaseRange getRange (SkillRange id, bool selectDead = false) {
            BaseRange range;
            if (isEnemyRandomRange(id)) {
                range = createEnemyRandomRange(id);
            }
            else if (isEnemyNearRange(id) || isEnemyNearScaledRange(id)) {
                range = createEnemyNearRange(id);
            }
            else if (isFriendRandomRange(id)) {
                range = createFriendRandomRange(id, selectDead);
            }
            else {
                range = createRange(id, selectDead);            
            }
            return range;
        }

        public static bool isEnemyRandomRange (SkillRange id) {
            return ENEMY_RANDOM_RANGE_TARGET_NUM.ContainsKey((int) id);
        }

        private static bool isFriendRandomRange (SkillRange id) {
            return FRIEND_RANDOM_RANGE_TARGET_NUM.ContainsKey((int) id);
        }

        private static EnemyRandomRange createEnemyRandomRange (SkillRange id) {
            return new EnemyRandomRange(id, ENEMY_RANDOM_RANGE_TARGET_NUM[(int)id]);
        }

        private static FriendRandomRange createFriendRandomRange (SkillRange id, bool selectDead) {
            return new FriendRandomRange(id, FRIEND_RANDOM_RANGE_TARGET_NUM[(int)id], selectDead);
        }

        private static bool isEnemyNearRange (SkillRange id) {
            return ENEMY_NEAR_RANGE_TARGET_NUM.ContainsKey((int) id);
        }

        private static EnemyNearRange createEnemyNearRange (SkillRange id) {
            int numTargets = 0;
            if (isEnemyNearRange(id)) {
                numTargets = ENEMY_NEAR_RANGE_TARGET_NUM[(int)id];
            }
            else if (isEnemyNearScaledRange(id)) {
                numTargets = ENEMY_NEAR_SCALED_RANGE_TARGET_NUM[(int)id];
            }
            return new EnemyNearRange(id, numTargets);
        }

        private static bool isEnemyNearScaledRange(SkillRange id) {
            return ENEMY_NEAR_SCALED_RANGE_TARGET_NUM.ContainsKey((int)id);
        }

        public static bool isEnemyScaledRange(SkillRange id) {
            return isEnemyNearScaledRange(id) || id == SkillRange.ENEMY_ALL_SCALED;
        }

        public static double getScaledRatio(SkillRange id, int targetsLeft) {
            if (!ScalePatternParams.ContainsKey((int)id)) {
                throw new Exception("Invalid range for getting scale ratio");
            }
            var paramArray = ScalePatternParams[(int)id];
            return paramArray[targetsLeft - 1];
        }

        private static bool isRowBasedRange(SkillRange rangeId) {
            return rangeId == SkillRange.ENEMY_FRONT_ALL || 
                   rangeId == SkillRange.ENEMY_REAR_ALL || 
                   rangeId == SkillRange.ENEMY_FRONT_MID_ALL;
        }

        public static bool canBeAoeRange(SkillRange rangeId) {
            bool canBe = isEnemyNearRange(rangeId) || 
                         isEnemyNearScaledRange(rangeId) || 
                         isRowBasedRange(rangeId) || 
                         rangeId == SkillRange.ENEMY_ALL ||
                         rangeId == SkillRange.ENEMY_ALL_SCALED;

            return canBe;    
        }

        private static BaseRange createRange (SkillRange id, bool selectDead) {
            switch (id) {
                case SkillRange.EITHER_SIDE:
                    return new EitherSideRange(id, selectDead); // either side, but not both
                case SkillRange.BOTH_SIDES:
                    return new BothSidesRange(id, selectDead);
                case SkillRange.SELF_BOTH_SIDES:
                    return new SelfBothSidesRange(id);
                case SkillRange.ALL:
                    return new AllRange(id);
                case SkillRange.ENEMY_ALL:
                case SkillRange.ENEMY_ALL_SCALED:
                    return new EnemyAllRange(id);
                case SkillRange.ENEMY_FRONT_ALL:
                    return new EnemyFrontAllRange(id);
                case SkillRange.ENEMY_REAR_ALL:
                    return new EnemyRearAllRange(id);
                case SkillRange.ENEMY_FRONT_MID_ALL:
                    return new EnemyFrontMidAllRange(id);
                case SkillRange.MYSELF:
                    return new SelfRange(id, selectDead);
                case SkillRange.RIGHT:
                    return new RightRange(id);
                default:
                    throw new Exception("Invalid range or not implemented");
            }
        }
    }

    abstract class BaseRange {
   
        SkillRange id;

        protected BaseRange(SkillRange id) {
            this.id = id;
        }

        public virtual List<Card> getTargets(Card executor, Func<Card, bool> skillCondFunc) {
            throw new Exception("Implement this");
        }

        public virtual List<Card> getTargets(Card executor) {
            return getTargets(executor, getCondFunc(executor));
        }

        protected List<Card> getBaseTargets(Func<Card, bool> condFunc) {
            var allCards = CardManager.getInstance().getAllMainCardsInPlayerOrder();
            return allCards.Where(condFunc).ToList();
        }

        // this is needed for the sake of random-targets skills
        public virtual List<Card> getAllPossibleTargets(Card executor) {
            return getTargets(executor);
        }

        protected Card getRandomCard(List<Card> cards) {
            return BBRandom.getRandomListItem(cards);
        }

        // returns a maximum of 'num' unique cards (shuffles and returns first n)
        protected List<Card> getRandomUniqueCards(List<Card> cards, int num) {
            var copy = new List<Card>(cards);
            copy.Shuffle();
            return copy.Take(num).ToList();
        }

        protected Func<Card, bool> getCondFunc(Card executor) {
            // by default, valid if card is not dead and belongs to the enemy
            return card => !card.isDead && (card.getPlayerId() != executor.getPlayerId());
        }

        protected bool satisfyDeadCondition(Card card, bool selectDead) {
            return (card.isDead && selectDead) || (!card.isDead && !selectDead);
        }
    }

    class BothSidesRange : BaseRange {
        readonly bool selectDead;
        public BothSidesRange(SkillRange id, bool selectDead) : base(id) {
            this.selectDead = selectDead;
        }
    
        public override List<Card> getTargets(Card executor) {
            var targets = new List<Card>();
        
            Card leftCard = CardManager.getInstance().getLeftSideCard(executor);
            if (leftCard != null && satisfyDeadCondition(leftCard, selectDead)) {
                targets.Add(leftCard);
            }
        
            Card rightCard = CardManager.getInstance().getRightSideCard(executor);
            if (rightCard != null && satisfyDeadCondition(rightCard, selectDead)) {
                targets.Add(rightCard);
            }
        
            return targets;
        }
    }

    class EnemyRandomRange : BaseRange {
        public readonly int numTarget;
    
        public EnemyRandomRange(SkillRange id, int numTarget) : base(id) {
            this.numTarget = numTarget;    
        }

        // use this for determining if there is a target only
        public override List<Card> getTargets(Card executor) {
            return getBaseTargets(getCondFunc(executor));
        }

        public override List<Card> getAllPossibleTargets(Card executor) {
            return getBaseTargets(getCondFunc(executor));
        }
    }

    class EitherSideRange : BothSidesRange {

        public EitherSideRange(SkillRange id, bool selectDead) : base(id, selectDead) {}
    
        public override List<Card> getTargets(Card executor) {
            var targets = base.getTargets(executor);
        
            if (targets.Count == 0) {
                return new List<Card>();
            }
            else {
                return new List<Card> {
                    BBRandom.getRandomListItem(targets)
                };
            }
        }
    }

    class RightRange : BaseRange {

        public RightRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets(Card executor) {
            List<Card> targets = new List<Card>();
            var partyCards = CardManager.getInstance().getPlayerCurrentMainCards(executor.player);
        
            for (var i = executor.formationColumn + 1; i < 5; i++) {
                if (!partyCards[i].isDead) {
                    targets.Add(partyCards[i]);
                }
            }
        
            return targets;
        }
    }

    class SelfRange : BaseRange {
        readonly bool selectDead;
        public SelfRange(SkillRange id, bool selectDead) :base(id) {
            this.selectDead = selectDead;
        }

        public override List<Card> getTargets(Card executor) {
            List<Card> targets = new List<Card>();

            if (satisfyDeadCondition(executor, selectDead)) {
                targets.Add(executor);
            }

            return targets;
        }
    }

    class SelfBothSidesRange : BaseRange {

        public SelfBothSidesRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets(Card executor) {
            List<Card> targets = new List<Card>();
        
            Card leftCard = CardManager.getInstance().getLeftSideCard(executor);
            if (leftCard != null && !leftCard.isDead) {
                targets.Add(leftCard);
            }
        
            if (!executor.isDead) { // should always be true
                targets.Add(executor);
            }

            Card rightCard = CardManager.getInstance().getRightSideCard(executor);
            if (rightCard != null && !rightCard.isDead) {
                targets.Add(rightCard);
            }
        
            return targets;
        }
    }

    class AllRange : BaseRange {

        public AllRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets(Card executor) {
            var partyCards = CardManager.getInstance().getPlayerCurrentMainCards(executor.player);

            return partyCards.Where(t => !t.isDead).ToList();
        }
    }

    class EnemyNearRange : BaseRange {
        static readonly Dictionary<int, int> MAX_DISTANCE_FROM_CENTER = new Dictionary<int, int> {
            {1, 1},
            {2, 1},
            {3, 1},
            {4, 2},
            {5, 2}
        };
    
        // specific to an instance, the max distance from the center enemy
        readonly int maxDistance;
        private readonly int numTarget;
            
        public EnemyNearRange(SkillRange id, int numTarget) : base(id) {
            maxDistance = MAX_DISTANCE_FROM_CENTER[numTarget];
            this.numTarget = numTarget;
        }
    
        public override List<Card> getTargets (Card executor) {
            // get center enemy
            var centerEnemy = CardManager.getInstance().getNearestSingleOpponentTarget(executor);

            if (centerEnemy == null) {
                return new List<Card>();
            }

            var enemyCards = CardManager.getInstance().getEnemyCurrentMainCards(executor.player);
        
            // only upto 2 and not 4 since the max distance is 2 anyway
            var offsetArray = new[] {0, -1, 1, -2, 2};
            List<Card> targets = new List<Card>();
            var targetCount = 0;
        
            // starting from the center enemy, go around it, adding targets when possible
            foreach (int i in offsetArray) {
                if (targetCount >= numTarget || Math.Abs(i) > maxDistance) {
                    break;
                }
                var currentEnemyIndex = centerEnemy.formationColumn + i;
                if (enemyCards.IsValidIndex(currentEnemyIndex)) {
                    var currentEnemyCard = enemyCards[currentEnemyIndex]; //todo: check this shit
                    if (currentEnemyCard != null && !currentEnemyCard.isDead) {
                        targetCount++;
                        targets.Add(enemyCards[currentEnemyIndex]);
                    }
                }
            }

            return targets;
        }
    }

    class EnemyAllRange : BaseRange {

        public EnemyAllRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets (Card executor) {
            var enemyCards = CardManager.getInstance().getEnemyCurrentMainCards(executor.player);
            return enemyCards.Where(currentEnemyCard => currentEnemyCard != null && !currentEnemyCard.isDead).ToList();
        }
    }

    class FriendRandomRange : BaseRange {
        readonly int numTargets;
        readonly bool selectDead;
        readonly bool isUnique;
        readonly bool includeSelf;

        public FriendRandomRange(SkillRange id, int numTargets, bool selectDead) : base(id) {
            this.numTargets = numTargets;
            this.selectDead = selectDead;
            isUnique = RangeFactory.FRIEND_RANDOM_RANGE_TARGET_NUM.ContainsKey((int)id);
            includeSelf = RangeFactory.INCLUDE_SELF.ContainsKey((int)id);
        }
    
        public override List<Card> getTargets (Card executor, Func<Card, bool> skillCondFunc) {
            List<Card> baseTargets = getBaseTargets(getCondFunc(executor, skillCondFunc));
            List<Card> targets = new List<Card>();

            if (baseTargets.Count != 0) {
                if (isUnique) {
                    targets = getRandomUniqueCards(baseTargets, numTargets);
                } 
                else {
                    for (var i = 0; i < numTargets; i++) {
                        targets.Add(getRandomCard(baseTargets));
                    }
                }
            }
            return targets;
        }

        public override List<Card> getAllPossibleTargets(Card executor) {
            return getBaseTargets(getCondFunc(executor));
        }

        private Func<Card, bool> getCondFunc(Card executor, Func<Card, bool> skillCondFunc) {
            return card => {
                if (card.getPlayerId() != executor.getPlayerId())
                    return false;

                if (card.id == executor.id && !includeSelf)
                    return false;

                if ((selectDead && !card.isDead) || (!selectDead && card.isDead))
                    return false;

                if (skillCondFunc != null && !skillCondFunc(card))
                    return false;

                return true;
            };
        }
    }

    abstract class BaseRowRange : BaseRange {
        private const int ROW_TYPE_COUNT = 3;

        protected BaseRowRange(SkillRange id) : base(id) {}

        protected List<Card> getSameRowCards(List<Card> cards, FormationRow row) {
            return cards.Where(card => card.getFormationRow() == row).ToList();
        }

        protected List<Card> getRowCandidates(List<Card> cards, FormationRow row, bool isAsc) {
            var candidates = new List<Card>();
            if (cards == null || cards.Count == 0) {
                return candidates;
            }

            int currentRow = (int) row;
            while (candidates.Count == 0) {
                var sameRowCards = getSameRowCards(cards, (FormationRow)currentRow);
                candidates.AddRange(sameRowCards);

                currentRow = (isAsc) ? currentRow % ROW_TYPE_COUNT + 1 : currentRow - 1;

                if (currentRow < 1) {
                    currentRow = (int)FormationRow.REAR;
                }

                if (currentRow == (int)row) {
                    break;
                }
            }

            return candidates;
        }
    }

    class EnemyFrontMidAllRange : BaseRowRange {

        public EnemyFrontMidAllRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets(Card executor) {
            var candidates = getBaseTargets(getCondFunc(executor));
            if (candidates.Count > 0) {
                var frontCards = getSameRowCards(candidates, FormationRow.FRONT);
                var centerCards = getSameRowCards(candidates, FormationRow.MID);

                if (frontCards.Count > 0 || centerCards.Count > 0) {
                    candidates = frontCards.Concat(centerCards).ToList();
                } else {
                    candidates = getSameRowCards(candidates, FormationRow.REAR);
                }
            }
            return candidates;
        }
    }

    class EnemyFrontAllRange : BaseRowRange {

        public EnemyFrontAllRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets(Card executor) {
            var candidates = getBaseTargets(getCondFunc(executor));
            if (candidates.Count > 0) {
                candidates = getRowCandidates(candidates, FormationRow.FRONT, true);
            }
            return candidates;
        }
    }

    class EnemyRearAllRange : BaseRowRange {

        public EnemyRearAllRange(SkillRange id) : base(id) {}

        public override List<Card> getTargets(Card executor) {
            var candidates = getBaseTargets(getCondFunc(executor));
            if (candidates.Count > 0) {
                candidates = getRowCandidates(candidates, FormationRow.REAR, false);
            }
            return candidates;
        }
    }
}
