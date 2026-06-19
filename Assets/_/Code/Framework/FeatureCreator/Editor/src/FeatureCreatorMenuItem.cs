using UnityEditor;

namespace FeatureCreator.Editor
{
    public static class FeatureCreatorMenuItem
    {
        #region Public API

        [MenuItem("Assets/Create/Feature", false, -235)]
        public static void CreateFeature()
        {
            string targetPath = GetSelectedFolderPath();
            FeatureCreatorWindow.ShowWindow(targetPath);
        }

        #endregion


        #region Main API

        private static string GetSelectedFolderPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path)) return "Assets";

            if (!AssetDatabase.IsValidFolder(path))
            {
                int lastSlash = path.LastIndexOf('/');
                path = lastSlash >= 0 ? path.Substring(0, lastSlash) : "Assets";
            }

            return path;
        }

        #endregion
    }
}