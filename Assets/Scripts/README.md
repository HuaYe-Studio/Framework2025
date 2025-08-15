# UI管理系统使用文档

## 概述

UI管理系统是一个专为Unity设计的完整UI管理解决方案，提供了层级化的UI界面管理、面板生命周期控制、状态管理和事件系统。系统采用模块化设计，支持灵活的UI组织和管理。

## 核心架构

### 主要组件

- **UIManager**: UI管理器核心实现，负责所有UI操作
- **IUIManager**: UI管理器接口，定义了完整的UI管理API
- **BaseUIPanelView**: UI面板基类，提供标准的面板功能
- **IUIPanel**: UI面板接口，定义面板生命周期
- **UIConfiger**: UI配置器，提供可视化配置和调试工具
- **UIPanelInfo**: 面板信息类，记录面板的状态和元数据
- **UIPanelState**: 面板状态枚举（Hidden、Shown、Destroyed）
- **UILayer**: 预定义UI层级枚举

### 层级系统

UI系统采用分层管理机制，每个层级对应一个Canvas：

```csharp
public enum UILayer
{
    Background = 0,    // 背景层
    Main = 100,        // 主界面层
    Window = 200,      // 普通窗口层
    Popup = 300,       // 弹窗层
    Tips = 400,        // 提示信息层
    Top = 9999         // 最高层
}
```

### 面板状态

每个面板都有明确的状态管理：

```csharp
public enum UIPanelState
{
    Hidden,     // 已创建但未显示
    Shown,      // 正在显示中
    Destroyed   // 已被销毁
}
```

## 快速开始

### 1. 系统初始化

UI系统需要在场景中配置UIConfiger组件进行初始化：

```csharp
// 通过BasePresenter获取UIManager
var uiManager = UIManager.Instance;

// 或者直接使用UIConfiger
// UIConfiger会自动处理初始化和配置
```

### 2. 创建UI面板

继承BaseUIPanelView创建你的UI面板：

```csharp
public class MainMenuPanelView : BaseUIPanelView
{
    public Button startButton;
    public Button settingsButton;
    public Button exitButton;
}

public class MainMenuPresenter : BasePresenter<MainMenuPanelView>
{
    void Start()
    {
        // 面板初始化时的逻辑
        View.startButton.onClick.AddListener(OnStartButtonClick);
        View.settingsButton.onClick.AddListener(OnSettingsButtonClick);
        View.exitButton.onClick.AddListener(OnExitButtonClick);

        View.OnShowAfter += OnPanelShowAfter;
    }

    private void OnPanelShowAfter(object arg)
    {
        // 面板显示后的逻辑
        Debug.Log("主菜单面板显示完成");
    }

    private void OnStartButtonClick()
    {
        // 开始游戏逻辑
    }

    private void OnSettingsButtonClick()
    {
        // 显示设置面板
        var uiManager = GetService<IUIManager>();
        uiManager.ShowPanel<MainMenuPanelView>();
    }

    private void OnExitButtonClick()
    {
        // 退出游戏逻辑
    }
}
```

### 3. 基本操作

```csharp
// 获取UI管理器
var uiManager = UIManager.Instance;

// 添加面板
var mainMenuPanel = uiManager.AddPanel<MainMenuPanel>(prefab, UILayer.Main);

// 显示面板
uiManager.ShowPanel<MainMenuPanel>();

// 隐藏面板
uiManager.HidePanel<MainMenuPanel>();

// 移除面板
uiManager.RemovePanel<MainMenuPanel>();
```

## 详细功能说明

### 层级管理

#### 创建层级

```csharp
// 创建新层级
uiManager.AddLayer("CustomLayer", 150);

// 添加现有Canvas作为层级
uiManager.AddLayer(existingCanvas, "MyLayer");

// 使用预定义层级
uiManager.AddLayer(UILayer.Popup.ToString(), (int)UILayer.Popup);
```

#### 层级操作

```csharp
// 检查层级是否存在
bool hasLayer = uiManager.HasLayer("Main");

// 获取层级Canvas
Canvas mainLayer = uiManager.GetLayer("Main");

// 移除层级
uiManager.RemoveLayer("CustomLayer");

// 弹出层级中的所有面板
uiManager.PopAllPanel("Popup");
```

### 面板管理

#### 添加面板

```csharp
// 通过预制体添加面板
var panel = uiManager.AddPanel<MyPanel>(prefab, "Main");

// 带自定义标识
var panel = uiManager.AddPanel<MyPanel>(prefab, "Main", "custom_key");

// 直接添加面板实例
uiManager.AddPanel(panelInstance, "Main");

// 使用枚举层级（扩展方法）
var panel = uiManager.AddPanel<MyPanel>(prefab, UILayer.Window);
```

#### 显示和隐藏

```csharp
// 显示面板
var panel = uiManager.ShowPanel<MyPanel>();

// 带参数显示
var panel = uiManager.ShowPanel<MyPanel>(new { level = 1, score = 100 });

// 通过标识显示
var panel = uiManager.ShowPanel("MyPanel_custom");

// 隐藏面板
uiManager.HidePanel<MyPanel>();
uiManager.HidePanel("MyPanel_custom");
uiManager.HidePanel(panelInstance);
```

