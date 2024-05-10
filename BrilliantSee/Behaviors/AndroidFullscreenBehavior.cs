using System.Runtime.Versioning;

namespace BrilliantSee.Behaviors
{
    internal class AndroidFullscreenBehavior : Behavior<Page>
    {
        protected override void OnAttachedTo(Page page)
        {
            base.OnAttachedTo(page);
            SetFullscreen(false);
            page.Disappearing += (s, e) => SetFullscreen(true);
        }

        private static void SetFullscreen(bool isVisible)
        {
#if ANDROID
            var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("Android Activity can't be null.");
            var window = activity.Window ?? throw new InvalidOperationException($"{nameof(activity.Window)} cannot be null");
#endif
            if (isVisible)
            {
#if ANDROID
                window!.DecorView.SystemUiFlags = 0;
                var uiModeManager = (Android.App.UiModeManager)activity.GetSystemService(Android.Content.Context.UiModeService)!;
                var isDarkTheme = uiModeManager!.NightMode is Android.App.UiNightMode.Yes;
                var BarColor = isDarkTheme ? "#000000" : "#FAFAFA";
                if (!isDarkTheme) window.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.LightNavigationBar | Android.Views.SystemUiFlags.LightStatusBar;
                window.SetNavigationBarColor(Android.Graphics.Color.ParseColor(BarColor));
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor(BarColor));
                window.ClearFlags(Android.Views.WindowManagerFlags.Fullscreen);
                window.ClearFlags(Android.Views.WindowManagerFlags.LayoutNoLimits);
#endif
            }
            else
            {
#if ANDROID
                window!.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.ImmersiveSticky | Android.Views.SystemUiFlags.HideNavigation;
                window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);
                window.AddFlags(Android.Views.WindowManagerFlags.LayoutNoLimits);
#endif
            }
        }
    }
}