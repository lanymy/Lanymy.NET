# Active Regression Backlog

本文件用于维护当前活跃任务，并为每项任务建立统一的回归验证视图。

状态说明：

- `未开始`：尚未进入实施
- `进行中`：已进入分析或实现阶段
- `待用户回归`：实现与定向验证已完成，等待用户执行完整回归
- `已归档`：已完成并迁入归档目录

## P0

### 1. Netty 客户端重连链取消边界治理

- 状态：`未开始`
- 目标模块：`Lanymy.Common.Instruments.Socket.Netty.Abstractions`
- 关联文档：[Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)
- 重点代码：
  - `src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseNettySocketClient.cs`
  - `src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseClientChannelHandler.cs`
- 任务目标：
  - 收敛多入口重连行为
  - 为重连链增加可取消边界
  - 避免停止后仍继续重连或重复重连
- 对应测试文件：
  - `src/UnitTests/Lanymy.Common.AllTests/SocketNettyTests.cs` 或同域新测试文件
- 功能验证：
  - 启动失败后按预期进入重连
  - 停止后不再继续重连
  - `ChannelInactive` 不会与主动停止产生重复重连
- 性能验证：
  - 重连失败循环不会引入明显的异常放大或无界任务堆积
  - 重连等待与触发次数在可预期范围内
- 稳定性验证：
  - 多轮启动/停止后状态一致
  - 重连过程中主动关闭不会残留后台链路
  - 异常后对象可观测状态清晰
- 用户完整回归状态：`未回归`
- 备注：
  - 当前这是最优先的长寿命生命周期风险点

### 2. Netty 主线三维回归测试补齐

- 状态：`未开始`
- 目标模块：`Lanymy.Common.Instruments.Socket.Netty.Abstractions`
- 关联文档：[Testing-Convention.md](../conventions/Testing-Convention.md)
- 重点代码：
  - `src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/`
  - `src/UnitTests/Lanymy.Common.AllTests/`
- 任务目标：
  - 为 Netty 主线建立功能正确性、性能验证、稳定性验证的最小有效回归集
- 对应测试文件：
  - `src/UnitTests/Lanymy.Common.AllTests/SocketNettyTests.cs` 或按职责拆分的新测试文件
- 功能验证：
  - 连接成功、连接失败、重连、主动关闭等行为正确
- 性能验证：
  - 高频重连或重复调用场景下无明显退化
- 稳定性验证：
  - 多轮运行后无状态漂移、无异常噪音放大
- 用户完整回归状态：`未回归`
- 备注：
  - 应与重连链治理同步推进，不建议完全拆离

## P1

### 3. 传统 Socket 主线 `.Wait()` 与同步阻塞异步收敛

- 状态：`未开始`
- 目标模块：`Lanymy.Common.Instruments.Socket.Abstractions`
- 关联文档：[Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)
- 重点代码：
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs`
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs`
- 任务目标：
  - 降低同步阻塞异步带来的吞吐下降、停机卡顿与死锁风险
- 对应测试文件：
  - `src/UnitTests/Lanymy.Common.AllTests/SocketLifecycleTests.cs`
  - `src/UnitTests/Lanymy.Common.AllTests/UdpLifecycleTests.cs`
- 功能验证：
  - 启动、发送、关闭语义保持一致
- 性能验证：
  - 高频发送与关闭链路不出现明显性能退化
- 稳定性验证：
  - 多轮启动关闭下不出现卡死、假死或资源残留
- 用户完整回归状态：`未回归`

### 4. 关闭链异常可观测性继续收口

- 状态：`未开始`
- 目标模块：`Lanymy.Common.Instruments.Socket.Abstractions`
- 关联文档：[Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)
- 重点代码：
  - `BaseUdpClient.cs`
  - `BaseTcpClient.cs`
  - `BaseTcpServerClient.cs`
  - `BaseTcpServer.cs`
- 任务目标：
  - 继续减少空 `catch`
  - 将业务错误、网络断开、关闭失败更清晰地区分上报
- 对应测试文件：
  - `src/UnitTests/Lanymy.Common.AllTests/SocketLifecycleTests.cs`
  - `src/UnitTests/Lanymy.Common.AllTests/SocketServerTests.cs`
  - `src/UnitTests/Lanymy.Common.AllTests/UdpLifecycleTests.cs`
- 功能验证：
  - 错误上报路径符合预期
- 性能验证：
  - 异常上报不会带来明显额外阻塞
- 稳定性验证：
  - 关闭尾声异常不会破坏最终状态闭合
- 用户完整回归状态：`未回归`

### 5. 主回归测试的性能与稳定性维度补强

- 状态：`未开始`
- 目标模块：`src/UnitTests/Lanymy.Common.AllTests`
- 关联文档：
  - [Testing-Convention.md](../conventions/Testing-Convention.md)
  - [architecture/testing/README.md](../architecture/testing/README.md)
- 任务目标：
  - 将现有偏样例式验证的用例逐步收敛为可回归的三维验证用例
- 对应测试文件：
  - 按具体模块落到现有 `*Tests.cs`
- 功能验证：
  - 断言真实覆盖关键功能契约
- 性能验证：
  - 为高频路径建立轻量、可重复、可比较趋势的验证
- 稳定性验证：
  - 为重复调用、批量处理、长时或多轮场景建立最小有效验证
- 用户完整回归状态：`未回归`

## P2

### 6. TCP 主线重复模式收敛

- 状态：`未开始`
- 目标模块：`Lanymy.Common.Instruments.Socket.Abstractions`
- 关联文档：[Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)
- 任务目标：
  - 在当前生命周期约束已收敛的基础上，进一步提炼公共守卫模式，减少后续漂移
- 对应测试文件：
  - `src/UnitTests/Lanymy.Common.AllTests/SocketLifecycleTests.cs`
  - `src/UnitTests/Lanymy.Common.AllTests/SocketServerTests.cs`
- 功能验证：
  - 重构后行为保持一致
- 性能验证：
  - 公共守卫抽取不引入额外热点退化
- 稳定性验证：
  - 重构后生命周期边界契约不回退
- 用户完整回归状态：`未回归`

### 7. 生命周期专题图补齐

- 状态：`未开始`
- 目标模块：`ai_docs/architecture/instruments`
- 关联文档：[Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)
- 任务目标：
  - 补齐 TCP Client、TCP ServerClient、UDP Client 生命周期图
- 对应测试文件：
  - 无直接测试文件
- 功能验证：
  - 文档与当前实现一致
- 性能验证：
  - 不适用
- 稳定性验证：
  - 文档能支撑后续边界回归分析
- 用户完整回归状态：`不适用`

## P3

### 8. AllTests 覆盖盲区与验证级别盘点

- 状态：`未开始`
- 目标模块：`src/UnitTests/Lanymy.Common.AllTests`
- 关联文档：[architecture/testing/README.md](../architecture/testing/README.md)
- 任务目标：
  - 区分主回归测试、样例式测试、历史噪音测试
  - 为后续补强提供优先级依据
- 对应测试文件：
  - `src/UnitTests/Lanymy.Common.AllTests/` 全量盘点
- 功能验证：
  - 明确哪些模块已具备有效回归
- 性能验证：
  - 明确哪些模块尚无性能验证
- 稳定性验证：
  - 明确哪些模块尚无稳定性验证
- 用户完整回归状态：`未回归`
