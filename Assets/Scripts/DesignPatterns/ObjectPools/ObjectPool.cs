using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private readonly Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();
    private readonly string path = "Prefabs/";

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

    public void PushObj(string name, GameObject obj)
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
}
