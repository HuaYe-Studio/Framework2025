using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 单例中心工厂
/// 可加工所有继承自IProduct的子接口，
/// </summary>
public class SingleFactory : Singleton<SingleFactory>
{
    /// <summary>
    /// 产品池
    /// </summary>
    private readonly Dictionary<Type, List<IProduct>> _typePool = new Dictionary<Type, List<IProduct>>();

    /// <summary>
    /// 类型池
    /// </summary>
    private readonly Dictionary<string, List<Type>> _keyMap = new Dictionary<string, List<Type>>();
    
    private readonly object _lock =  new object();

    /// <summary>
    /// 注册产品
    /// </summary>
    /// <param name="key"></param>
    /// <param name="product">产品实例</param>
    public void Register<T>(string key, T product) where T : IProduct
    {
        if (product == null) return;

        lock (_lock)
        {
            var t = product.GetType();
            if (!_typePool.TryGetValue(t, out var pool))
            {
                pool = new List<IProduct>();
                _typePool[t] = pool;
            }
            pool.Add(product);

            if (!_keyMap.TryGetValue(key, out var typeList))
            {
                typeList = new List<Type>();
                _keyMap[key] = typeList;
            }
            if (!typeList.Contains(t))
                typeList.Add(t);
        }


    }
    
    /// <summary>
    /// 清空指定key的指定注册产品
    /// </summary>
    /// <param name="key"></param>
    /// <param name="product"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Unregister<T>(string key, T product) where T : IProduct
    {
        if (product == null) return false;
    
        lock (_lock)
        {
            var t = product.GetType();
            
            if (_typePool.TryGetValue(t, out var pool))
            {
                pool.Remove(product);
                if (pool.Count == 0)
                {
                    _typePool.Remove(t);
                }
            }
            
            if (_keyMap.TryGetValue(key, out var typeList))
            {
                bool hasOtherInstances = _typePool.Any(kvp => 
                    kvp.Key == t && kvp.Value.Count > 0);
            
                if (!hasOtherInstances)
                {
                    typeList.Remove(t);
                    if (typeList.Count == 0)
                    {
                        _keyMap.Remove(key);
                    }
                }
            
                return true;
            }
        
            return false;
        }
    }

    /// <summary>
    /// 清空所有注册产品
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _typePool.Clear();
            _keyMap.Clear();
        }
    }

    /// <summary>
    /// 清空指定key的注册产品
    /// </summary>
    /// <param name="key"></param>
    public void Clear(string key)
    {
        lock (_lock)
        {
            if (_keyMap.TryGetValue(key, out var typeList))
            {
                foreach (var type in typeList)
                {
                    if (_typePool.ContainsKey(type))
                    {
                        _typePool.Remove(type);
                    }
                }
                _keyMap.Remove(key);
            }
        }
    }

    /// <summary>
    /// 获取产品函数,按子接口取货
    /// </summary>
    public List<T> GetProduct<T>(string key) where T : IProduct
    {
        lock (_lock)
        {
            if (!_keyMap.TryGetValue(key, out var typeList))
                return new List<T>();
            
            return typeList
                .SelectMany(t => _typePool.TryGetValue(t, out var l) ? l : Enumerable.Empty<IProduct>())
                .OfType<T>()
                .ToList();
        }
        
    }
}