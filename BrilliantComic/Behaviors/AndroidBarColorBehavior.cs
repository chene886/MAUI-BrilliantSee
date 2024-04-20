using System.Runtime.Versioning;

namespace BrilliantSee.Behaviors
{
    internal class AndroidBarColorBehavior : Behavior<Page>
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
#endif

#if ANDROID
                window!.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.LightStatusBar;
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FAFAFA"));
                window!.DecorView.SystemUiFlags |= Android.Views.SystemUiFlags.LightNavigationBar;
                window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#FAFAFA"));
#endif
        }
    }
}