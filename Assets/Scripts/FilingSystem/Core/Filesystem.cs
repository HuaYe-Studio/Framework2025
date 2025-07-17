using System;
using System.Collections.Generic;
using FilingSystem.Test;
using UnityEngine;

namespace FilingSystem.Core
{
    public sealed class Filesystem : IFileSystem
    {
        public IReadOnlyList<string> FileNames => _fileNames;
        private List<string> _fileNames { get; set; } = new();

        private bool _inited;
        private bool _loaded;
        
        private IStorage _storage;
        private readonly List<ISavable> _savableProperties = new();
        private readonly Dictionary<ISavable, object> _defaultValues = new();
        private string _currentFile = string.Empty;

        public void DeleteFile(string fileName)
        {
            MakeSureInited();
            _storage.AssignSavePath(fileName);
            _storage.Clear();
            _storage.WriteToDisk();
            _fileNames.Remove(fileName);
            Debug.Log("删除成功");
        }

        public void DeleteAll()
        {
            MakeSureInited();
            foreach (var fileName in _fileNames)
            {
                DeleteFile(fileName);
            }
        }

        private void MakeSureInited()
        {
            if (string.IsNullOrEmpty(_currentFile))
            {
                if (_fileNames == null)
                {
                    OnInit();
                }


                if (_fileNames!.Count == 0)
                {
                    _fileNames.Add("file1");
                }

                _currentFile = _fileNames[0];
                _storage.Save("FileNames", _fileNames);
                _storage.WriteToDisk();
            }
        }

        //具体实现委托给IStorage
        private T Load<T>(string key, T defaultValue, out bool isNew)
        {
            MakeSureInited();
            return _storage.Load(key, defaultValue, out isNew);
        }

        private T LoadFileInner<T>(T defaultValue) where T : IFileInfoPayload
        {
            MakeSureInited();
            var fileInfo = SaveFileInfo(_currentFile, defaultValue);
            LoadFileInner();
            return fileInfo.Payload;
        }

        private void LoadFileInner()
        {
            MakeSureInited();
            SaveFileName();
            //指定路径
            _storage.AssignSavePath(_currentFile);
            //加载数据
            _storage.ReadFromDisk();
            //为每个已注册值赋值
            foreach (var savable in _savableProperties)
            {
                savable.OnLoad?.Invoke();
            }
            
            _loaded = true;
        }

        public T LoadFile<T>(string fileName = "file1", T defaultValue = default) where T : IFileInfoPayload
        {
            _currentFile = fileName;
            return LoadFileInner(defaultValue);
        }
        
        //必须在所有值都注册完毕后，调用LoadFile，否则无法填充相应的值
        public void LoadFile(string fileName = "file1")
        {
            _currentFile = fileName;
            LoadFileInner();
        }

        /// <summary>
        /// 用于将存档写入文件
        /// </summary>
        private void SaveFileName()
        {
            MakeSureInited();
            
            //指定路径
            _storage.AssignSavePath();
            
            //读取磁盘
            _storage.ReadFromDisk();
            
            //写入缓存区
            _storage.Save("FileNames", _fileNames);
            
            //写入磁盘
            _storage.WriteToDisk();
        }
        
        //用于保存Payload
        private FileInfo<T> SaveFileInfo<T>(string fileName, T payload) where T : IFileInfoPayload
        {
            MakeSureInited();
            
            //指定路径
            _storage.AssignSavePath();
            
            //读取磁盘
            _storage.ReadFromDisk();
            
            var infos = _storage.Load("FileInfos" + typeof(T).Name, new List<FileInfo<T>>(), out _);
            var targetInfo = infos.Find(file => file.FileName == fileName);
            if (targetInfo != null)
            {
                targetInfo.LastModifyTime = DateTime.Now;
                targetInfo.Payload = payload;
            }
            else
            {
                targetInfo = new FileInfo<T>
                {
                    FileName = fileName,
                    CreateTime = DateTime.Now,
                    LastModifyTime = DateTime.Now,
                    Payload = payload
                };
                infos.Add(targetInfo);
            }
            
            //写入缓存区
            _storage.Save("FileInfos" + typeof(T).Name, infos);
            //写入硬盘
            _storage.WriteToDisk();
            return targetInfo;
        }
        //具体实现委托给IStorage
        private void Save<T>(string key, T value)
        {
            MakeSureInited();
            _storage.Save(key, value);
        }

        public void SaveFile<T>(T payload) where T : IFileInfoPayload
        {
            if (!_loaded)
            {
                Debug.LogWarning("需要先加载一个存档");
                return;
            }
            SaveFileInfo(_currentFile, payload);
            SaveFile();
        }

        public void SaveFile()
        {
            if (!_loaded)
            {
                Debug.LogWarning("需要先加载一个存档");
                return;
            }
            _storage.AssignSavePath(_currentFile);
            _storage.Clear();
            //依次调用onSave方法，把注册数据写入缓存区
            foreach (var savable in _savableProperties)
            {
                savable.OnSave?.Invoke();
            }
            //将缓存区中的数据写入文件
            _storage.WriteToDisk();
            //将存档名写入文件
            SaveFileName();
        }

        private void OnInit()
        {
            if (!_inited)
            {
                _storage = new JsonStorage();
                _storage.AssignSavePath();
                _storage.ReadFromDisk();
                _fileNames = _storage.Load("FileNames", new List<string>(), out _);
                _inited = true;
            }
        }
        
        //可以自定义存档名或者默认存档名
        public void CreateFile(string fileName)
        {
            MakeSureInited();

            if (_fileNames.Contains(fileName))
            {
                Debug.LogError($"已存在存档{fileName}");
                return;
            }
            
            _fileNames.Add(fileName);
            SaveFileName();
        }

        public IReadOnlyList<FileInfo<T>> GetFileInfos<T>() where T : IFileInfoPayload
        {
            MakeSureInited();
            _storage.AssignSavePath();
            _storage.ReadFromDisk();
            return _storage.Load("FileInfos" + typeof(T).Name, new List<FileInfo<T>>(), out _);
        }

        public void Register<T>(SavableProperty<T> savable, string name)
        {
            if (!_savableProperties.Contains(savable))
            {
                _savableProperties.Add(savable);
                _defaultValues.Add(savable, savable.Value);
                var saveItem = (ISavable)savable;
                saveItem.OnLoad += () =>
                {
                    var value = Load(name, (T)_defaultValues[savable], out var isNew);
                    if (!isNew)
                        savable.Value = value;
                    else
                        savable.Value = (T)_defaultValues[savable];
                };
                saveItem.OnSave += () => Save(name, savable.Value);
            }
            else Debug.Log("重复注册值");
        }

        public void Unregister<T>(SavableProperty<T> savable)
        {
            if (_savableProperties.Contains(savable))
            {
                _savableProperties.Remove(savable);
                var saveItem = (ISavable)savable;
                saveItem.OnLoad = null;
                saveItem.OnSave = null;
            }
            else Debug.Log("重复注销值");
        }

        public void CreateFile<T>(string fileName, T payload) where T : IFileInfoPayload
        {
            MakeSureInited();
            if (_fileNames.Contains(fileName)) return;
            _fileNames.Add(fileName);
            SaveFileInfo(fileName, payload);
            SaveFileName();
        }
    }
}
