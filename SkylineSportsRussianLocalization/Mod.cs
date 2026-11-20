using Game;
using Game.Modding;
using Game.SceneFlow;

namespace SkylineSportsRussianLocalization
{
    public sealed class Mod : IMod
    {
        public void OnLoad(UpdateSystem updateSystem)
        {
            GameManager.instance.localizationManager.AddSource("ru-RU", new LocaleRU());
            RuntimeTranslation.Install();
        }

        public void OnDispose()
        {
            RuntimeTranslation.Uninstall();
        }
    }
}
