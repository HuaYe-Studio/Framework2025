#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class WatchWindow : EditorWindow
{
    // 单例窗口实例
    private static WatchWindow _window;
    
    // 滚动位置 - 用于支持长列表
    private Vector2 _scrollPosition;
    
    // 菜单项创建窗口
    [MenuItem("Tools/Debug/Watch Window")]
    public static void ShowWindow()
    {
        _window = GetWindow<WatchWindow>("变量监控");
        _window.minSize = new Vector2(300, 400); // 设置最小尺寸
    }
    
    // 核心渲染方法
    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("WatchManager 未初始化：当前不在运行模式", MessageType.Warning);
            return;
        }
        
        // 1. 检查管理器是否存在
        if (!WatchManager.Instance)
        {
            EditorGUILayout.HelpBox("WatchManager 未初始化", MessageType.Warning);
            return;
        }
        
        // 2. 获取所有监控字段
        List<WatchManager.WatchedField> fields = 
            WatchManager.Instance.GetWatchedFields();
        
        // 3. 无数据提示
        if (fields.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "没有可监控的变量\n使用 [Watchable] 特性标记字段", 
                MessageType.Info);
            return;
        }
        
        // 4. 滚动视图开始
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        
        // 5. 遍历渲染每个字段
        foreach (var field in fields)
        {
            DrawField(field);
        }
        
        // 6. 滚动视图结束
        EditorGUILayout.EndScrollView();
    }
    
    // 字段渲染方法
    private void DrawField(WatchManager.WatchedField field)
    {
        try
        {
            // A. 反射获取当前值
            object value = field.Field.GetValue(field.Target);
            
            // B. 确定显示名称
            string displayName = string.IsNullOrEmpty(field.Attribute.DisplayName)
                ? field.Field.Name
                : field.Attribute.DisplayName;
            
            string targetName = field.Target.ToString();
            
            // C. 绘制UI容器
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // 盒子样式
            {
                // D. 标题行
                EditorGUILayout.LabelField($"{displayName} : {targetName}", EditorStyles.boldLabel);
                
                // E. 值行
                EditorGUILayout.BeginHorizontal();
                {
                    // 值显示
                    EditorGUILayout.LabelField(value?.ToString() ?? "null");
                    
                    // 类型显示
                    EditorGUILayout.LabelField(
                        $"({field.Field.FieldType.Name})", 
                        EditorStyles.miniLabel, // 小号字体
                        GUILayout.Width(80));    // 固定宽度
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        catch
        {
            // F. 异常处理（目标已销毁）
            EditorGUILayout.LabelField(
                $"{field.Field.Name} (目标已销毁)", 
                EditorStyles.miniLabel);
        }
    }
    
    
    //更新GUI
    // 添加刷新控制变量
    private double _lastUpdateTime;
    private const double UPDATE_INTERVAL = 0.1; // 每秒刷新10次
    
    void OnEnable()
    {
        // 添加编辑器更新回调
        EditorApplication.update += OnEditorUpdate;
        
        // 初始更新时间戳
        _lastUpdateTime = EditorApplication.timeSinceStartup;
    }
    
    void OnDisable()
    {
        // 移除回调
        EditorApplication.update -= OnEditorUpdate;
    }
    
    // 新增：编辑器每帧更新
    private void OnEditorUpdate()
    {
        // 计算时间差
        double currentTime = EditorApplication.timeSinceStartup;
        double delta = currentTime - _lastUpdateTime;
        
        // 达到刷新间隔时重绘
        if (delta >= UPDATE_INTERVAL)
        {
            Repaint(); // 请求重绘窗口
            _lastUpdateTime = currentTime;
        }
    }
}
#endif