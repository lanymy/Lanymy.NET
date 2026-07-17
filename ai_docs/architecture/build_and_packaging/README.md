# build_and_packaging

本目录用于分析 `Lanymy.NET` 的构建、版本与打包体系。

## 1. 当前结构

当前仓库没有采用 `Directory.Build.props` / `Directory.Build.targets` 作为唯一入口，而是维护了一套自定义 MSBuild 母板，集中放在：

- `Build/MsBuildFiles/Lanymy.Commons.Header.props`
- `Build/MsBuildFiles/Lanymy.Commons.props`
- `Build/MsBuildFiles/Lanymy.Commons.Footer.props`
- `Build/MsBuildFiles/Lanymy.Commons.Package.Header.props`
- `Build/MsBuildFiles/Lanymy.Commons.Package.Footer.props`
- `Build/MsBuildFiles/Lanymy.Commons.targets`

多数 `Commons` 项目都会在 `csproj` 中导入：

```xml
<Import Project="../../../Build/MsBuildFiles/Lanymy.Commons.Header.props" />
...
<Import Project="../../../Build/MsBuildFiles/Lanymy.Commons.Footer.props" />
```

## 2. 关键职责分工

| 文件 | 当前职责 |
|------|------|
| `Lanymy.Commons.Header.props` | 导入公共 props 和包头配置，是项目共享入口 |
| `Lanymy.Commons.props` | 定义 `CurrentTargetFrameworks`、`NoWarn`、`LangVersion` 等公共属性 |
| `Lanymy.Commons.Variables.props` | 定义 `CurrentBuildProjectName`、`CurrentReleaseNumbers`、时间戳版本等共享变量 |
| `Lanymy.Commons.Package.Header.props` | 导入分类 props，例如 Helpers / Instruments / ExtensionFunctions |
| `Lanymy.Commons.Package.Footer.props` | 定义程序集版本、文件版本、NuGet 元数据和 `GeneratePackageOnBuild` |
| `Lanymy.Commons.targets` | 补充图标、许可证、文档文件输出等构建后配置 |

## 3. 当前已确认行为

- 公共目标框架默认来自 `Lanymy.Commons.props`，当前值为：
  - `netstandard2.1`
  - `net8.0`
- 具体项目通常在自身 `csproj` 中再追加 `net48`
- 包级配置启用了 `GeneratePackageOnBuild=true`
- 大部分库项目会在构建时生成 NuGet 包
- `Lanymy.Common.All` 通过 `ProjectReference` 聚合基础层、Helpers、Instruments 和 ExtensionFunctions

## 4. 当前关注点

- `Build/MsBuildFiles/` 下的共享 props / targets 母板
- 多项目统一版本号和包元数据
- `GeneratePackageOnBuild` 行为
- 聚合包与子包的构建关系

## 5. 推荐入口

- `Build/MsBuildFiles/Lanymy.Commons.props`
- `Build/MsBuildFiles/Lanymy.Commons.Variables.props`
- `Build/MsBuildFiles/Lanymy.Commons.targets`
- `Build/MsBuildFiles/Lanymy.Commons.Package.Header.props`
- `Build/MsBuildFiles/Lanymy.Commons.Package.Footer.props`
- `src/Commons/Lanymy.Common.All/Lanymy.Common.All.csproj`
- [Import-Chain.md](./Import-Chain.md)

## 6. 当前风险点

- 构建逻辑分散在多份自定义 props / targets 中，理解成本高于常规 `Directory.Build.*` 方案。
- `CurrentTargetFrameworks` 与历史变量文件中的默认值存在演进痕迹，后续调整时需要先确认实际生效链路。
- 自动打包对所有库项目都生效，修改共享配置时影响面较大。

## 7. 后续建议补充

- 构建文件之间的导入链路图
- 版本号来源与覆盖规则
- 打包输出策略
