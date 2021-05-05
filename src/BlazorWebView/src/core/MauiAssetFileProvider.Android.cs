using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class MauiAssetFileProvider
	{
		IDirectoryContents PlatformGetDirectoryContents(string subpath)
			=> new AndroidMauiAssetDirectoryContents(subpath);

		IFileInfo PlatformGetFileInfo(string subpath)
			=> new AndroidMauiAssetFileInfo(subpath);

		IChangeToken? PlatformWatch(string filter)
			=> null;
	}

	class AndroidMauiAssetFileInfo : IFileInfo
	{
		public AndroidMauiAssetFileInfo(string asset)
		{
			var itemsCount = Android.App.Application.Context.Assets?.List(asset)?.Length ?? 0;

			PhysicalPath = asset;
			IsDirectory = itemsCount > 0;
			Length = IsDirectory ? itemsCount : 1;
			Name = IsDirectory
				? new DirectoryInfo(asset)?.Name ?? asset
				: Path.GetFileName(asset);
		}

		public bool Exists => true;
		public long Length { get; }
		public string PhysicalPath { get; }
		public string Name { get; }
		public DateTimeOffset LastModified { get; }
		public bool IsDirectory { get; }

		public Stream CreateReadStream()
			=> Android.App.Application.Context.Assets?.Open(PhysicalPath)
				?? throw new FileNotFoundException();
	}

	class AndroidMauiAssetDirectoryContents : IDirectoryContents
	{
		public AndroidMauiAssetDirectoryContents(string subpath)
		{
			var sep = Java.IO.File.Separator ?? "/";

			var dir = subpath.Replace("/", sep);

			var assets = Android.App.Application.Context.Assets?.List(dir);

			foreach (var a in assets ?? Array.Empty<string>())
				files.Add(new AndroidMauiAssetFileInfo(subpath.TrimEnd(sep.ToCharArray()) + sep + a));
		}

		List<AndroidMauiAssetFileInfo> files = new List<AndroidMauiAssetFileInfo>();

		public bool Exists
			=> files.Any();

		public IEnumerator<IFileInfo> GetEnumerator()
			=> files.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> files.GetEnumerator();
	}
}
