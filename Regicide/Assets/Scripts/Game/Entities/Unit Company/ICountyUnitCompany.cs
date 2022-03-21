
namespace Regicide.Game.Entity
{
    public interface ICountyUnitCompany 
    {
        public County County { get; }
        void SetAffiliatedCounty(County county);
    }
}