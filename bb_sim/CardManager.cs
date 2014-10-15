using System;
using System.Collections.Generic;
using System.Linq;

namespace bb_sim {
    /**
     * A helper class for BattleModel, provides the card-related methods
     */
    class CardManager {

        private static CardManager _instance;
        private readonly BattleModel battle;

        public static CardManager getInstance() {
            return _instance ?? (_instance = new CardManager());
        }

        private CardManager() {
            if (_instance != null) {
                throw new Exception("Error: Instantiation failed: Use getInstance() instead of new.");
            }
            _instance = this;

            battle = BattleModel.getInstance();
        }

        /**
         * Allows to create a new instance
         * Used for testing only
         */
        public static void removeInstance() {
            _instance = null;
        }

        private Comparison<Card> getSortFunc(BattleTurnOrderType type) {
            switch (type) {
                case BattleTurnOrderType.AGI:
                    return (a, b) => b.getAGI().CompareTo(a.getAGI());
                case BattleTurnOrderType.ATK:
                    return (a, b) => b.getATK().CompareTo(a.getATK());
                case BattleTurnOrderType.DEF:
                    return (a, b) => b.getDEF().CompareTo(a.getDEF());
                case BattleTurnOrderType.WIS:
                    return (a, b) => b.getWIS().CompareTo(a.getWIS());
                default:
                    // no HP for now
                    throw new Exception("Invalid turn order type!");
            }
        }

        public void sortAllCurrentMainCards() {
            var sortFunc = getSortFunc(battle.turnOrderBase);
            battle.allCurrentMainCards.Sort(sortFunc);
        }

        public List<Card> getPlayerCurrentMainCardsByProcOrder(Player player) {
            var playerCards = getPlayerCurrentMainCards(player);
            var copy = playerCards.ToList();

            copy.Sort((a, b) => a.procIndex - b.procIndex); // ascending based on proc index

            return copy;
        }
    
        /**
         * Get the card to the left of a supplied card. Return null if the supplied card is at the leftmost 
         * position in the formation
         */
        public Card getLeftSideCard (Card card) {
            var playerCards = getPlayerCurrentMainCards(card.player);
            var column = card.formationColumn;
            if (column == 0) { // leftmost position
                return null;
            }
            else if (column <= 4 && column >= 1) { // just to be safe
                return playerCards[column - 1];
            }
            else {
                throw new Exception("Invalid card index");
            }
        }
    
        /**
         * Get the card to the right of a supplied card. Return null if the supplied card is at the rightmost 
         * position in the formation
         */
        public Card getRightSideCard (Card card) {
            var playerCards = getPlayerCurrentMainCards(card.player);
            var column = card.formationColumn;
            if (column == 4) { // rightmost position
                return null;
            }
            else if (column >= 0 && column <= 3) { // just to be safe
                return playerCards[column + 1];
            }
            else {
                throw new Exception("Invalid card index");
            }
        }
    
        /**
         * Get a card by its id
         */
        Card getCardById(int id) {
            return battle.allCardsById[id];
        }
    
        /**
         * Get all the current main cards of a player
         */
        public List<Card> getPlayerCurrentMainCards (Player player) {
            if (player == battle.player1) {
                return battle.p1_mainCards;
            }
            else if (player == battle.player2) {
                return battle.p2_mainCards;
            }
            else {
                throw new Exception("Invalid player");
            }
        }

        public List<Card> getPlayerCurrentReserveCards (Player player) {
            if (player == battle.player1) {
                return battle.p1_reserveCards;
            }
            else if (player == battle.player2) {
                return battle.p2_reserveCards;
            }
            else {
                throw new Exception("Invalid player");
            }
        }

        public List<Card> getPlayerAllCurrentCards (Player player) {
            return getPlayerCurrentMainCards(player).Concat(getPlayerCurrentReserveCards(player)).ToList();
        }

        public List<Card> getEnemyCurrentMainCards (Player player) {
            if (player == battle.player1) {
                return battle.p2_mainCards;
            }
            else if (player == battle.player2) {
                return battle.p1_mainCards;
            }
            else {
                throw new Exception("Invalid player");
            }
        }

