using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Windows.ApplicationModel;
using Windows.Storage;
using FileAttributes = System.IO.FileAttributes;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class MauiAssetFileProvider
	{
		IDirectoryContents PlatformGetDirectoryContents(string subpath)
			=> new WindowsMauiAssetDirectoryContents(subpath);

		IFileInfo? PlatformGetFileInfo(string subpath)
		{
			// TODO: HACK: For now we strip out the folder because it is not preserved in the WinUI assets. We need to address this in the MauiAsset targets.
			var fileWithoutFolders = Path.GetFileName(subpath);

			// TODO: Also, instead of using WinUI Package APIs, we're going directly to the filesystem. They all seem to throw. Maybe we're on the wrong thread?
			var packagePath = Package.Current.InstalledLocation.Path;
			var assetFilePath = Path.Combine(packagePath, "Assets", fileWithoutFolders);

			var assetFileInfo = new FileInfo(assetFilePath);
			return assetFileInfo.Exists ? new WindowsMauiAssetFileInfo(assetFileInfo) : null;
		}

		IChangeToken? PlatformWatch(string filter)
			=> null;
	}

	class WindowsMauiAssetDirectoryContents : IDirectoryContents
	{
		public WindowsMauiAssetDirectoryContents(string path)
		{
			var dir = Package.Current.InstalledLocation.GetFolderAsync(path).GetResults();

			var items = dir.GetItemsAsync().GetResults();

			foreach (var item in items)
			{
				var fileInfo = new FileInfo(item.Path);
				if (fileInfo.Exists)
				{
					contents.Add(new WindowsMauiAssetFileInfo(fileInfo));
				}
			}
		}

		List<WindowsMauiAssetFileInfo> contents = new List<WindowsMauiAssetFileInfo>();

		public bool Exists { get; }

		public IEnumerator<IFileInfo> GetEnumerator()
			=> contents.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> contents.GetEnumerator();
	}

	class WindowsMauiAssetFileInfo : IFileInfo
	{
		public WindowsMauiAssetFileInfo(FileInfo fileInfo)
		{
			IsDirectory = fileInfo.Exists && (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
			Name = fileInfo.Name;
			Exists = fileInfo.Exists;
			PhysicalPath = fileInfo.FullName;
			LastModified = fileInfo.LastWriteTimeUtc;
			Length = fileInfo.Exists ? fileInfo.Length : 0;
		}

		public bool Exists { get; }
		public long Length { get; }
		public string PhysicalPath { get; }
		public string Name { get; }
		public DateTimeOffset LastModified { get; }
		public bool IsDirectory { get; }

		public Stream CreateReadStream()
			=> File.OpenRead(PhysicalPath);
	}
}
