
namespace Regicide.Game.Units
{
    public abstract class TroopUnit : Unit
    {
        private TroopUnitWeapon[] _troopWeapons = new TroopUnitWeapon[3];

        public ITroopUnitHeadArmor HeadArmor { get; private set; } = null;
        public ITroopUnitTorsoArmor TorsoArmor { get; private set; } = null;
        public ITroopUnitLegArmor LegArmor { get; private set; } = null;
        public TroopUnitWeapon PrimaryWeapon { get => _troopWeapons[0]; }
        public TroopUnitWeapon SecondaryWeapon { get => _troopWeapons[1]; }
        public TroopUnitWeapon TertiaryWeapon { get => _troopWeapons[2]; }

        public void EquipHeadArmor(ITroopUnitHeadArmor headArmor)
        {
            HeadArmor = headArmor;
            _onUnitChange?.Invoke(this);
        }

        public void EquipTorsoArmor(ITroopUnitTorsoArmor torsoArmor)
        {
            TorsoArmor = torsoArmor;
            _onUnitChange?.Invoke(this);
        }

        public void EquipLegArmor(ITroopUnitLegArmor legArmor)
        {
            LegArmor = legArmor;
            _onUnitChange?.Invoke(this);
        }

        public void SetPrimaryWeapon(TroopUnitWeapon weapon)
        {
            _troopWeapons[0] = weapon;
            _onUnitChange?.Invoke(this);
        }

        public void SetSecondaryWeapon(TroopUnitWeapon weapon)
        {
            _troopWeapons[1] = weapon;
            _onUnitChange?.Invoke(this);
        }

        public void SetTertiaryWeapon(TroopUnitWeapon weapon)
        {
            _troopWeapons[2] = weapon;
            _onUnitChange?.Invoke(this);
        }

        public bool TryGetWeaponOfType<T>(out T weapon)
        {
            for (int weaponIndex = 0; weaponIndex < _troopWeapons.Length; weaponIndex++)
            {
                if (_troopWeapons[weaponIndex] is T weaponOfType)
                {
                    weapon = weaponOfType;
                    return true;
                }
            }
            weapon = default(T);
            return false;
        }
    }
}