        public Card getValidSingleTarget (List<Card> cards) {
            List<int> possibleIndices = new List<int>();
            for (var i = 0; i < 5; i++) {
                if (!cards[i].isDead) {
                    possibleIndices.Add(i);
                }
            }

            if (possibleIndices.Count == 0) {
                return null;
            }

            // get a random index from the list of possible indices
            int randomIndex = BBRandom.getRandomListItem(possibleIndices); 
            return cards[randomIndex];
        }
    
        public Card getNearestSingleOpponentTarget (Card executor) {
            List<Card> oppCards = getPlayerCurrentMainCards(battle.getOppositePlayer(executor.player));
            var executorIndex = executor.formationColumn;
        
            var offsetArray = new[] {0, -1, 1, -2, 2, -3, 3, -4, 4};
        
            for (var i = 0; i < offsetArray.Length; i++) {
                var index = executorIndex + offsetArray[i];
                if (index >= 0 && index < oppCards.Count) {
                    var currentOppCard = oppCards[index];
                    if (currentOppCard != null && !currentOppCard.isDead) {
                        return currentOppCard;
                    }
                }
            }
        
            // if it reaches this point, there's no target, so return null
            return null;
        }

        /**
         * Check if all cards of a player has died
         */
        public bool isAllDeadPlayer (Player player) {
            var reserveCond = true;
            if (battle.isBloodClash) {
                if (!isNoReserveLeft(player)) reserveCond = false;
            }
            return isAllMainCardsDead(player) && reserveCond;
        }

        /**
         * Check if all the current main cards of a player has died
         */
        private bool isAllMainCardsDead (Player player) {
            var mainCards = getPlayerCurrentMainCards(player);
            return mainCards.All(c => c.isDead);
        }

        /**
         * Check if a player has no reserve left. Use it only when battle is bloodclash.
         */
        private bool isNoReserveLeft (Player player) {
            var reserveCards = getPlayerCurrentReserveCards(player);
            return reserveCards.All(c => c == null);
        }
    
        /**
         * Return true if a card is in a list of cards, or false if not
         */
        public bool isCardInList(Card card, List<Card> list) {
            return list.Any(c => c.id == card.id);
        }

        /**
         * Return true if two cards are the same (same id)
         */
        public bool isSameCard(Card card1, Card card2) {
            return card1.id == card2.id;
        }

        /**
         * Use these when the order of the cards are unimportant
         */
        public List<Card> getAllCurrentMainCards() {
            return battle.allCurrentMainCards;
        }
        public List<Card> getAllCurrentCards() {
            var bt = battle;
            return bt.p1_mainCards.Concat(bt.p2_mainCards).Concat(bt.p1_reserveCards).Concat(bt.p2_reserveCards).ToList();
        }

        // remember to call this when there's a change in membership of p1_mainCards or p2_mainCards
        public void updateAllCurrentMainCards() {
            battle.allCurrentMainCards = battle.p1_mainCards.Concat(battle.p2_mainCards).ToList();
        }

        // use to switch a single card in the allCurrentMainCards with another card. This is used to update
        // allCurrentMainCards while still maintaining the current order (the order won't be updated until
        // the beginning of a game turn)
        public void switchCardInAllCurrentMainCards(Card oldCard, Card newCard) {
            var allCurrentMainCards = battle.allCurrentMainCards;
            var found = false;
            for (var i = 0; i < allCurrentMainCards.Count; i++) {
                if (allCurrentMainCards[i].id == oldCard.id) {
                    found = true;
                    allCurrentMainCards[i] = newCard;
                    break;
                }
            }

            if (!found) {
                throw new Exception("Card not found!");
            }
        }

        /**
         * Use this when the order of the cards are important: player 1 cards -> player 2 cards
         */
        public List<Card> getAllMainCardsInPlayerOrder() {
            return battle.p1_mainCards.Concat(battle.p2_mainCards).ToList();
        }

        public double getTotalHPRatio(List<Card> cards) {
            double totalRemainHp = 0;
            double totalOriginalHp = 0;
            foreach (var card in cards.Where(card => card != null)) {
                if (!card.isDead) {
                    totalRemainHp += card.getHP();
                }
                totalOriginalHp += card.originalStats.hp;
            }
            return totalRemainHp / totalOriginalHp;
        }
    }
}
