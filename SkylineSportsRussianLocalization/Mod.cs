using Game;
using Game.Modding;
using Game.SceneFlow;

namespace SkylineSportsRussianLocalization
{
    public sealed class Mod : IMod
    {
        public void OnLoad(UpdateSystem updateSystem)
        {
            // This helper Setting type reproduces the original mod's locale-key identity.
            var identity = new Skyline_Sports_Project.Setting(this);
            GameManager.instance.localizationManager.AddSource("ru-RU", new LocaleRU(identity));
        }

        public void OnDispose()
        {
        }
    }
}