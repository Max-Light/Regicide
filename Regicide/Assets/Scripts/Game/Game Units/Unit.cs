using Regicide.Game.BattleSimulation;
using System;
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class Unit : IUnit, IObservable, IDamager, IDamageable
    {
        public class Model
        {
            public uint UnitId { get; private set; } = 0;
            public string Unit { get; private set; } = "";
            public string Description { get; private set; } = "";
            public Sprite Sprite { get; private set; } = null;

            public Model(uint unitId, string unit, string description, Sprite sprite)
            {
                UnitId = unitId;
                Unit = unit;
                Description = description;
                Sprite = sprite;
            }
        }

        private float _health = 100;
        private float _maxHealth = 100;
        protected Action _onUnitChange = null;

        public virtual Model UnitModel => null;
        public float Health => _health;

        public Unit()
        {
            _health = _maxHealth;
        }

        protected virtual void TakeDamage(float damage) 
        {
            _health = Mathf.Clamp(_health - damage, 0, float.MaxValue);
            _onUnitChange?.Invoke();
        }

        public void AddObserver(Action action)
        {
            _onUnitChange += action;
        }

        public void RemoveObserver(Action action)
        {
            _onUnitChange -= action;
        }

        public virtual void PopulateDamageReport(DamageReport damageReport) { }
        public virtual void ReceiveDamage(DamageReport damage) { }
    }
}