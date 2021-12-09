using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nekoyume.Game.Light;
using UnityEngine;

namespace Nekoyume
{
    public static class LightPalette
    {
        private static List<GameObject> _globalLightObjects;
        private static List<GameObject> _pointLightObjects;
        private static LightPreset _data;
        private static LightPaletteScriptableObject _scriptableObject;

        private static LightPaletteScriptableObject ScriptableObject
        {
            get
            {
                if (_scriptableObject != null)
                {
                    return _scriptableObject;
                }

                var path = Path.Combine("ScriptableObject", "UI_LightPalette");
                _scriptableObject = Resources.Load<LightPaletteScriptableObject>(path);
                return _scriptableObject;
            }
        }

        private static List<GameObject> GlobalLightObjects
        {
            get
            {
                if (_globalLightObjects != null)
                {
                    return _globalLightObjects;
                }

                _globalLightObjects = new List<GameObject>();
                _globalLightObjects.AddRange(ScriptableObject.globalLights);
                return _globalLightObjects;
            }
        }

        private static List<GameObject> PointLightObjects
        {
            get
            {
                if (_pointLightObjects != null)
                {
                    return _pointLightObjects;
                }

                _pointLightObjects = new List<GameObject>();
                _pointLightObjects.AddRange(ScriptableObject.pointLights);
                return _pointLightObjects;
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

            var light = GlobalLightObjects[data.lightType];
            if (light == null)
            {
                Debug.LogError($"[Global light is not exist] light type : {data.lightType}");
                return null;
            }

            return light;
        }

        public static GameObject GetPointLight(int id)
        {
            var light = PointLightObjects[id];
            if (light == null)
            {
                Debug.LogError($"[Point light is not exist] id : {id}");
                return null;
            }

            return light;
        }
    }
}
