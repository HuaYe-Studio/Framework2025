using System.IO;
using UnityEngine;
using UnityEditor;
public static class ABundleManager

{
    private static string _path;
    /// <summary>
    /// 加载ab包
    /// </summary>
    /// <param name="bundlePath"></param>
    /// <returns></returns>
    private static AssetBundle LoadBundleFromFile(string bundlePath)
    {
        //路径设置
        string projectPath = Directory.GetParent(Application.dataPath)?.FullName;

        if (projectPath != null) _path = System.IO.Path.Combine(projectPath, "Bundles/output");
        //获取ab包
        AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(_path, bundlePath));
        
        return assetBundle;
    }
    
    /// <summary>
    /// 从ab包中加载资源
    /// </summary>
    /// <param name="bundlePath">ab包路径</param>
    /// <param name="name">预制件名称</param>
    /// <returns></returns>
    public static T LoadGameObjectFromBundle<T>(string bundlePath, string name) where T : UnityEngine.Object
    {
        //加载ab包
        AssetBundle assetBundle = LoadBundleFromFile(bundlePath);
        
        //从ab包中加载物体
        T go = assetBundle.LoadAsset<T>(name);
        
        return go;
    }
    
    
}