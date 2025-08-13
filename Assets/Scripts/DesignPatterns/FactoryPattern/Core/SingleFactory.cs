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

    /// <summary>
    /// 注册产品
    /// </summary>
    /// <param name="key"></param>
    /// <param name="product">产品实例</param>
    public void Register<T>(string key, T product) where T : IProduct
    {
        if (product == null) return;

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

    /// <summary>
    /// 获取产品函数,按子接口取货
    /// </summary>
    public T[] GetProduct<T>(string key) where T : IProduct
    {
        if (!_keyMap.TryGetValue(key, out var typeList))
            return Array.Empty<T>();
        
        var result = typeList
            .SelectMany(t => _typePool.TryGetValue(t, out var l) ? l : Enumerable.Empty<IProduct>())
            .OfType<T>()
            .ToArray();

        return result;
    }
}