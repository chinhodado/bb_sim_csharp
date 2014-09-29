using System;
using System.Collections.Generic;
using System.Linq;

namespace bb_sim {
    class BattleModel {

        const int MAX_TURN_NUM = 5;
        RandomBrigType p1RandomMode;
        RandomBrigType p2RandomMode;
        public readonly bool isBloodClash;
        public readonly ProcOrderType procOrderType = ProcOrderType.ANDROID;

        readonly CardManager cardManager;

        public readonly Player player1;
        public readonly Player player2;

        bool isFinished;
        Player playerWon;
    
        // The two players' main cards. The order of the cards in these two arrays should never be changed.
        // When a reserve comes out, replace the main card in here with the reserve
        public readonly List<Card> p1_mainCards = new List<Card>();
        public readonly List<Card> p2_mainCards = new List<Card>();

        // The two player's reserve cards. When a reserve comes out, move it to the main cards (i.e. delete it here)
        public readonly List<Card> p1_reserveCards = new List<Card>();
        public readonly List<Card> p2_reserveCards = new List<Card>();

        // The original main cards. Should be created once and never modified
        public readonly List<Card> p1_originalMainCards = new List<Card>();
        public readonly List<Card> p2_originalMainCards = new List<Card>();

        // The original reserve cards. Should be created once and never modified
        public readonly List<Card> p1_originalReserveCards = new List<Card>();
        public readonly List<Card> p2_originalReserveCards = new List<Card>();
    
        // contains all cards in play. Should be re-created and re-sorted every turn, and updated when either player's main cards changed.
        public List<Card> allCurrentMainCards = new List<Card>();

        // Contains all cards in play (both main and reserve of both players). Used for quickly get a card by its id
        public readonly Dictionary<int, Card> allCardsById = new Dictionary<int,Card>();

        // store recently dead cards with ondeath skills waiting to proc
        List<Card> onDeathCards = new List<Card>();

        // turn order info
        public BattleTurnOrderType turnOrderBase = BattleTurnOrderType.AGI;
        public int turnOrderChangeEffectiveTurns;
        public bool turnOrderChanged;
    
        // Turn-dependent. Remember to update these when it's a new card's turn. Maybe move to a separate structure?
        Player currentPlayer;
        Player oppositePlayer;

        // just a copy of p1_mainCards, p2_mainCards, p1_reserveCards, p2_reserveCards, turn-dependent
        List<Card> currentPlayerMainCards;
        List<Card> currentPlayerReserveCards;
        public List<Card> oppositePlayerMainCards;
        List<Card> oppositePlayerReserveCards;

        private int currentTurn;

        private static BattleModel _instance;

        public static BattleModel getInstance()  {
            if (_instance == null) {
                throw new Exception("Error: you should not make this object this way");
            }
            return _instance;
        }

