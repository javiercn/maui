using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class MauiAssetFileProvider : IFileProvider
	{
		public MauiAssetFileProvider(string contentRoot)
		{
			ContentRoot = contentRoot;
		}

		public string ContentRoot { get; }

		IDirectoryContents IFileProvider.GetDirectoryContents(string subpath)
			=> PlatformGetDirectoryContents(ResolveSubPath(subpath));

		IFileInfo? IFileProvider.GetFileInfo(string subpath)
			=> PlatformGetFileInfo(ResolveSubPath(subpath));

		IChangeToken? IFileProvider.Watch(string filter)
			=> PlatformWatch(filter);

		private string ResolveSubPath(string subpath)
		{
			return
				string.IsNullOrEmpty(ContentRoot)
				? subpath
				: Path.Combine(ContentRoot, subpath);
		}
	}
}
