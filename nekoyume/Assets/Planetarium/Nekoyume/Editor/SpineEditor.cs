using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Nekoyume.Game.Character;
using Spine.Unity;
using Spine.Unity.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Planetarium.Nekoyume.Editor
{
    // TODO: Costume, NPC 로직 추가
    // TODO: 사용자가 알기 쉽게 예외 상황 전부 알림 띄워주기.
    public static class SpineEditor
    {
        private const string FindAssetFilter = "CharacterAnimator t:AnimatorController";

        private const string FullCostumePrefabPath = "Assets/Resources/Character/FullCostume";

        private const string FullCostumeSpineRootPath =
            "Assets/AddressableAssets/Character/FullCostume";

        private const string MonsterPrefabPath = "Assets/Resources/Character/Monster";
        private const string MonsterSpineRootPath = "Assets/AddressableAssets/Character/Monster";

        private const string NPCPrefabPath = "Assets/Resources/Character/NPC";
        private const string NPCSpineRootPath = "Assets/AddressableAssets/Character/NPC";

        private const string PlayerPrefabPath = "Assets/Resources/Character/Player";
        private const string PlayerSpineRootPath = "Assets/AddressableAssets/Character/Player";

        private static readonly Vector3 Position = Vector3.zero;

        /// <summary>
        /// 헤어 스타일을 결정하는 정보를 스파인이 포함하지 않기 때문에 이곳에 하드코딩해서 구분해 준다.
        /// </summary>
        private static readonly string[] HairType1Names =
        {
            "10230000", "10231000", "10232000", "10233000", "10234000", "10235000"
        };

        [MenuItem("Assets/9C/Create Spine Prefab", true)]
        public static bool CreateSpinePrefabValidation()
        {
            return Selection.activeObject is SkeletonDataAsset;
        }

        [MenuItem("Assets/9C/Create Spine Prefab", false, 0)]
        public static void CreateSpinePrefab()
        {
            if (!(Selection.activeObject is SkeletonDataAsset skeletonDataAsset))
            {
                return;
            }

            CreateSpinePrefabInternal(skeletonDataAsset);
        }

        [MenuItem("Tools/9C/Create Spine Prefab(All FullCostume)", false, 0)]
        public static void CreateSpinePrefabAllOfFullCostume()
        {
            CreateSpinePrefabAllOfPath(FullCostumeSpineRootPath);
        }

        [MenuItem("Tools/9C/Create Spine Prefab(All Monster)", false, 0)]
        public static void CreateSpinePrefabAllOfMonster()
        {
            CreateSpinePrefabAllOfPath(MonsterSpineRootPath);
        }

        // FIXME: ArgumentNotFoundException 발생.
        // NPC의 경우에 `Idle_01`과 같이 각 상태 분류의 첫 번째 작명에 `_01`이라는 숫자가 들어가 있기 때문에태
        // `CharacterAnimation.Type`을 같이 사용할 수 없는 상황이다.
        // 따라서 NPC 스파인의 상태 작명을 수정한 후에 사용해야 한다.
        // [MenuItem("Tools/9C/Create Spine Prefab(All NPC)", false, 0)]
        public static void CreateSpinePrefabAllOfNPC()
        {
            CreateSpinePrefabAllOfPath(NPCSpineRootPath);
        }

        [MenuItem("Tools/9C/Create Spine Prefab(All Player)", false, 0)]
        public static void CreateSpinePrefabAllOfPlayer()
        {
            CreateSpinePrefabAllOfPath(PlayerSpineRootPath);
        }

        private static string GetPrefabPath(string prefabName)
        {
            string pathFormat = null;
            if (IsFullCostume(prefabName))
            {
                pathFormat = FullCostumePrefabPath;
            }

            if (IsMonster(prefabName))
            {
                pathFormat = MonsterPrefabPath;
            }

            if (IsNPC(prefabName))
            {
                pathFormat = NPCPrefabPath;
            }

            if (IsPlayer(prefabName))
            {
                pathFormat = PlayerPrefabPath;
            }

            return string.IsNullOrEmpty(pathFormat)
                ? null
                : Path.Combine(pathFormat, $"{prefabName}.prefab");
        }

        private static void CreateSpinePrefabInternal(SkeletonDataAsset skeletonDataAsset)
        {
            var assetPath = AssetDatabase.GetAssetPath(skeletonDataAsset);
            var assetFolderPath = assetPath.Replace(Path.GetFileName(assetPath), "");
            var animationAssetsPath = Path.Combine(assetFolderPath, "ReferenceAssets");
            var split = assetPath.Split('/');
            var prefabName = split[split.Length > 1 ? split.Length - 2 : 0];
            var prefabPath = GetPrefabPath(prefabName);

            if (!ValidateSpineResource(prefabName, skeletonDataAsset))
            {
                if (IsPlayer(prefabName))
                {
                    Debug.LogError("ValidationSpineResource() return false");
                    return;
                }

                Debug.LogWarning("ValidationSpineResource() return false");
            }

            CreateAnimationReferenceAssets(skeletonDataAsset);

        }

        #region Character Type

        private static bool IsFullCostume(string prefabName)
        {
            return prefabName.StartsWith("4");
        }

        private static bool IsMonster(string prefabName)
        {
            return prefabName.StartsWith("2");
        }

        private static bool IsNPC(string prefabName)
        {
            return prefabName.StartsWith("3");
        }

        private static bool IsPlayer(string prefabName)
        {
            return prefabName.StartsWith("1");
        }

        #endregion

        #region Validate Spine Resource

        private static bool ValidateSpineResource(
            string prefabName,
            SkeletonDataAsset skeletonDataAsset)
        {
            if (IsFullCostume(prefabName))
            {
                return ValidateForFullCostume(skeletonDataAsset);
            }

            if (IsMonster(prefabName))
            {
                return ValidateForMonster(skeletonDataAsset);
            }

            if (IsNPC(prefabName))
            {
                return ValidateForNPC(skeletonDataAsset);
            }

            if (IsPlayer(prefabName))
            {
                return ValidateForPlayer(skeletonDataAsset);
            }

            return false;
        }

        private static bool ValidateForFullCostume(SkeletonDataAsset skeletonDataAsset) =>
            ValidateForPlayer(skeletonDataAsset);

        private static bool ValidateForMonster(SkeletonDataAsset skeletonDataAsset)
        {
            return false;
        }

        private static bool ValidateForNPC(SkeletonDataAsset skeletonDataAsset) => true;

        private static bool ValidateForPlayer(SkeletonDataAsset skeletonDataAsset)
        {
            var result = true;

            return result;
        }

        #endregion

        // CharacterAnimation.Type에서 포함하지 않는 것을 이곳에서 걸러낼 수도 있겠다.
        /// <summary>
        /// `SkeletonDataAssetInspector.CreateAnimationReferenceAssets(): 242`
        /// </summary>
        /// <param name="skeletonDataAsset"></param>
        private static void CreateAnimationReferenceAssets(SkeletonDataAsset skeletonDataAsset)
        {
            const string assetFolderName = "ReferenceAssets";

            var parentFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(skeletonDataAsset));
            var dataPath = parentFolder + "/" + assetFolderName;
            if (AssetDatabase.IsValidFolder(dataPath))
            {
                Directory.Delete(dataPath, true);
            }

            AssetDatabase.CreateFolder(parentFolder, assetFolderName);

            var nameField =
                typeof(AnimationReferenceAsset).GetField(
                    "animationName",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            if (nameField is null)
            {
                throw new NullReferenceException(
                    "typeof(AnimationReferenceAsset).GetField(\"animationName\", BindingFlags.NonPublic | BindingFlags.Instance);");
            }

            var skeletonDataAssetField = typeof(AnimationReferenceAsset).GetField(
                "skeletonDataAsset",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (skeletonDataAssetField is null)
            {
                throw new NullReferenceException(
                    "typeof(AnimationReferenceAsset).GetField(\"skeletonDataAsset\", BindingFlags.NonPublic | BindingFlags.Instance);");
            }

        }

        private static void CreateSpinePrefabAllOfPath(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                Debug.LogWarning($"Not Found Folder! {path}");
                return;
            }

            var subFolderPaths = AssetDatabase.GetSubFolders(path);
            foreach (var subFolderPath in subFolderPaths)
            {
                var id = Path.GetFileName(subFolderPath);
                var skeletonDataAssetPath = Path.Combine(subFolderPath, $"{id}_SkeletonData.asset");
                Debug.Log($"Try to create spine prefab with {skeletonDataAssetPath}");
                var skeletonDataAsset =
                    AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonDataAssetPath);
                if (ReferenceEquals(skeletonDataAsset, null) || skeletonDataAsset == null)
                {
                    Debug.LogError($"Not Found SkeletonData from {skeletonDataAssetPath}");
                    continue;
                }

                CreateSpinePrefabInternal(skeletonDataAsset);
            }
        }

        // NOTE: 모든 캐릭터는 원본의 해상도를 보여주기 위해서 Vector3.one 사이즈로 스케일되어야 맞습니다.
        // 하지만 이 프로젝트는 2D 리소스의 ppu와 카메라 사이즈가 호환되지 않아서 임의의 스케일을 설정합니다.
        // 이마저도 아트 단에서 예상하지 못한 스케일 이슈가 생기면 "300005"와 같이 예외적인 케이스가 발생합니다.
        // 앞으로 이런 예외가 많아질 것을 대비해서 별도의 함수로 뺍니다.
        private static Vector3 GetPrefabLocalScale(string prefabName)
        {
            switch (prefabName)
            {
                default:
                    return new Vector3(.64f, .64f, 1f);
                case "300005":
                    return new Vector3(.8f, .8f, 1f);
            }
        }
    }
}
