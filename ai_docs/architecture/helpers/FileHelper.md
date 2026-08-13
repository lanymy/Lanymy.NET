# FileHelper

本文档用于分析 `Lanymy.Common.Helpers.FileHelper` 模块的职责、实现风格与维护关注点。

## 模块定位

- 面向文件、目录、二进制文件的轻量静态辅助模块。
- 核心目标是把常见文件系统操作封装成直接可调用的工具方法。
- 与 `PathHelper`、`CompressionHelper`、`SecurityHelper` 存在协作关系，其中 `PathHelper` 负责目录初始化与路径格式化。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.FileHelper/FileHelper.cs`

## 主要能力

- 文件哈希计算
  - `GetFileHashCode`
  - `GetStreamHashCode`
  - `GetBytesHashCode`
- 文件复制与批量复制
  - `CopyFile`
  - `CopyFiles`
- 文件移动
  - `MoveFile`
- 文件夹复制与删除
  - `CopyFolderToNewFoler`
  - `CopyFolderToNewFolerWithResult`
  - `DeleteFolder`
  - `DeleteFolderWithResult`
- 二进制文件读写
  - `CreateBinaryFile`
  - `GetBinaryFileBytes`
  - `ReadToBuffer`
  - `ReadExactly`

## 实现特征

- 整体仍是典型的早期静态 Helper 风格：方法直接对 `System.IO` 操作做薄封装。
- 哈希能力通过 `HashAlgorithm.Create(hashAlgorithmType.ToString())` 进行字符串驱动的算法选择，属于兼容优先的实现方式。
- 复制和创建文件前会显式调用 `PathHelper.InitDirectoryPath(...)`，说明该模块默认把“目标路径父目录自动补齐”视为标准行为。
- 支持对 `ScheduleFileInfoModel` 的调度式批量复制，说明它不仅服务基础 IO，也被上层流程工具复用。

## 维护时需要注意

- `CopyFolderToNewFoler` 内部吞掉了异常，调用方无法知道失败原因，只能从结果侧推断是否成功。
- `DeleteFolder` 也是布尔返回值 + `catch` 吞异常风格，适合历史兼容，但不利于现代诊断。
- 当前已经补充结果型入口；若需要拿到失败原因，优先使用 `CopyFolderToNewFolerWithResult` / `DeleteFolderWithResult`。
- `GetBinaryFileBytes` 一次性把整个文件读入内存，更适合中小文件，不适合超大文件场景。
- 读取二进制流时，不能假设单次 `Read(...)` 一定读满；当前已补充统一的循环读取辅助以收紧这类边界。
- `MoveFile` 在源文件不存在时直接无操作，没有显式错误提示。
- 方法命名里存在历史拼写问题，如 `CopyFolderToNewFoler`，文档与调用时都要注意保持一致。

## 适合的使用场景

- 工具脚本式的文件复制、移动、校验
- 需要快速补目录的本地文件写入
- 简单二进制文件落盘与读取

## 不适合直接扩张的方向

- 复杂的大文件分块传输
- 需要细粒度异常诊断的业务流程
- 高并发文件调度与重试控制

## 后续建议

- 可继续补一份“FileHelper 与 PathHelper 的职责边界”专题，避免两者能力继续交叉。
- 当前已经开始把“吞异常”路径拆成兼容层与严格层两套 API，后续可以沿同样模式继续整理其他目录/文件操作。
- 对所有依赖文件头/长度解析的模块，优先复用统一的“读满/EOF”辅助，避免再次出现单次 `Read(...)` 假设。