#### 查询面板

```csharp
// 检查面板是否存在
bool hasPanel = uiManager.HasPanel<MyPanel>();
bool hasPanelByKey = uiManager.HasPanel("MyPanel_custom");

// 获取面板实例
var panel = uiManager.GetPanel<MyPanel>();
var panelByKey = uiManager.GetPanel("MyPanel_custom");

// 获取面板数组
var allPanels = uiManager.GetAllPanels();
var activePanels = uiManager.GetAllActivePanels();
var layerPanels = uiManager.GetActivePanels("Main");
```

### 状态管理

#### 状态查询

```csharp
// 获取面板状态
var state = uiManager.GetPanelState<MyPanel>();
var stateByKey = uiManager.GetPanelState("MyPanel_custom");

// 检查状态转换合法性
bool canShow = uiManager.CanPanelTransitionTo("MyPanel", UIPanelState.Shown);

// 按状态查询面板
var shownPanels = uiManager.GetPanelsByState(UIPanelState.Shown);
var layerShownPanels = uiManager.GetPanelsByState("Main", UIPanelState.Shown);

// 获取状态统计
var statistics = uiManager.GetPanelStateStatistics();
foreach (var kvp in statistics)
{
    Debug.Log($"{kvp.Key}: {kvp.Value} 个面板");
}
```

### 事件系统

UIManager提供了完整的事件系统，可以监听面板和层级的各种状态变化：

```csharp
// 订阅面板事件
uiManager.OnPanelAdded += (key, panel) => {
    Debug.Log($"面板添加: {key}");
};

uiManager.OnPanelShown += (key, panel) => {
    Debug.Log($"面板显示: {key}");
};

uiManager.OnPanelHidden += (key, panel) => {
    Debug.Log($"面板隐藏: {key}");
};

uiManager.OnPanelStateChanged += (key, panel, oldState, newState) => {
    Debug.Log($"面板状态变化: {key} {oldState} -> {newState}");
};

// 订阅层级事件
uiManager.OnLayerAdded += (layerName, canvas) => {
    Debug.Log($"层级添加: {layerName}");
};

uiManager.OnLayerRemoved += (layerName, canvas) => {
    Debug.Log($"层级移除: {layerName}");
};
```

### 面板生命周期

BaseUIPanelView提供了丰富的生命周期回调：

```csharp
public class MyPanel : BaseUIPanelView
{
    protected override void OnPanelInitialized()
    {
        // 面板初始化完成
        Debug.Log("面板初始化");
    }

    protected override void OnPanelShowBefore(object arg)
    {
        // 面板显示前
        Debug.Log($"准备显示面板，参数: {arg}");
    }

    protected override void OnPanelShowAfter(object arg)
    {
        // 面板显示后
        Debug.Log("面板显示完成");
    }

    protected override void OnPanelHideBefore(object arg)
    {
        // 面板隐藏前
        Debug.Log("准备隐藏面板");
    }

    protected override void OnPanelHideAfter(object arg)
    {
        // 面板隐藏后
        Debug.Log("面板隐藏完成");
    }

    protected override void OnPanelDestroy()
    {
        // 面板销毁前
        Debug.Log("面板即将销毁");
    }
}
```

### 高级功能

#### 栈式面板管理

```csharp
// 弹出单个面板
uiManager.PopPanel("Window");

// 弹出多个面板
uiManager.PopPanels("Window", 3);

// 弹出层级所有面板
uiManager.PopAllPanel("Popup");

// 弹出所有层级的所有面板
uiManager.PopAllLayersPanels();
```

#### 批量清理

```csharp
// 清理指定层级的所有面板
uiManager.ClearPanels("Window");

// 清理所有面板
uiManager.ClearAllPanels();
```

## 配置和调试

### UIConfiger配置

UIConfiger提供了可视化的配置界面，包括：

- **初始化选项**: 系统启动时的自动配置
- **默认层级**: 预定义的UI层级设置
- **默认面板**: 自动加载的面板列表
- **调试信息**: 实时显示系统状态
- **编辑器工具**: 调试和测试按钮

### 配置参数说明

```csharp
[Header("初始化选项")]
[SerializeField] private bool isClearLayer;      // 清空现有层级
[SerializeField] private bool isClearPanel;     // 清空现有面板
[SerializeField] private bool isAddLayer;       // 自动添加层级
[SerializeField] private bool isAddPanel;       // 自动添加面板
[SerializeField] private bool createDefaultLayers = true;  // 创建默认层级
[SerializeField] private bool addDefaultPanels = true;     // 添加默认面板
```

### 调试工具

UIConfiger提供了丰富的调试工具：

- **统计所有面板**: 显示所有面板的状态信息
- **统计所有层级**: 显示所有层级的详细信息
- **统计面板状态**: 显示各状态面板的数量统计
- **清理所有面板**: 一键清理所有面板

## 最佳实践

### 1. 面板设计原则

