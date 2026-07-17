# AGENTS-INDEX

## 1. 文档目的

- 本文件用于为 `Lanymy.NET` 提供路径级项目索引。
- 目标是帮助 AI 和接手开发者快速定位源码入口，而不是重复解释所有实现细节。
- 当前仓库的核心判断是：这是一个“多项目拆分 + 聚合包”的通用类库仓库，不是单体业务应用。

## 2. 仓库主目录

| 路径 | 说明 |
|------|------|
| `Build/` | 公共 MSBuild 母板与共享构建配置 |
| `Documents/` | 历史文档目录，当前不作为 AI 协作文档主入口 |
| `imgs/` | 仓库静态资源 |
| `src/` | 解决方案、核心类库与测试项目 |

## 3. 解决方案入口

| 路径 | 说明 |
|------|------|
| `src/Lanymy.NET.sln` | 主解决方案入口 |
| `README.md` | 项目简介 |
| `AGENTS.md` | 仓库级协作规则入口 |

## 4. 构建与文档入口

| 路径 | 说明 |
|------|------|
| `Build/MsBuildFiles/Lanymy.Commons.Header.props` | 共享构建入口头部导入 |
| `Build/MsBuildFiles/Lanymy.Commons.props` | 公共目标框架、`NoWarn`、语言版本等共享属性 |
| `Build/MsBuildFiles/Lanymy.Commons.Package.Footer.props` | 版本号、包元数据、`GeneratePackageOnBuild` |
| `Build/MsBuildFiles/Lanymy.Commons.targets` | 公共打包资源与文档输出配置 |
| `Documents/` | 历史文档目录，当前不作为 AI 协作文档主树 |
| `ai_docs/` | 当前 AI 协作文档主树 |

## 5. Commons 模块索引

`src/Commons/` 是当前仓库的核心区域。可以按以下方式理解：

### 5.1 基础层

| 路径 | 说明 |
|------|------|
| `src/Commons/Lanymy.Common.Abstractions` | 公共模型、接口、结果对象等基础抽象 |
| `src/Commons/Lanymy.Common.ConstKeys` | 常量键定义 |
| `src/Commons/Lanymy.Common.Enums` | 公共枚举定义 |

### 5.2 ExtensionFunctions

| 路径 | 说明 |
|------|------|
| `src/Commons/Lanymy.Common.ExtensionFunctions.All` | 扩展方法聚合包 |
| `src/Commons/Lanymy.Common.ExtensionFunctions.DeepCloneExtensions` | 深拷贝扩展 |
| `src/Commons/Lanymy.Common.ExtensionFunctions.EnumExtensions` | 枚举扩展 |
| `src/Commons/Lanymy.Common.ExtensionFunctions.ExpressExtensions` | 表达式相关扩展 |
| `src/Commons/Lanymy.Common.ExtensionFunctions.ObjectExtensions` | 对象扩展 |
| `src/Commons/Lanymy.Common.ExtensionFunctions.StringExtensions` | 字符串扩展 |

### 5.3 Helpers

| 路径 | 说明 |
|------|------|
| `src/Commons/Lanymy.Common.Helpers.All` | Helpers 聚合包 |
| `src/Commons/Lanymy.Common.Helpers.ArrayHelper` | 数组辅助 |
| `src/Commons/Lanymy.Common.Helpers.BytesHelper` | 字节与十六进制转换 |
| `src/Commons/Lanymy.Common.Helpers.CompressionHelper` | 压缩辅助 |
| `src/Commons/Lanymy.Common.Helpers.DateTimeHelper` | 日期时间辅助 |
| `src/Commons/Lanymy.Common.Helpers.EmailHelper` | 邮件辅助 |
| `src/Commons/Lanymy.Common.Helpers.EnumHelper` | 枚举辅助 |
| `src/Commons/Lanymy.Common.Helpers.FileHelper` | 文件操作辅助 |
| `src/Commons/Lanymy.Common.Helpers.FormatHelper` | 格式化辅助 |
| `src/Commons/Lanymy.Common.Helpers.GenericityHelper` | 泛型辅助 |
| `src/Commons/Lanymy.Common.Helpers.HttpHelper` | HTTP 请求辅助 |
| `src/Commons/Lanymy.Common.Helpers.ImageHelper` | 图像辅助 |
| `src/Commons/Lanymy.Common.Helpers.IsolatedStorageHelper` | 独立存储辅助 |
| `src/Commons/Lanymy.Common.Helpers.NetworkHelper` | 网络辅助 |
| `src/Commons/Lanymy.Common.Helpers.PathHelper` | 路径辅助 |
| `src/Commons/Lanymy.Common.Helpers.PcInfoHelper` | 设备 / 系统信息辅助 |
| `src/Commons/Lanymy.Common.Helpers.ProcessHelper` | 进程辅助 |
| `src/Commons/Lanymy.Common.Helpers.QrCodeHelper` | 二维码辅助 |
| `src/Commons/Lanymy.Common.Helpers.RegexHelper` | 正则辅助 |
| `src/Commons/Lanymy.Common.Helpers.SecurityHelper` | 安全与加密辅助 |
| `src/Commons/Lanymy.Common.Helpers.SerializeHelper.*` | 二进制、文件、JSON、DataTable 序列化辅助 |
| `src/Commons/Lanymy.Common.Helpers.SerialNumberHelper` | 流水号辅助 |
| `src/Commons/Lanymy.Common.Helpers.VerificationCodeHelper` | 验证码辅助 |
| `src/Commons/Lanymy.Common.Helpers.VersionHelper` | 版本辅助 |
| `src/Commons/Lanymy.Common.Helpers.Win32Helper` | Win32 辅助 |

