
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class UnitEquipmentSlot 
    {
        private static UnitEquipmentSlotModel _emptyEquipmentSlotModel = new UnitEquipmentSlotModel("", "", null);

        public class UnitEquipmentSlotModel 
        {
            public string Name { get; private set; } = "";
            public string Description { get; private set; } = "";
            public Sprite Sprite { get; private set; } = null;

            public UnitEquipmentSlotModel(string name, string description, Sprite sprite)
            {
                Name = name;
                Description = description;
                Sprite = sprite;
            }
        }

        public virtual UnitEquipmentSlotModel EquipmentSlotModel { get => _emptyEquipmentSlotModel; }
    }
}