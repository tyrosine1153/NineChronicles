using System.Linq;
using Nekoyume.Helper;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

namespace Nekoyume.UI
{
    public class AreaAttackCutscene : HudWidget
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation = null;

        private const string AttachmentName = "cutscene_01";
        private const string SlotName = "cutscene";

        private SkeletonAnimation SkeletonAnimation => skeletonAnimation;

        public static void Show(int armorId)
        {
            var cutScene = Create<AreaAttackCutscene>(true);
            var time = UpdateCutscene(cutScene, armorId);
            Destroy(cutScene.gameObject, time);
        }

        private static float UpdateCutscene(AreaAttackCutscene cutscene, int armorId)
        {
            var sprite = SpriteHelper.GetAreaAttackCutsceneSprite(armorId);
            var shader = Shader.Find("Sprites/Default");
            var material = new Material(shader);
            return 0;
        }
    }
}
