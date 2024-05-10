using System.Runtime.Versioning;

namespace BrilliantSee.Behaviors
{
    internal class AndroidInitBehavior : Behavior<Page>
    {
        protected override void OnAttachedTo(Page page)
        {
            base.OnAttachedTo(page);
            SetColor();
        }

        private static void SetColor()
        {
#if ANDROID
            var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("Android Activity can't be null.");
            var window = activity.Window ?? throw new InvalidOperationException($"{nameof(activity.Window)} cannot be null");

            window!.Attributes!.LayoutInDisplayCutoutMode = Android.Views.LayoutInDisplayCutoutMode.ShortEdges;

            var uiModeManager = (Android.App.UiModeManager)activity.GetSystemService(Android.Content.Context.UiModeService)!;
            var isDarkTheme = uiModeManager!.NightMode is Android.App.UiNightMode.Yes;
            var BarColor = isDarkTheme ? "#000000" : "#FAFAFA";
            if (!isDarkTheme) window.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.LightNavigationBar | Android.Views.SystemUiFlags.LightStatusBar;
            window.SetNavigationBarColor(Android.Graphics.Color.ParseColor(BarColor));
            window.SetStatusBarColor(Android.Graphics.Color.ParseColor(BarColor));

            activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }
    }
}