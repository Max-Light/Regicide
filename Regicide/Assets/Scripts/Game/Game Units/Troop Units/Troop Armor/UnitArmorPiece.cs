using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class UnitArmorPiece : IUnitArmor
    {
        public class Model
        {
            private float _damageReduction = 0;

            public uint ArmorId { get; private set; } = 0;
            public string Armor { get; private set; } = "";
            public string Description { get; private set; } = "";
            public Sprite Sprite { get; private set; } = null;
            public float DamageRduction
            {
                get => _damageReduction;
                set
                {
                    _damageReduction = Mathf.Clamp01(value);
                }
            }
            public float Weigth { get; private set; } = 0;
        }

        public virtual Model ArmorModel => null;
    }
}