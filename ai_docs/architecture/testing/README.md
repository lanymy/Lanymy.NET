# testing

本目录用于分析 `Lanymy.NET` 的测试项目和验证现状。

## 0. 相关规则入口

- 功能修改后的测试补齐协作约定见 [../../conventions/Testing-Convention.md](../../conventions/Testing-Convention.md)；其中“功能回归”默认同时包含功能正确性、性能验证和稳定性验证。

## 1. 当前结构

- `src/UnitTests/Lanymy.Common.AllTests`：主单元测试项目
- `src/UnitTests/Lanymy.Common.AppTests`：Xamarin.Forms 示例 / 历史验证项目

## 2. 已确认的测试配置

`Lanymy.Common.AllTests` 当前使用：

- `Microsoft.NET.Test.Sdk`
- `MSTest.TestAdapter`
- `MSTest.TestFramework`
- `coverlet.collector`

目标框架为：

- `net8.0`

这说明当前真正具备现代 CLI 测试入口的核心测试项目，是 `Lanymy.Common.AllTests`。

## 3. 当前测试内容特征

- 测试覆盖了部分 Helpers、Crypto、Version、Network、WorkTask 等模块
- 有些测试偏“样例调用验证”
- 有些测试才是真正的断言型回归测试
- `AppTests` 更偏历史演示 / 示例性质，不应与主单元测试等量齐观

## 4. 当前关注点

- 测试覆盖是否真实反映关键能力
- 样例式测试与有效回归测试的区分
- 历史 AppTests 的保留价值

## 5. 当前维护建议

- 新增能力优先补到 `Lanymy.Common.AllTests`
- 对存在等待、外部环境依赖或示例式调用的测试，应单独标记其验证级别
- 在决定重构某个公共模块前，先确认是否已有有效断言覆盖
- 后续功能修改默认需要同步补齐 `Lanymy.Common.AllTests` 对应用例，完整回归由用户手动执行，并用于确认功能、性能与稳定性

## 6. 当前风险点

- 测试分布不均，不能默认认为“有测试项目就代表高保障”
- 历史 AppTests 会给仓库带来一定维护噪音
- 若后续做框架升级或依赖替换，测试侧可能无法完全覆盖兼容性退化

## 7. 后续建议补充

- 测试项目清单
- 当前覆盖盲区
- 需要优先补强的模块
