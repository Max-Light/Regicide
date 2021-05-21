
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopCompanyBattleScenario : BattleScenario
    {
        private TroopUnitDamage _troopDamageReport_1 = new TroopUnitDamage();
        private TroopUnitDamage _troopDamageReport_2 = new TroopUnitDamage();


        public TroopCompanyBattleScenario(TroopBattleLineFormation battleLineFormation1, TroopBattleLineFormation battleLineFormation2) : base(battleLineFormation1, battleLineFormation2)
        {
            _battleCycleTimeLength = 3;
        }

        public override void UpdateBattleInflictions()
        {
            IDamager[] battleLine1Damagers = _battleLine_1.GetDamagers(this);
            IDamager[] battleLine2Damagers = _battleLine_2.GetDamagers(this);

            IDamageable[] battleLine1Damageables = _battleLine_1.GetDamageables(this);
            IDamageable[] battleLine2Damageables = _battleLine_2.GetDamageables(this);

            if (battleLine1Damagers.Length == battleLine2Damagers.Length)
            {
                Debug.Log("Updating battle");
                ExchangeProportionalBattleInflictions(battleLine1Damagers, battleLine2Damagers, battleLine1Damageables, battleLine2Damageables);
            }
            else if (battleLine1Damagers.Length > battleLine2Damagers.Length)
            {
                Debug.Log("Updating unproportional battle");
                ExchangeUnproportionalBattleInflictions(battleLine1Damagers, battleLine2Damagers, battleLine1Damageables, battleLine2Damageables);
            }
            else
            {
                Debug.Log("Updating unproportional battle");
                ExchangeUnproportionalBattleInflictions(battleLine2Damagers, battleLine1Damagers, battleLine2Damageables, battleLine1Damageables);
            }
        }

        private void ExchangeProportionalBattleInflictions(IDamager[] battleLine1Damagers, IDamager[] battleLine2Damagers, IDamageable[] battleLine1Damageables, IDamageable[] battleLine2Damageables)
        {
            for (int troopIndex = 0; troopIndex < battleLine1Damagers.Length; troopIndex++)
            {
                battleLine1Damagers[troopIndex].PopulateDamageReport(_troopDamageReport_1);
                battleLine2Damagers[troopIndex].PopulateDamageReport(_troopDamageReport_2);

                battleLine1Damageables[troopIndex].ReceiveDamage(_troopDamageReport_2);
                battleLine2Damageables[troopIndex].ReceiveDamage(_troopDamageReport_1);
            }
        }

        private void ExchangeUnproportionalBattleInflictions(IDamager[] largeBattleLineDamagers, IDamager[] smallBattleLineDamagers, IDamageable[] largeBattleLineDamageables, IDamageable[] smallBattleLineDamageables)
        {
            int commonTroopAmountPerSlot = largeBattleLineDamagers.Length / smallBattleLineDamagers.Length;
            int remainingTroopAmount = largeBattleLineDamagers.Length % smallBattleLineDamagers.Length;

            for (int smallBattleLineTroopIndex = smallBattleLineDamagers.Length - 1; smallBattleLineTroopIndex >= 0; smallBattleLineTroopIndex--)
            {
                int troopCombatRange = commonTroopAmountPerSlot;
                if (remainingTroopAmount > 0)
                {
                    troopCombatRange++;
                    remainingTroopAmount--;
                }
                int chosenTroopFromLargeBattleLine = Random.Range((smallBattleLineTroopIndex + 1) - troopCombatRange, smallBattleLineTroopIndex + 1);

                largeBattleLineDamagers[chosenTroopFromLargeBattleLine].PopulateDamageReport(_troopDamageReport_1);
                smallBattleLineDamagers[smallBattleLineTroopIndex].PopulateDamageReport(_troopDamageReport_2);

                largeBattleLineDamageables[chosenTroopFromLargeBattleLine].ReceiveDamage(_troopDamageReport_2);
                smallBattleLineDamageables[smallBattleLineTroopIndex].ReceiveDamage(_troopDamageReport_1);
            }
        }
    }
}