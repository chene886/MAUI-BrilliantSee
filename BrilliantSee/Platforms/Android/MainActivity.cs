using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;

namespace BrilliantSee
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        //protected override void OnCreate(Bundle? savedInstanceState)
        //{
        //    Window!.Attributes!.LayoutInDisplayCutoutMode = Android.Views.LayoutInDisplayCutoutMode.ShortEdges;

        //    var uiModeManager = (UiModeManager)GetSystemService(UiModeService)!;
        //    var isDarkTheme = uiModeManager!.NightMode is UiNightMode.Yes;
        //    var BarColor = isDarkTheme ? "#000000" : "#FAFAFA";
        //    Window.DecorView.SystemUiFlags = isDarkTheme ? Window.DecorView.SystemUiFlags : Android.Views.SystemUiFlags.LightNavigationBar | Android.Views.SystemUiFlags.LightStatusBar;
        //    Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor(BarColor));
        //    Window.SetStatusBarColor(Android.Graphics.Color.ParseColor(BarColor));

        //    RequestedOrientation = ScreenOrientation.Portrait;

        //    base.OnCreate(savedInstanceState);
        //}
    }
}