
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class TestRun : BaseViewModel
    {
        Compute compute = new Compute();

        private void ResetValues()
        {
            IntMultiThreaded = IntSingleThreaded
                = FloatMultiThreaded = FloatSingleThreaded = null;

            RaisePropertyChanged(nameof(FloatSingleThreaded));
            RaisePropertyChanged(nameof(IntSingleThreaded));
            RaisePropertyChanged(nameof(FloatMultiThreaded));
            RaisePropertyChanged(nameof(IntMultiThreaded));
        }

        public Command Retry => new Command(StartTest);

        public void StartTest()
        {
            const int iterations = 100 * 1000 * 1000;
            const int threads = 16;

            ResetValues();

            Task.Run(() => {
                compute.RunFlops64Bit(iterations);
                FloatSingleThreaded = compute.LastResultGigaOPS;
                Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatSingleThreaded)));
                

                compute.RunInops32Bit(iterations);
                IntSingleThreaded = compute.LastResultGigaOPS;
                Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(IntSingleThreaded)));

                compute.RunXopsMultiThreaded(iterations, threads, inops: false, precision64Bit: true);
                FloatMultiThreaded = compute.LastResultGigaOPS;
                Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatMultiThreaded)));

                compute.RunXopsMultiThreaded(iterations, threads, inops: true);
                IntMultiThreaded = compute.LastResultGigaOPS;
                Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(IntMultiThreaded)));
            });

        }

        public void BreakTest()
        {

        }

        public bool TestStarted
        {
            get { return false; }
        }

        public double? FloatSingleThreaded { get; private set; }
        public double? FloatMultiThreaded { get; private set; }
        public double? IntSingleThreaded { get; private set; }
        public double? IntMultiThreaded { get; private set; }

    }
}
