using BrilliantComic.Models;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services
{
    public class DBService
    {
        /// <summary>
        /// 数据库文件名
        /// </summary>
        private const string _dbFile = "BrilliantComic.db3";

        /// <summary>
        /// 数据库打开标志
        /// </summary>
        private const SQLite.SQLiteOpenFlags _flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        private readonly SourceService _sourceService;
        private SQLiteAsyncConnection _db;

        public DBService(SourceService sourceService)
        {
            _sourceService = sourceService;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, _dbFile);
            _db = new SQLiteAsyncConnection(dbPath, _flags);

            _ = InitAsync();
        }

        /// <summary>
        /// 初始化数据库，创建表
        /// </summary>
        /// <returns></returns>
        private async Task InitAsync()
        {
            await _db.CreateTableAsync<DBComic>();
            await _db.CreateTableAsync<SettingItem>();
            if (await _db.Table<SettingItem>().CountAsync() > 0) { return; }

            var Share = "我正在使用BrilliantComic看漫画，资源丰富、功能强大，快来下载体验吧！下载地址：https://";
            var Disclaimer = "免责声明（Disclaimer）\r\n\r\nBrilliantComic（下称为：本软件）提醒您：在使用本软件前，请您务必仔细阅读并透彻理解本声明。您可以选择不使用本软件，但如果您使用本软件，您的使用行为将被视为对本声明全部内容的认可:\r\n\r\n一、本软件是一款解析指定规则并获取内容的工具，为广大资源爱好者提供一种方便、快捷舒适的搜索体验。当您搜索想要的资源时，本软件会根据您所使用的规则将该搜索词以关键词的形式提交到各个第三方资源网站。各第三方网站返回的内容与本软件无关，本软件作者对其概不负责，亦不承担任何法律责任。\r\n\r\n二、任何通过使用本软件而链接到的第三方网页均系他人制作或提供，您可能从第三方网页上获得其他服务，本软件作者对其合法性概不负责，亦不承担任何法律责任。\r\n\r\n三、本软件尊重并保护所有使用本软件的用户的个人隐私权，我们不储存用户的任何个人信息和浏览信息。用户产生的所有数据均缓存到本地，卸载即消失，请妥善管理您的数据。\r\n\r\n四、本软件致力于最大程度地减少资源爱好者在自行搜寻过程中的无意义的时间浪费，通过专业搜索展示不同网站中资源情况。本软件在为广大资源爱好者提供方便、快捷舒适的试看体验的同时，也使优秀的互联网作品得以迅速、更广泛的传播，从而达到了在一定程度促进互联网作品充分繁荣发展之目的。本软件鼓励广大资源爱好者通过本软件发现优秀的互联网作品及其提供商，并建议观看正版资源。任何单位或个人认为通过本软件搜索链接到的第三方网页内容可能涉嫌侵犯其信息网络传播权，应该及时向本软件作者提出书面权力通知，并提供身份证明、权属证明及详细侵权情况证明。本软件作者在收到上述法律文件后，将会及时联系，并依法尽快断开相关链接内容。\r\n\r\n五、鉴于本软件以非人工检索方式、根据您键入的关键字自动生成到第三方资源的链接，除本软件注明之服务条款外，其他一切因使用本软件而可能遭致的意外、疏忽、侵权及其造成的损失（包括因下载被搜索链接到的第三方网站内容而感染电脑病毒），本软件作者对其概不负责，亦不承担任何法律责任。\r\n\r\n六、本软件搜索结果根据您键入的关键字自动搜索获得并生成，不代表本软件作者赞成被搜索链接到的第三方资源上的内容或立场。\r\n\r\n七、任何网站如果不想被本软件收录（即不被搜索到），应该及时向本软件作者反映，或者在其网站页面中根据拒绝蜘蛛协议（Robots Exclusion Protocol）加注拒绝收录的标记，否则，本软件将依照惯例视其为可收录网站。\r\n\r\n八、本软件没有任何的破解功能，只是个资源浏览器，对网页做搬运处理，使每一个网页都以同样的界面显示，优化网页浏览的体验，只能观看网络上第三方网站提供的免费资源。\r\n\r\n九、本软件无法观看第三方网页收费资源，只能对可以免费获取免费观看部分做搬运，软件没有任何破解功能，也不会提供任何破解服务，收费资源请支持正版。本软件不存储任何资源资源，资源资源均由第三方网站提供。\r\n\r\n十、请不要使用本软件浏览当地国家法律所禁止的内容，如用户在使用过程中发现第三方网站内容违法了当地国家法律，请立即联系作者，会第一时间删除并断开相关链接和相关资源源。\r\n\r\n十一、有问题可与本软件开发作者联系修改，本开发作者遵循避风港原则。\r\n\r\n十二、您访问的有些网站或其上的广告如果存在您不喜欢的内容或有其他问题，您可自行离开那些网站，本浏览器及本作者与这些网站无关、也不负任何责任。\r\n\r\n十三、本软件只是一个阅读漫画的工具，您浏览的资源网页由其域名所有者提供、制作，网页的内容及广告也是由域名所有者负责，与本软件无关，本作者不负任何责任。\r\n\r\n十四、本软件基于HttpClient进行资源内容请求，机器无法识别资源内容是否合规，如您发现任何低俗、色情、盗版的内容请通知我们联系我们进行搜索屏蔽。为此给您带来的不便，敬请谅解！本软件始终如一的支持维护著作版权，也愿意为优秀资源的宣传推广做出应有的贡献！为广大漫迷和作者提供一个绿色健康的资源搜索平台。";
            var Policy = "隐私政策（Privacy Policy）\r\n\r\nBrilliantComic（下称为：本软件）尊重并保护所有使用服务用户的个人隐私权。为了给您提供更准确、更有个性化的服务，本软件会按照本隐私权政策的规定使用和披露您的个人信息。但本软件将以高度的勤勉、审慎义务对待这些信息。除本隐私权政策另有规定外，在未征得您事先许可的情况下，本软件不会将这些信息对外披露或向第三方提供。目前，本软件无后台服务器，不储存用户的任何个人信息和浏览信息。用户产生的所有数据均缓存到本地，卸载即消失，请妥善管理您的数据。本软件会不时更新本隐私权政策。您在同意本软件服务使用协议之时，即视为您已经同意本隐私权政策全部内容。本隐私权政策属于本软件服务使用协议不可分割的一部分。";
            var Agreement = "用户协议（User Agreement）\r\n\r\n欢迎您使用BrilliantComic（以下称“本软件”），请阅读和理解《BrilliantComic用户服务协议》（以下简称“本协议”），特别是免除或者限制本作者责任的免责条款及对用户的权利限制条款。您使用本软件则意味着接受本协议条款，否则您将无权下载、安装或使用及其相关服务。您的下载、安装或其他使用行为，将视为对本协议的接受，并同意接受各项条款的约束。\r\n本协议是您与本软件作者之间关于您下载、安装、使用、复制本软件，以及使用作者相关服务所订立的协议。“用户”或“您”是指通过本作者提供的获取软件授权的途径获得软件授权许可和软件产品的个人或单一实体。\r\n本软件作者有权随时根据有关法律法规的变化、作者经营状况、经营策略调整和软件升级等原因修订本协议。更新后的协议条款将会在本软件官方网站上或本软件上公布，公布即有效代替原来的协议条款，用户可随时使用本软件查阅最新版协议条款。如果用户不同意协议改动内容，请立即停止使用本软件。如果用户继续使用本软件，则视为接受本协议的改动。\r\n未满18周岁的未成年人应在法定监护人陪同下阅读本协议。\r\n\r\n一、服务内容及使用须知\r\n本软件是本作者为用户提供的漫画阅读工具，本软件本身不直接上传、提供内容，对用户接入、浏览、访问、传输的内容不做任何修改或编辑。本浏览器仅提供相关的资源浏览服务，除此之外与相关服务有关的设备（如个人电脑、手机、及其他与接入互联网或移动网有关的装置）及所需的费用（如为接入互联网而支付的电话费及上网费、为使用移动网而支付的手机费）均应由用户自行负担。用户不得滥用本软件的服务，作者在此郑重提请您注意，任何经由本服务以接入、浏览、访问、传输即时信息、电子邮件或任何其他方式传输的资讯、资料、文字、软件、音乐、音讯、照片、图形、视讯、信息、用户的登记资料或其他资料（以下简称“内容”），无论系公开还是私下传输，均由内容提供者、使用者对其接入、浏览、访问、传输、使用行为自行承担责任。本软件无法控制经由本服务传输之内容，也无法对用户的使用行为进行全面控制，因此不能保证内容的合法性、正确性、完整性、真实性或品质；您已预知使用本服务时，可能会接触到令人不快、不适当等内容，并同意将自行加以判断并承担所有风险，而不依赖于本软件。\r\n若用户使用本软件服务的行为不符合本协议，作者在经由通知、举报等途径发现时有权做出独立判断，且有权在无需事先通知用户的情况下立即终止向用户提供部分或全部服务。用户若通过本软件散布和传播反动、色情或其他违反国家法律、法规的信息，本软件的系统记录有可能作为用户违反法律法规的证据；因用户进行上述内容在本软件的接入、浏览、访问、传输等行为而导致任何第三方提出索赔要求或衍生的任何损害或损失，由用户自行承担全部责任。\r\n作者有权对用户使用本软件服务的情况进行监督，如经由通知、举报等途径发现用户在使用本软件所提供的网络服务时违反任何本协议的规定，作者有权要求用户改正或直接采取一切作者认为必要的措施（包括但不限于暂停或终止用户使用软件服务的权利）以减轻用户不当行为造成的影响。\r\n\r\n二、所有权\r\n作者保留对以下各项内容、信息完全的、不可分割的所有权及知识产权:除用户自行上载、传播的内容外，本软件及其所有元素，包括但不限于所有数据、技术、软件、代码、用户界面以及与其相关的任何衍生作品。\r\n未经作者同意，上述资料均不得在任何媒体直接或间接发布、播放、出于播放或发布目的而改写或再发行，或者被用于其他任何商业目的。上述资料或其任何部分仅可作为私人用途而保存在某台计算机内。本软件不就由上述资料产生或在传送或递交全部或部分上述资料过程中产生的延误、不准确、错误和遗漏或从中产生或由此产生的任何损害赔偿，以任何形式向用户或任何第三方负法律、经济责任。\r\n作者为提供服务而使用的任何软件（包括但不限于软件中所含的任何图像、照片、动画、录像、录音、音乐、文字和附加程序、随附的帮助材料）的一切权利均属于该软件的著作权人，未经该软件的著作权人许可，用户不得对该软件进行反向工程（reverseengineer）、反向编译（decompile）或反汇编（disassemble），或以其他方式发现其原始编码，以及实施任何涉嫌侵害著作权的行为。\r\n\r\n三、保证与承诺\r\n用户保证，其在使用本软件的过程中不得并禁止直接或间接的:\r\n（1）删除、隐匿、改变本软件上显示或其中包含的任何专利、版权、商标或其他所有权声明；\r\n（2）以任何方式干扰或企图干扰本软件其任何部分或功能的正常运行；\r\n（3）避开、尝试避开或声称能够避开任何内容保护机制或者本软件数据度量工具；\r\n（4）未获得作者事先书面同意以书面格式或图形方式使用源自作者的任何注册或未注册的商品、服务标志、作者徽标（LOGO）、URL或其他标志；\r\n（5）使用任何标志，包括但不限于以对此类标志的所有者的权利的玷污、削弱和损害的方式使用作者标志，或者以违背本协议的方式为自己或向其他任何人设定或声明设定任何义务或授予任何权利或权限；\r\n（6）提供跟踪功能，包括但不限于识别其他用户在个人主页上查看或操作；\r\n（7）自动将浏览器窗口定向到其他网页；\r\n（8）未经授权冒充他人或获取对本软件的访问权或者未经用户明确同意，让任何其他人亲自识别该用户；\r\n（9）用户违反上述任何一款的保证，作者均有权就其情节，对其做出警告、屏蔽、直至取消资格的处罚；如因用户违反上述保证而给本软件、本软件用户或作者的任何合作伙伴造成损失，用户自行负责承担一切法律责任并赔偿损失。\r\n用户承诺:\r\n（1）用户利用本软件提供的服务上传、发布、传送或通过其他方式传播的内容，不得含有任何违反国家法律法规政策的信息，包括但不限于下列信息:\r\n\t（a）反对宪法所确定的基本原则的；\r\n\t（b）危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家统一的；\r\n\t（c）损害国家荣誉和利益的；\r\n\t（d）煽动民族仇恨、民族歧视，破坏民族团结的；\r\n\t（e）破坏国家宗教政策，宣扬邪教和封建迷信的；\r\n\t（f）散布谣言，扰乱社会秩序，破坏社会稳定的；\r\n\t（g）散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；\r\n\t（h）侮辱或者诽谤他人，侵害他人合法权益的；\r\n（2）含有法律、行政法规禁止的其他内容的。\r\n（3）用户不得为任何非法目的而使用本软件服务系统；不得以任何形式使用本软件服务侵犯作者的商业利益，包括并不限于发布非经作者许可的商业广告；不得利用本软件服务系统进行任何可能对互联网或移动网正常运转造成不利影响的行为。\r\n（4）用户不得利用本软件的服务从事以下活动:\r\n\t（a）未经允许，进入计算机信息网络或者使用计算机信息网络资源的；\r\n\t（b）未经允许，对计算机信息网络功能进行删除、修改或者增加的；\r\n\t（c）未经允许，对进入计算机信息网络中存储、处理或者传输的数据和应用程序进行删除、修改或者增加的；\r\n\t（d）故意制作、传播计算机病毒等破坏性程序的；\r\n\t（e）其他危害计算机信息网络安全的行为。\r\n（5）如因用户利用本软件提供的服务上传、发布、传送或通过其他方式传播的内容存在权利瑕疵或侵犯了第三方的合法权益（包括但不限于专利权、商标权、著作权及著作权邻接权、肖像权、隐私权、名誉权等）而导致作者或与作者合作的其他单位面临任何投诉、举报、质询、索赔、诉讼；或者使作者或者与作者合作的其他单位因此遭受任何名誉、声誉或者财产上的损失，用户应积极地采取一切可能采取的措施，以保证作者及与作者合作的其他单位免受上述索赔、诉讼的影响。同时用户对作者及与作者合作的其他单位因此遭受的直接及间接经济损失负有全部的损害赔偿责任。\r\n\r\n四、知识产权保护\r\n如果用户上传的内容允许其他用户下载、查看、收听或以其他方式访问或分发，其必须保证该内容的发布和相关行为实施符合相关知识产权法律法规中相关的版权政策，包括但不限于:\r\n（1）用户在收到侵权通知之时，应立即删除或禁止访问声明的侵权内容，并同时联系递送通知的人员以了解详细信息。\r\n（2）用户知悉并同意作者将根据相关法律法规对第三方发出的合格的侵权通知进行处理，并按照要求删除或禁止访问声明的侵权内容，采用并实施适当的政策，以期杜绝在相应条件下重复侵权。\r\n\r\n五、隐私权保护\r\n作者充分尊重用户个人信息的保护，隐私权保护声明介绍了您在使用作者的服务时，作者如何处理您的个人信息。您使用作者的服务，即表示您同意作者可以按照隐私政策进行处理相应信息。\r\n\r\n六、法律责任与免责\r\n（1）鉴于网络服务的特殊性，用户同意本软件有权随时变更、中断或终止部分或全部的服务。如变更、中断或终止的服务属于免费服务，本软件无需通知用户，也无需对任何用户或任何第三方承担任何责任。\r\n（2）用户理解，本软件需要定期或不定期地对提供网络服务的平台或相关的设备检修，作者无需为此承担任何责任，但本软件应尽可能事先进行通告。\r\n（3）本软件可在任何时候为任何原因变更本服务或删除其部分功能。本软件可在任何时候取消或终止对用户的服务。本软件取消或终止服务的决定不需要理由或通知用户。一旦服务取消，用户使用本服务的权利立即终止。一旦本服务取消或终止，用户在本服务中储存的任何信息可能无法恢复。\r\n（4）本软件不保证（包括但不限于）:\r\n\t（a）本软件适合用户的使用要求；\r\n\t（b）本软件不受干扰，及时、安全、可靠或不出现错误；及用户经由本软件取得的任何产品、服务或其他材料符合用户的期望；\r\n\t（c）用户使用经由本软件下载或取得的任何资料，其风险由用户自行承担；因该等使用导致用户电脑系统损坏或资料流失，用户应自己负完全责任。\r\n（5）基于以下原因而造成的利润、商业信誉、资料损失或其他有形或无形损失，本软件不承担任何直接、间接的赔偿:\r\n\t（a）对本软件的使用或无法使用；\r\n\t（b）经由本软件购买或取得的任何产品、资料或服务；\r\n\t（c）用户资料遭到未授权的使用或修改；及其他与本软件相关的事宜。\r\n\r\n七、其他\r\n（1）在适用法律法规允许的范围内，本协议最终解释权归作者所有。\r\n（2）本协议已经公布即生效，作者有权随时对协议内容进行修改，修改后的结果公布于本软件网站上。如果不同意本软件对本协议相关条款所做的修改，用户有权停止使用网络服务。如果用户继续使用网络服务，则视为用户接受作者对本协议相关条款所做的修改。\r\n（3）本协议项下本软件对于用户所有的通知均可通过网页公告、电子邮件、手机短信或常规的信件传送等方式进行；该等通知于发送之日视为已送达收件人。\r\n（4）本协议的订立、执行和解释及争议的解决均应适用中国法律并受中国法院管辖。如双方就本协议内容或其执行发生任何争议，双方应尽量友好协商解决；协商不成时，任何一方均可向合同签订地的人民法院提起诉讼。.本协议构成双方对本协议之约定事项及其他有关事宜的完整协议，除本协议规定的之外，未赋予本协议各方其他权利。\r\n（5）如本协议中的任何条款无论因何种原因完全或部分无效或不具有执行力，本协议的其余条款仍应有效并具有约束力。\r\n\r\n八、如何联系我们\r\n如果您对我们的隐私政策及对您个人信息的处理有任何疑问、意见、建议或投诉，请通过邮箱:3256366564@qq.com与我们联系。在一般情况下，我们会在10个工作日内对您的请求予以答复。\r\n\r\n九、隐私政策的生效\r\n本隐私政策版本更新日期为2024年4月8日，将于2024年4月8日正式生效。\r\n";

            var defaultSettingItems = new List<SettingItem>
            {
                new SettingItem { Name = "包子漫画", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "古风漫画", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "Goda漫画", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "Goda(英)", Value = "IsSelected", Category = "Source" },
                //new SettingItem { Name = "mangahasu", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "分享应用", Value = "去分享", Category = "通用" ,Description = Share},
                new SettingItem { Name = "错误反馈", Value = "去反馈", Category = "通用" },
                new SettingItem { Name = "支持开源", Value = "去支持", Category = "通用" },
                new SettingItem { Name = "免责声明", Value = "查看声明", Category = "关于" ,Description = Disclaimer},
                new SettingItem { Name = "隐私政策", Value = "查看政策", Category = "关于" ,Description = Policy},
                new SettingItem { Name = "用户协议", Value = "查看协议", Category = "关于" ,Description = Agreement},
                new SettingItem { Name = "ModelId", Value = "", Category = "AIModel" },
                new SettingItem { Name = "ApiKey", Value ="", Category = "AIModel" },
                new SettingItem { Name = "ApiUrl", Value = "", Category = "AIModel" },
                new SettingItem { Name = "AudioStatus", Value = "false", Category = "Audio" },
            };
            await _db.InsertAllAsync(defaultSettingItems);
        }

        /// <summary>
        /// 获取漫画集合
        /// </summary>
        /// <param name="category">需要获取的漫画类型</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<Comic>> GetComicsAsync(DBComicCategory category)
        {
            var dbComics = await _db.Table<DBComic>()
                .Where(i => i.Category == category)
                .ToListAsync();
            var ret = dbComics
                .Select(i => _sourceService.GetComic(i.SourceName)!.CreateComicFromDBComic(i))
                .Where(c => c is not null)
                .Select(c => c!).ToList();

            return ret;
        }

        /// <summary>
        /// 检查漫画是否存在
        /// </summary>
        /// <param name="comic">需要检查的漫画</param>
        /// <param name="category">漫画的类型</param>
        /// <returns></returns>
        public async Task<bool> IsComicExistAsync(Comic comic, DBComicCategory category)
        {
            return await _db.Table<DBComic>().Where(i => i.Url == comic.Url && i.Category == category).CountAsync() > 0;
        }

        /// <summary>
        /// 保存漫画
        /// </summary>
        /// <param name="comic">需要保存的漫画</param>
        /// <param name="category">漫画的类型</param>
        /// <returns></returns>
        public async Task<int> SaveComicAsync(Comic comic, DBComicCategory category)
        {
            var dbComic = comic.CreateDBComicFromComic(comic);
            dbComic.Category = category;
            if (_db.Table<DBComic>().Where(i => i.Url == comic.Url && i.Category == category).CountAsync().Result > 0)
            {
                await this.DeleteComicAsync(comic, category);
            }
            return await _db.InsertAsync(dbComic);
        }

        /// <summary>
        /// 更新漫画信息
        /// </summary>
        /// <param name="comic">需要更新的漫画</param>
        /// <returns></returns>
        public async Task<int> UpdateComicAsync(Comic comic)
        {
            var dbComic = comic.CreateDBComicFromComic(comic);
            var existingComic = await _db.Table<DBComic>()
                                    .Where(c => c.Url == comic.Url && c.Category == comic.Category)
                                    .FirstOrDefaultAsync();
            // 如果存在具有相同Url和LastReadedChapterIndex的记录，更新那个记录
            dbComic.Id = existingComic.Id; // 确保我们更新的是正确的记录
            return await _db.UpdateAsync(dbComic);
        }

        /// <summary>
        /// 删除漫画
        /// </summary>
        /// <param name="comic">需要删除的漫画</param>
        /// <param name="category">漫画的类型</param>
        /// <returns></returns>
        public async Task<int> DeleteComicAsync(Comic comic, DBComicCategory category)
        {
            return await _db.Table<DBComic>().DeleteAsync(i => i.Url == comic.Url && i.Category == category);
        }

        /// <summary>
        /// 获取对应类别的设置项
        /// </summary>
        /// <param name="category">类别</param>
        /// <returns></returns>
        public async Task<List<SettingItem>> GetSettingItemsAsync(string category)
        {
            return await _db.Table<SettingItem>().Where(i => i.Category == category).ToListAsync();
        }

        public async Task<string> GetSettingItemMessageAsync(string value)
        {
            var item = await _db.Table<SettingItem>().Where(i => i.Value == value).FirstOrDefaultAsync();
            return item.Description;
        }

        /// <summary>
        /// 更新设置项
        /// </summary>
        /// <param name="setting">设置项</param>
        /// <returns></returns>
        public async Task<int> UpdateSettingItemAsync(SettingItem setting)
        {
            return await _db.UpdateAsync(setting);
        }

        /// <summary>
        /// 获取语音功能状态
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetAudioStatus()
        {
            var Item = await _db.Table<SettingItem>().Where(i => i.Category == "Audio").FirstOrDefaultAsync();
            return Item is null ? false : Item.Value == "true";
        }
    }
}