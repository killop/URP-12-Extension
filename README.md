

Table of Contents
- [X-Post-Processing](#x-post-processing)
- [Timeline-Volume](#timeline-volume)
- [URP-12-GammaUIAndSplitResolution](#urp-12-gammauiandsplitresolution)
## X-Post-Processing
* 主要搬运自[X-PostProcessing-URP](https://github.com/tkonexhh/X-PostProcessing-URP), 深度集成到URP12 里面，在原有官方基础上新增70+ 后处理效果
* 采用UPR 12 内置的RenderTargetSwapBuff 减少不必要的 RT分配
* 增加了后处理shader的 ShaderTripper 功能，减少了shader编译时间 
## Timeline-Volume
* 针对URP内置的volume framework 增加timeline轨道,方便 timeline 控制各种后效的效果
## URP-12-GammaUIAndSplitResolution
* 本项目囊括了[URP-12-GammaUIAndSplitResolution](https://github.com/killop/URP-12-GammaUIAndSplitResolution)里面所有的功能
