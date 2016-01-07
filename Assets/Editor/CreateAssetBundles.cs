using UnityEditor;
using UnityEngine;

public class CreateAssetBundles {
    
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("E:/Example/Unity/NCKH_2015_TheMuseum/BaoTang/AssetBundles", 
            BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log(EditorUserBuildSettings.activeBuildTarget);
    }

}
