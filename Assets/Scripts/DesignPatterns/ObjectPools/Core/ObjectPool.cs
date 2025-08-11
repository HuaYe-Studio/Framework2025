using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    private enum PoolType
    {
        LRU,
        LFU
    };
    private PoolType _poolType = PoolType.LRU;
    
    private readonly Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();
    private readonly string path = "Prefabs/";

    /// <summary>
    /// 从池中获取一个对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetObj(string name)
    {
        GameObject obj = null;

        if (_objPool[name] != null && _objPool[name].Count > 0)
        {
            obj = _objPool[name][0];
            _objPool[name].Remove(obj);
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(path + name));
        }

        obj.SetActive(true);
        return obj;
    }
    
    

    /// <summary>
    /// 向池中归还一个对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public void ReturnObj(string name, GameObject obj)
    {
        obj.SetActive(false);
        
        if (_objPool[name] != null)
        {
            _objPool[name].Add(obj);
        }
        else
        {
            _objPool.Add(name, new List<GameObject>() { obj });
        }
        
        return;
    }

    /// <summary>
    /// 清空指定类的池子
    /// </summary>
    /// <param name="name"></param>
    public void ClearObj(string name)
    {
        _objPool[name].Clear();
    }
    
    /// <summary>
    /// 清空所有类的池
    /// </summary>
    public void ClearAllObj()
    {
        _objPool.Clear();
    }
}
