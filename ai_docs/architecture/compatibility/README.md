# compatibility

本目录用于分析 `Lanymy.NET` 的多目标框架兼容策略。

## 1. 当前兼容策略概览

当前仓库的公共目标框架来自 `Build/MsBuildFiles/Lanymy.Commons.props`，默认包含：

- `netstandard2.1`
- `net8.0`

而大量实际库项目会在各自 `csproj` 中额外追加：

- `net48`

所以从维护视角看，当前仓库是一个典型的：

`现代 .NET + 标准库兼容层 + .NET Framework 历史兼容层`

三层并存的仓库。

## 2. 当前关注点

- 主体项目普遍同时面向 `netstandard2.1`、`net8.0` 与 `net48`
- 条件依赖与条件编译差异
- 历史依赖在新框架下的适配成本

## 3. 已确认的兼容性信号

- `HttpHelper` 仍依赖 `Microsoft.AspNet.WebApi.Client`
- `ImageHelper`、`SecurityHelper` 等模块仍使用 `System.Drawing.Common`
- `QrCodeHelper` 已切换到 `SkiaSharp.QrCode`，体现出对新跨平台场景的适配
- `AppTests` 仍是 Xamarin.Forms 体系，说明测试侧也保留了历史技术栈痕迹

## 4. 维护兼容性时的建议

- 不要只看 `net8.0` 分支行为来判断能否安全修改公共 API。
- 看到条件 `PackageReference` 时，应同时检查对应目标框架是否仍有真实使用价值。
- 替换历史依赖前，应先确认：
  - 哪些项目引用了该能力
  - 哪些目标框架仍受该依赖约束
  - 聚合包是否会放大影响面

## 5. 推荐入口

- `Build/MsBuildFiles/Lanymy.Commons.props`
- 各 `*.csproj` 中的 `TargetFrameworks`
- `Documents/编译符号.txt`

## 6. 当前风险点

- 多目标框架会显著提升升级、测试、打包和依赖替换成本。
- 历史依赖与现代依赖并存，容易出现只修一侧、另一侧退化的问题。
- 若未来需要收缩兼容面，必须先做使用面调查，不能直接按技术偏好裁剪。

## 7. 后续建议补充

- 当前支持矩阵
- 各依赖包的兼容性说明
- 逐步收敛兼容面的评估方案
