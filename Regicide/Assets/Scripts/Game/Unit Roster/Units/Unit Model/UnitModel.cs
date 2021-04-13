using UnityEngine;

namespace Regicide.Game.Units
{
    public class UnitModel
    {
        public uint UnitId { get; private set; } = 0;
        public string Unit { get; private set; } = "";
        public string Description { get; private set; } = "";
        public Sprite Sprite { get; private set; } = null;

        public UnitModel(uint unitId, string unit, string description, Sprite sprite)
        {
            UnitId = unitId;
            Unit = unit;
            Description = description;
            Sprite = sprite;
        }
    }
}