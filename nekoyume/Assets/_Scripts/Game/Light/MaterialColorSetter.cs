using UnityEngine;

namespace Nekoyume.Game.Light
{
    public class MaterialColorSetter : MonoBehaviour, IColorSetter
    {
        [GradientUsage(true)]
        [SerializeField] private Gradient gradient = null;
        [SerializeField] private string colorName = null;
        [SerializeField] private Material[] materials = null;

        public void Refresh()
        {
        }

        public void SetColor(float time)
        {
            foreach (var material in materials)
                material.SetColor(colorName, gradient.Evaluate(time));
        }
    }
}
