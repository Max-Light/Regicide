
using UnityEngine;

namespace Regicide.Game.GameResources
{
    public class ResourceRateBuilder 
    {
        private ResourceItem _resource = null;
        private float _rate = 0;
        private string _source = "";
        private int _expirationDays = -1;

        public ResourceRateBuilder WithRate(float rate)
        {
            _rate = rate;
            return this;
        }

        public ResourceRateBuilder WithSource(string source)
        {
            _source = source;
            return this;
        }

        public ResourceRateBuilder WithExpirationDays(int expirationDays)
        {
            if (expirationDays > 0)
            {
                _expirationDays = expirationDays;
            }
            else
            {
                Debug.LogError("Expiration days must be a number value greater than 0");
            }
            return this;
        }

        public ResourceRateModifier Build()
        {
            if (_resource == null)
            {
                Debug.LogError("Resource rate builder must include a resource type.");
                return null;
            }

            if (_expirationDays == -1)
            {
                return new ResourceRateModifier(_resource, _rate, _source);
            }
            else
            {
                return new TemporaryResourceRateModifier(_resource, _rate, _source, _expirationDays);
            }
        }

        public static implicit operator ResourceRateModifier(ResourceRateBuilder builder)
        {
            return builder.Build();
        }

        public ResourceRateBuilder(ResourceItem resource)
        {
            _resource = resource;
        }
    }
}