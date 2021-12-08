using System.Collections.Generic;
using UnityEngine;

namespace Nekoyume
{
    [CreateAssetMenu(fileName = "UI_LightPalette", menuName = "Scriptable Object/Light Palette", order = int.MaxValue)]
    public class LightPaletteScriptableObject : ScriptableObject
    {
        [SerializeField]
        public List<GameObject> lights;
    }
}
