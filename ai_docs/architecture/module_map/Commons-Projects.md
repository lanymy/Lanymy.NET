# Commons-Projects

## 1. 文档目的

- 本文件用于列出 `src/Commons/` 下当前全量项目清单，并按层次分组。
- 当前统计口径为 `src/Commons/**/*.csproj`。

## 2. 当前规模

- `Commons` 当前共包含 **73** 个项目。

## 3. 基础层

| 项目 | 说明 |
|------|------|
| `Lanymy.Common.Abstractions` | 基础抽象、模型、接口 |
| `Lanymy.Common.ConstKeys` | 常量键定义 |
| `Lanymy.Common.Enums` | 公共枚举 |

## 4. ExtensionFunctions

| 项目 | 说明 |
|------|------|
| `Lanymy.Common.ExtensionFunctions.All` | 扩展方法聚合包 |
| `Lanymy.Common.ExtensionFunctions.DeepCloneExtensions` | 深拷贝扩展 |
| `Lanymy.Common.ExtensionFunctions.EnumExtensions` | 枚举扩展 |
| `Lanymy.Common.ExtensionFunctions.ExpressExtensions` | 表达式扩展 |
| `Lanymy.Common.ExtensionFunctions.ObjectExtensions` | 对象扩展 |
| `Lanymy.Common.ExtensionFunctions.StringExtensions` | 字符串扩展 |

## 5. Helpers

| 项目 | 说明 |
|------|------|
| `Lanymy.Common.Helpers.All` | Helpers 聚合包 |
| `Lanymy.Common.Helpers.ArrayHelper` | 数组辅助 |
| `Lanymy.Common.Helpers.BytesHelper` | 字节辅助 |
| `Lanymy.Common.Helpers.CompressionHelper` | 压缩辅助 |
| `Lanymy.Common.Helpers.DateTimeHelper` | 时间辅助 |
| `Lanymy.Common.Helpers.EmailHelper` | 邮件辅助 |
| `Lanymy.Common.Helpers.EnumHelper` | 枚举辅助 |
| `Lanymy.Common.Helpers.FileHelper` | 文件辅助 |
| `Lanymy.Common.Helpers.FormatHelper` | 格式辅助 |
| `Lanymy.Common.Helpers.GenericityHelper` | 泛型辅助 |
| `Lanymy.Common.Helpers.HttpHelper` | HTTP 辅助 |
| `Lanymy.Common.Helpers.ImageHelper` | 图像辅助 |
| `Lanymy.Common.Helpers.IsolatedStorageHelper` | 独立存储辅助 |
| `Lanymy.Common.Helpers.NetworkHelper` | 网络辅助 |
| `Lanymy.Common.Helpers.PathHelper` | 路径辅助 |
| `Lanymy.Common.Helpers.PcInfoHelper` | 设备信息辅助 |
| `Lanymy.Common.Helpers.ProcessHelper` | 进程辅助 |
| `Lanymy.Common.Helpers.QrCodeHelper` | 二维码辅助 |
| `Lanymy.Common.Helpers.RegexHelper` | 正则辅助 |
| `Lanymy.Common.Helpers.SecurityHelper` | 安全辅助 |
| `Lanymy.Common.Helpers.SerializeHelper.Binary` | 二进制序列化辅助 |
| `Lanymy.Common.Helpers.SerializeHelper.DataTable` | DataTable 序列化辅助 |
| `Lanymy.Common.Helpers.SerializeHelper.File` | 文件序列化辅助 |
| `Lanymy.Common.Helpers.SerializeHelper.Json` | JSON 序列化辅助 |
| `Lanymy.Common.Helpers.SerialNumberHelper` | 流水号辅助 |
| `Lanymy.Common.Helpers.VerificationCodeHelper` | 验证码辅助 |
| `Lanymy.Common.Helpers.VersionHelper` | 版本辅助 |
| `Lanymy.Common.Helpers.Win32Helper` | Win32 辅助 |

## 6. Instruments

### 6.1 聚合层

| 项目 | 说明 |
|------|------|
| `Lanymy.Common.Instruments.All` | Instruments 聚合包 |

### 6.2 Array / Cache / CMD / Compresser

| 项目 |
|------|
| `Lanymy.Common.Instruments.ArrayManipulater` |
| `Lanymy.Common.Instruments.ArrayManipulater.Abstractions` |
| `Lanymy.Common.Instruments.Cache.Abstractions` |
| `Lanymy.Common.Instruments.Cache.CoreMemoryCache` |
| `Lanymy.Common.Instruments.Cache.CoreMemoryCache.Abstractions` |
| `Lanymy.Common.Instruments.Cache.CustomMemoryCache` |
| `Lanymy.Common.Instruments.CMD` |
| `Lanymy.Common.Instruments.CMD.Abstractions` |
| `Lanymy.Common.Instruments.Compresser` |
| `Lanymy.Common.Instruments.Compresser.Abstractions` |

### 6.3 Crawler / Crypto / EnumMapper / Ffmpeg

| 项目 |
|------|
| `Lanymy.Common.Instruments.Crawler` |
| `Lanymy.Common.Instruments.Crawler.Abstractions` |
| `Lanymy.Common.Instruments.Crypto` |
| `Lanymy.Common.Instruments.Crypto.Abstractions` |
| `Lanymy.Common.Instruments.EnumMapper` |
| `Lanymy.Common.Instruments.EnumMapper.Abstractions` |
| `Lanymy.Common.Instruments.Ffmpeg` |
| `Lanymy.Common.Instruments.Ffmpeg.Abstractions` |

### 6.4 File / IsolatedStorage / Navigation / PipeLine

| 项目 |
|------|
| `Lanymy.Common.Instruments.FileTextManipulater` |
| `Lanymy.Common.Instruments.FileTextManipulater.Abstractions` |
| `Lanymy.Common.Instruments.IsolatedStorages` |
| `Lanymy.Common.Instruments.IsolatedStorages.Abstractions` |
| `Lanymy.Common.Instruments.NavigationPage` |
| `Lanymy.Common.Instruments.NavigationPage.Abstractions` |
| `Lanymy.Common.Instruments.PipeLine` |
| `Lanymy.Common.Instruments.PipeLine.Abstractions` |

### 6.5 Serializer / Socket / Toolkits / WorkTask

| 项目 |
|------|
| `Lanymy.Common.Instruments.Serializer.Abstractions` |
| `Lanymy.Common.Instruments.Serializer.JsonNetJsonSerializer` |
| `Lanymy.Common.Instruments.Socket.Abstractions` |
| `Lanymy.Common.Instruments.Socket.ImplementDemo` |
| `Lanymy.Common.Instruments.Socket.Netty.Abstractions` |
| `Lanymy.Common.Instruments.Toolkits` |
| `Lanymy.Common.Instruments.WorkTask.Abstractions` |
| `Lanymy.Common.Instruments.WorkTaskQueue` |

## 7. 顶层总聚合包

| 项目 | 说明 |
|------|------|
| `Lanymy.Common.All` | 整体总聚合包 |

## 8. 阅读建议

- 如果要理解仓库能力面，先看本文件的分组，再回到 [README.md](./README.md)。
- 如果要理解对外暴露面，优先看 `Lanymy.Common.All` 与三个 `*.All` 聚合包。
- 如果要分析改单影响面，优先定位该项目属于基础层、能力层还是聚合层。
