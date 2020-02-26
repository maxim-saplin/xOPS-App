using AppKit;
using Foundation;
using Saplin.xOPS.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace Saplin.xOPS.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow _window;
        public AppDelegate()
        {
            var style = NSWindowStyle.Resizable | NSWindowStyle.Titled | NSWindowStyle.FullSizeContentView;

            var rect = new CoreGraphics.CGRect(200, 1000, 450, 600);
            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            _window.Title = "xOPS";
            _window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = true;
            _window.WindowShouldClose += WindowShouldClose;
            //_window.TitleVisibility = NSWindowTitleVisibility.Hidden;
            _window.TitlebarAppearsTransparent = true;
            //_window.StandardWindowButton(NSWindowButton.CloseButton).Hidden = true;
            _window.StandardWindowButton(NSWindowButton.ZoomButton).Hidden = true;
            _window.StandardWindowButton(NSWindowButton.MiniaturizeButton).Hidden = true;
            _window.MakeKeyWindow();
        }

        public override NSWindow MainWindow
        {
            get { return _window; }
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            var app = new App();
            LoadApplication(app);
            base.DidFinishLaunching(notification);
        }

        private bool WindowShouldClose(NSObject sender)
        {
            return true;
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}
