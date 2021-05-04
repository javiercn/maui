using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Microsoft.Maui.Controls.Platform
{
	class ShellImagePart : IImageSourcePart
	{
		public IImageSource Source 
		{ 
			get; 
			set; 
		}

		public bool IsAnimationPlaying { get; set; }
		public bool IsLoading { get; private set; }

		public void UpdateIsLoading(bool isLoading)
		{
			IsLoading = isLoading;
		}
	}
}
