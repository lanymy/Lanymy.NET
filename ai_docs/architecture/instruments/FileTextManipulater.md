# FileTextManipulater

本文档用于分析 `Lanymy.Common.Instruments.FileTextManipulater*` 的当前职责与边界语义。

## 模块定位

- 这是仓库里一组轻量文本文件基础设施：
  - `BaseFileTextReadOrWrite`
  - `FileTextReader`
  - `FileTextWriter`
- 它们的职责不是结果建模，而是把“文本文件存在、目录存在、编码初始化”这些重复样板收敛起来。

## 当前实现特征

- `BaseFileTextReadOrWrite`
  - 构造时统一初始化目录
  - 目标文件不存在时主动创建空文件
  - `ifOverWriteFile == true` 时先删旧文件，再创建新文件
- `FileTextReader`
  - 基于 `StreamReader`
  - 提供 `ReadAll()` / `ReadLine()` / `IfHaveString()`
- `FileTextWriter`
  - 基于 `StreamWriter`
  - 默认追加写入
  - `Write(...)` / `WriteLine(...)` 每次都会 `Flush()`

## 当前锁定的边界

### 1. 自动建目录

- 传入的父目录不存在时，构造阶段会先通过 `PathHelper.InitDirectoryPath(...)` 建好目录。

### 2. 缺失文件自动建空文件

- `FileTextReader` 读取一个不存在的文件时，不会直接抛“文件不存在”，而是先创建空文件。
- 这也解释了为何上层 `JsonSerializeHelper.DeserializeFromJsonFile(...)` 对缺失文件会返回 `default(T)`，而不是先抛 `FileNotFoundException`。

### 3. 覆盖与追加

- `FileTextWriter(..., ifOverWriteFile: false)` 维持历史“追加”语义。
- `FileTextWriter(..., ifOverWriteFile: true)` 会清空旧内容后重建文件，再写入新内容。

## 本轮治理结论

- `FileTextManipulater` 仍保持轻量基础设施定位。
- 本轮没有为它新增 `WithResult(...)` 或结果模型，原因是：
  - 它主要承担底层文件准备和文本流读写
  - 真正需要“失败诊断”的上层场景，应该在 `JsonSerializeHelper`、`IsolatedStorageHelper` 这类 helper 层暴露严格结果语义

## 维护时需要注意

- 任何修改都要注意它对 `JsonSerializeHelper`、`Serializer.*`、`IsolatedStorage` 等上层模块的连带影响。
- “缺失文件自动创建空文件”是一个历史语义，不是顺手可以删掉的实现细节。
