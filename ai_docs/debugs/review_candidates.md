# 待复验点清单

本文档用于集中记录当前在文档分析过程中识别到的“值得继续做完整逻辑复验”的实现点。

## 使用方式

- 本页不是最终结论页，而是排查入口页。
- 这里只记录：
  - 位置
  - 当前观察
  - 为什么值得复验
  - 建议优先级

## 高优先级

### 1. `HttpHelper.HttpGetAsync<TReturnDataModel>` 调用了 `HttpPostAsync`

- 位置：
  - `src/Commons/Lanymy.Common.Helpers.HttpHelper/HttpHelper.cs`
- 当前观察：
  - 泛型版 `HttpGetAsync<TReturnDataModel>` 内部是：
    - `var html = await HttpPostAsync(url, parameters);`
- 为什么值得复验：
  - 从语义看，这很像明显的实现错误或历史复制粘贴残留。
  - 如果这里确实在运行中被调用，行为会与方法名严重不一致。
- 优先级：
  - 高

### 2. `CustomMemoryCache.SetValue` 可能没有覆盖新值

- 位置：
  - `src/Commons/Lanymy.Common.Instruments.Cache.CustomMemoryCache/CustomMemoryCache.cs`
- 当前观察：
  - `AddOrUpdate(key, value, (k, v) => v)`
- 为什么值得复验：
  - 更新分支返回的是旧值 `v`，不是新值 `value`。
  - 如果没有其他外围约束，这意味着同 key 更新可能无效。
- 优先级：
  - 高

### 3. `BaseUdpClient.Close()` 中的空判断方向可疑

- 位置：
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs`
- 当前观察：
  - 关闭时使用：
    - `if (_ReceiveWorkTaskQueue.IfIsNull())`
    - `if (_SendWorkTaskQueue.IfIsNull())`
- 为什么值得复验：
  - 按语义看，应该是“非空才 Stop/Dispose”。
  - 当前条件很像写反，会导致真正存在的队列没有被停止释放。
- 优先级：
  - 高

## 中优先级

### 4. `JsonNetJsonSerializer` 时间格式候选点已复验为“注释误导”，不是实现错误

- 位置：
  - `src/Commons/Lanymy.Common.Instruments.Serializer.JsonNetJsonSerializer/JsonNetJsonSerializer.cs`
  - `src/Commons/Lanymy.Common.ConstKeys/DateTimeFormatKeys.cs`
- 当前观察：
  - `JsonNetJsonSerializer.DATE_FORMAT_STRING` 实际引用的是 `DateTimeFormatKeys.DATE_TIME_FORMAT_1`
  - 常量值本身已经是 `yyyy-MM-dd HH:mm:ss.fff`
  - 问题出在两处 XML 注释仍写成了 `hh`
- 为什么值得复验：
  - 这一项最初看起来像运行时格式错误，但继续追到常量定义后，确认只是注释与实现不一致。
  - 若不回写结论，后续阅读者容易再次把它当成真实实现缺陷。
- 优先级：
  - 已完成复验，可降出主修复队列

### 5. `BaseTcpClient` 不支持重启是否符合预期

- 位置：
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs`
- 当前观察：
  - `_IsFirstStart` 在关闭后阻止再次 `Start()`
- 为什么值得复验：
  - 继续复验后，当前仓库内没有搜到该抽象类的派生实现，也没有明显的直接调用面。
  - 现阶段更像“一次性连接对象”的明确设计限制，而不是已经坐实的实现错误。
  - 若后续出现真实调用方把它当作“可重连客户端”使用，再升级为修复项更稳妥。
- 优先级：
  - 中，暂作为设计约束观察点保留

### 6. `BaseTcpServer` 收包循环递归已改为循环实现

- 位置：
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs`
- 当前观察：
  - `OnServerClientReceiveDataLoopEvent(...)` 原先递归调用自身处理连续包
- 为什么值得复验：
  - 原实现语义上成立，但极端连续大批量拆包时会额外放大调用深度风险。
  - 该点现已改成 `while` 循环，保留行为但去掉递归层级增长。
- 优先级：
  - 已完成修正，可移出主候选队列

## 低优先级

### 7. `ImageHelper.SaveBitmapToImageFile` 的对象释放副作用已修正

- 位置：
  - `src/Commons/Lanymy.Common.Helpers.ImageHelper/ImageHelper.cs`
- 当前观察：
  - 原实现使用 `using (var image = encryptImage)`
  - 当前已改为仅执行 `encryptImage.Save(...)`
- 为什么值得复验：
  - 继续追调用链后，`LanymyCrypto` 会在保存完成后显式 `Dispose()` 同一个 `Bitmap`
  - 原 Helper 的隐式释放会让所有权语义变得混乱，也增加调用方误判对象生命周期的概率
- 优先级：
  - 已完成修正，可移出主候选队列

### 8. `PathHelper` 路径类型判断已增强为“存在性优先 + 语法优先 + 扩展名兜底”

- 位置：
  - `src/Commons/Lanymy.Common.Helpers.PathHelper/PathHelper.cs`
- 当前观察：
  - 原实现多处直接把“有扩展名”视为文件路径
  - 当前已改为优先判断：
    - 目录分隔符结尾
    - `Directory.Exists(path)`
    - `File.Exists(path)`
  - 只有无法确认时才退回 `Path.HasExtension(path)`
- 为什么值得复验：
  - 继续追调用面后，这组能力会影响 `InitDirectoryPath`、`GetFolderPath`、`GetPathType`、`GetFileName`
  - 如果不增强，已存在的无扩展名文件和带点号目录都可能被误判
- 优先级：
  - 已完成增强，并补了最小单测锁定行为

## 建议复验顺序

1. `HttpHelper`
2. `CustomMemoryCache`
3. `BaseUdpClient`
4. `BaseTcpClient`
5. 已无剩余明确修复项

## 后续建议

- 如果下一步开始进入“完整逻辑复验”阶段，可以按本页顺序逐项核实，并把结果回写到 `debugs/` 对应专题目录。
- 第一轮已落地的静态复验结果见 [review_round_01.md](./review_round_01.md)。
