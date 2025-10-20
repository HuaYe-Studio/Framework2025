using System.IO;
using UnityEditor;
using UnityEngine;

public static class BundleEditor
{
    private static string _path;
    /// <summary>
    /// 
    /// </summary>
    [MenuItem("Tools/AssetBundle/Packing")]
    public static void Pack()
    {
        
        string projectPath = Directory.GetParent(Application.dataPath)?.FullName;

        if (projectPath != null) _path = System.IO.Path.Combine(projectPath, "Bundles/output");


        if(Directory.Exists(_path) == false)
        {
            Directory.CreateDirectory(_path);
        }
    
        BuildPipeline.BuildAssetBundles(_path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    
        AssetDatabase.Refresh();
        Debug.Log($"AssetBundles 已打包到: {_path}");
    }
    
}