        public BattleModel (GameData data, GameOption option, string tierListString) {
    
            if (_instance != null) {
                throw new Exception("Error: Instantiation failed: Use getInstance() instead of new.");
            }
            _instance = this;
            cardManager = CardManager.getInstance();

            procOrderType = option.procOrder;
            if (option.battleType == BattleType.BLOOD_CLASH) {
                isBloodClash = true;
            }
        
            FormationType p1_formation;
            FormationType p2_formation;
            List<int> p1_cardIds;
            List<int> p2_cardIds;
            List<int> p1_warlordSkillIds = new List<int>();
            List<int> p2_warlordSkillIds;

            List<int> availableSkills = SkillDatabase.getAvailableSkillsForSelect();

            //if (option.p1RandomMode) {
            //    p1RandomMode = option.p1RandomMode;
            //    p1_formation = pickRandomProperty(Formation.FORMATION_CONFIG);
            //    p1_cardIds = BrigGenerator.getBrig(option.p1RandomMode, tierListString, isBloodClash);

            //    for (var i = 0; i < 3; i++) {
            //        p1_warlordSkillIds.Add(+getRandomElement(availableSkills));
            //    }
            //}
            //else {
                p1_formation = data.p1_formation;
                p1_cardIds = data.p1_cardIds;
                p1_warlordSkillIds = data.p1_warlordSkillIds;
            //}

            //if (option.p2RandomMode) {
            //    p2RandomMode = option.p2RandomMode;
            //    p2_formation = pickRandomProperty(Formation.FORMATION_CONFIG);
            //    p2_cardIds = BrigGenerator.getBrig(option.p2RandomMode, tierListString, isBloodClash);

            //    for (var i = 0; i < 3; i++) {
            //        p2_warlordSkillIds.push(+getRandomElement(availableSkills));
            //    }
            //}
            //else {
                p2_formation = data.p2_formation;
                p2_cardIds = data.p2_cardIds;
                p2_warlordSkillIds = data.p2_warlordSkillIds;
            //}
        
            player1 = new Player(1, "Player 1", new Formation(p1_formation), 1); // me
            player2 = new Player(2, "Player 2", new Formation(p2_formation), 1); // opp
        
            // create the cards        
            for (var i = 0; i < 10; i++) {

                if (i >= 5 && !isBloodClash) break;
                var p1_cardInfo = FamDatabase.get(p1_cardIds[i]);
                var p2_cardInfo = FamDatabase.get(p2_cardIds[i]);

                // make the skill array for the current fam
                List<int> p1fSkillIdArray = p1_cardInfo.skills;
                if (p1_cardInfo.isWarlord) {
                    p1fSkillIdArray = p1_warlordSkillIds;
                }

                List<int> p2fSkillIdArray = p2_cardInfo.skills;
                if (p2_cardInfo.isWarlord) {
                    p2fSkillIdArray = p2_warlordSkillIds;
                }

                var player1Skills = makeSkillArray(p1fSkillIdArray);
                var player2Skills = makeSkillArray(p2fSkillIdArray);
            
                // now make the cards and add them to the appropriate collections
                var card1 = new Card(p1_cardIds[i], player1, i, player1Skills);
                var card2 = new Card(p2_cardIds[i], player2, i, player2Skills);

                if (i < 5) {
                    p1_mainCards[i] = card1;
                    p2_mainCards[i] = card2;

                    p1_originalMainCards[i] = card1;
                    p2_originalMainCards[i] = card2;

                    allCurrentMainCards.Add(card1);
                    allCurrentMainCards.Add(card2);
                }
                else if (i >= 5 && isBloodClash) {
                    p1_reserveCards[i % 5] = card1;
                    p2_reserveCards[i % 5] = card2;

                    p1_originalReserveCards[i % 5] = card1;
                    p2_originalReserveCards[i % 5] = card2;
                }

                allCardsById[card1.id] = card1;
                allCardsById[card2.id] = card2;
            }

            cardManager.sortAllCurrentMainCards();
        }

        /**
         * Resets everything
         * Used for testing only
         */
        static void resetAll() {
            removeInstance();
            CardManager.removeInstance();
        }

        /**
         * Allows to create a new instance
         * Used for testing only
         */
        static void removeInstance() {
            _instance = null;
        }
    
        /**
         * Given an array of skill ids, return an array of Skills
         */
        List<Skill> makeSkillArray (List<int> skills) {
            return skills.Select(t => new Skill(t)).ToList();
        }

        public Player getOppositePlayer (Player player) {
            if (player == player1) {
                return player2;
            }
            else if (player == player2) {
                return player1;
            }
            else {
                throw new Exception("Invalid player");
            }
        }

