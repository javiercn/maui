﻿using Foundation;
using UIKit;
using WebKit;
using RectangleF = CoreGraphics.CGRect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Graphics;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class BlazorWebViewHandler : ViewHandler<IBlazorWebView, WKWebView>
	{
		private IOSWebViewManager? _webviewManager;
		private ObservableCollection<RootComponent>? _rootComponents;

		private const string AppOrigin = "app://0.0.0.0/";
		private const string BlazorInitScript = @"
			window.__receiveMessageCallbacks = [];
			window.__dispatchMessageCallback = function(message) {
				window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });
			};
			window.external = {
				sendMessage: function(message) {
					window.webkit.messageHandlers.webwindowinterop.postMessage(message);
				},
				receiveMessage: function(callback) {
					window.__receiveMessageCallbacks.push(callback);
				}
			};

			Blazor.start();

			(function () {
				window.onpageshow = function(event) {
					if (event.persisted) {
						window.location.reload();
					}
				};
			})();
		";

		protected override WKWebView CreateNativeView()
		{
			var config = new WKWebViewConfiguration();

			config.Preferences.SetValueForKey(NSObject.FromObject(true), new NSString("developerExtrasEnabled"));

			config.UserContentController.AddScriptMessageHandler(new WebViewScriptMessageHandler(MessageReceived), "webwindowinterop");
			config.UserContentController.AddUserScript(new WKUserScript(
				new NSString(BlazorInitScript), WKUserScriptInjectionTime.AtDocumentEnd, true));

			// iOS WKWebView doesn't allow handling 'http'/'https' schemes, so we use the fake 'app' scheme
			config.SetUrlSchemeHandler(new SchemeHandler(this), urlScheme: "app");

			return new WKWebView(RectangleF.Empty, config)
			{
				BackgroundColor = UIColor.Clear,
				AutosizesSubviews = true
			};
		}

		private void MessageReceived(Uri uri, string message)
		{
			_webviewManager?.MessageReceivedInternal(uri, message);
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			SetDesiredSize(widthConstraint, heightConstraint);

			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}

		void SetDesiredSize(double width, double height)
		{
			if (NativeView != null)
			{
				var x = NativeView.Frame.X;
				var y = NativeView.Frame.Y;

				NativeView.Frame = new RectangleF(x, y, width, height);
			}
		}

		protected override void DisconnectHandler(WKWebView nativeView)
		{
			//nativeView.StopLoading();

			//_webViewClient?.Dispose();
			//_webChromeClient?.Dispose();
		}

		private bool RequiredStartupPropertiesSet =>
			//_webview != null &&
			HostPage != null &&
			Services != null;

		private string? HostPage { get; set; }
		private ObservableCollection<RootComponent>? RootComponents
		{
			get => _rootComponents;
			set
			{
				if (_rootComponents != null)
				{
					// Remove any previously-known root components and unhook events
					_rootComponents.Clear();
					_rootComponents.CollectionChanged -= OnRootComponentsCollectionChanged;
				}

				_rootComponents = value;

				if (_rootComponents != null)
				{
					// Add new root components and hook events
					if (_rootComponents.Count > 0 && _webviewManager != null)
					{
						_ = _webviewManager.Dispatcher.InvokeAsync(async () =>
						{
							foreach (var component in _rootComponents)
							{
								await component.AddToWebViewManagerAsync(_webviewManager);
							}
						});
					}
					_rootComponents.CollectionChanged += OnRootComponentsCollectionChanged;
				}
			}
		}

		private void StartWebViewCoreIfPossible()
		{
			if (!RequiredStartupPropertiesSet ||
				_webviewManager != null)
			{
				return;
			}
			if (NativeView == null)
			{
				throw new InvalidOperationException($"Can't start {nameof(BlazorWebView)} without native web view instance.");
			}

			// We assume the host page is always in the root of the content directory, because it's
			// unclear there's any other use case. We can add more options later if so.
			var contentRootDir = Path.GetDirectoryName(HostPage) ?? string.Empty;
			var hostPageRelativePath = Path.GetRelativePath(contentRootDir, HostPage!);
			var fileProvider = new PhysicalFileProvider(root: contentRootDir);

			_webviewManager = new IOSWebViewManager(this, NativeView, Services!, MauiDispatcher.Instance, fileProvider, hostPageRelativePath);
			if (RootComponents != null)
			{
				foreach (var rootComponent in RootComponents)
				{
					// Since the page isn't loaded yet, this will always complete synchronously
					_ = rootComponent.AddToWebViewManagerAsync(_webviewManager);
				}
			}
			_webviewManager.Navigate("/");
		}

		public static void MapHostPage(BlazorWebViewHandler handler, IBlazorWebView webView)
		{
			handler.HostPage = webView.HostPage;
			handler.StartWebViewCoreIfPossible();
		}

		public static void MapRootComponents(BlazorWebViewHandler handler, IBlazorWebView webView)
		{
			handler.RootComponents = webView.RootComponents;
			handler.StartWebViewCoreIfPossible();
		}

		private void OnRootComponentsCollectionChanged(object? sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs eventArgs)
		{
			// If we haven't initialized yet, this is a no-op
			if (_webviewManager != null)
			{
				// Dispatch because this is going to be async, and we want to catch any errors
				_ = _webviewManager.Dispatcher.InvokeAsync(async () =>
				{
					var newItems = eventArgs.NewItems!.Cast<RootComponent>();
					var oldItems = eventArgs.OldItems!.Cast<RootComponent>();

					foreach (var item in newItems.Except(oldItems))
					{
						await item.AddToWebViewManagerAsync(_webviewManager);
					}

					foreach (var item in oldItems.Except(newItems))
					{
						await item.RemoveFromWebViewManagerAsync(_webviewManager);
					}
				});
			}
		}

		private sealed class WebViewScriptMessageHandler : NSObject, IWKScriptMessageHandler
		{
			private Action<Uri, string> _messageReceivedAction;

			public WebViewScriptMessageHandler(Action<Uri, string> messageReceivedAction)
			{
				_messageReceivedAction = messageReceivedAction ?? throw new ArgumentNullException(nameof(messageReceivedAction));
			}

			public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
			{
				if (message is null)
				{
					throw new ArgumentNullException(nameof(message));
				}
				_messageReceivedAction(new Uri(AppOrigin), ((NSString)message.Body).ToString());
			}
		}

		private class SchemeHandler : NSObject, IWKUrlSchemeHandler
		{
			private readonly BlazorWebViewHandler _webViewHandler;

			public SchemeHandler(BlazorWebViewHandler webViewHandler)
			{
				_webViewHandler = webViewHandler;
			}

			[Export("webView:startURLSchemeTask:")]
			public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
			{
				var responseBytes = GetResponseBytes(urlSchemeTask.Request.Url.AbsoluteString, out var contentType, statusCode: out var statusCode);
				if (statusCode == 200)
				{
					using (var dic = new NSMutableDictionary<NSString, NSString>())
					{
						dic.Add((NSString)"Content-Length", (NSString)(responseBytes.Length.ToString(CultureInfo.InvariantCulture)));
						dic.Add((NSString)"Content-Type", (NSString)contentType);
						// Disable local caching. This will prevent user scripts from executing correctly.
						dic.Add((NSString)"Cache-Control", (NSString)"no-cache, max-age=0, must-revalidate, no-store");
						using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, statusCode, "HTTP/1.1", dic);
						urlSchemeTask.DidReceiveResponse(response);
					}
					urlSchemeTask.DidReceiveData(NSData.FromArray(responseBytes));
					urlSchemeTask.DidFinish();
				}
			}

			private byte[] GetResponseBytes(string url, out string contentType, out int statusCode)
			{
				var allowFallbackOnHostPage = url.EndsWith("/");
				if (_webViewHandler._webviewManager!.TryGetResponseContentInternal(url, allowFallbackOnHostPage, out statusCode, out var statusMessage, out var content, out var headers))
				{
					statusCode = 200;
					using var ms = new MemoryStream();

					content.CopyTo(ms);
					content.Dispose();

					contentType = headers["Content-Type"];

					return ms.ToArray();
				}
				else
				{
					statusCode = 404;
					contentType = string.Empty;
					return Array.Empty<byte>();
				}
			}

			[Export("webView:stopURLSchemeTask:")]
			public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
			{
			}
		}
	}
}
