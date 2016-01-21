using UnityEditor;
using UnityEngine;

public class CreateAssetBundles {
    
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("C:/Users/Mihn Porus/Documents/GitHub/NCKH_VS2/AssetBundles", 
            BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log(EditorUserBuildSettings.activeBuildTarget);
    }

}