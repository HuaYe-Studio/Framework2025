using UnityEngine;

public class LogInitializer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 确保一直存在
        Logger.InitializeFileLogging(); // 初始化日志文件
    }
}