        /**
         * Use this for damage because of attacks
         */
        public void processDamagePhase(Card attacker, Card target, Skill skill, double scaledRatio = 1) {
            var damage = getWouldBeDamage(attacker, target, skill, scaledRatio);
        
            bool isMissed = attacker.willMiss();
            if (isMissed) {
                damage = 0;
                attacker.justMissed = true;
            }
            else {
                attacker.justMissed = false;
            }

            bool evaded = target.justEvaded;
            if (evaded) {
                damage = 0;
            }

            bool isKilled = false;
            if (!isMissed && !evaded && skill.skillFunc == SkillFunc.KILL) {
                if (BBRandom.get() <= skill.skillFuncArg2) { // probability check
                    isKilled = true;
                }
            }
            if (isKilled) {
                damage = target.getHP() + target.status.hpShield;
            }
        
            // HP shield skill
            var hpShield = target.status.hpShield;
            if (hpShield > 0 && !isMissed && !evaded && !isKilled) {
                if (damage >= hpShield) {
                    target.status.hpShield = 0;
                    damage -= hpShield;
                } else {
                    target.status.hpShield = hpShield - damage;
                    damage = 0;
                }
            }
        
            // survive
            var surviveSkill = target.getSurviveSkill();
            SkillLogicData defenseData  = new SkillLogicData {
                executor = target,
                skill = surviveSkill,
                attacker = attacker,
                wouldBeDamage = damage
            };
        
            if (surviveSkill != null && surviveSkill.willBeExecuted(defenseData) && !isKilled && !isMissed && !evaded) {
                surviveSkill.execute(defenseData);
                damage = target.getHP() - 1;
            }     
    
            target.changeHP(-1 * damage);
            target.lastBattleDamageTaken = damage;
            attacker.lastBattleDamageDealt = damage;

            if (target.isDead) {
                addOnDeathCard(target);
            }
        }

        private double getWouldBeDamage(Card attacker, Card target, Skill skill, double scaledRatio = 1) {
            var skillMod = skill.skillFuncArg1;
            var ignorePosition = Skill.isPositionIndependentAttackSkill(skill.id);
    
            double baseDamage = 0;
            
            switch (skill.skillCalcType) {
                case (SkillCalcType.DEFAULT):
                case (SkillCalcType.WIS):
                    baseDamage = SkillCalc.getDamageCalculatedByWIS(attacker, target);
                    break;
                case (SkillCalcType.ATK):
                    baseDamage = SkillCalc.getDamageCalculatedByATK(attacker, target, ignorePosition);
                    break;
                case (SkillCalcType.AGI):
                    baseDamage = SkillCalc.getDamageCalculatedByAGI(attacker, target, ignorePosition);
                    break;
            }
            
            // apply the multiplier
            var damage = skillMod * baseDamage;

            damage *= scaledRatio;
            
            // apply the target's ward
            switch (skill.ward) {
                case WardType.PHYSICAL:
                    damage = Math.Round(damage * (1 - target.status.attackResistance));
                    break;
                case WardType.MAGICAL:
                    damage = Math.Round(damage * (1 - target.status.magicResistance));
                    break;
                case WardType.BREATH:
                    damage = Math.Round(damage * (1 - target.status.breathResistance));
                    break;
                default :
                    throw new Exception("Wrong type of ward. Maybe you forgot to include in the skill?");
            }

            return damage;
        }

        // todo: move this to Card?
        // use this when there's no executorId for the MinorEvent, like for poison. Also use it for non-attacks like healing, etc.
        public void damageToTargetDirectly(Card target, double damage) {
            target.changeHP(-1 * damage);

            if (target.isDead) {
                addOnDeathCard(target);
            }
        }

        public void processAffliction(Card executor, Card target, Skill skill) {
            AfflictionType type = (AfflictionType) skill.skillFuncArg2;
            double prob = skill.skillFuncArg3;

            if (type == AfflictionType.NO_AFFLICTION) {
                return;
            }

            AfflectOptParam option = new AfflectOptParam();

            if (skill.skillFuncArg4 != 0) {
                if (type == AfflictionType.POISON) {
                    // envenom percent
                    option.percent = skill.skillFuncArg4;
                }
                else {
                    // turn num for silent & blind
                    option.turnNum = (int) skill.skillFuncArg4;
                }
            }

            if (skill.skillFuncArg5 != 0) {
                option.missProb = skill.skillFuncArg5;
            }
            
            if (BBRandom.get() <= prob){
                target.setAffliction(type, option);
            }
        }
   
