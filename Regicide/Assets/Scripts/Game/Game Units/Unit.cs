
using System;
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class Unit : IUnit, IObservable<Unit>, IDamageable
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
        protected Action<Unit> _onUnitChange = null;

        public virtual Model UnitModel => null;
        public float Health => _health;
        public DamageReductionAttribute DamageReductionReport { get => DamageReductionAttribute.None; }

        public virtual bool IsAlive => _health != 0;

        public Unit()
        {
            _health = _maxHealth;
        }

        protected virtual void TakeDamage(float damage) 
        {
            _health = Mathf.Clamp(_health - damage, 0, float.MaxValue);
            _onUnitChange?.Invoke(this);
        }

        public void AddObserver(Action<Unit> action)
        {
            _onUnitChange += action;
        }

        public void RemoveObserver(Action<Unit> action)
        {
            _onUnitChange -= action;
        }

        public void ReceiveDamage(float damage) { TakeDamage(damage); }
    }
}