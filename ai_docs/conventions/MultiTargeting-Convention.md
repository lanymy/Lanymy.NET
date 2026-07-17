# MultiTargeting-Convention

## 目的

- 统一多目标框架类库项目的维护约定。

## 约定

- 修改 `TargetFramework` 或 `TargetFrameworks` 前，先评估兼容影响面。
- 条件 `PackageReference` 应与目标框架条件保持一致。
- 修改跨框架公共 API 时，应优先保证行为语义一致。
- 如涉及历史依赖替换，应记录受影响框架范围。
