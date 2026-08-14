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
  - `CopyFileWithResult`
  - `CopyFiles`
  - `CopyFilesWithResult`
- 文件移动
  - `MoveFile`
  - `MoveFileWithResult`
- 文件夹复制与删除
  - `CopyFolderToNewFoler`
  - `CopyFolderToNewFolerWithResult`
  - `DeleteFolder`
  - `DeleteFolderWithResult`
- 二进制文件读写
  - `CreateBinaryFile`
  - `CreateBinaryFileWithResult`
  - `GetBinaryFileBytes`
  - `GetBinaryFileBytesWithResult`
  - `ReadToBuffer`
  - `ReadExactly`

## 实现特征

- 整体仍是典型的早期静态 Helper 风格：方法直接对 `System.IO` 操作做薄封装。
- 哈希能力现在通过显式 `switch` 映射到具体算法实例，避免字符串驱动工厂在不同运行时上的不稳定差异。
- 复制和创建文件前会显式调用 `PathHelper.InitDirectoryPath(...)`，说明该模块默认把“目标路径父目录自动补齐”视为标准行为。
- 支持对 `ScheduleFileInfoModel` 的调度式批量复制，说明它不仅服务基础 IO，也被上层流程工具复用。

## 维护时需要注意

- `CopyFolderToNewFoler` 内部吞掉了异常，调用方无法知道失败原因，只能从结果侧推断是否成功。
- `DeleteFolder` 也是布尔返回值 + `catch` 吞异常风格，适合历史兼容，但不利于现代诊断。
- 当前已经补充结果型入口；若需要拿到失败原因，优先使用 `CopyFolderToNewFolerWithResult` / `DeleteFolderWithResult`。
- 当前已经补充结果型入口；若需要拿到失败原因，优先使用 `CopyFileWithResult` / `MoveFileWithResult` / `CreateBinaryFileWithResult` / `GetBinaryFileBytesWithResult` / `CopyFolderToNewFolerWithResult` / `DeleteFolderWithResult`。
- 批量复制若需要知道“哪一项失败”，优先使用 `CopyFilesWithResult`，它会返回逐项明细与成功/失败计数。
- `CopyFilesWithResult` 现在还会补充批次级上下文：
  - `RequestedItemCount`
  - `Exception`
  - `FirstException`
  这样空集合、参数级失败和逐项失败可以明确区分，不需要再伪造一条子项失败结果。
- `CopyFile(ScheduleFileInfoModel)` / `CopyFiles(...)` 兼容层仍保持参数非法直接抛异常、首项失败立即中断的老语义；若要拿到完整批次明细，应改用结果型入口。
- `GetBinaryFileBytes` 一次性把整个文件读入内存，更适合中小文件，不适合超大文件场景。
- `GetBinaryFileBytesWithResult` 适合 helper-on-helper 场景复用，能同时保留 `FilePath`、`BytesLength` 和异常信息，避免上层再自行拼装读取诊断。
- 读取二进制流时，不能假设单次 `Read(...)` 一定读满；当前已补充统一的循环读取辅助以收紧这类边界。
- 哈希计算的 `offset` 语义当前已经通过测试锁定：
  - 有效偏移表示“从该位置开始对剩余内容计算哈希”
  - 越界偏移会回退到“从流开头计算完整内容”，不再受调用前 `Stream.Position` 影响
- 兼容层 `CopyFolderToNewFoler` 仍保持历史语义：失败时吞异常，不向外抛出。
- 兼容层 `DeleteFolder` 仍保持历史语义：失败时返回 `false`。
- 兼容层 `MoveFile` 仍保持历史语义：源文件不存在时静默无操作；严格诊断场景应改用 `MoveFileWithResult`。
- 兼容层 `CreateBinaryFile` 仍保持历史语义：空数组或 null 输入时直接无操作。
- 方法命名里存在历史拼写问题，如 `CopyFolderToNewFoler`，文档与调用时都要注意保持一致。
- `ReadToBuffer(...)` / `ReadExactly(...)` 这类底层流读取辅助已经有专项测试锁住“分段读取”和“提前 EOF”行为，后续依赖方应优先复用，不再假设单次 `Read(...)` 读满。

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
- 当前已经开始把“吞异常/弱诊断”路径拆成兼容层与严格层两套 API，批量复制也已纳入同一模式；后续可以继续看是否要覆盖更多聚合型文件操作。
- 对所有依赖文件头/长度解析的模块，优先复用统一的“读满/EOF”辅助，避免再次出现单次 `Read(...)` 假设。
- 严格层的批量结果优先把“批次本身失败”和“某个子项失败”分层表达，不把集合级参数错误混成子项明细。
