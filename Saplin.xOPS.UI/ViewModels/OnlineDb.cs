using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Input;
using Saplin.xOPS.UI.Misc;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class OnlineDb : BaseViewModel
    {
        public const string cpdt_web_url = @"https://maxim-saplin.github.io/xOPS-Web/";
        //const string cpdt_web_url = @"https2://maxim-saplin2.github2.io/xOPS-Web/";
        //const string cpdt_web_url = @"http://localhost:3000/";
        const string locale_param = "lang=";
        const string inapp_param = "inapp=";
        const string yd_param = "yd=";
        const string flt_64b_param = "flt_64b=";
        const string int_64b_param = "int_64b=";
        const string flt_thrd_param = "flt_thrd=";
        const string int_thrd_param = "int_thrd=";
        const string cnt_param = "cnt=";
        const string i_param = "i=";
        const string cpu_param = "cpu=";
        const string cores_param = "cores=";
        const string ram_param = "ram=";
        const string mdl_param = "mdl=";
        const string v_param = "v=";
        const string d_param = "d=";

        public bool Initialized { get; protected set; }

        WebView webView;

        public void BindWebView(WebView webView)
        {
            //// WPF webView is ActiveX and buggy. If there's no Internet Connection upon app launch WebView mail fail with OutOfMemory exception. Do not bind webView on WPF with no internet connection
            //var di = DependencyService.Get<IWpfWebViewInfo>();
            //if (di != null && !di.InternetConnected()) return;

            if (Device.RuntimePlatform == Device.WPF) throw new InvalidOperationException("WPF WebView not supported");

            this.webView = webView;
            webView.Navigating += Navigating;
            webView.Navigated += Navigated;
            webView.Source = Url;

            VmLocator.L11n.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(L11n._Locale))
                {
                    webView.Source = Url;
                }
            };

            Initialized = true;
        }

        private const string d_param_format = "ddMMyyHHmmss";

        private string GetEnvParams()
        {
            var prms = locale_param + VmLocator.L11n._Locale +
                         "&" + i_param + VmLocator.Options.IID +
                         "&" + v_param + VmLocator.Options.Version +
                         "&" + d_param + DateTime.UtcNow.ToString(d_param_format);

            var di = DependencyService.Get<IDeviceInfo>();

            if (di != null)
            {
                prms += "&" + cpu_param + Uri.EscapeUriString(di.GetCPU());
                prms += "&" + mdl_param + Uri.EscapeUriString(di.GetModelName());
                prms += "&" + ram_param + Uri.EscapeUriString(Math.Round(di.GetRamSizeGb(), 1).ToString());
            }

            prms += "&" + cores_param + Environment.ProcessorCount;

            return prms;
        }

        private string TrimQueryString(string prms)
        {
            if (prms.Length > 1023)
            {
                return prms.Substring(0, 1023);
            }

            return prms;
        }

        public string Url
        {
            get
            {
                var prms = inapp_param + Device.RuntimePlatform + "&" + GetEnvParams();

                prms = TrimQueryString(prms);

                return cpdt_web_url + "?" + prms;
            }
        }

        public string UrlNotInApp
        {
            get
            {
                var prms = GetEnvParams();

                prms = TrimQueryString(prms);

                return cpdt_web_url + "?" + prms;
            }
        }

        [DataContract]
        internal class CompareJson
        {
            [DataMember] public double ST_FLT; // GFLOPS
            [DataMember] public double MT_FLT; // GFLOPS
            [DataMember] public double ST_INT; // GINOPS
            [DataMember] public double MT_INT; // GINOPS
        }

        public string GetCompareUrl(TestRun run, Options options)
        {
            var compare = new CompareJson()
            {
                ST_FLT = Math.Round(run.FloatSingleThreaded.Value, 2),
                MT_FLT = Math.Round(run.FloatMultiThreaded.Value, 2),
                ST_INT = Math.Round(run.IntSingleThreaded.Value, 2),
                MT_INT = Math.Round(run.IntMultiThreaded.Value, 2),
            };

            var json = "";

            var mem = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(CompareJson));
            ser.WriteObject(mem, compare);
            mem.Position = 0;
            var sr = new StreamReader(mem);
            json = sr.ReadToEnd();

            json = Uri.EscapeDataString(json);

            var prms = inapp_param + Device.RuntimePlatform +
                "&" + yd_param + json +
                "&" + cnt_param + run.NumberOfRepeats +
                "&" + flt_64b_param + (options.Float64Bit ? "1" : "0") +
                "&" + int_64b_param + (options.Int64Bit ? "1" : "0") +
                "&" + flt_thrd_param + options.FloatThreads +
                "&" + int_thrd_param + options.IntThreads +
                "&" + GetEnvParams();

            prms = TrimQueryString(prms);

            return cpdt_web_url + "?" + prms;
        }

        public void PreLoadComparison(TestRun run, Options options)
        {
            if (!navigatedSuccesfullyAtLeastOnce) return; // if there were previuos issues with webview and there was no a single successfull navigtation prior to comparison then don't attempt to set any webview properties

            var url = GetCompareUrl(run, options);
            if (CheckUrlChanged(url)) webView.Source = url;
        }

        public void Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Source != null)
            {
                if (!e.Url.StartsWith(cpdt_web_url))
                {
                    wpfFail = true;
                    NavigatedNotSuccesfully(); // WPF, IE11 - if the URL is incorrect, the page is being redirected to some other URL, though Navigated will still get Success and original URL
                }
                else wpfFail = false;
            }
            else if (e.NavigationEvent == WebNavigationEvent.Forward) NavigatedSuccesfully(); // macOS hack
        }

        public void Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success && !wpfFail) NavigatedSuccesfully();
            else NavigatedNotSuccesfully();
        }

        private void NavigatedSuccesfully()
        {
            //if (navigatedNotSuccesfully) return;
            IsEnabled = true;
            navigatedNotSuccesfully = false;
            navigatedSuccesfullyAtLeastOnce = true;
            RaisePropertyChanged(nameof(NotAvailable));

            if (doShowAfterNavigated)
            {
                //base.DoShow(doShowAfterNavigated);
                doShowAfterNavigated = false;
            }
        }

        private bool navigatedNotSuccesfully = false;
        private bool navigatedSuccesfullyAtLeastOnce = false;
        private void NavigatedNotSuccesfully()
        {
            IsEnabled = false;
            navigatedNotSuccesfully = true;
            RaisePropertyChanged(nameof(NotAvailable));
            doShowAfterNavigated = false;

            // Start reconnection attempts only only for non WPF apps. In WPF WebView failed browsing attempts can't be traced
            if (Device.RuntimePlatform != Device.WPF) Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (navigatedNotSuccesfully && !VmLocator.TestRun.TestStarted)
                {
                    webView.Source = Url;
                }

                return false;
            });
        }

        public bool NotAvailable
        {
            get
            {
                return navigatedNotSuccesfully;
            }
        }

        private bool isEnabled = false;

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool doShowAfterNavigated = false;
        private bool wpfFail;

        private bool CheckUrlChanged(string url)
        {
            if (webView == null) return false;

            var webViewUrl = (webView.Source as UrlWebViewSource).Url;

            var d = "&" + d_param;

            if (webViewUrl.Contains(d))
            {
                var index = webViewUrl.IndexOf(d, StringComparison.InvariantCulture);
                webViewUrl = webViewUrl.Remove(index, d.Length + d_param_format.Length);
            }

            if (url.Contains(d))
            {
                var index = url.IndexOf(d, StringComparison.InvariantCulture);
                url = url.Remove(index, d.Length + d_param_format.Length);
            }

            return url != webViewUrl;
        }

        public void SendPageHit(string actionName)
        {
            if (!navigatedNotSuccesfully)
                try // WPF might fail with an unhandled exception if there's no document loaded
                {
                    webView?.EvaluateJavaScriptAsync(
                        "gtag('config', 'UA-17809502-3', {page_location:location.toString()+'" + "&" + actionName + "=" + DateTime.UtcNow.ToString(d_param_format) + "'});");
                }
                catch { };
        }
    }
}
