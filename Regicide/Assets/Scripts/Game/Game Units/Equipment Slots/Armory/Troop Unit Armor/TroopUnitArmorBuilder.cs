

using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Units
{
    public class TroopUnitArmorBuilder 
    {
        private struct ArmorOption 
        {
            public uint ArmorId { get; private set; }
            public float PercentageAmount { get; private set; }

            public ArmorOption(uint armorId, float percentageScale)
            {
                ArmorId = armorId;
                PercentageAmount = percentageScale;
            }
        }

        private List<ArmorOption> _armorOptions = new List<ArmorOption>();
        private float _totalPercentageAmount = 0;

        public TroopUnitArmorBuilder AddOption(uint armorId, float percentageScale)
        {
            if (percentageScale > 0)
            {
                _totalPercentageAmount += percentageScale;
                _armorOptions.Add(new ArmorOption(armorId, percentageScale));
            }
            return this;
        }

        public TroopUnitArmor Build(uint armorId)
        {
            return UnitArmorFactory.GetArmor(armorId);
        }

        public static implicit operator TroopUnitArmor(TroopUnitArmorBuilder builder)
        {
            return builder.Build(builder.ChooseArmorOption());
        }

        private uint ChooseArmorOption()
        {
            float chosenPercentageScale = Random.Range(0, _totalPercentageAmount);
            float accumulatedAmount = 0;
            for (int optionIndex = 0; optionIndex < _armorOptions.Count; optionIndex++)
            {
                accumulatedAmount += _armorOptions[optionIndex].PercentageAmount;
                if (chosenPercentageScale <= accumulatedAmount)
                {
                    return _armorOptions[optionIndex].ArmorId;
                }
            }
            Debug.LogError("Could not successfully choose an armor option");
            return 0;
        }
    }
}