### 5.4 Instruments

| 路径 | 说明 |
|------|------|
| `src/Commons/Lanymy.Common.Instruments.All` | Instruments 聚合包 |
| `src/Commons/Lanymy.Common.Instruments.ArrayManipulater*` | 数组处理机制 |
| `src/Commons/Lanymy.Common.Instruments.Cache*` | 缓存抽象与实现 |
| `src/Commons/Lanymy.Common.Instruments.CMD*` | 命令行工具封装 |
| `src/Commons/Lanymy.Common.Instruments.Compresser*` | 压缩机制 |
| `src/Commons/Lanymy.Common.Instruments.Crawler*` | 爬取相关抽象与实现 |
| `src/Commons/Lanymy.Common.Instruments.Crypto*` | 加解密机制 |
| `src/Commons/Lanymy.Common.Instruments.EnumMapper*` | 枚举映射 |
| `src/Commons/Lanymy.Common.Instruments.Ffmpeg*` | ffmpeg 封装 |
| `src/Commons/Lanymy.Common.Instruments.FileTextManipulater*` | 文本文件处理 |
| `src/Commons/Lanymy.Common.Instruments.IsolatedStorages*` | 独立存储抽象与实现 |
| `src/Commons/Lanymy.Common.Instruments.NavigationPage*` | 导航页机制 |
| `src/Commons/Lanymy.Common.Instruments.PipeLine*` | 管道处理机制 |
| `src/Commons/Lanymy.Common.Instruments.Serializer*` | 序列化抽象与实现 |
| `src/Commons/Lanymy.Common.Instruments.Socket*` | Socket / Netty 抽象与示例 |
| `src/Commons/Lanymy.Common.Instruments.Toolkits` | 杂项工具机制 |
| `src/Commons/Lanymy.Common.Instruments.WorkTask*` | 后台工作队列与任务机制 |

### 5.5 总聚合包

| 路径 | 说明 |
|------|------|
| `src/Commons/Lanymy.Common.All` | 整体总聚合包 |

## 6. 测试与示例入口

| 路径 | 说明 |
|------|------|
| `src/UnitTests/Lanymy.Common.AllTests` | 当前主单元测试项目 |
| `src/UnitTests/Lanymy.Common.AppTests` | Xamarin.Forms 示例 / 历史 AppTests 项目 |
| `src/UnitTests/Lanymy.Common.AppTests/Lanymy.Common.AppTests.Android` | Android 示例入口 |
| `src/UnitTests/Lanymy.Common.AppTests/Lanymy.Common.AppTests.iOS` | iOS 示例入口 |

## 7. 推荐阅读顺序

1. [../AGENTS-Lanymy.Common.md](./AGENTS-Lanymy.Common.md)
2. [../../AGENTS.md](../../AGENTS.md)
3. [../README.md](../README.md)
4. [../architecture/module_map/README.md](../architecture/module_map/README.md)
5. [../architecture/build_and_packaging/README.md](../architecture/build_and_packaging/README.md)
6. [../../src/Lanymy.NET.sln](file:///e:/Code/Git/My/Lanymy.NET/src/Lanymy.NET.sln)
