using UnityEditor;

namespace NiftyGlyphs
{
    public static class UnicodeGlyphUtils
    {
        public static bool TryLoad<TAsset>(string fileName, out TAsset asset) where TAsset : UnityEngine.Object
        {
            string searchString = $"t:{typeof(TAsset).Name} {fileName}";
            string[] assetGuids = AssetDatabase.FindAssets(searchString);
            if (assetGuids.Length > 0)
            {
                string firstAssetGUID = assetGuids[0];
                string firstAssetPath = AssetDatabase.GUIDToAssetPath(firstAssetGUID);
                asset = AssetDatabase.LoadAssetAtPath<TAsset>(firstAssetPath);
                return true;
            }
            asset = default;
            return false;
        }
    }
}