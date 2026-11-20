using Game;
using Game.Modding;
using Game.SceneFlow;

namespace SkylineSportsRussianLocalization
{
    public sealed class Mod : IMod
    {
        public void OnLoad(UpdateSystem updateSystem)
        {
            // LocaleRU resolves the original mod's live Setting instance at dictionary-read time.
            GameManager.instance.localizationManager.AddSource("ru-RU", new LocaleRU());
        }

        public void OnDispose()
        {
        }
    }
}