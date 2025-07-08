using System;
using System.Threading;
using UnityEngine;

public class Singleton<T> where T :Singleton<T>,new()
{
    /// <summary>
    /// 单例模板
    /// </summary>
    
    private static readonly Lazy<T> _instance = new Lazy<T>(() =>
    {
        //创建实例时调用初始化函数
        var instance = new T();
        instance.Init();
        return new T();
    },LazyThreadSafetyMode.ExecutionAndPublication);
    
    //全局访问点
    public static T Instance  => _instance.Value;

    //初始化函数，可以在子类中覆写
    protected virtual void Init()
    {
        
    }
}
