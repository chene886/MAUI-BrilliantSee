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
                window!.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.LightStatusBar;
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FAFAFA"));
                window!.DecorView.SystemUiFlags |= Android.Views.SystemUiFlags.LightNavigationBar;
                window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#FAFAFA"));
                //开启硬件加速
                activity.RunOnUiThread(() => activity.Window.DecorView.SetLayerType(Android.Views.LayerType.Hardware, null));
#endif
            }
            else
            {
#if ANDROID
                window!.DecorView.SystemUiFlags = (Android.Views.SystemUiFlags.Fullscreen | Android.Views.SystemUiFlags.HideNavigation | Android.Views.SystemUiFlags.ImmersiveSticky | Android.Views.SystemUiFlags.LayoutFullscreen | Android.Views.SystemUiFlags.LayoutHideNavigation);
                //关闭硬件加速
                activity.RunOnUiThread(() => activity.Window.DecorView.SetLayerType(Android.Views.LayerType.Software, null));
#endif
            }
        }
    }
}