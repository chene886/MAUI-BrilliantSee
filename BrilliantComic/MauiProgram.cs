using BrilliantComic.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BrilliantComic.ViewModels;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Media;
using Plugin.Maui.Audio;

namespace BrilliantComic
{
    public static class MauiProgram
    {
        public static IServiceProvider? servicesProvider;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 添加服务
            AddServices(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            servicesProvider = builder.Services.BuildServiceProvider();
            return builder.Build();
        }

        /// <summary>
        /// 向maui app中添加服务
        /// </summary>
        /// <param name = "services"></param>
        private static void AddServices(IServiceCollection services)
        {
            //视图
            services.AddSingleton<FavoritePage>();
            services.AddSingleton<HistoryPage>();
            services.AddTransient<SettingPage>();
            services.AddTransient<SearchPage>();
            services.AddTransient<DetailPage>();
            services.AddTransient<BrowsePage>();
            services.AddTransient<AIPage>();

            //视图模型
            services.AddSingleton<FavoriteViewModel>();
            services.AddSingleton<HistoryViewModel>();
            services.AddTransient<SettingViewModel>();
            services.AddTransient<SearchViewModel>();
            services.AddTransient<DetailViewModel>();
            services.AddTransient<BrowseViewModel>();
            services.AddTransient<AIViewModel>();

            //服务
            services.AddSingleton<DBService>();
            services.AddSingleton<SourceService>();
            services.AddSingleton<AIService>();
            services.AddSingleton(AudioManager.Current);
        }
    }
}