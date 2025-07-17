using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FilingSystem.Core
{
    public sealed class JsonStorage : IStorage
    {
        //默认数据路径
        private string _savePath = Path.Combine(Application.persistentDataPath, "fileInfo.json");

        //缓冲区，以键值对的形式存储数据，string就是对应数据的key，object以JObject的形式存在
        private Dictionary<string, object> _saveData = new();

        public void ReadFromDisk()
        {
            if (File.Exists(_savePath))
            {
                var json = File.ReadAllText(_savePath);
                //从指定路径下读取数据
                _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            else
            {
                _saveData.Clear();
            }
        }

        //从文件中读取
        public void ReadFromText(string json)
        {
            _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        //缓冲区数据写入磁盘
        public void WriteToDisk()
        {
            var json = JsonConvert.SerializeObject(_saveData);
            File.WriteAllText(_savePath, json);
        }

        //覆盖原缓冲区内数据
        public void Save<T>(string key, T saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("Saving null data");
                return;
            }

            if (!_saveData.TryAdd(key, saveData))
            {
                _saveData[key] = saveData;
            }
        }
        
        //从缓冲区读取数据
        public T Load<T>(string key, T defaultValue, out bool newValue)
        {
            newValue = false;
            //缓冲区中没有数据，返回默认值，newValue设置为true
            if (!_saveData.TryGetValue(key, out var saveValue))
            {
                newValue = true;
                return defaultValue;
            }
            //特殊处理,JsonConvert.DeserializeObject<T>对bool和string直接转换会报错
            if (typeof(T) == typeof(bool) || typeof(T) == typeof(string))
            {
                return (T)saveValue;
            }
            //特殊处理，List类数据为”[]"时，直接转化也会报错
            if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                if (saveValue.ToString() == "[]")
                {
                    newValue = true;
                    return defaultValue;
                }

                if (saveValue is not JArray && saveValue is IList)
                {
                    return (T)saveValue;
                }
            }

            //反序列化
            return JsonConvert.DeserializeObject<T>(saveValue.ToString());
        }

        //指定存储路径
        public void AssignSavePath(string path = "fileInfo")
        {
            _savePath = Path.Combine(Application.persistentDataPath, path + ".json");
        }

        public void Clear()
        {
            _saveData.Clear();
        }
        
    }
}    