using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Nekoyume.Game.Light
{
    public class LightColorSetter : MonoBehaviour, IColorSetter
    {
        [SerializeField] private Gradient gradient = null;

        private Light2D[] _lights;

        public void Refresh()
        {
            _lights = GetComponentsInChildren<Light2D>();
        }

        public void SetColor(float time)
        {
            foreach (var light in _lights)
                light.color = gradient.Evaluate(time);
        }
    }
}
