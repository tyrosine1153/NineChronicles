using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nekoyume.Game.Light;
using UnityEngine;

namespace Nekoyume
{
    public static class LightPalette
    {
        private static List<GameObject> _objects;
        private static LightPreset _data;

        private static List<GameObject> Objects
        {
            get
            {
                if (_objects != null)
                {
                    return _objects;
                }

                _objects = new List<GameObject>();
                var path = Path.Combine("ScriptableObject", "UI_LightPalette");
                var so  = Resources.Load<LightPaletteScriptableObject>(path);
                _objects.AddRange(so.lights);
                return _objects;
            }
        }

        private static LightPreset Data
        {
            get
            {
                if (_data != null)
                {
                    return _data;
                }

                var path = Path.Combine("Light", "Data", "LightPreset");
                var json = Resources.Load<TextAsset>(path).ToString();
                _data = JsonUtility.FromJson<LightPreset>(json);
                return _data;
            }
        }

        public static GameObject GetBackgroundGlobalLight(string prefabName)
        {
            var preset = Data.backgroundGlobalLight;
            if (preset == null)
            {
                Debug.LogError($"[backgroundGlobalLight is not exist]");
                return null;
            }

            var data = preset.FirstOrDefault(x => x.name.Equals(prefabName));
            if (data == null)
            {
                Debug.LogError($"[Data is not exist] background prefab name :{prefabName}");
                return null;
            }

            var light = Objects[data.lightType];
            if (light == null)
            {
                Debug.LogError($"[Light is not exist] light type : {data.lightType}");
                return null;
            }

            return light;
        }
    }
}
