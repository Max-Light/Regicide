
namespace Regicide.Game.Entities
{
    public interface ICountyUnitCompany 
    {
        public County County { get; }
        void SetAffiliatedCounty(County county);
    }
}