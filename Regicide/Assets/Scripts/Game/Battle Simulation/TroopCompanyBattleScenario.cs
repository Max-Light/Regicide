
using Regicide.Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopCompanyBattleScenario : BattleScenario
    {
        public TroopCompanyBattleScenario(TroopBattleLineFormation battleLineFormation1, TroopBattleLineFormation battleLineFormation2) : base(battleLineFormation1, battleLineFormation2)
        {
            _battleCycleTimeLength = 3f;
        }

        public override void UpdateBattleInflictions()
        {
            BalanceBattleLine(
                out IReadOnlyList<IDamager> battleLineDamagers_1, out IReadOnlyList<IDamageable> battleLineDamageables_1, 
                out IReadOnlyList<IDamager> battleLineDamagers_2, out IReadOnlyList<IDamageable> battleLineDamageables_2, 
                out int battleLineTroopAmount
                );
            ExchangeBattleInflictions(battleLineDamagers_1, battleLineDamageables_1, battleLineDamagers_2, battleLineDamageables_2, battleLineTroopAmount);
        }

        private void BalanceBattleLine(out IReadOnlyList<IDamager> battleLineDamagers_1, out IReadOnlyList<IDamageable> battleLineDamageables_1, out IReadOnlyList<IDamager> battleLineDamagers_2, out IReadOnlyList<IDamageable> battleLineDamageables_2, out int battleLineTroopAmount)
        {
            battleLineDamagers_1 = _battleLine_1.GetDamagers(this);
            battleLineDamagers_2 = _battleLine_2.GetDamagers(this);
            battleLineDamageables_1 = _battleLine_1.GetDamageables(this);
            battleLineDamageables_2 = _battleLine_2.GetDamageables(this);

            battleLineTroopAmount = Mathf.Min(battleLineDamagers_1.Count, battleLineDamagers_2.Count);
            BalanceSelectedBattleLine(ref battleLineDamagers_1, ref battleLineDamageables_1, battleLineTroopAmount);
            BalanceSelectedBattleLine(ref battleLineDamagers_2, ref battleLineDamageables_2, battleLineTroopAmount);
        }

        private void BalanceSelectedBattleLine(ref IReadOnlyList<IDamager> damagers, ref IReadOnlyList<IDamageable> damageables, int amountToChoose)
        {
            if (damagers.Count > amountToChoose && amountToChoose > 0)
            {
                int commonTroopAmountPerSlot = damagers.Count / amountToChoose;
                int remainingTroopAmount = damagers.Count % amountToChoose;
                int startIndex = 0;
                List<IDamager> chosenBattleLineDamagers = new List<IDamager>();
                List<IDamageable> chosenBattleLineDamageables = new List<IDamageable>();
                for (int troopIndex = 0; troopIndex < amountToChoose; troopIndex++)
                {
                    int chooseTroopRange = commonTroopAmountPerSlot;
                    if (remainingTroopAmount > 0)
                    {
                        chooseTroopRange++;
                        remainingTroopAmount--;
                    }
                    int chosenTroopIndex = Random.Range(startIndex, chooseTroopRange + 1);
                    startIndex += chooseTroopRange;
                    chosenBattleLineDamagers.Add(damagers[chosenTroopIndex]);
                    chosenBattleLineDamageables.Add(damageables[chosenTroopIndex]);
                }
                damagers = chosenBattleLineDamagers;
                damageables = chosenBattleLineDamageables;
            }
        }

        private void ExchangeBattleInflictions(IReadOnlyList<IDamager> battleLineDamagers_1, IReadOnlyList<IDamageable> battleLineDamageables_1, IReadOnlyList<IDamager> battleLineDamagers_2, IReadOnlyList<IDamageable> battleLineDamageables_2, int inflictionAmount)
        {
            for (int inflictionIndex = inflictionAmount - 1; inflictionIndex >= 0; inflictionIndex--)
            {
                DamageReport battleLineDamageReport_1 = battleLineDamagers_1[inflictionIndex].DamageReport;
                DamageReport battleLineDamageReport_2 = battleLineDamagers_2[inflictionIndex].DamageReport;

                battleLineDamageables_1[inflictionIndex].ReceiveDamage(battleLineDamageReport_2);
                battleLineDamageables_2[inflictionIndex].ReceiveDamage(battleLineDamageReport_1);
            }
        }
    }
}