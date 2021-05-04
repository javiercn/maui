using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Microsoft.Maui.Graphics;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Platform
{
	internal class PageContainer : ViewGroup
	{
		public PageContainer(Context context, IViewHandler child, bool inFragment = false) : base(context)
		{
			Id = AView.GenerateViewId();
			Child = child;
			IsInFragment = inFragment;
			AddView((AView)child.NativeView);
		}

		public IViewHandler Child { get; set; }

		public bool IsInFragment { get; set; }

		protected PageContainer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var deviceIndependentLeft = Context.FromPixels(l);
			var deviceIndependentTop = Context.FromPixels(t);
			var deviceIndependentRight = Context.FromPixels(r);
			var deviceIndependentBottom = Context.FromPixels(b);

			var destination = Rectangle.FromLTRB(deviceIndependentLeft, deviceIndependentTop,
				deviceIndependentRight, deviceIndependentBottom);
			Child.VirtualView.Arrange(destination);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			var deviceIndependentWidth = widthMeasureSpec.ToDouble(Context);
			var deviceIndependentHeight = heightMeasureSpec.ToDouble(Context);
			var size = Child.VirtualView.Measure(widthMeasureSpec, heightMeasureSpec);
			var nativeWidth = Context.ToPixels(size.Width);
			var nativeHeight = Context.ToPixels(size.Height);
			SetMeasuredDimension((int)nativeWidth, (int)nativeHeight);
		}
	}
}