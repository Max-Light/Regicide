
using System;

namespace Regicide.Game.GameResources
{
    public class ResourceRateModifier : IResourceRate, IObservable
    {
        private Action _onRateChange = null;
        private float _rate = 0;

        public ResourceItem Resource { get; private set; }
        public float Rate 
        { 
            get => _rate;
            set
            {
                _rate = value;
                _onRateChange?.Invoke();
            }
        }
        public string Source { get; private set; } = "Base";

        public ResourceRateModifier(ResourceItem resource, float rate, string source)
        {
            Resource = resource;
            Rate = rate;
            Source = source;
        }

        public virtual void UpdateResourceAmount(ResourceStock resourceStock)
        {
            Resource.Amount += Rate;
        }

        public void AddObserver(Action action)
        {
            _onRateChange += action;
        }

        public void RemoveObserver(Action action)
        {
            _onRateChange -= action;
        }
    }
}