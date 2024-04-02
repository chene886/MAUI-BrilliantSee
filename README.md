# BrilliantComic
## 项目介绍
.NET平台下MAUI跨平台框架漫画阅读器，旨在为已适配的漫画网站提供更好的阅读体验。

- 项目当前属于开发阶段，目前适配了四个图源网站（包含一个英文漫画网站）、部分功能待完善...
- 主打安卓平台，可能存在异常，请谅解...
  
## 待解决问题
- CollectionView复用回收机制下向上翻阅卡顿，闪退问题
  - 目前换成了ListView，卡顿、闪退问题已改善，但当加载图片太大时会闪退 (20240315)
- 部分源网站适配困难问题，如需要cookies，以及部分采用js动态加载的页面
- 安卓部分版本（API：24< 、>30)无法正常调整状态栏和导航栏样式
- 减少适配网站冗余重复代码，提高代码质量，提升适配效率
  - 减少了Source类和Comic类及对应实体的冗余代码，减少了适配网站时多做的重复工作(20240328)

## 后续发展方向：
- 适配更多图源网站以覆盖全网资源（包含国外作品）
  - 本来有一个英文网站，因为性能问题关掉了，后续考虑适配其他外文网站 (20240315)
- 拓宽领域至小说、动漫甚至影视作品
- 顺应当下潮流，开发基于semantic-kernel的AI交互助手，便利用户操作
  - 目前ai助手基础代码已完成，但在调用时不太顺利，比如往kernel导入plugins(20240315)
  - ai助手插件已正常导入，但由于sk的planner似乎不见了的原因，且其他替代方案不顺利，目前只能进行部分单步操作(20240318)
  - 现在设置语音识别功能让每个页面执行手动能执行的单步操作，除了全新的体验以外比较鸡肋，考虑后面实现多步动作(20240327)
  - 目前语音功能几乎是不可用的，语音解码慢和识别准确率低，大部分功能无法正常运行(20240327)

## 项目部分界面截图
### AI助手页
<img src="img/aiPage1.jpg" width="360px" />
<img src="img/aiPage2.jpg" width="360px" />

### 搜索页
- 搜索漫画、选择图源
<img src="img/searchPage.jpg" width="360px" />

### 漫画详情页
- 收藏、浏览器打开、漫画倒序
- 一键跳转最后浏览章节
<img src="img/detailPage.jpg" width="360px" />

### 收藏页
- 一键检查更新
<img src="img/favoritePage.jpg" width="360px" />

### 历史记录页
- 清空历史记录
<img src="img/historyPage.jpg" width="360px" />

### 漫画浏览页
<img src="img/browsePage.jpg" width="360px" />

### 系统设置页（暂无功能）
<img src="img/settingPage.jpg" width="360px" />

## 适配漫画网站
添加适配网站仅需在 **BrilliantComic.Models** 项目中添加：
- 实现 **Source** 抽象功能的图源类、
- 继承**Comic** 类、实现抽象方法的漫画类、
- 继承**Chapter** 类、实现抽象方法的章节类。

在 **BrilliantComic.Services** 项目中：
- 在**SourceService**中注册图源和对应漫画。
- 在**DBService**初始化图源设置项。

再根据设配网站的需求略作调整即可

## 声明

项目仅用于学习交流，禁止其它任何用途。

项目使用部分图标来源：[Icons8](https://icons8.com) 、[Iconfont](https://www.iconfont.cn)

项目基础：[卧龙Brilliant_see](https://gitee.com/long2023/brilliant_see?_from=gitee_search)
