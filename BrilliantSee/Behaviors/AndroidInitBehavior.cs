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

                window!.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.LightStatusBar;
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FAFAFA"));
                window!.DecorView.SystemUiFlags |= Android.Views.SystemUiFlags.LightNavigationBar;
                window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#FAFAFA"));
                //强制竖屏
                activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }
    }
}