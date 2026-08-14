# Helper Result API Adoption Audit

本文档记录本轮对 `WithResult(...)` 严格入口调用面的盘点结果，目标是明确：

- 当前仓库里还有哪些地方在调用旧兼容入口
- 哪些调用点适合直接切到严格层
- 哪些调用点应继续保留兼容层

## 1. 盘点范围

本轮仅覆盖已完成“兼容层 / 严格层”改造的模块：

- `CompressionHelper`
- `FileHelper`
- `ProcessHelper`
- `EmailHelper`
- `NetworkHelper`
- `PcInfoHelper`
- `SecurityAesHelper`
- `SecurityHelper`（RSA 入口）

说明：

- 当前清单中的后续扫描与收口范围以 `src/Commons` 为主
- 本文档保留历史上已完成的 `AboutViewModel` adoption 记录，但后续建议不再把 `AppTests` 作为新的推进范围

## 2. 盘点结论

### 2.1 真实非测试调用点

当前扫描结果里，真实非测试调用点只有 2 类：

1. `AboutViewModel`
   - 文件：`src/UnitTests/Lanymy.Common.AppTests/Lanymy.Common.AppTests/ViewModels/AboutViewModel.cs`
   - 原调用：`NetworkHelper.GetLocalIpList()`
   - 处理结论：**已切到严格层**
   - 原因：
     - 该调用点属于展示/示例层，不需要兼容层的抛异常行为
     - 旧代码还存在“直接取第 0 个元素”的潜在越界风险
   - 当前改为：
     - `GetLocalIpListWithResult()`
     - 仅在 `IsSuccess` 且列表非空时继续取值

2. `FileSerializeHelper`
   - 文件：`src/Commons/Lanymy.Common.Helpers.SerializeHelper.File/FileSerializeHelper.cs`
   - 原调用：`FileHelper.CreateBinaryFile(...)`
   - 处理结论：**已补严格层，并把内部写入切到严格层**
   - 原因：
     - 该模块本身就是 helper-on-helper 组合封装，适合直接沿用现有模式补 `WithResult(...)`
     - 写入和读取两端都存在明确的异常诊断价值
   - 当前改为：
     - 新增 `SerializeToBytesFileWithResult(...)`
     - 新增 `DeserializeFromBytesFileWithResult(...)`
     - 内部写入改为走 `FileHelper.CreateBinaryFileWithResult(...)`

### 2.2 测试调用点

测试项目中仍大量保留旧兼容入口调用，这些调用点**本轮有意保留**，因为它们承担的是“锁住历史语义”的职责。

典型示例：

- `FileHelperTests`
  - 验证兼容层 `MoveFile(...)`、`DeleteFolder(...)`、`CreateBinaryFile(...)` 等历史边界
  - 并通过严格层测试锁定批量复制的批次级异常、请求项数量与逐项失败明细
  - 同时锁定 `ScheduleFileInfoModel` 空值抛异常、批量兼容层首项失败即中断、以及哈希 `offset` 的尾段计算语义
- `NetworkHelperTests`
  - 验证兼容层 `PingIP(...)`、`GetLocalIP()`、`GetIpAddressByIpString(...)` 的旧语义
- `PcInfoHelperTests`
  - 验证兼容层 `CreateShortcut(...)`、`GetIPV4()`、`GetLocalIpAddress()` 的旧语义
- `EmailHelperTests`
  - 验证旧入口 `SendEmail(...)` 仍然可用
  - 并通过严格层测试锁定“拆分发送失败”和“批量一次发送失败”两条路径都会返回逐收件人明细

### 2.3 `SerializeHelper.*` 专项扫描结论

本轮继续对 `SerializeHelper.*` 做了一次专项盘点，结论如下：

1. `JsonSerializeHelper`
   - 文件级 API 直接接触文件系统，具备明确的失败诊断价值
   - 处理结论：**已补严格层**
   - 新增：
     - `SerializeToJsonFileWithResult(...)`
     - `SerializeToJsonFileWithResultAsync(...)`
     - `DeserializeFromJsonFileWithResult(...)`
     - `DeserializeFromJsonFileWithResultAsync(...)`
   - 兼容层保留：
     - `DeserializeFromJsonFile(...)` / `DeserializeFromJsonFileAsync(...)` 在缺失文件时仍沿底层行为创建空文件并返回 `default(T)`

