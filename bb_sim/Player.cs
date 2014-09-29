namespace bb_sim {
    class Player {
        public readonly int id;
        public readonly string name;
        public readonly Formation formation;
        double multiplier;

        /**
         * @param int id The player id. Usually 1 means the player/me and 2 means the opponent/opposing side
         * @param string name The name of the player
         * @param Formation formation The formation used by the player
         * @param double multiplier Any multiplier the player has, either by all out attack or by title
         */

        public Player(int id, string name, Formation formation, double multiplier) {
            this.id = id;
            this.name = name;
            this.formation = formation;
            this.multiplier = multiplier;
        }
    }
}
