using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class Unit : IUnit, IObservable
    {
        private float _health = 100;
        protected Action _onUnitChange = null;

        public virtual UnitModel Model => null;

        public float Health => _health;

        public virtual void TakeDamage(float damage) 
        {
            _health = Mathf.Clamp(_health - damage, 0, float.MaxValue);
        }

        public void AddObserver(Action action)
        {
            _onUnitChange += action;
        }

        public void RemoveObserver(Action action)
        {
            _onUnitChange -= action;
        }
    }
}