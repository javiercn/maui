using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class MauiAssetFileProvider
	{
		IDirectoryContents PlatformGetDirectoryContents(string subpath)
			=> new iOSMauiAssetDirectoryContents(subpath);

		IFileInfo PlatformGetFileInfo(string subpath)
			=> new iOSMauiAssetFileInfo(subpath, false);

		IChangeToken? PlatformWatch(string filter)
			=> null;
	}

	class iOSMauiAssetFileInfo : IFileInfo
	{
		public iOSMauiAssetFileInfo(string path, bool isDirectory)
		{
			PhysicalPath = path;
			IsDirectory = isDirectory;

			if (isDirectory)
			{
				var dirInfo = new DirectoryInfo(path);
				Length = dirInfo.EnumerateFileSystemInfos()?.Count() ?? 0;
				Name = dirInfo.Name;
				Exists = dirInfo.Exists;
			}
			else
			{
				var fileInfo = new FileInfo(path);
				Length = fileInfo?.Length ?? 0;
				Name = fileInfo?.Name ?? path;
				Exists = fileInfo?.Exists ?? false;
			}
		}

		public bool Exists { get; }
		public long Length { get; }
		public string PhysicalPath { get; }
		public string Name { get; }
		public DateTimeOffset LastModified { get; }
		public bool IsDirectory { get; }

		public Stream CreateReadStream()
			=> File.OpenRead(PhysicalPath)
				?? throw new FileNotFoundException();
	}

	class iOSMauiAssetDirectoryContents : IDirectoryContents
	{
		public iOSMauiAssetDirectoryContents(string subpath)
		{
			var resPath = NSBundle.MainBundle.BundlePath;

			var dirPath = Path.Combine(resPath, subpath);

			foreach (var file in Directory.GetFiles(dirPath))
				files.Add(new iOSMauiAssetFileInfo(file, false));

			foreach (var dir in Directory.GetDirectories(dirPath))
				files.Add(new iOSMauiAssetFileInfo(dir, true));
		}

		List<iOSMauiAssetFileInfo> files = new List<iOSMauiAssetFileInfo>();

		public bool Exists
			=> files.Any();

		public IReadOnlyList<iOSMauiAssetFileInfo> GetFiles()
			=> files;

		public IEnumerator<IFileInfo> GetEnumerator()
			=> files.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> files.GetEnumerator();
	}
}