        public void processDebuff(Card executor, Card target, Skill skill) {
            StatusType status; 
            double multi;
            bool isNewLogic = false, isFlat = false; // for caster-based debuff

            if (skill.skillFunc == SkillFunc.DEBUFFATTACK || skill.skillFunc == SkillFunc.DEBUFFINDIRECT) {
                status = (StatusType) skill.skillFuncArg2;
                multi  = skill.skillFuncArg4;
            }
            else if (skill.skillFunc == SkillFunc.DEBUFF) {
                status = (StatusType) skill.skillFuncArg2;
                multi  = skill.skillFuncArg1;
            }
            else if (skill.skillFunc == SkillFunc.CASTER_BASED_DEBUFF) {
                // todo: arg3 may also be status
                status = (StatusType) skill.skillFuncArg2;
                multi = skill.skillFuncArg1;
                isNewLogic = true;
            }
            else if (skill.skillFunc == SkillFunc.CASTER_BASED_DEBUFF_ATTACK || skill.skillFunc == SkillFunc.CASTER_BASED_DEBUFF_MAGIC) {
                status = (StatusType) skill.skillFuncArg2;
                multi = skill.skillFuncArg4;
                isNewLogic = true;
            }
            else if (skill.skillFunc == SkillFunc.ONHIT_DEBUFF) {
                // todo: arg3 may also be status
                status = (StatusType) skill.skillFuncArg2;
                multi = skill.skillFuncArg1;
                isNewLogic = true;

                if (skill.skillFuncArg4 != 0) {
                    multi = skill.skillFuncArg4;
                    isFlat = true;
                }
            }
            else {
                throw new Exception("Wrong skill to use with processDebuff()");
            }

            double baseAmount;
            if (isFlat) {
                baseAmount = -100;
            }
            else if (!isNewLogic) {
                baseAmount = SkillCalc.getDebuffAmount(executor, target);
            }
            else {
                baseAmount = SkillCalc.getCasterBasedDebuffAmount(executor);
            }

            var amount = Math.Floor(baseAmount * multi);

            target.changeStatus(status, amount, isNewLogic);
        }

        public int startBattle () {
       
            performOpeningSkills();
        
            while (!isFinished) {
                currentTurn++;

                // process turn order change
                if (turnOrderChangeEffectiveTurns == 0) {
                    turnOrderBase = BattleTurnOrderType.AGI;
                }
                else {
                    turnOrderChangeEffectiveTurns--;
                }

                cardManager.updateAllCurrentMainCards();
                cardManager.sortAllCurrentMainCards();

                // assuming both have 5 cards
                for (var i = 0; i < 10 && !isFinished; i++) {
                    var currentCard = allCurrentMainCards[i];

                    currentPlayer = currentCard.player;
                    currentPlayerMainCards = cardManager.getPlayerCurrentMainCards(currentPlayer);
                    currentPlayerReserveCards = cardManager.getPlayerCurrentReserveCards(currentPlayer);

                    oppositePlayer = getOppositePlayer(currentPlayer);
                    oppositePlayerMainCards = cardManager.getPlayerCurrentMainCards(oppositePlayer);
                    oppositePlayerReserveCards = cardManager.getPlayerCurrentReserveCards(oppositePlayer);

                    if (currentCard.isDead) {
                        var column = currentCard.formationColumn;
                        if (isBloodClash && currentPlayerReserveCards[column] != null) {
                            // swich in reserve
                            var reserveCard = currentPlayerReserveCards[column];

                            cardManager.switchCardInAllCurrentMainCards(currentCard, reserveCard);
                            currentPlayerMainCards[column] = reserveCard;
                            currentPlayerReserveCards[column] = null;

                            currentCard = reserveCard;

                            // proc opening skill when switch in
                            var openingSkill = currentCard.getRandomOpeningSkill();
                            if (openingSkill != null) {
                                SkillLogicData data = new SkillLogicData {
                                    executor = currentCard,
                                    skill = openingSkill
                                };
                                if (openingSkill.willBeExecuted(data)) {
                                    openingSkill.execute(data);
                                }
                            }
                        }
                        else {
                            continue;
                        }
                    }

                    // procs active skill if we can
                    processActivePhase(currentCard, "FIRST");                
                    if (isFinished) break;

                    if (!currentCard.isDead && currentCard.status.willAttackAgain != 0) {
                        processActivePhase(currentCard, "FIRST");
                        // todo: send a minor event log and handle it
                        currentCard.status.willAttackAgain = 0;
                        if (isFinished) break;
                    }                

                    // todo: make a major event if a fam missed a turn
                    if (!currentCard.isDead) {
                        currentCard.updateAffliction();
                        processOnDeathPhase();
                    }

                    checkFinish();
                }

                if (!isFinished) {
                    processEndTurn();
                }
            }
            return playerWon.id;
        }

