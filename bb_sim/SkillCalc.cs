using System;
using System.Collections.Generic;

namespace bb_sim {
    static class SkillCalc {

        public static double getDamageCalculatedByATK(Card attacker, Card defender, bool ignorePosition) {
            const double ATTACK_FACTOR = 0.3;
            const double DIFF_FACTOR = 0.2;

            var POS_ATTACK_FACTOR = new Dictionary<FormationRow, double> {
                {FormationRow.REAR, 0.8},
                {FormationRow.MID, 1},
                {FormationRow.FRONT, 1.2}
            };

            var POS_DAMAGE_FACTOR = new Dictionary<FormationRow, double> {
                {FormationRow.REAR, 0.8},
                {FormationRow.MID, 1},
                {FormationRow.FRONT, 1.2}
            };
    
            var baseDamage = attacker.getATK() * ATTACK_FACTOR;
            var damage = ((attacker.getATK() - defender.getDEF()) * DIFF_FACTOR) + baseDamage;
    
            if (!ignorePosition) {
                damage *= POS_ATTACK_FACTOR[attacker.getFormationRow()];
                damage *= POS_DAMAGE_FACTOR[defender.getFormationRow()];
            }
    
            //set lower limit
            if (damage < baseDamage * 0.1) {
                damage = baseDamage * 0.1;
            }
    
            damage = Math.Floor(damage * BBRandom.get(0.9, 1.1));
    
            return damage;
        }

        public static double getDamageCalculatedByAGI(Card attacker, Card defender, bool ignorePosition) {
            const double ATTACK_FACTOR = 0.3;
            const double DIFF_FACTOR = 0.2;

            var POS_ATTACK_FACTOR = new Dictionary<FormationRow, double> {
                {FormationRow.REAR, 0.8},
                {FormationRow.MID, 1},
                {FormationRow.FRONT, 1.2}
            };

            var POS_DAMAGE_FACTOR = new Dictionary<FormationRow, double> {
                {FormationRow.REAR, 0.8},
                {FormationRow.MID, 1},
                {FormationRow.FRONT, 1.2}
            };
    
            var baseDamage = attacker.getAGI() * ATTACK_FACTOR;
            var damage = ((attacker.getAGI() - defender.getDEF()) * DIFF_FACTOR) + baseDamage;
    
            if (!ignorePosition) {
                damage *= POS_ATTACK_FACTOR[attacker.getFormationRow()];
                damage *= POS_DAMAGE_FACTOR[defender.getFormationRow()];
            }
    
            //set lower limit
            if (damage < baseDamage * 0.1) {
                damage = baseDamage * 0.1;
            }

            damage = Math.Floor(damage * BBRandom.get(0.9, 1.1));
    
            return damage;
        }

        public static double getDamageCalculatedByWIS(Card attacker, Card defender) {
            const double ATTACK_FACTOR = 0.3;
            const double WIS_DEF_FACTOR = 0.5;
            const double DIFF_FACTOR = 0.2;
 
            var baseDamage = attacker.getWIS() * ATTACK_FACTOR;
            var targetWisDef = (defender.getWIS() + defender.getDEF()) * WIS_DEF_FACTOR;
            var damage = ((attacker.getWIS() - targetWisDef) * DIFF_FACTOR) + baseDamage;

            //set lower limit
            if (damage < baseDamage * 0.1) {
                damage = baseDamage * 0.1;
            }

            damage = Math.Floor(damage * BBRandom.get(0.9, 1.1));

            return damage;
        }

        public static double getHealAmount(Card executor) {
            const double HEAL_FACTOR = 0.3;
            var amount = executor.getWIS() * HEAL_FACTOR;

            amount = Math.Floor(amount * BBRandom.get(0.9, 1.1));

            return amount;
        }

        public static double getDebuffAmount(Card executor, Card target) {
            const double FACTOR = 1.0;

            var value = (executor.getWIS() - target.getWIS()) * FACTOR;
            var min = executor.getWIS() * 0.1;

            if (value < min) {
                value = min;
            }

            return -1 * value;
        }

        public static double getCasterBasedDebuffAmount(Card executor) {
            const double FACTOR = 1.2;

            var value = executor.getWIS() * FACTOR;

            return -1 * value;
        }
    }
}
