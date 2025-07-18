using UnityEngine;
using System.IO;
using System.Linq;
using System;

public static class Logger
{
    //日志级别枚举 (Debug, Info, Warning, Error)
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
    
    //当前最低记录级别
    public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Info; // 默认Info及以上
    
    //为性能优化提供快速检查属性 (尤其对Debug)
    //根据当前最低记录级别设定是否启用不同分级的日志
    public static bool IsDebugEnabled => CurrentLogLevel <= LogLevel.Debug;
    public static bool IsInfoEnabled => CurrentLogLevel <= LogLevel.Info;
    public static bool IsWarningEnabled => CurrentLogLevel <= LogLevel.Warning;
    public static bool IsErrorEnabled => CurrentLogLevel <= LogLevel.Error;
    
    
    //文件写入相关变量
    private static StreamWriter _logFileWriter = null;
    private static readonly object _fileLock = new object(); // 文件写入锁，保证线程安全
    private static string _logFilePath; // 存储当前日志文件路径

    
    
    
    
    //核心日志方法 (控制台 + 基础格式化) 
    //传入日志级别、日志信息以及object（用于跟踪）
    private static void LogInternal(LogLevel level, string message, UnityEngine.Object context = null)
    {
        //检查级别是否足够（若不够直接结束方法）
        if (level < CurrentLogLevel) return;

        //基础格式化： [时间] [级别] 消息
        //（$代表字符串插值，第一个大括号是具体时间，这样结果大致为[2024-07-18 10:30:15.500] [error] abcd 这样的格式）
        string formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
        
        //根据级别调用 Unity Debug 原生方法 (利用其颜色和上下文定位!)
        switch (level)
        {
            case LogLevel.Debug:
            case LogLevel.Info:
                Debug.Log(formattedMessage, context); // Info 和 Debug 都用 Log
                break;
            case LogLevel.Warning:
                Debug.LogWarning(formattedMessage, context);
                break;
            case LogLevel.Error:
                Debug.LogError(formattedMessage, context);
                break;

        }
        
        //将日志写入文件
        WriteToFile(level, formattedMessage,context);
    }
    
    
    //文件日志
    
    //文件日志初始化
    public static void InitializeFileLogging()
    {
        try
        {
            // 确定日志目录 (确保存在)
            string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
            Directory.CreateDirectory(logDirectory); // 不存在则创建

            // 生成带日期的日志文件名 (按天滚动)
            string logFileName = $"game_log_{DateTime.Now:yyyyMMdd}.log";
            
            //拼接最终路径
            _logFilePath = Path.Combine(logDirectory, logFileName);

            // 创建或追加到文件, StreamWriter 用 UTF8 编码，自动 Flush
            //（看不懂）
            _logFileWriter = new StreamWriter(_logFilePath, true, System.Text.Encoding.UTF8)
            {
                AutoFlush = true // 确保日志及时写入，但注意性能
            };

            // 写一个初始化成功的日志 (用 Info)
            LogInfo($"日志系统文件初始化成功。日志路径: {_logFilePath}");

            //执行清除旧日志方法
            CleanOldLogFiles();
        }
        catch (Exception ex)
        {
            //（看不懂）
            // 文件初始化失败是严重问题，用 Unity Error 确保能看到
            Debug.LogError($"初始化日志文件失败! {ex.Message}");
            // 关闭文件写入器，避免后续错误
            _logFileWriter?.Close();
            _logFileWriter = null;
        }
    }
    
    //日志文件的写入
    private static void WriteToFile(LogLevel level ,string formattedMessage, UnityEngine.Object context)
    {
        if (_logFileWriter == null) return; // 确保初始化了

        try
        {
            //(看不懂线程相关的)
            lock (_fileLock) // 加锁保证线程安全 
            {
                // 构建更丰富的行信息 (可选添加：场景名、帧数)
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                //int frameCount = Time.frameCount;

                // 处理上下文对象信息
                string contextInfo = context != null ? $" | Context: {context.name} ({context.GetType().Name})" : "";

                // 组合最终写入文件的行
                //string fileLine = $"{formattedMessage} | Scene: {sceneName} | Frame: {frameCount}{contextInfo}";
                //(我觉得暂时不用输出帧数)
                string fileLine = $"{formattedMessage} | Scene: {sceneName}{contextInfo}";
                
                // 写入行
                _logFileWriter.WriteLine(fileLine);
            }
        }
        catch (Exception ex)
        {
            // 文件写入失败也要报告，但避免递归调用自己导致死循环
            Debug.LogError($"写入日志文件失败! {ex.Message}"); // 用原生 Debug 确保输出
        }
    }
    
    
    //清除旧日志
    //（清除最新7个以外的日志）
    public static void CleanOldLogFiles()
    {
        try
        {
            //确定日志目录
            string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
            
            //获取所有日志文件并转换为列表
            //（暂时还不懂列表）
            //（空的txt文件并不会算在这里面）
            var logFiles = Directory.GetFiles(logDirectory, "game_log_*.log").ToList();
            
            // 按文件名排序（适用于 yyyyMMdd 格式）
            logFiles = logFiles.OrderByDescending(f => Path.GetFileName(f)).ToList();
            
            
            //保留最新7个文件，删除其余
            for (int i = 7; i < logFiles.Count; i++)
            {
                File.Delete(logFiles[i]);
                LogInfo($"已删除旧日志: {Path.GetFileName(logFiles[i])}");//记录删除了什么
            }
            
            //显示清理结果
            if (logFiles.Count > 7)
            {
                LogInfo($"日志清理完成: 删除 {logFiles.Count - 7} 个旧文件");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"日志清理失败: {ex.Message}");
        }
    }
    
    
    
    
    //api
    //（使用LogDebug防止与UnityEngine.Debug产生命名冲突）
    // Debug
    public static void LogDebug(string message, UnityEngine.Object context = null)
    {
        if (IsDebugEnabled) // 先检查级别避免不必要的字符串拼接开销
            LogInternal(LogLevel.Debug, message, context);
    }

    // Info
    public static void LogInfo(string message, UnityEngine.Object context = null)
    {
        if (IsInfoEnabled)
            LogInternal(LogLevel.Info, message, context);
    }

    // Warning
    public static void LogWarning(string message, UnityEngine.Object context = null)
    {
        if (IsWarningEnabled)
            LogInternal(LogLevel.Warning, message, context);
    }

    // Error
    public static void LogError(string message, UnityEngine.Object context = null)
    {
        if (IsErrorEnabled)
            LogInternal(LogLevel.Error, message, context);
    }

    // Error with Exception (非常重要！)
    //（没看懂对于Exception的处理）
    public static void LogError(Exception exception, string message = "", UnityEngine.Object context = null)
    {
        if (IsErrorEnabled)
        {
            string fullMessage = $"{message}\nException: {exception.GetType().Name}: {exception.Message}\nStack Trace:\n{exception.StackTrace}";
            LogInternal(LogLevel.Error, fullMessage, context);
        }
    }
}
