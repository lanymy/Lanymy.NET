# QrCodeHelper

## 1. 模块定位

- 对应项目：`src/Commons/Lanymy.Common.Helpers.QrCodeHelper`
- 当前模块定位是二维码生成辅助能力。

## 2. 当前兼容策略信号

从项目文件看，当前二维码模块已经明确切换到：

- `SkiaSharp.QrCode`

并保留了历史注释，说明此前考虑过：

- `QRCoder`
- `System.Drawing.Common`

当前注释直接说明：`QRCoder` 不适合较新的跨平台场景。

## 3. 当前结构特点

- 属于 Helpers 体系下的静态能力模块
- 重点不在复杂业务编排，而在二维码生成的直接调用体验
- 这个模块是当前仓库“历史依赖向跨平台依赖迁移”的一个代表样本

## 4. 当前维护关注点

- 后续若继续做跨平台和新框架兼容性整理，`QrCodeHelper` 可以作为优先参考案例。
- 若仓库中其他图像/绘图类模块仍依赖 `System.Drawing.Common`，可以对照本模块的迁移路径做评估。

## 5. 当前风险点

- 当前模块虽然已经换到 `SkiaSharp.QrCode`，但仓库中其他图像相关模块未必同步完成迁移。
- 若后续统一图像能力栈，需要同时考虑 `ImageHelper`、`SecurityHelper` 等模块。

## 6. 推荐阅读入口

1. `Lanymy.Common.Helpers.QrCodeHelper.csproj`
2. `QrCodeHelper.cs`
3. `SkiaSharpQrCodeHelper.cs`

## 7. 后续建议

- 可补一份“图像与二维码能力跨平台迁移专题”
- 可对比 `ImageHelper` 和 `QrCodeHelper` 的依赖差异，判断后续统一方向