        private void addOnDeathCard(Card card) {
            if (card.hasOnDeathSkill()) {
                onDeathCards.Add(card);
            }
        }

        private void checkFinish() {
            var noOnDeathRemain = onDeathCards.Count == 0;
            if (cardManager.isAllDeadPlayer(oppositePlayer) && noOnDeathRemain) {
                playerWon = currentPlayer;
            }
            else if (cardManager.isAllDeadPlayer(currentPlayer) && noOnDeathRemain) {
                playerWon = oppositePlayer;
            }

            if (playerWon != null) {
                isFinished = true;
            }
        }

        // return true if battle has ended, false if not
        private void processActivePhase(Card currentCard, string nth) {
            var activeSkill = currentCard.getRandomActiveSkill();
        
            if (nth == "FIRST" && currentCard.isMounted) {
                activeSkill = currentCard.getFirstActiveSkill();
            }
            else if (nth == "SECOND" && currentCard.isMounted) {
                activeSkill = currentCard.getSecondActiveSkill();
            }
        
            if (activeSkill != null) {
                SkillLogicData data = new SkillLogicData {
                    executor = currentCard,
                    skill = activeSkill
                };
                if (activeSkill.willBeExecuted(data)) {
                    activeSkill.execute(data);
                }
                else {
                    executeNormalAttack(currentCard);
                }
            }
            else {
                executeNormalAttack(currentCard);
            }

            processOnDeathPhase();

            checkFinish();
            if (isFinished) {
                return;
            }
            else if (nth == "FIRST" && currentCard.isMounted && !currentCard.isDead) {
                processActivePhase(currentCard, "SECOND");
            }
        }

        private void processOnDeathPhase() {
            // make a copy
            List<Card> hasOnDeath = onDeathCards.ToList();

            onDeathCards = new List<Card>();

            foreach (var card in hasOnDeath) {
                var skill = card.getInherentOnDeathSkill();
                SkillLogicData data = new SkillLogicData {
                    executor = card,
                    skill = skill
                };
                if (skill != null && skill.willBeExecuted(data)) {
                    skill.execute(data);
                }

                skill = card.getBuffOnDeathSkill();
                data = new SkillLogicData {
                    executor = card,
                    skill = skill
                };
                // clear it
                card.clearBuffOnDeathSkill();
                if (skill != null && skill.willBeExecuted(data)) {
                    skill.execute(data);
                }
            }

            // at the end, if there are newly addition to recentlyDeadCards, recursively repeat the process 
            if (onDeathCards.Count != 0) {
                processOnDeathPhase();
            }
        }
    
        /**
         * Called at the end of two player's turn
         */
        private void processEndTurn() {
            if (currentTurn >= MAX_TURN_NUM) {
         
                var p1Cards = cardManager.getPlayerAllCurrentCards(player1);
                var p2Cards = cardManager.getPlayerAllCurrentCards(player2);
                
                var p1Ratio = cardManager.getTotalHPRatio(p1Cards);
                var p2Ratio = cardManager.getTotalHPRatio(p2Cards);
                
                playerWon = p1Ratio >= p2Ratio ? player1 : player2;
                isFinished = true;
            }
            else if (isBloodClash) {
                // add skill probability to those still alive
                var allCards = cardManager.getAllCurrentCards();
                foreach (var tmpCard in allCards) {
                    if (tmpCard != null && !tmpCard.isDead) {
                        tmpCard.bcAddedProb += 10;
                    }
                }
            }
        }

