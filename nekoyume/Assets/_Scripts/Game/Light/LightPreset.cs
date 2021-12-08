using System;

namespace Nekoyume.Game.Light
{
    [Serializable]
    public class LightPreset
    {
        public BackgroundGlobalLight[] backgroundGlobalLight;
    }

    [Serializable]
    public class BackgroundGlobalLight
    {
        public string name;
        public int lightType;
    }
}
