
using System;

namespace Regicide.Game.GameResources
{
    public abstract class ResourceItem : IResourceType, IObservable
    {
        private float _amount = 0;
        private Action _onResourceAmountChange = null;

        public float Amount 
        { 
            get => _amount;
            set
            {
                _onResourceAmountChange?.Invoke();
                Amount = value;
            }
        }
        public virtual ResourceItemModel Model => null;

        public void AddObserver(Action action)
        {
            _onResourceAmountChange += action;
        }

        public void RemoveObserver(Action action)
        {
            _onResourceAmountChange += action;
        }
    }
}