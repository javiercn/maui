using System;
using System.Drawing;
using System.Threading;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace Microsoft.Maui
{
	public static class TransformationExtensions
	{
		static CALayer? Layer;
		static bool IsInteractive;
		static CGPoint OriginalAnchor;
		static Rectangle LastBounds;
		static int UpdateCount;

		public static void UpdateTranslationX(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateTranslationY(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateScale(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateRotation(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateRotationX(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateRotationY(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateAnchorX(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		public static void UpdateAnchorY(this UIView nativeView, IView view)
		{
			nativeView.UpdateTransformation(view);
		}

		internal static void UpdateTransformation(this UIView nativeView, IView view)
		{
			if (Layer == null)
			{
				Layer = nativeView.Layer;
				IsInteractive = nativeView.UserInteractionEnabled;
				OriginalAnchor = Layer.AnchorPoint;
			}

			if (view == null)
				return;

			bool shouldInteract;

			if (view is ILayout layout)
				shouldInteract = layout.IsEnabled;
			else
				shouldInteract = view.IsEnabled;

			if (IsInteractive != shouldInteract)
			{
				nativeView.UserInteractionEnabled = shouldInteract;
				IsInteractive = shouldInteract;
			}

			var boundsChanged = LastBounds != view.Frame;

			var thread = !boundsChanged && !Layer.Frame.IsEmpty;

			var anchorX = (float)view.AnchorX;
			var anchorY = (float)view.AnchorY;
			var translationX = (float)view.TranslationX;
			var translationY = (float)view.TranslationY;
			var rotationX = (float)view.RotationX;
			var rotationY = (float)view.RotationY;
			var rotation = (float)view.Rotation;
			var scale = (float)view.Scale;
			var scaleX = (float)view.ScaleX * scale;
			var scaleY = (float)view.ScaleY * scale;
			var width = (float)view.Width;
			var height = (float)view.Height;
			var x = (float)view.Frame.X;
			var y = (float)view.Frame.Y;

			var updateTarget = Interlocked.Increment(ref UpdateCount);

			void Update()
			{
				if (updateTarget != UpdateCount)
					return;

				var visualElement = view;

				var parent = view.Parent;

				var shouldRelayoutSublayers = false;

				if (Layer != null && Layer.Hidden)
				{
					Layer.Hidden = false;
					if (!Layer.Frame.IsEmpty)
						shouldRelayoutSublayers = true;
				}

				if (Layer != null && !Layer.Hidden)
				{
					Layer.Hidden = true;
					shouldRelayoutSublayers = true;
				}

				// Ripe for optimization
				var transform = CATransform3D.Identity;

				bool shouldUpdate = width > 0 && height > 0 && parent != null && boundsChanged;

				// Dont ever attempt to actually change the layout of a Page unless it is a ContentPage
				// iOS is a really big fan of you not actually modifying the View's of the UIViewControllers
				if (shouldUpdate)
				{
					var target = new RectangleF(x, y, width, height);

					// Must reset transform prior to setting frame...
					if (Layer != null && Layer.AnchorPoint != OriginalAnchor)
						Layer.AnchorPoint = OriginalAnchor;

					if (Layer != null)
						Layer.Transform = transform;

					nativeView.Frame = target;

					if (Layer != null && shouldRelayoutSublayers)
						Layer.LayoutSublayers();
				}
				else if (width <= 0 || height <= 0)
				{
					if (Layer != null)
						Layer.Hidden = true;

					return;
				}

				if (Layer != null)
					Layer.AnchorPoint = new PointF(anchorX, anchorY);

				const double epsilon = 0.001;

				// Position is relative to anchor point
				if (Math.Abs(anchorX - .5) > epsilon)
					transform = transform.Translate((anchorX - .5f) * width, 0, 0);
				if (Math.Abs(anchorY - .5) > epsilon)
					transform = transform.Translate(0, (anchorY - .5f) * height, 0);


				if (Math.Abs(translationX) > epsilon || Math.Abs(translationY) > epsilon)
					transform = transform.Translate(translationX, translationY, 0);

				// Not just an optimization, iOS will not "pixel align" a view which has m34 set
				if (Math.Abs(rotationY % 180) > epsilon || Math.Abs(rotationX % 180) > epsilon)
					transform.m34 = 1.0f / -400f;

				if (Math.Abs(rotationX % 360) > epsilon)
					transform = transform.Rotate(rotationX * (float)Math.PI / 180.0f, 1.0f, 0.0f, 0.0f);

				if (Math.Abs(rotationY % 360) > epsilon)
					transform = transform.Rotate(rotationY * (float)Math.PI / 180.0f, 0.0f, 1.0f, 0.0f);

				transform = transform.Rotate(rotation * (float)Math.PI / 180.0f, 0.0f, 0.0f, 1.0f);

				if (Math.Abs(scaleX - 1) > epsilon || Math.Abs(scaleY - 1) > epsilon)
					transform = transform.Scale(scaleX, scaleY, scale);

				if (Foundation.NSThread.IsMain)
				{
					if (Layer != null)
						Layer.Transform = transform;
					return;
				}

				CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					if (Layer != null)
						Layer.Transform = transform;
				});
			}

			Update();

			LastBounds = view.Frame;
		}
	}
}