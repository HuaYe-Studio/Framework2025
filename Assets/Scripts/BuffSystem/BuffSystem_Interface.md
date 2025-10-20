# BuffSystem 接口文档

> 更新日期：2025-10-01

## 目录结构

```
BuffSystem/
├─ BuffConstant/        # 各种常量定义（回调、名称、标签）
├─ BuffDesign/          # Buff 核心接口与运行时逻辑
├─ BuffCallbackImpl/    # 默认回调实现示例
├─ BuffModel/           # Buff 配置存储
└─ Test/                # 使用示例脚本
```

## 核心流程概览

1. **配置阶段**：在 `BuffModel` 中注册 `BuffData`，指定基础属性、回调模块与标签。
2. **运行阶段**：外部模块通过 `BuffManager.AddBuff` 施加 Buff，`BuffManager` 负责管理持续时间、Tick 与堆叠规则。
3. **回调执行**：当回调时机满足时（如 `OnTick`），`BuffData` 调用绑定的 `BuffModule`，也可触发运行时自定义委托。
4. **清理阶段**：通过持续时间耗尽或显式调用 `RemoveBuff`，按策略触发移除流程。

---

## 常量模块 `BuffConstant/BuffConstant.cs`

### `BuffConstant.BuffCallback`

| 常量                                              | 说明                |
| ------------------------------------------------- | ------------------- |
| `ON_CREATE`                                       | Buff 创建完成时触发 |
| `ON_TICK`                                         | Tick 间隔到达时触发 |
| `ON_REMOVE`                                       | Buff 完全移除时触发 |
| `ON_ADD_STACK`                                    | 层数增加时触发      |
| `ON_REDUCE_STACK`                                 | 层数减少时触发      |
| `ON_KILL` / `ON_BE_KILL` / `ON_HIT` / `ON_BE_HIT` | 业务扩展示例        |

### `BuffConstant.BuffName`

| 常量                         | 示例含义                        |
| ---------------------------- | ------------------------------- |
| `CAST_POISON`                | 施毒 Buff（区域类）             |
| `POISON`                     | 中毒 Buff（目标身上的持续效果） |
| `CAST_ELECTRIC` / `ELECTRIC` | 电击相关示例                    |

> 建议所有新增 Buff 都在此补充常量，避免硬编码。

### `BuffConstant.BuffTag`

| 常量            | 含义        |
| --------------- | ----------- |
| `VISIBLE_IN_UI` | 对玩家可见  |
| `DEBUFF`        | 减益效果    |
| `BOOST_BUFF`    | 增益效果    |
| `AREA_BUFF`     | 区域性 Buff |

标签可组合使用，便于过滤或批量操作。

---

## Buff 配置与回调（`BuffDesign`）

### `BuffData`

| 字段                  | 类型               | 默认    | 说明                        |
| --------------------- | ------------------ | ------- | --------------------------- |
| `Id`                  | `string`           | -       | Buff 唯一标识               |
| `Desc`                | `string`           | -       | 文本描述                    |
| `Icon`                | `Sprite`           | `null`  | UI 图标                     |
| `MaxStack`            | `int`              | `1`     | 最大叠层数                  |
| `IsForever`           | `bool`             | `false` | 是否永久（忽略 `Duration`） |
| `Duration`            | `float`            | `0`     | 持续时间（秒）              |
| `TickTime`            | `float`            | `-1`    | Tick 间隔，为负表示无 Tick  |
| `BuffUpdateEnum`      | `enum`             | -       | 叠层策略，详见下节          |
| `BuffRemoveEnum`      | `enum`             | -       | 移除策略，详见下节          |
| `TriggerTickOnCreate` | `bool`             | `false` | 创建时是否立即触发 `OnTick` |
| `BuffModules`         | `List<BuffModule>` | `new()` | 绑定的回调模块集合          |
| `Tags`                | `List<string>`     | `new()` | 自定义标签                  |

关键方法：

- `ExistBuffModule(string callbackName)`：是否存在指定回调。
- `ExistTag(string tag)`：Buff 是否含某个标签。
- `TriggerBuffModule(string callbackName, BuffRunTimeInfo info, params object[] paramList)`：先激活运行时委托，再逐个执行匹配的 `BuffModule`。

### `BuffModule`

```csharp
public struct BuffModule
{
    public string CallbackName; // 回调时机
    public BuffCallback Callback; // 对应逻辑
}
```

### `BuffCallback`

- 基类：`public abstract class BuffCallback`
- 覆写 `Apply(BuffRunTimeInfo info, params object[] customParams)` 实现具体行为。

#### 默认实现示例（`BuffCallbackImpl`）

| 类                  | 构造参数          | Apply 行为                                                           |
| ------------------- | ----------------- | -------------------------------------------------------------------- |
| `CastBuff2Target`   | `string buffId`   | 在目标对象上的 `BuffManager` 添加指定 Buff                           |
| `CastDamageByStack` | `float baseValue` | 按 `CurStack * baseValue` 计算伤害（示例中输出日志，可接入血量系统） |

### Buff 策略枚举

- `BuffUpdateEnum`
  - `AddTime`：延长剩余持续时间。
  - `ReplaceAndAddStack`：重置持续时间并尝试叠层。
  - `KeepAndAddStack`：保持剩余时间，仅叠层。
- `BuffRemoveEnum`
  - `Clear`：直接清除，触发 `OnReduceStack` → `OnRemove`。
  - `Reduce`：层数 -1 并刷新持续时间；若层数降至 0 则再触发 `OnRemove`。

### 运行时接口

#### `BuffRunTimeInfo`

