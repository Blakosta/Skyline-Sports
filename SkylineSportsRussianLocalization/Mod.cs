using Game;
using Game.Modding;
using Game.SceneFlow;

namespace SkylineSportsRussianLocalization
{
    /// <summary>
    /// Independent localization add-on. It does not replace or modify Skyline Sports.
    /// </summary>
    public sealed class Mod : IMod
    {
        public void OnLoad(UpdateSystem updateSystem)
        {
            GameManager.instance.localizationManager.AddSource("ru-RU", new LocaleRU());
        }

        public void OnDispose()
        {
        }
    }
}
