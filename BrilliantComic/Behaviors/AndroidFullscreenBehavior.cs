using Android.Views;
using Color = Android.Graphics.Color;

namespace BrilliantComic.Behaviors
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
                window!.DecorView.SystemUiFlags = SystemUiFlags.LightStatusBar;
                window.SetStatusBarColor(Color.ParseColor("#FAFAFA"));
                window!.DecorView.SystemUiFlags |= SystemUiFlags.LightNavigationBar;
                window.SetNavigationBarColor(Color.ParseColor("#FAFAFA"));
#endif
            }
            else
            {
#if ANDROID
                window!.DecorView.SystemUiFlags = (SystemUiFlags.Fullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);

#endif
            }
        }
    }
}