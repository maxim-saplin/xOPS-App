// Copyright(c) 2020 Maxim Saplin

//Xamarin SDK

//The MIT License(MIT)

//Copyright(c) .NET Foundation Contributors

//All rights reserved.

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(Saplin.xOPS.WPF.WebViewRenderer2))]
namespace Saplin.xOPS.WPF
{
	// Used source of original renderer though add try catch to avoid app crash when URL is unavailable
	public class WebViewRenderer2 : ViewRenderer<WebView, Microsoft.Toolkit.Wpf.UI.Controls.WebView>, IWebViewDelegate
	{
		WebNavigationEvent _eventState;
		bool _updating;

		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			if (e.OldElement != null) // Clear old element event
			{
				e.OldElement.EvalRequested -= OnEvalRequested;
				e.OldElement.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;
				e.OldElement.GoBackRequested -= OnGoBackRequested;
				e.OldElement.GoForwardRequested -= OnGoForwardRequested;
				e.OldElement.ReloadRequested -= OnReloadRequested;
			}

			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new Microsoft.Toolkit.Wpf.UI.Controls.WebView());

					Control.NavigationStarting += WebBrowserOnNavigating;
					Control.NavigationCompleted += WebBrowserOnNavigated;

					//Control.Navigated += WebBrowserOnNavigated;
					//Control.Navigating += WebBrowserOnNavigating;
				}

				// Update control property 
				Load();

				// Suscribe element event
				Element.EvalRequested += OnEvalRequested;
				Element.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
				Element.GoBackRequested += OnGoBackRequested;
				Element.GoForwardRequested += OnGoForwardRequested;
				Element.ReloadRequested += OnReloadRequested;
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == WebView.SourceProperty.PropertyName)
			{
				if (!_updating)
					Load();
			}
		}

		void Load()
		{
			if (Element.Source != null)
				Element.Source.Load(this);

			UpdateCanGoBackForward();
		}

		public void LoadHtml(string html, string baseUrl)
		{
			if (html == null)
				return;

			Control.NavigateToString(html);
		}

		public void LoadUrl(string url)
		{
			if (url == null)
				return;

			Control.Source = new Uri(url, UriKind.RelativeOrAbsolute);
		}


		void OnEvalRequested(object sender, EvalRequested eventArg)
		{
			Control.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Control.InvokeScript("eval", eventArg.Script)));
		}

		async Task<string> OnEvaluateJavaScriptRequested(string script)
		{
			if (!navigatingSent) return null; // Had a version of windows where due to some reasons WebView was broken and infinite wait happend in this method. Empericaly found that in that case this method was called without prior Navigating event being fired

			var tcr = new TaskCompletionSource<string>();
			var task = tcr.Task;

			Device.BeginInvokeOnMainThread(() => {
				try
				{
					tcr.SetResult((string)Control.InvokeScript("eval", new[] { script }));
				}
				catch { }
			});

			return await task.ConfigureAwait(false);
		}

		void OnGoBackRequested(object sender, EventArgs eventArgs)
		{
			if (Control.CanGoBack)
			{
				_eventState = WebNavigationEvent.Back;
				Control.GoBack();
			}

			UpdateCanGoBackForward();
		}

		void OnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			if (Control.CanGoForward)
			{
				_eventState = WebNavigationEvent.Forward;
				Control.GoForward();
			}
			UpdateCanGoBackForward();
		}

		void OnReloadRequested(object sender, EventArgs eventArgs)
		{
			Control.Refresh();
		}

		void SendNavigated(UrlWebViewSource source, WebNavigationEvent evnt, WebNavigationResult result)
		{
			Console.WriteLine("SendNavigated : " + source.Url);
			_updating = true;
			((IElementController)Element).SetValueFromRenderer(WebView.SourceProperty, source);
			_updating = false;

			Element.SendNavigated(new WebNavigatedEventArgs(evnt, source, source.Url, result));

			UpdateCanGoBackForward();
			_eventState = WebNavigationEvent.NewPage;
		}

		void UpdateCanGoBackForward()
		{
			((IWebViewController)Element).CanGoBack = Control.CanGoBack;
			((IWebViewController)Element).CanGoForward = Control.CanGoForward;
		}

		void WebBrowserOnNavigated(object sender, WebViewControlNavigationCompletedEventArgs navigationEventArgs)
		{
			if (navigationEventArgs.Uri == null) return;

			string url = navigationEventArgs.Uri.IsAbsoluteUri ? navigationEventArgs.Uri.AbsoluteUri : navigationEventArgs.Uri.OriginalString;
			SendNavigated(new UrlWebViewSource { Url = url }, _eventState, WebNavigationResult.Success);
			UpdateCanGoBackForward();
		}

		bool navigatingSent = false;

		void WebBrowserOnNavigating(object sender, WebViewControlNavigationStartingEventArgs navigatingEventArgs)
		{
			navigatingSent = true;
			
			if (navigatingEventArgs.Uri == null) return;

			string url = navigatingEventArgs.Uri.IsAbsoluteUri ? navigatingEventArgs.Uri.AbsoluteUri : navigatingEventArgs.Uri.OriginalString;
			var args = new WebNavigatingEventArgs(_eventState, new UrlWebViewSource { Url = url }, url);

			Element.SendNavigating(args);

			navigatingEventArgs.Cancel = args.Cancel;

			// reset in this case because this is the last event we will get
			if (args.Cancel)
				_eventState = WebNavigationEvent.NewPage;
		}

		void WebBrowserOnNavigationFailed(object sender, NavigationFailedEventArgs navigationFailedEventArgs)
		{
			if (navigationFailedEventArgs.Uri == null) return;

			string url = navigationFailedEventArgs.Uri.IsAbsoluteUri ? navigationFailedEventArgs.Uri.AbsoluteUri : navigationFailedEventArgs.Uri.OriginalString;
			SendNavigated(new UrlWebViewSource { Url = url }, _eventState, WebNavigationResult.Failure);
		}

		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.NavigationCompleted -= WebBrowserOnNavigated;
					Control.NavigationStarting -= WebBrowserOnNavigating;
					Control.Source = null;
					Control.Dispose();
				}

				if (Element != null)
				{
					Element.EvalRequested -= OnEvalRequested;
					Element.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;
					Element.GoBackRequested -= OnGoBackRequested;
					Element.GoForwardRequested -= OnGoForwardRequested;
					Element.ReloadRequested -= OnReloadRequested;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}
	}
}
