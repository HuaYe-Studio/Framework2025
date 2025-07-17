namespace FilingSystem.Core
{
    public static class StorageUtil
    {
        private static JsonStorage _jsonStorage;
        private static IStorage Storage => _jsonStorage ??= new JsonStorage();

        public static T LoadByJson<T>(string key, T defaultValue, out bool newValue)
        {
            Storage.AssignSavePath("setting");
            Storage.ReadFromDisk();
            return Storage.Load(key, defaultValue,out newValue );
        }

        public static void SaveByJson<T>(string key, T value)
        {
            Storage.AssignSavePath("setting");
            Storage.ReadFromDisk();
            Storage.Save(key, value);
            Storage.WriteToDisk();
        }
    }
}