| 成员                                       | 说明                         |
| ------------------------------------------ | ---------------------------- |
| `BuffData` / `Creator` / `Target`          | Buff 配置与关联对象          |
| `DurationTimer` / `TickTimer` / `CurStack` | 只读属性（通过显式接口实现） |
| `RegisterBuffAction(string, Action)`       | 注册运行时回调，若重复则追加 |
| `TriggerBuffAction(string)`                | 触发已注册委托               |

> `BuffRunTimeInfo` 同时实现 `IBuffTicker`（管理持续时间、Tick、层数）与 `IBuffAction`（自定义委托）。

#### `IBuffManager`

| 方法                                         | 用途                          |
| -------------------------------------------- | ----------------------------- |
| `ClearBuff()`                                | 清除所有 Buff（含待添加队列） |
| `AddBuff(string buffId, GameObject creator)` | 施加或刷新 Buff               |
| `RemoveBuff(string buffId)`                  | 按 ID 移除指定 Buff           |

#### `IBuffTicker`

约束 `BuffManager` 持有的内部计时器：`float DurationTimer { get; set; }`、`float TickTimer { get; set; }`、`int CurStack { get; set; }`。

#### `IBuffAction`

开放给外部系统注册/触发自定义行为：

```csharp
void RegisterBuffAction(string callbackName, Action action);
void TriggerBuffAction(string callbackName);
```

---

## 核心管理器 `BuffDesign/BuffManager.cs`

- 继承 `MonoBehaviour`，实现 `IBuffManager`。
- 内部维护两个容器：
  - `_buffRunTimeInfos`：当前 Buff 列表。
  - `_buff2Add`：待添加 Buff 队列（避免遍历中修改集合）。

### 生命周期

- `Update()`：
  1. 对每个 Buff 更新 `DurationTimer` / `TickTimer`，到期后加入删除列表。
  2. 触发满足条件的 `OnTick`，并重置 Tick 计时。
  3. 按删除列表逐个执行移除流程。
  4. 将 `_buff2Add` 中的 Buff 注入正表。

### Buff 添加逻辑

- 创建新 Buff 时：
  - 初始化 `DurationTimer` 和 `TickTimer`。
  - 触发 `OnCreate`，必要时立即执行一次 `OnTick`。
- 已存在时按 `BuffUpdateEnum` 调整持续时间及层数，并触发 `OnAddStack`。

### Buff 移除逻辑

- 优先按引用查找，找不到则按 `BuffData.Id` 匹配。
- `Clear`：触发 `OnReduceStack` → `OnRemove` → 从列表移除。
- `Reduce`：层数 -1 → 重置持续时间 → 触发 `OnReduceStack`；若层数 ≤ 0 再执行 `OnRemove` 并移除。

### 其他公共方法

- `GetBuffs()`：返回当前 Buff 的列表副本。
- `BuffExist(string buffId)`：校验 Buff 是否存在（包含空值防御）。
- `TriggerCustom(string callBackName, params object[] paramList)`：对所有 Buff 广播自定义回调。

---

## Buff 配置仓库 `BuffModel/BuffModel.cs`

- 静态字典 `_buffDatas` 缓存所有配置。
- `GetBuffData(string buffId)`：读取配置，未定义则返回 `null`。

### 示例配置

```csharp
BuffConstant.BuffConstant.BuffName.POISON => new BuffData
{
    MaxStack            = 5,
    Duration            = 1f,
    TickTime            = 1f,
    BuffUpdateEnum      = BuffUpdateEnum.ReplaceAndAddStack,
    BuffRemoveEnum      = BuffRemoveEnum.Reduce,
    BuffModules = new List<BuffModule>
    {
        new() { CallbackName = ON_TICK, Callback = new CastDamageByStack(1f) }
    },
    Tags = { DEBUFF, VISIBLE_IN_UI }
};
```

- `CastPoison` Buff 使用 `CastBuff2Target` 模块定时向目标施加 `Poison`。
- 可扩展：新增字典项或改造为从外部数据源加载。

---

## 测试脚本 `Test/BuffTest.cs`

- 绑定在拥有 `BuffManager` 的对象上。
- `Awake` 时缓存 `BuffManager`，`Update` 监听 `P` 键：
  - 进入毒区：`AddBuff(CAST_POISON, gameObject)`。
  - 离开毒区：`RemoveBuff(CAST_POISON)`。
- 通过日志模拟简单流程。

---

## 使用流程示例

```csharp
// 1. 在对象上挂载 BuffManager 组件
var manager = gameObject.GetComponent<BuffManager>();

// 2. 施加 Buff（creator 可为当前对象或来源对象）
manager.AddBuff(BuffConstant.BuffConstant.BuffName.POISON, creator);

// 3. 监听 Buff 事件（在新增 Buff 后取得运行时信息并注册）
foreach (var info in manager.GetBuffs())
{
    info.RegisterBuffAction(BuffConstant.BuffConstant.BuffCallback.ON_REMOVE, () =>
    {
        Debug.Log($"{info.BuffData.Id} 已结束");
    });
}

// 4. 在合适的时机移除 Buff
manager.RemoveBuff(BuffConstant.BuffConstant.BuffName.POISON);
```

---

## 扩展建议与注意事项

- **配置来源**：根据项目规模，可改造为 ScriptableObject、表格或服务器下发数据。
- **回调模块**：推荐继承 `BuffCallback` 并组合 `BuffModule`，保持配置与逻辑解耦。
- **标签系统**：利用 `Tags` 搭配过滤逻辑（如 UI 分类、数值调整、条件触发）。
- **线程安全**：当前实现假设运行在 Unity 主线程，若有并发需求需自行加锁或队列化。
- **调试**：`BuffManager.GetBuffs()` 返回副本，避免外部修改；若需实时观察可扩展调试界面。