        private void executeNormalAttack(Card attacker) {

            if (!attacker.canAttack()) {
                return;
            }

            attacker.autoAttack.execute(new SkillLogicData {
                executor = attacker,
                skill = attacker.autoAttack
            });
        }

        /**
         * Process the protecting sequence. Return true if a protect has been executed
         * or false if no protect has been executed
         *
         * @param targetsAttacked optional, set to null when multiple protect/hit is allowed
         */
        public bool processProtect(Card attacker, Card targetCard, Skill attackSkill, Dictionary<int, bool> targetsAttacked, double scaledRatio = 1) {
            // now check if someone on the enemy side can protect before the damage is dealt
            var enemyCards = cardManager.getEnemyCurrentMainCards(attacker.player);
            var protectSkillActivated = false; //<- has any protect skill been activated yet?
            for (var i = 0; i < enemyCards.Count && !protectSkillActivated; i++) {
                if (enemyCards[i].isDead) {
                    continue;
                }
                var protectSkill = enemyCards[i].getRandomProtectSkill();
                if (protectSkill != null) {
                    var protector = enemyCards[i];

                    // if a fam that has been attacked is not allowed to protect (like in the case of AoE), continue
                    if (targetsAttacked != null && targetsAttacked.ContainsKey(protector.id) && targetsAttacked[protector.id]) {
                        continue;
                    }

                    SkillLogicData protectData = new SkillLogicData {
                        executor = protector,
                        skill = protectSkill,
                        attacker = attacker,    // for protect
                        attackSkill = attackSkill, // for protect
                        targetCard = targetCard,  // for protect
                        targetsAttacked = targetsAttacked,  // for protect
                        scaledRatio = scaledRatio
                    };

                    if (protectSkill.willBeExecuted(protectData)) {
                        protectSkillActivated = true;
                        protectSkill.execute(protectData);
                    }
                }
                else {
                    // this fam doesn't have a protect skill, move on to the next one
                    continue;
                }
            }
            return protectSkillActivated;
        }

        private void performOpeningSkills () {
            // the cards sorted by proc order
            var p1cards = cardManager.getPlayerCurrentMainCardsByProcOrder(player1);
            var p2cards = cardManager.getPlayerCurrentMainCardsByProcOrder(player2);

            foreach (Card c in p1cards) {
                var skill1 = c.getRandomOpeningSkill();
                if (skill1 != null) {
                    SkillLogicData data = new SkillLogicData {
                        executor = c,
                        skill = skill1
                    };
                    if (skill1.willBeExecuted(data)) {
                        skill1.execute(data);
                    }
                }
            }
        
            foreach (Card c in p2cards) {
                var skill2 = c.getRandomOpeningSkill();
                if (skill2 != null) {
                    SkillLogicData data =  new SkillLogicData {
                        executor = c,
                        skill = skill2
                    };
                    if (skill2.willBeExecuted(data)) {
                        skill2.execute(data);
                    }
                }
            }

            // reset the turn order changed flag at the end of opening phase
            turnOrderChanged = false;
        }
    }

    /**
     * A structure for holding a battle's data
     */
    class GameData {
        public FormationType p1_formation;
        public FormationType p2_formation;
        public List<int> p1_cardIds;
        public List<int> p2_cardIds;
        public List<int> p1_warlordSkillIds;
        public List<int> p2_warlordSkillIds;
    }

    /**
     * A structure for holding a battle's option
     */
    struct GameOption {
        public RandomBrigType p1RandomMode;
        public RandomBrigType p2RandomMode;
        public ProcOrderType procOrder;
        public BattleType battleType;
    }
}
