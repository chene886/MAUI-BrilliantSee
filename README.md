# BrilliantComic
## 项目介绍
.NET平台下MAUI跨平台框架漫画阅读器，旨在为已适配的漫画网站提供更好的阅读体验。

- 项目当前属于开发阶段，目前仅适配一个图源网站、部分功能待完善...
- 主打安卓平台，可能存在异常，请谅解...

## 后续发展方向：
- 适配更多图源网站以覆盖全网资源（包含国外作品）
- 拓宽领域至小说、动漫甚至影视作品
- 顺应当下潮流，开发基于semantic-kernel的AI交互助手，便利用户操作

## 项目部分界面截图
### 搜索页
- 搜索漫画、选择图源
<img src="img/searchPage_1.jpg" width="360px" />

### 漫画详情页
- 收藏、浏览器打开、漫画倒序
- 一键跳转最后浏览章节
<img src="img/detailPage_1.jpg" width="360px" />

### 收藏页
- 一键检查更新
<img src="img/favoritePage_1.jpg" width="360px" />

### 历史记录页
- 清空历史记录
<img src="img/historyPage_1.jpg" width="360px" />

### 漫画浏览页
<img src="img/browsePage_1.jpg" width="360px" />

## 适配漫画网站
添加适配网站仅需在 **BrilliantComic.Models** 项目中添加：
- 实现 **ISource** 接口功能的图源类、
- 继承**Comic** 抽象类实现抽象方法的漫画类、
- 继承**Chapter** 抽象类的章节类、
- 在**SourceService**中注册图源和图源名。
再根据设配网站的需求略作调整即可

## 声明

项目仅用于学习交流，禁止其它任何用途。

项目使用部分图标来源：[Icons8](https://icons8.com)

项目基础：[卧龙Brilliant_see](https://gitee.com/long2023/brilliant_see?_from=gitee_search)
