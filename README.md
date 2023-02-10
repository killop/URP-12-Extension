

Table of Contents
- [概要](#概要)
- [X-Post-Processing](#x-post-processing)
- [Timeline-Volume](#timeline-volume)
- [URP-12-GammaUIAndSplitResolution](#urp-12-gammauiandsplitresolution)
- [AdaptivePerformance](#adaptiveperformance)

## 概要
  这个库使用的URP版本是 URP 12.1.10,适配unity 2021.3.18f1,在绝大多数安卓苹果手机上，都测试过，没有发现问题。 URP 12.1.7 +2021.3.4 这个组合在部分机型是有bug的（深度贴图适配问题）
## X-Post-Processing
* 主要搬运自[X-PostProcessing-URP](https://github.com/tkonexhh/X-PostProcessing-URP), 深度集成到URP12 里面，在原有官方基础上新增70+ 后处理效果
* 采用UPR 12 内置的RenderTargetSwapBuff 减少不必要的 RT分配
* 增加了后处理shader的 ShaderTripper 功能，减少了shader编译时间 
## Timeline-Volume
* 针对URP内置的volume framework 增加timeline轨道,方便 timeline 控制各种后效的效果
## URP-12-GammaUIAndSplitResolution
* 本项目囊括了[URP-12-GammaUIAndSplitResolution](https://github.com/killop/URP-12-GammaUIAndSplitResolution)里面所有的功能
## AdaptivePerformance
* 给出URP 如何做适配