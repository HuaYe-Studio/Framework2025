using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WatchManager : MonoBehaviour
{
    // 单例实例
    private static WatchManager _instance;
    
    // 单例访问器
    public static WatchManager Instance
    {
        get
        {
            // 延迟初始化：首次访问时创建实例
            if (_instance == null)
            {
                CreateInstance();
            }
            return _instance;
        }
    }
    
    // 创建单例实例
    private static void CreateInstance()
    {
        if (!Application.isPlaying) 
        {
            // 调试日志，发布时可移除
#if UNITY_EDITOR
            Logger.LogWarning("WatchManager 创建被阻止：当前不在运行模式");
#endif
            return;
        }
        // 创建GameObject承载管理器
        var go = new GameObject("WatchManager");
        
        // 添加WatchManager组件
        _instance = go.AddComponent<WatchManager>();
        
        // 设置为跨场景不销毁
        DontDestroyOnLoad(go);
    }
    
    
    // 监控数据结构
    public class WatchedField
    {
        public object Target;      // 被监控对象实例
        public FieldInfo Field;    // 字段反射信息
        public WatchableAttribute Attribute; // 特性信息
    }
    
    // 存储所有监控字段的列表
    //readonly以防引用被替换
    private readonly List<WatchedField> _watchedFields = new List<WatchedField>();
    
    
    // 注册对象的所有监控字段
    public void Register(object target)
    {
        
        //如果不在编译器中则跳过注册
#if UNITY_EDITOR
        #else
        return;
#endif
        // 获取目标对象的类型
        Type type = target.GetType();
        
        // 获取所有字段（包括公有和私有）
        FieldInfo[] fields = type.GetFields(
            BindingFlags.Public | 
            BindingFlags.NonPublic | 
            BindingFlags.Instance | BindingFlags.Static);
        
        // 遍历所有字段
        foreach (FieldInfo field in fields)
        {
            
            // 检查字段是否有Watchable特性
            var attr = field.GetCustomAttribute<WatchableAttribute>();
            if (attr != null)
            {
                // 添加到监控列表
                _watchedFields.Add(new WatchedField
                {
                    Target = target,
                    Field = field,
                    Attribute = attr
                });
            }
        }
    }
    
    // 获取所有监控字段
    public List<WatchedField> GetWatchedFields()
    {
        return _watchedFields;
    }
}