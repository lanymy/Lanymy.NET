# 第一轮静态复验记录

本文档记录基于源码静态阅读完成的第一轮高优先级复验结果。

## 复验范围

- `HttpHelper`
- `CustomMemoryCache`
- `BaseUdpClient.Close()`

## 结论总览

- 已基本确认实现问题：
  - `HttpHelper.HttpGetAsync<TReturnDataModel>` 调用了错误的方法
  - `CustomMemoryCache.SetValue` 更新分支没有写入新值
  - `BaseUdpClient.Close()` 的队列释放条件判断方向写反
- 当前仍属于静态复验结论：
  - 尚未通过运行测试进一步验证触发路径
  - 但从源码本身已经足够判断这三项都具备较高修复价值

## 1. HttpHelper 泛型 GET 实现错误

- 文件：
  - [HttpHelper.cs:L99-L105](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.HttpHelper/HttpHelper.cs#L99-L105)
- 当前代码：
  ```csharp
  public static async Task<TReturnDataModel> HttpGetAsync<TReturnDataModel>(string url, IReadOnlyDictionary<string, object> parameters = null) where TReturnDataModel : class
  {
      var html = await HttpPostAsync(url, parameters);
      return JsonSerializeHelper.DeserializeFromJson<TReturnDataModel>(html);
  }
  ```
- 复验结论：
  - 这里不是“命名和实现风格略有偏差”，而是明确的行为错误。
  - 同文件中字符串版 `HttpGetAsync` 已经正确调用 `httpClient.GetAsync(...)`，泛型版却走了 `HttpPostAsync(...)`。
- 影响：
  - 所有调用泛型版 GET 的路径，都会实际走 POST 语义。
  - 若服务端对 GET / POST 区分严格，行为会直接错误。

## 2. HttpHelper multipart 泛型重载也存在错误转发

- 文件：
  - [HttpHelper.cs:L264-L268](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.HttpHelper/HttpHelper.cs#L264-L268)
- 当前代码：
  ```csharp
  public static async Task<TReturnDataModel> HttpPostMultipartFormDataAsync<TReturnDataModel>(string url, List<BaseAttachmentInfoModel> attachmentList) where TReturnDataModel : class
  {
      var html = await HttpPostAsync(url, attachmentList);
      return JsonSerializeHelper.DeserializeFromJson<TReturnDataModel>(html);
  }
  ```
- 复验结论：
  - 这里也不是调用 multipart 自身的字符串版，而是转发到了普通 `HttpPostAsync<TPostDataModel>`。
  - 这会把附件列表直接 JSON 序列化，而不是走 `MultipartFormDataContent` 提交。
- 影响：
  - 泛型版 multipart 接口和非泛型版 multipart 接口行为不一致。
  - 若调用方使用泛型版，附件上传行为大概率会失真。

## 3. CustomMemoryCache 更新分支确认不会覆盖新值

- 文件：
  - [CustomMemoryCache.cs:L32-L35](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Cache.CustomMemoryCache/CustomMemoryCache.cs#L32-L35)
- 当前代码：
  ```csharp
  public override void SetValue(string key, object value)
  {
      _DicCache.AddOrUpdate(key, value, (k, v) => v);
  }
  ```
- 复验结论：
  - `AddOrUpdate` 的第三个参数是“已有键时如何生成新值”。
  - 当前返回旧值 `v`，不是新值 `value`。
  - 因此同 key 重复写入时，缓存值不会被更新。
- 关联影响点：
  - [EnumHelper.cs:L47-L56](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.EnumHelper/EnumHelper.cs#L47-L56) 使用了 `CustomMemoryCache` 做枚举映射缓存。
  - 虽然 `EnumHelper` 当前路径更像“首次写入后长期读取”，不一定立刻暴露错误，但这个缓存实现本身已经不符合常规预期。

## 4. BaseUdpClient.Close() 的释放条件判断方向确认写反

- 文件：
  - [BaseUdpClient.cs:L291-L312](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L291-L312)
- 当前代码：
  ```csharp
  if (_ReceiveWorkTaskQueue.IfIsNull())
  {
      _ReceiveWorkTaskQueue.StopAsync().Wait();
      _ReceiveWorkTaskQueue.Dispose();
      _ReceiveWorkTaskQueue = null;
  }

  if (_SendWorkTaskQueue.IfIsNull())
  {
      _SendWorkTaskQueue.StopAsync().Wait();
      _SendWorkTaskQueue.Dispose();
      _SendWorkTaskQueue = null;
  }
  ```
- 关联扩展实现：
  - [ObjectExtensions.cs:L24-L27](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.ExtensionFunctions.ObjectExtensions/ObjectExtensions.cs#L24-L27)
  ```csharp
  public static bool IfIsNull(this object o)
  {
      return null == o;
  }
  ```
- 复验结论：
  - `IfIsNull()` 为真时表示对象是 `null`。
  - 当前代码只有在队列已经是 `null` 的时候才会执行 `StopAsync()` 和 `Dispose()`。
  - 这不仅方向写反，而且一旦真进到分支里还可能触发空引用异常，只是被外层 `try/catch` 吞掉。
- 影响：
  - 正常运行过的 UDP 客户端在关闭时，收发队列大概率没有被正确停止和释放。

## 当前建议

### 建议优先修复顺序

1. `HttpHelper` 两个错误转发
2. `CustomMemoryCache.SetValue`
3. `BaseUdpClient.Close()`

### 修复前建议

- 先把本页的三项结论同步到对应专题文档中，避免后续阅读者仍把它们当作“仅候选问题”。
- 如果下一步直接进入修复，建议同时补最小验证用例或至少补一页修复验证记录。

## 后续补充

- `JsonNetJsonSerializer` 相关的“`hh` / `HH`”疑点已继续复验：
  - 实际运行时格式来自 `DateTimeFormatKeys.DATE_TIME_FORMAT_1`
  - 常量值本身已经是 `yyyy-MM-dd HH:mm:ss.fff`
  - 当前问题不是实现错误，而是 `JsonNetJsonSerializer.cs` 与 `DateTimeFormatKeys.cs` 的注释仍写成了 `hh`
- 上述注释误导已修正，不再作为后续实现缺陷处理。
- `BaseTcpServer` 的连续拆包路径已继续处理：
  - `OnServerClientReceiveDataLoopEvent(...)` 原实现使用递归处理连续包
  - 当前已改成等价的 `while` 循环实现，移除了不必要的调用深度增长
- `BaseTcpClient` 的“不可重启”限制暂未改动：
  - 继续检索后，当前仓库内没有发现该抽象类的派生实现或明确调用面
  - 现阶段更接近设计限制，而不是能直接坐实的实现缺陷
- `ImageHelper.SaveBitmapToImageFile(...)` 的对象所有权问题已继续处理：
  - 原实现会在方法内部通过 `using` 释放传入的 `Bitmap`
  - 继续追调用链后，`LanymyCrypto` 在保存完成后还会显式 `Dispose()` 同一个对象
  - 当前已改成“只负责保存，不负责释放入参”，把对象生命周期还给调用方统一管理
- `PathHelper` 的路径类型判断已继续增强：
  - 原实现主要依赖 `Path.HasExtension(...)` 推断“文件路径还是目录路径”
  - 当前已改成“存在性优先 + 目录语法优先 + 扩展名兜底”
  - 同时补了最小单测，覆盖“已存在无扩展名文件”和“已存在带点号目录”两类边界

## 配套汇总

- 当前这一阶段已实际落地的源码修复与验证状态，已单独汇总到 [fixed_issues_summary.md](./fixed_issues_summary.md)。
