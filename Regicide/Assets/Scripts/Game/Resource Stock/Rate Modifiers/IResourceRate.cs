
namespace Regicide.Game.GameResources
{
    public interface IResourceRate
    {
        ResourceItem Resource { get; }
        float Rate { get; set; }
        string Source { get; }
        abstract void UpdateResourceAmount(ResourceStock resourceStock);
    }
}