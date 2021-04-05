
namespace Regicide.Game.GameResources
{
    public interface IResourceRate
    {
        ResourceItem Resource { get; }
        float Rate { get; }
        string Source { get; }
    }
}