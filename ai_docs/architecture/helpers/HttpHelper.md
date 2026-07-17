# HttpHelper

## 1. 模块定位

- 对应项目：`src/Commons/Lanymy.Common.Helpers.HttpHelper`
- 对应实现：`src/Commons/Lanymy.Common.Helpers.HttpHelper/HttpHelper.cs`
- 该模块提供的是“直接可调用的 HTTP 请求辅助能力”，而不是可注入的现代 HTTP 服务封装。

## 2. 当前暴露能力

从当前实现看，`HttpHelper` 主要提供：

- `GetResponseContentDataAsync<T>()`
- `GetResponseContentStringAsync()`
- `HttpGetAsync(...)`
- `HttpPostAsync(...)`
- `HttpPostAsync<T>()`
- `HttpPostMultipartFormDataAsync(...)`

能力特点：
- 同时支持字符串返回和泛型反序列化返回
- 支持对象转 JSON 后 POST
- 支持 multipart form-data 附件上传

## 3. 当前实现风格

- 以 `public static` 方法为主
- 内部直接使用 `HttpClientFactory.Create()`
- 泛型结果依赖 JSON 反序列化辅助类
- URL 参数通过字符串拼接生成
- 附件上传依赖附件信息模型与流封装

## 4. 当前维护关注点

- 这是典型的“开箱即用”工具类，调用门槛低，但配置能力有限。
- 若后续要引入统一超时、重试、日志、认证头等能力，当前结构扩展性一般。
- 若后续要提升可测试性，当前静态 API 风格会成为约束点。

## 5. 当前风险点

- 当前泛型 `HttpGetAsync<T>` 实现内部调用的是 `HttpPostAsync(url, parameters)` 语义上值得复核。
- 参数拼接未体现 URL 编码处理，后续若处理复杂参数需要谨慎。
- 未见统一响应状态检查或异常包装层，调用侧可能需要自行兜底。

## 6. 推荐阅读顺序

1. `HttpHelper.cs`
2. `Lanymy.Common.Helpers.SerializeHelper.Json`
3. `Lanymy.Common.Abstractions.Models.AttachmentInfoModels`

## 7. 后续建议

- 可单独补一份“HTTP 辅助 API 一致性评估”
- 可后续评估是否要拆出更现代的可注入实现层
- 当前第一轮静态复验结果见 [../../debugs/review_round_01.md](../../debugs/review_round_01.md)
