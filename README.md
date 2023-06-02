

Table of Contents
- [概要](#概要)
- [X-Post-Processing](#x-post-processing)
- [Timeline-Volume](#timeline-volume)
- [URP-12-GammaUIAndSplitResolution](#urp-12-gammauiandsplitresolution)
- [AdaptivePerformance](#adaptiveperformance)
- [更新日志](#更新日志)
    - [2023-05-16](#2023-05-16)
    - [2023-05-16](#2023-05-16-1)

## 概要
  这个库使用的URP版本是 URP 12.1.10,适配unity 2021.3.18f1,在绝大多数安卓苹果手机上，都测试过，没有发现问题。 URP 12.1.7 +2021.3.4 这个组合在部分机型是有bug的（深度贴图适配问题）
## X-Post-Processing
* 主要搬运自[X-PostProcessing-URP](https://github.com/tkonexhh/X-PostProcessing-URP), 深度集成到URP12 里面，在原有官方基础上新增70+ 后处理效果
* 采用UPR 12 内置的RenderTargetSwapBuff 减少不必要的 RT分配
* 增加了后处理shader的 ShaderTripper 功能，减少了shader编译时间 
## Timeline-Volume
* 针对URP内置的volume framework 增加timeline轨道,方便 timeline 控制各种后效的效果
## URP-12-GammaUIAndSplitResolution
* 本项目囊括了[URP-12-GammaUIAndSplitResolution](https://github.com/killop/URP-12-GammaUIAndSplitResolution)里面所有的功能，并且了优化
* 而且 UI相机的target 是不是硬件屏幕还是 swapbuffer，你可以任意切换，说白了，这个库实现了 市面上所有的关于 UI 和场景相机分辨率分离的 思路，按需切换即可
## AdaptivePerformance
* 给出URP 如何做适配

## 更新日志

#### 2023-05-16
* 所以增加了 sEnableUICameraUseSwapBuffer 静态变量来控制 UI相机使用 swapBuffer，这样UI 相机也支持后处理，但是多了一次blit操作，有效率消耗，所以建议这个选项只在必要的时候打开
* 原来的 sIsGammaCorrectEnable 也加回来了，来控制是不是要启动 Gamma矫正
#### 2023-05-16
* 实现了 [URP渲染优化总结](https://zhuanlan.zhihu.com/p/626256175?)文章里面 所说的优化
* 场景相机用 swapbuffer 渲染，UI 相机直接用cameraTarget渲染，省去了一次blit 操作和一张RT
* 这次更新之后，暂时不支持 UI相机的gamma空间渲染，如果想支持，也是非常简单，修改UI 相机的cameraTarget为RT，UI元素在gamma 空间渲染，最终用FinaBlitPass 到屏幕上，参考2023-05-16 之前的版本