2. `BinarySerializeHelper`
   - 当前主要是“对象 <-> JSON 文本字节数组”的纯内存转换
   - 处理结论：**本轮不补结果层**

3. `DataTableSerializeHelper`
   - 当前主要是 `List<T>` 与 `DataTable` 的结构转换
   - 处理结论：**本轮不补结果层**

原因：

- `Binary / DataTable` 目前没有直接承担文件、网络、进程等外部边界
- 当前更适合保持轻量转换 helper 定位，避免为了结果模型而把纯转换层做重

### 2.4 `CompressionHelper` 与剩余 helper-on-helper 调用链结论

本轮继续对 `CompressionHelper` 和剩余 helper-on-helper 调用链做了一次专项盘点，结论如下：

1. `CompressionHelper`
   - 文件级 API 直接接触文件系统，具备明确的失败诊断价值
   - 处理结论：**已补严格层**
   - 新增：
     - `CompressSourceFileToCompressFileWithResult(...)`
     - `CompressSourceFileToCompressFileWithResultAsync(...)`
     - `DecompressSourceFileFromCompressFileWithResult(...)`
     - `DecompressSourceFileFromCompressFileWithResultAsync(...)`
   - 兼容层保留：
     - 源文件或压缩文件不存在时仍静默 no-op

2. 剩余 helper-on-helper 调用链
   - `BinarySerializeHelper`、`DataTableSerializeHelper`、`DeepCloneExtensions`
   - 处理结论：**本轮不继续扩散改造**
   - 原因：
     - 主要仍是纯内存结构转换
     - 暂未出现新的“外部边界 + 弱语义”高价值入口

### 2.5 `SecurityHelper / Crypto` 与 `SecurityAesHelper` 扫描结论

本轮继续对 `SecurityHelper`、`SecurityAesHelper` 与 `LanymyAesCrypto` 做了一次专项盘点，结论如下：

1. `SecurityHelper`
   - 主体入口大多已直接返回 `EncryptDigestInfoModel` / `EncryptStringFileDigestInfoModel` / `EncryptModelFileDigestInfoModel<T>` 等结果型对象
   - 处理结论：**本轮不重复补结果层**
   - 原因：
     - 现有主流程已具备 `IsSuccess` 和上下文诊断能力
     - 继续叠加 `WithResult(...)` 会形成重复抽象

2. `SecurityAesHelper`
   - 文件级 API `EncryptModelToFile(...)` / `DecryptModelFromFile<T>(...)` 仍停留在 `void / T` 语义
   - 处理结论：**已补严格层**
   - 新增：
     - `EncryptModelToFileWithResult(...)`
     - `DecryptModelFromFileWithResult<T>(...)`
   - 兼容层保留：
     - 文件写入失败继续抛异常
     - 文件不存在、解密失败、反序列化失败继续抛异常

3. 真实非测试调用面
   - 当前未发现仓库内新的非测试 `SecurityAesHelper` 文件调用点
   - 在明确排除 `AppTests` 之后，`src/Commons` 内也未发现新的真实 adoption 替换点
   - 处理结论：**本轮不做额外调用替换，只补齐严格层结果上下文**

4. 本轮额外收口
   - `SecurityAesFileOperationResultModel<T>`
     - 补齐 `ModelTypeName`
     - 补齐 `ModelTypeFullName`
     - 补齐 `ErrorMessage`
   - `EncryptModelToFileWithResult(...)` / `DecryptModelFromFileWithResult<T>(...)`
     - 成功时回填模型类型上下文
     - 失败时同步保留异常消息，避免上层只拿到 `Exception` 而没有轻量错误文本

### 2.6 `SecurityHelper` RSA 入口扫描结论

本轮继续对 `SecurityHelper` 中的 RSA 入口做了一次专项盘点，结论如下：

1. RSA 兼容层现状
   - `CreateRsaKeyBlobBase64String(...)`
   - `CreateRsaKeyBlobBytes(...)`
   - `RsaEncryptBytesToBytes(...)` / `RsaDecryptBytesFromBytes(...)`
   - `RsaEncryptStringToBase64String(...)` / `RsaDecryptStringFromBase64String(...)`
   - 其中密钥生成入口仍是 `out` 参数 + 直接抛异常
   - 加解密入口通过吞异常来保留 `null / 空字符串` 的历史语义

