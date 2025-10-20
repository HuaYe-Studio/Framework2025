using UnityEngine;


/// <summary>
/// 饿汉Mono单例模板
/// </summary>
/// <typeparam name="T"></typeparam>
    public class MonoSingletonHungry<T> : MonoBehaviour   where T : new()
    {
        private static T _instance =  new T();
        public static T Instance { get { return _instance; } }
    }
