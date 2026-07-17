# ImageHelper

本文档用于分析 `Lanymy.Common.Helpers.ImageHelper` 模块。

## 模块定位

- 这是一个非常轻量的图片辅助模块。
- 当前职责聚焦在 `Bitmap` 与图片文件之间的双向转换。
- 在仓库中的主要价值不是提供丰富图像处理能力，而是为 `SecurityHelper` 等上层模块提供最小落盘与读取支持。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.ImageHelper/ImageHelper.cs`

## 主要能力

- `SaveBitmapToImageFile`
  - 把 `Bitmap` 保存为图片文件
  - 当前固定保存为 `Png`
- `GetBitmapFromImageFile`
  - 从图片文件加载 `Bitmap`

## 实现特征

- 模块非常薄，几乎就是 `System.Drawing` API 的直接包装。
- `SaveBitmapToImageFile` 现在只负责保存文件，不再接管传入 `Bitmap` 的释放时机。
- 保存格式固定为 `ImageFormat.Png`，没有对外暴露格式选择。
- `GetBitmapFromImageFile` 使用 `Bitmap.FromFile(...)`，属于同步、直接的本地文件读取路径。

## 在仓库中的角色

- 它更像一个“协作型 Helper”，不是独立的大能力模块。
- 目前最明显的协作场景是在 `LanymyCrypto` 中把加密结果写成图片文件，或从图片文件中恢复 `Bitmap`。

## 维护时需要注意

- `SaveBitmapToImageFile` 不再释放传入对象，调用方需要自行管理 `Bitmap.Dispose()` 的时机。
- 该模块完全依赖 `System.Drawing`，跨平台适配时需要重点复验运行条件。
- 目前异常处理采用吞异常并返回 `false` 的方式，便于兼容，但不利于问题定位。

## 风险点

- 方法数量虽少，但都与资源释放、文件句柄、图形依赖有关，出问题时往往不是逻辑错，而是环境错。
- 该模块仍然没有把“保存失败原因”往上传递，调用方拿到的只有 `true/false`，排障信息偏少。

## 后续建议

- 可继续补一份“ImageHelper 与 SecurityHelper 图像加密链路”专题。
- 若后续要现代化，建议明确是否继续保留 `System.Drawing` 路线。
