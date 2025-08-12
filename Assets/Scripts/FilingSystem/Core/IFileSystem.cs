using System;
using System.Collections.Generic;
using FilingSystem.Test;

namespace FilingSystem.Core
{
    public interface IFileSystem
    {
        /// <summary>
        /// 提供所有存档名
        /// </summary>
        IReadOnlyList<string> FileNames { get;}
        
        
        /// <summary>
        /// 根据给定FileInfoPayload返回对应的FileInfo
        /// FileInfoPayload，用来存储业务相关的自定义数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IReadOnlyList<FileInfo<T>> GetFileInfos<T>() where T : IFileInfoPayload;
        
        /// <summary>
        ///  注册一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void Register<T>(SavableProperty<T> savable, string name);
        
        /// <summary>
        /// 注销一个值
        /// </summary>
        /// <param name="savable"></param>
        /// <typeparam name="T"></typeparam>
        void Unregister<T>(SavableProperty<T> savable);
        
        /// <summary>
        /// 创建存档
        /// </summary>
        /// <param name="fileName"></param>
        void CreateFile(string fileName);
        
        /// <summary>
        /// 创建存档，同时创建一个Payload存储自定义信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="payload"></param>
        /// <typeparam name="T"></typeparam>
        void CreateFile<T>(string fileName, T payload) where T: IFileInfoPayload;
        
        /// <summary>
        /// 加载存档，返回存档对应的Payload
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LoadFile<T>(string fileName = "file1",T defaultValue = default) where T : IFileInfoPayload;
        
        /// <summary>
        /// 加载存档
        /// </summary>
        /// <param name="fileName"></param>
        void LoadFile(string fileName = "file1");
        
        /// <summary>
        /// 保存存档，同时写入Payload
        /// </summary>
        /// <param name="payload"></param>
        /// <typeparam name="T"></typeparam>
        void SaveFile<T>(T payload) where T : IFileInfoPayload;
        
        /// <summary>
        /// 保存存档
        /// </summary>
        void SaveFile();
        
        /// <summary>
        /// 删除存档
        /// </summary>
        /// <param name="fileName"></param>
        void DeleteFile(string fileName);
        
        /// <summary>
        /// 删除所有存档
        /// </summary>
        void DeleteAll();
    }

    public interface ISavable
    {
        Action OnLoad { get; set; }
        Action OnSave { get; set; }
    }

    public sealed class SavableProperty<T> : ISavable
    {
        public T Value;
        Action ISavable.OnLoad { get; set; } = () => { };
        Action ISavable.OnSave { get; set; } = () => { };
    }
}