```csharp
public class GameMenuPanel : BaseUIPanelView
{
    [Header("面板配置")]
    [SerializeField] private bool autoHideOnEscape = true;
    
    [Header("UI组件")]
    [SerializeField] private Button[] menuButtons;
    
    protected override void OnPanelInitialized()
    {
        // 集中处理UI组件的事件绑定
        InitializeButtons();
        
        // 设置面板特定的配置
        if (autoHideOnEscape)
        {
            // 添加ESC键隐藏逻辑
        }
    }
    
    private void InitializeButtons()
    {
        foreach (var button in menuButtons)
        {
            if (button != null)
            {
                button.onClick.AddListener(() => OnMenuButtonClick(button.name));
            }
        }
    }
    
    protected override void OnPanelDestroy()
    {
        // 清理资源和事件监听
        foreach (var button in menuButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }
}
```

### 2. 参数传递最佳实践

```csharp
// 定义参数数据结构
public class ShopPanelArgs
{
    public int shopType;
    public int playerLevel;
    public int playerGold;
}

public class ShopPanel : BaseUIPanelView
{
    protected override void OnPanelShowBefore(object arg)
    {
        if (arg is ShopPanelArgs shopArgs)
        {
            // 使用强类型参数
            InitializeShop(shopArgs);
        }
    }
    
    private void InitializeShop(ShopPanelArgs args)
    {
        // 根据参数初始化商店界面
    }
}

// 使用时
var shopArgs = new ShopPanelArgs 
{ 
    shopType = 1, 
    playerLevel = 10, 
    playerGold = 1000 
};
uiManager.ShowPanel<ShopPanel>(shopArgs);
```

### 3. 层级管理建议

```csharp
// 推荐的层级使用方式
public static class UILayers
{
    public const string BACKGROUND = "Background";
    public const string MAIN = "Main";
    public const string WINDOW = "Window";
    public const string POPUP = "Popup";
    public const string TIPS = "Tips";
    public const string TOP = "Top";
}

// 使用常量而不是硬编码字符串
uiManager.AddPanel<MainMenuPanel>(prefab, UILayers.MAIN);
```

### 4. 错误处理

```csharp
public class SafeUIOperations
{
    private readonly IUIManager _uiManager;
    
    public T ShowPanelSafely<T>(object arg = null) where T : class, IUIPanel
    {
        try
        {
            if (!_uiManager.HasPanel<T>())
            {
                Debug.LogWarning($"面板 {typeof(T).Name} 不存在，无法显示");
                return null;
            }
            
            return _uiManager.ShowPanel<T>(arg);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"显示面板 {typeof(T).Name} 时发生错误: {ex.Message}");
            return null;
        }
    }
}
```

## 常见问题解答

### Q: 如何处理面板显示动画？

A: 重写BaseUIPanelView的PerformShow和PerformHide方法：

```csharp
protected override void PerformShow(System.Action onComplete = null)
{
    // 自定义显示动画
    transform.localScale = Vector3.zero;
    transform.DOScale(Vector3.one, 0.3f)
        .SetEase(Ease.OutBack)
        .OnComplete(() => onComplete?.Invoke());
}

protected override void PerformHide(System.Action onComplete = null)
{
    // 自定义隐藏动画
    transform.DOScale(Vector3.zero, 0.2f)
        .OnComplete(() => onComplete?.Invoke());
}
```

### Q: 如何实现面板之间的通信？

A: 使用事件系统或者通过UIManager传递参数：

```csharp
// 方法1: 通过事件系统
public class PanelA : BaseUIPanelView
{
    public static event System.Action<int> OnDataChanged;
    
    private void UpdateData(int newData)
    {
        OnDataChanged?.Invoke(newData);
    }
}

public class PanelB : BaseUIPanelView
{
    protected override void OnPanelInitialized()
    {
        PanelA.OnDataChanged += OnDataReceived;
    }
    
    private void OnDataReceived(int data)
    {
        // 处理接收到的数据
    }
}

// 方法2: 通过ShowPanel传递参数
uiManager.HidePanel<PanelA>();
uiManager.ShowPanel<PanelB>(new { sourceData = someData });

//方法3: 通过事件中心
```

## 扩展和自定义

### 创建UI管理器扩展

```csharp
public static class MyUIManagerExtensions
{
    public static void ShowPanelWithFade<T>(this IUIManager uiManager, float fadeTime = 0.3f) 
        where T : class, IUIPanel
    {
        var panel = uiManager.ShowPanel<T>();
        if (panel is BaseUIPanelView basePanel)
        {
            // 添加淡入效果
            basePanel.CanvasGroup.alpha = 0f;
            basePanel.CanvasGroup.DOFade(1f, fadeTime);
        }
    }
}
```

## 总结

UI管理系统提供了：

- ✅ 完整的UI层级管理
- ✅ 灵活的面板生命周期控制
- ✅ 强大的状态管理和事件系统
- ✅ 可视化配置和调试工具
- ✅ 扩展友好的架构设计
- ✅ 丰富的API和工具方法

通过合理使用这些功能，可以构建出高效、可维护的UI系统。建议从简单的面板开始，逐步掌握各种高级功能。 