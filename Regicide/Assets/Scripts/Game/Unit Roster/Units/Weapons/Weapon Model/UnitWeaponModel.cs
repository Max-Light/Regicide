using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Units
{
    public class UnitWeaponModel 
    {
        public uint WeaponId { get; private set; } = 0;
        public string Weapon { get; private set; } = "";
        public string Description { get; private set; } = "";
        public Sprite Sprite { get; private set; } = null;
    }
}