namespace FilingSystem.Core
{
    public interface IStorage
    {
        /// <summary>
        /// 指定存档路径
        /// </summary>
        /// <param name="path"></param>
        void AssignSavePath(string path = "fileInfo");
        
        /// <summary>
        /// 从磁盘中读取（需要先调用AssignSavePath指定存档路径，默认为Unity持久化路径下的fileInfo文件)
        /// 数据存入缓冲区
        /// </summary>
        void ReadFromDisk();
        
        /// <summary>
        /// 从文件中读取
        /// </summary>
        /// <param name="json"></param>
        void ReadFromText(string json);
        
        /// <summary>
        /// 将缓冲区内信息写入磁盘
        /// </summary>
        void WriteToDisk();
        
        /// <summary>
        /// 将缓冲区内数据取出
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="newValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Load<T>(string key, T defaultValue, out bool newValue);
        
        /// <summary>
        /// 将对应键的数据写入缓冲区
        /// </summary>
        /// <param name="key"></param>
        /// <param name="saveData"></param>
        /// <typeparam name="T"></typeparam>
        void Save<T>(string key, T saveData);
        
        /// <summary>
        /// 清除缓冲区
        /// </summary>
        void Clear();
    }
}