2. 处理结论：**已补严格层**
   - 新增：
     - `CreateRsaKeyBlobBase64StringWithResult(...)`
     - `CreateRsaKeyBlobBytesWithResult(...)`
     - `RsaEncryptBytesToBytesWithResult(...)`
     - `RsaDecryptBytesFromBytesWithResult(...)`
     - `RsaEncryptStringToBase64StringWithResult(...)`
     - `RsaDecryptStringFromBase64StringWithResult(...)`
   - 对应结果模型：
     - `SecurityRsaKeyBlobBytesResultModel`
     - `SecurityRsaKeyBlobStringResultModel`
     - `SecurityRsaBytesOperationResultModel`
     - `SecurityRsaStringOperationResultModel`

3. 兼容层保留
   - 密钥生成入口失败时继续直接抛异常
   - 二进制 RSA 入口失败时仍返回 `null`
   - 字符串 RSA 入口失败时仍返回空字符串

4. 真实非测试调用面
   - 当前未发现仓库内新的 RSA 严格层 adoption 点
   - 处理结论：**本轮不做额外调用替换**

### 2.7 `SecurityHelper` 证书入口扫描结论

本轮继续对 `SecurityHelper` 中的证书导入入口做了一次专项盘点，结论如下：

1. 证书兼容层现状
   - `LoadCertificate(byte[] rawData)`
   - `LoadCertificate(byte[] rawData, string password)`
   - `LoadCertificate(string certFileFullPath)`
   - `LoadCertificate(string certFileFullPath, string password)`
   - `LoadCertificate(X509Certificate2 certificate)`
   - 当前都直接操作系统证书存储，且在缺失文件、证书格式错误、权限不足时直接抛异常

