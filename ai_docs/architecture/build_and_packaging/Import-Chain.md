# Import-Chain

## 1. 文档目的

- 本文件用于说明 `Lanymy.NET` 当前自定义 MSBuild 母板的导入链路。
- 目标是帮助后续维护者在修改公共构建配置前，先明确“哪个文件影响什么”。

## 2. 典型项目导入路径

多数 `Commons` 项目的典型结构如下：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <Import Project="../../../Build/MsBuildFiles/Lanymy.Commons.Header.props" />

  <PropertyGroup>
    <TargetFrameworks>$(CurrentTargetFrameworks);net48</TargetFrameworks>
  </PropertyGroup>

  <Import Project="../../../Build/MsBuildFiles/Lanymy.Commons.Footer.props" />
</Project>
```

## 3. Header / Footer 链路

当前已确认的导入链路为：

```text
项目.csproj
├── Import: Lanymy.Commons.Header.props
│   ├── Import: Lanymy.Commons.props
│   │   └── Import: Lanymy.Commons.Variables.props
│   └── Import: Lanymy.Commons.Package.Header.props
│       ├── Import: Lanymy.Common.ExtensionFunctions.props
│       ├── Import: Lanymy.Common.Instruments.props
│       └── Import: Lanymy.Common.Helpers.props
└── Import: Lanymy.Commons.Footer.props
    ├── Import: Lanymy.Commons.Package.Footer.props
    └── Import: Lanymy.Commons.targets
```

## 4. 各节点影响面

| 节点 | 主要影响面 |
|------|------|
| `Lanymy.Commons.Variables.props` | 基础变量、项目名、版本变量、时间戳版本 |
| `Lanymy.Commons.props` | 公共目标框架、`NoWarn`、`LangVersion` |
| `Lanymy.Common.*.props` | 按模块族设置命名空间、分类属性 |
| `Lanymy.Commons.Package.Footer.props` | 程序集版本、文件版本、NuGet 包元数据、自动打包 |
| `Lanymy.Commons.targets` | 打包附带文件、文档文件输出等后处理配置 |

## 5. 当前理解方式

- `Header.props` 更像“前置公共环境”
- `Package.Header.props` 更像“模块分类注入层”
- `Package.Footer.props` 更像“打包元数据层”
- `Targets` 更像“构建输出修饰层”

## 6. 修改建议

- 改 `Lanymy.Commons.props` 前，先评估是否会影响所有库项目的目标框架。
- 改 `Lanymy.Commons.Package.Footer.props` 前，先评估是否会影响所有包的版本和发布行为。
- 改 `Lanymy.Common.Helpers.props` / `Instruments.props` / `ExtensionFunctions.props` 前，先确认是否只是命名空间影响，还是还会牵连分类规则。

## 7. 当前风险点

- 这套链路不是标准 `Directory.Build.*` 方案，新接手时容易遗漏真实生效顺序。
- Header / Footer 名称直观，但真正影响面跨越属性、打包、targets 三层，修改前必须回读完整链路。
