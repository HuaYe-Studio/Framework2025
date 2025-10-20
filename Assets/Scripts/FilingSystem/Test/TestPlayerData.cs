using System;

namespace FilingSystem.Test
{
    public sealed class FileInfo<T> where T : IFileInfoPayload 
    {
        public string FileName;
        public DateTime CreateTime;
        public DateTime LastModifyTime;
        public T Payload;
    }

    public interface IFileInfoPayload
    {
        
    }

    //在Payload中加入任意想加入的信息
    public sealed class TestFileInfoPayload : IFileInfoPayload 
    {
        public string CharacterName;
        public string CharacterType;
        public string CurrentScene;
        public float GameProgress;
    }

}
