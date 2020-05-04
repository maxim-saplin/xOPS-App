
using Xamarin.Forms;

namespace Saplin.xOPS.UI.Misc
{
    public static class ScreenOn
    {
        static IKeepScreenOn screenOn = DependencyService.Get<IKeepScreenOn>();

        public static void Enable()
        {
            screenOn?.Enable();
        }

        public static void Disable()
        {
            screenOn?.Disable();
        }
    }
}
