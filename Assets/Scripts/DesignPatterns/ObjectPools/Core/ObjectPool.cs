using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    private enum PoolType { LRU, LFU };
    private PoolType _poolType = PoolType.LRU;

    // 对象池字典
    private readonly Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();

    // 暖池配置
    private readonly Dictionary<string, int> _PoolSizes = new Dictionary<string, int>();
    private readonly Dictionary<string, int> _warmPoolCreatedCounts = new Dictionary<string, int>();

    //资源路径
    private readonly string path = "Prefabs/";

    /// <summary>
    /// 暖池
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <param name="size">暖池大小</param>
    public void WarmUp(string name, int size)
    {
        // 记录该类型对象的预设容量与已创建数量
        _PoolSizes[name] = size;
        _warmPoolCreatedCounts[name] = 0;

        // 确保该类型在对象池字典中存在条目
        if (!_objPool.ContainsKey(name))
        {
            _objPool.Add(name, new List<GameObject>());
        }

        // 按预设容量实例化并缓存对象
        for (int i = 0; i < size; i++)
        {
            GameObject obj = CreateNewObject(name);
            obj.SetActive(false);
            _objPool[name].Add(obj);
            _warmPoolCreatedCounts[name]++;
        }
    }

    /// <summary>
    /// 获取一个对象
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <returns>对象引用</returns>
    public GameObject GetObj(string name)
    {
        GameObject obj = null;

        // 若对象池中有闲置对象，则直接取出
        if (_objPool.TryGetValue(name, out var poolList) && poolList.Count > 0)
        {
            obj = poolList[0];
            poolList.RemoveAt(0);
        }
        // 若未达到暖池预设容量，则创建新的暖池对象
        else if (_PoolSizes.TryGetValue(name, out int warmSize) &&
                _warmPoolCreatedCounts.TryGetValue(name, out int createdCount) &&
                createdCount < warmSize)
        {
            obj = CreateNewObject(name);
            _warmPoolCreatedCounts[name]++;
        }
        // 否则直接实例化新对象
        else
        {
            obj = CreateNewObject(name);
        }

        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 创建新对象
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <returns>对象引用</returns>
    private GameObject CreateNewObject(string name)
    {
        // 从 Resources 加载预设
        GameObject prefab = Resources.Load<GameObject>(path + name);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found: " + path + name);
            return new GameObject("FallbackObject");
        }
        // 实例化并返回
        return GameObject.Instantiate(prefab);
    }

    /// <summary>
    /// 归还对象
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <param name="obj">对象引用</param>
    public void ReturnObj(string name, GameObject obj)
    {
        obj.SetActive(false);

        // 确保该类型在对象池字典中存在条目
        if (!_objPool.ContainsKey(name))
        {
            _objPool.Add(name, new List<GameObject>());
        }

        // 判断是否为暖池对象
        bool isWarmPoolObject = _warmPoolCreatedCounts.ContainsKey(name) &&
                               _warmPoolCreatedCounts[name] > 0;

        // 若属于暖池或当前缓存数量未超限，则回收；否则销毁
        if (isWarmPoolObject || _objPool[name].Count < GetWarmPoolSize(name))
        {
            _objPool[name].Add(obj);
        }
        else
        {
            GameObject.Destroy(obj);
        }
    }

    /// <summary>
    /// 扩充对象池大小
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <param name="addSize">扩充大小</param>
    public void ExpandPoolSize(string name, int addSize)
    {
        if (addSize <= 0) return;

        // 确保配置字典与对象池字典均存在条目
        if (!_PoolSizes.ContainsKey(name))
        {
            _PoolSizes[name] = 0;
            _warmPoolCreatedCounts[name] = 0;
        }

        if (!_objPool.TryGetValue(name, out var poolList))
        {
            poolList = new List<GameObject>();
            _objPool[name] = poolList;
        }

        // 按增量创建新对象并缓存
        for (int i = 0; i < addSize; i++)
        {
            GameObject obj = CreateNewObject(name);
            obj.SetActive(false);
            poolList.Add(obj);
        }

        // 同步扩展配置
        _PoolSizes[name]            += addSize;
        _warmPoolCreatedCounts[name] += addSize;
    }

    /// <summary>
    /// 返回对象池大小
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <returns>对象池池大小</returns>
    private int GetWarmPoolSize(string name)
    {
        return _PoolSizes.TryGetValue(name, out int size) ? size : 0;
    }

    /// <summary>
    /// 清空指定类型池子
    /// </summary>
    /// <param name="name">对象名称</param>
    public void ClearObj(string name)
    {
        if (_objPool.TryGetValue(name, out var poolList))
        {
            foreach (var obj in poolList)
            {
                // 仅销毁非暖池对象
                if (!IsWarmPoolObject(name))
                {
                    GameObject.Destroy(obj);
                }
            }
            poolList.Clear();
        }

        // 重置已创建计数
        if (_warmPoolCreatedCounts.ContainsKey(name))
        {
            _warmPoolCreatedCounts[name] = 0;
        }
    }

    /// <summary>
    /// 检查该对象类是否暖池
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <param name="obj">对象引用</param>
    /// <returns></returns>
    private bool IsWarmPoolObject(string name)
    {
        // 简化实现，实际项目中可能需要更精确的标识
        return _PoolSizes.ContainsKey(name);
    }

    /// <summary>
    /// 清空所有池
    /// </summary>
    public void ClearAllObj()
    {
        // 逐个清空所有类型
        foreach (var name in new List<string>(_objPool.Keys))
        {
            ClearObj(name);
        }
        _objPool.Clear();

        // 重置所有已创建计数
        foreach (var key in _warmPoolCreatedCounts.Keys)
        {
            _warmPoolCreatedCounts[key] = 0;
        }
    }
}