2. 处理结论：**已补严格层**
   - 新增：
     - `LoadCertificateWithResult(byte[] rawData, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
     - `LoadCertificateWithResult(byte[] rawData, string password, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
     - `LoadCertificateWithResult(string certFileFullPath, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
     - `LoadCertificateWithResult(string certFileFullPath, string password, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
     - `LoadCertificateWithResult(X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
   - 对应结果模型：
     - `SecurityCertificateLoadResultModel`

3. 兼容层保留
   - 默认继续写入 `LocalMachine`
   - 失败时继续直接抛异常

4. adoption 结论
   - 当前未发现仓库内新的非测试证书导入调用点
   - 处理结论：**本轮不做额外调用替换**

### 2.8 `SecurityHelper` 文件 / 图片边界入口扫描结论

本轮继续对 `SecurityHelper` 中直接触达文件系统和图片文件的入口做了一次专项盘点，结论如下：

1. 当前入口现状
   - `EncryptBytesToFile(...)` / `DecryptBytesFromFile(...)`
   - `EncryptStringToFile(...)` / `DecryptStringFromFile(...)`
   - `EncryptFileToFile(...)` / `DecryptFileFromFile(...)`
   - `EncryptBytesToImageFile(...)` / `DecryptBytesFromImageFile(...)`
   - `EncryptStringToImageFile(...)` / `DecryptStringFromImageFile(...)`
   - `EncryptModelToImageFile(...)` / `DecryptModelFromImageFile(...)`
   - 这组入口当前都直接返回 `Encrypt*DigestInfoModel`

2. 处理结论：**本轮不重复补 `WithResult(...)`**
   - 原因：
     - 现有 digest model 已经承担结果对象职责
     - 已暴露 `IsSuccess`、路径、模型类型、哈希摘要等关键上下文
     - 继续叠加新的 `WithResult(...)` 会形成重复抽象

3. 本轮实际修正
   - `LanymyCrypto.EncryptBytesToImageFile(...)`
   - 当底层 `ImageHelper.SaveBitmapToImageFile(...)` 返回 `false` 时，不再误报成功
   - 现在会返回：
     - `IsSuccess == false`
     - `ErrorMessage = \"Failed to save encrypted bitmap to image file.\"`
   - `LanymyCrypto.EncryptBytesToFile(...)` / `EncryptStringToFile(...)`
     - 成功后补齐 `SourceBytes` / `SourceString`
   - `LanymyCrypto.EncryptBytesToFile(...)` / `EncryptStringToFile(...)` / `EncryptFileToFile(...)` / `DecryptFileFromFile(...)`
     - 文件型输出目标现在会自动创建父目录，避免和 `FileHelper` / `CompressionHelper` 的文件边界行为不一致
   - `LanymyCrypto.DecryptStringFromBitmap(...)`
     - 未传 `encoding` 时补齐默认编码回退，避免空引用
   - `LanymyCrypto.DecryptModelFromBytes(...)` / `DecryptModelFromBase64String(...)` / `DecryptModelFromFile(...)` / `DecryptModelFromBitmap(...)` / `DecryptModelFromImageFile(...)`
     - 成功后补齐 `ModelTypeName` / `ModelTypeFullName`
   - `LanymyCrypto.DecryptModelFromBase64String(...)`
     - 成功后保留输入 `EncryptedBase64String`
   - `GetEncryptDigestInfoModelFromEncryptedStream(...)`
     - 通过测试锁定正文篡改后的失败语义和 `ErrorMessage`
   - `GetEncryptDigestInfoModelFromEncryptedStream(...)`
     - 对截断/结构损坏的加密流返回失败 digest，而不是直接抛异常
   - `LanymyCrypto.DencryptStreamFromStream(...)`
     - 错误密钥或损坏密文时返回 `IsSuccess == false` 与明确 `ErrorMessage`
   - `EncryptModelToBitmap(...)` / `DecryptModelFromBitmap(...)`
     - 通过专项测试锁定位图模型回环与元数据回填
   - `DecryptStringFromBase64String(...)` / `DecryptModelFromBase64String(...)`
     - 非法 Base64 输入返回失败 digest，并保留输入载体字段
   - `DecryptModelFromBytes(...)` / `DecryptModelFromFile(...)` / `DecryptModelFromBitmap(...)` / `DecryptModelFromImageFile(...)`
     - 模型反序列化失败时返回失败 digest，而不是直接抛异常
   - `DecryptStringFromBitmap(...)` / `DecryptStringFromImageFile(...)`
     - 非法位图内容、非法图片文件内容返回失败 digest
   - `GetEncryptDigestInfoModelFromEncryptedFile(...)`
     - 文件型 digest model 在成功与失败两侧都保留 `EncryptedFileFullPath`
   - `DencryptStreamFromStream(...)`
     - 通过专项测试锁定错误密钥失败语义与可寻址目标流的回收行为
   - `EncryptStreamToStream(...)` / `DencryptStreamFromStream(...)`
     - 通过专项测试锁定底层流链路回环与摘要字段一致性
   - `GetEncryptDigestInfoModelFromEncryptedStream(...)`
     - 通过专项测试锁定空流失败语义与 `DencryptHeaderInfoModelJsonString` 的字段一致性
   - `GetEncryptDigestInfoModelFromEncryptedFile(...)`
     - 通过专项测试锁定默认通用 digest 不额外暴露文件路径字段

4. adoption 结论
   - 当前未发现仓库内新的非测试文件 / 图片边界调用点需要额外切换
   - 处理结论：**本轮通过补测试、收紧文件型输出父目录准备与修正成功判定来收口，不做额外调用替换**

### 2.9 `PathHelper` 扫描结论

本轮继续对 `PathHelper` 做了一次专项盘点，结论如下：

1. 当前模块定位
   - 主要承担路径规范化、目录补齐、路径类型推断与通配符过滤
   - 虽然会调用 `Directory.Exists(...)` / `File.Exists(...)` 辅助判断，但主体仍属于轻量路径工具

2. 处理结论：**本轮不补结果层**
   - 原因：
     - 当前高价值问题不在“缺少失败诊断”，而在路径标准化边界是否稳定
     - `GetFolderPath(...)` / `InitDirectoryPath(...)` 这类入口更适合通过实现细化和边界测试锁定语义，而不是再叠加 `WithResult(...)`

3. 本轮已锁定的边界
   - 仅文件名且带扩展名（如 `README.txt`）传入 `GetFolderPath(...)` 时返回空字符串
   - 真实存在但无扩展名的文件（如 `README`）会继续按文件处理，`GetFolderPath(...)` 返回父目录，`GetFileName(...)` 返回文件名本身
   - 以 `Path.AltDirectorySeparatorChar` 结尾的目录路径会被收敛到当前平台主目录分隔符
   - `InitDirectoryPath(...)` 对使用 `/` 结尾的目录路径仍能正确创建目标目录
   - 明确以目录分隔符结尾、但尚未创建的路径，在 `GetPathType(...)` 中仍按目录处理

### 2.10 `IsolatedStorageHelper` / `FileTextManipulater` 扫描结论

本轮继续对独立存储与文本文件基础设施做了一次专项盘点，结论如下：

1. `IsolatedStorageHelper` 处理结论：**补严格层**
   - 原因：
     - 当前对外兼容入口仍是 `void / string / T`
     - “缺文件 / 错密钥 / 反序列化失败 / 自定义存储根目录”这几类诊断信息在旧入口里不够清晰
   - 本轮新增：
     - `SaveStringWithResult(...)`
     - `GetStringWithResult(...)`
     - `SaveModelWithResult<T>(...)`
     - `GetModelWithResult<T>(...)`
   - 本轮新增结果模型：
     - `IsolatedStorageStringOperationResultModel`
     - `IsolatedStorageModelOperationResultModel<T>`

2. `IsolatedStorageHelper` 本轮锁定的兼容层语义
   - `GetString(...)`：
     - 缺失存储文件时继续返回 `string.Empty`
     - 错误密钥或内容损坏时继续返回 `null`
   - `GetModel<T>(...)`：
     - 缺失存储文件时继续返回 `default(T)`
     - 模型 JSON 无法反序列化时继续抛异常

3. `IsolatedStorageHelper` 本轮额外收口
   - `SaveModelWithResult<T>(...)`
     - 模型 JSON 序列化已移入 `try` 块内部
     - 结果对象初始化阶段不再因为序列化异常而直接漏抛
   - `SaveStringWithResult(...)` / `GetStringWithResult(...)` / `SaveModelWithResult<T>(...)` / `GetModelWithResult<T>(...)`
     - 异常路径会统一回填 `ErrorMessage`
   - `GetModelWithResult<T>(...)`
     - 模型反序列化失败时，继续保留 `SerializedString` 与异常文本，方便定位损坏内容

4. `FileTextManipulater` 处理结论：**本轮不补结果层**
   - 原因：
     - 当前模块更适合作为底层文件文本基础设施
     - 高价值问题在于历史边界是否稳定，而不是额外包装结果对象
   - 本轮通过测试锁定：
     - 自动创建父目录
     - `FileTextWriter` 默认追加写入
     - `ifOverWriteFile == true` 时覆盖旧内容
     - `FileTextReader` 读取缺失文件时自动创建空文件

### 2.12 `ProcessHelper` / `BaseCmd` 一致性复核结论

本轮继续对 `ProcessHelper` 与 `BaseCmd` 的内部关系做了一次专项复核，结论如下：

1. 当前关系
   - `BaseCmd` 当前只复用 `ProcessHelper.GetProcessStartInfo("cmd", true)` 生成启动配置
   - 尚未直接采用 `ProcessResultModel`

2. 处理结论：**不新增内部 adoption 替换，只补结果上下文**
   - 原因：
     - 当前 `BaseCmd` 自身已维护输出缓冲、退出码和异常
     - 若强行接入 `ProcessResultModel`，会混入更大范围的命令执行模型调整

3. 本轮额外收口
   - `ProcessResultModel`
     - 新增 `ErrorMessage`
     - 新增 `CreateNoWindow`
     - 新增 `UseShellExecute`
     - 新增 `WaitedForExit`
   - `StartProcessWithResult(...)` / `RunProcessWithResult(...)`
     - 会回填关键输入上下文
     - 进程异常失败时同步回填 `ErrorMessage`
     - 已等待退出且退出码非 0 时，返回明确的退出码文本说明

### 2.11 `VersionHelper` / `DateTimeHelper` / `FormatHelper` / `RegexHelper` / `EnumHelper` 扫描结论

本轮继续对剩余一批轻量 Helper 做了一次专项盘点，结论如下：

1. 模块共性判断
   - 这 5 个模块都更偏：
     - 纯内存计算
     - 元数据读取
     - 文本格式转换
     - 正则匹配
     - 枚举映射缓存
   - 处理结论：**本轮不补 `WithResult(...)` 严格层**
   - 原因：
     - 当前高价值问题不在“失败诊断不足”
     - 更适合通过边界测试与专题文档锁定历史语义

2. `VersionHelper`
   - 本轮处理结论：**补测试 + 最小实现修正**
   - 本轮修正：
     - `GetCallDomainAssemblyVersion()` / `GetCallDomainAssemblyFileVersion()` 优先取 `Assembly.GetEntryAssembly()`，取不到时回退到 helper 自身程序集，避免特殊宿主下空引用
     - `GetCallDomainAssemblyFileVersion()` 改为使用 `AssemblyName.Name` 计算文件名，不再依赖 `Assembly.ToString()` 文本切割
   - 本轮锁定：
     - `GetAssemblyVersion(...)`
     - `GetCallingAssemblyVersion()`
     - `GetFileVersionString(...)`
     - `GetFileVersion(...)`
     - 调用域程序集版本 / 文件版本解析

3. `DateTimeHelper`
   - 本轮处理结论：**补测试，不改结果层形态**
   - 本轮锁定：
     - 1970 / 2000 / 实例化基线的回环换算
     - 季度起止日期和季度枚举
     - 毫秒间隔正负号
     - 周一日期换算
     - Java Long 时间戳转换

4. `FormatHelper`
   - 本轮处理结论：**补测试，不改结果层形态**
   - 本轮锁定：
     - XML 缩进格式化
     - 空字符串 / 空 `XmlDocument` 输入返回 `string.Empty`
     - Base64 文件名 / 目录名转换的可逆性

5. `RegexHelper`
   - 本轮处理结论：**补测试，不改结果层形态**
   - 本轮锁定：
     - 手机号、数字、正整数、IPv4
     - 中文姓名、中英文姓名、Email、URL、时间、十六进制文本
     - 通配符匹配
     - 绝对路径 / 相对路径的当前历史判定规则
   - 特别说明：
     - `IsAllNumeric(string.Empty)` 当前继续返回 `true`
     - `IsRelativePath(...)` 当前仍保留仓库历史的宽松文本语义

6. `EnumHelper`
   - 本轮处理结论：**补测试，不改结果层形态**
   - 本轮锁定：
     - 枚举映射缓存复用
     - 枚举项字典 / 列表查询
     - 指定项过滤
     - Flags 枚举拆解与 `HasFlag(...)`

## 3. 当前 adoption 状态

### 3.1 已直接采用严格层的地方

- 各 helper 自身兼容层实现内部，已经统一转发到对应 `WithResult(...)`
- `AboutViewModel` 已切到 `NetworkHelper.GetLocalIpListWithResult()`
- `FileSerializeHelper` 已补 `WithResult(...)` 严格入口，并把内部文件写入接到严格层
- `FileSerializeHelper` 读取侧已切到 `FileHelper.GetBinaryFileBytesWithResult()`
- `JsonSerializeHelper` 已补 JSON 文件读写的同步 / 异步严格入口
- `CompressionHelper` 已补文件压缩/解压的同步 / 异步严格入口
- `SecurityAesHelper` 已补 AES 模型文件读写的严格入口
- `SecurityAesHelper` 严格层结果现已补齐模型类型与错误消息上下文
- `SecurityHelper` 已补 RSA 二进制与字符串加解密的严格入口
- `LanymyIsolatedStorage` 文件系统模式下的原始字节读取已切到 `FileHelper.GetBinaryFileBytesWithResult()`
- `Lanymy.Common.AllTests` 中已为所有严格入口补齐定向回归

### 3.2 当前刻意不切换的地方

- 所有“兼容层历史语义验证”测试
- `FileHelper.GetBinaryFileBytes(...)` 兼容入口本身及其历史语义锁定用例

## 4. 没有继续扩散改造的原因

这轮没有继续大范围替换兼容层调用，主要是因为：

- 真实非测试调用点本来就很少
- `src/Commons` 范围内，这轮收口后已经没有新的文件边界 helper-on-helper 读取路径继续直接依赖 `FileHelper.GetBinaryFileBytes(...)`
- 当前仓库这批 `WithResult(...)` 更主要服务“新调用方”和“诊断能力”
- 若把所有旧调用全部改掉，会混入额外的上层 API 设计调整，超出本轮治理范围

## 5. 下一轮若继续推进的优先级

若后续继续做 adoption，建议按下面顺序：

1. 先看 `Commons` 内部封装层
   - 例如 `SerializeHelper.*` 这类 helper-on-helper 调用
   - 这一步要连带评估上层 API 是否也需要补结果模型

2. 再看结果模型语义一致性
   - 重点复核 `Exception`、`ErrorMessage`、输入上下文字段是否在失败路径留全

3. 最后再看测试代码
   - 只在需要新增严格层断言时扩展，不主动替换兼容层验证用例

## 6. 回归要求

若后续继续把旧调用切到严格层，默认需要：

- 补对应定向测试
- 至少构建受影响项目
- 再跑 `Lanymy.Common.AllTests` 的 `net8.0` 全量回归
