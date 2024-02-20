# BrilliantComic

.NET平台下MAUI跨平台框架漫画阅读器，旨在为已适配的漫画网站提供更好的阅读体验。

- 项目当前属于开发阶段，部分功能待完善...
- 主打安卓平台，可能存在异常，请谅解...

## 项目部分界面截图
<img src="img/favoritePage.jpg" width="360px" />
<br />
<img src="img/historyPage.jpg" width="360px" />
<br />
<img src="img/searchPage.jpg" width="360px" />
<br />
<img src="img/detailPage.jpg" width="360px" />
<br />
<img src="img/browsePage.jpg" width="360px" />
<br />


## 适配漫画网站

添加适配网站仅需在 **BrilliantComic.Models** 项目中添加：
- 实现 **ISource** 接口功能的图源类、
- 继承**Comic** 抽象类实现抽象方法的漫画类、
- 继承**Chapter** 抽象类的章节类。
并根据设配网站的需求略作调整即可

## 声明

项目仅用于学习交流，禁止其它任何用途。

项目使用部分图标来源：[Icons8](https://icons8.com)
