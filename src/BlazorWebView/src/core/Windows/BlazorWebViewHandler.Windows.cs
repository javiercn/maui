using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui.Handlers;
using WebView2Control = Microsoft.UI.Xaml.Controls.WebView2;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class BlazorWebViewHandler : ViewHandler<IBlazorWebView, WebView2Control>
	{
		private WebView2WebViewManager? _webviewManager;
		private ObservableCollection<RootComponent>? _rootComponents;

		protected override WebView2Control CreateNativeView()
		{
			return new WebView2Control();
		}

		protected override void DisconnectHandler(WebView2Control nativeView)
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

			// We use the entry assembly's location to determine where the static assets got copied to.
			// It is critical to avoid calling System.IO.Path APIs that use the "current directory"
			// when they are passed in a relative path, because the app has no control over where
			// the "current directory" is (it could be anywhere).
			var entryAssemblyPath = global::System.Reflection.Assembly.GetEntryAssembly()!.Location;
			var entryAssemblyFolder = Path.GetDirectoryName(entryAssemblyPath);

			// We assume the host page is always in the root of the content directory, because it's
			// unclear there's any other use case. We can add more options later if so.
			var hostPageFullPath = Path.Combine(entryAssemblyFolder!, HostPage!);
			var contentRootDirFullPath = Path.GetDirectoryName(hostPageFullPath) ?? string.Empty;
			var hostPageRelativePath = Path.GetRelativePath(contentRootDirFullPath, hostPageFullPath);

			var fileProvider = new PhysicalFileProvider(contentRootDirFullPath);

			_webviewManager = new WebView2WebViewManager(new WinUIWebView2Wrapper(NativeView), Services!, MauiDispatcher.Instance, fileProvider, hostPageRelativePath);
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
	}
}
