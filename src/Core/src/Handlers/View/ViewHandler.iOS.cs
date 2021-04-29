using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using NativeView = UIKit.UIView;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler
	{
		CALayer? _layer;
		bool _isInteractive;
		CGPoint? _originalAnchor;
		Rectangle? _lastBounds;

		partial void SettingDefault(NativeView? nativeView)
		{
			_layer = nativeView?.Layer;
			_isInteractive = nativeView?.UserInteractionEnabled ?? false;
			_originalAnchor = _layer?.AnchorPoint;
			_lastBounds = Rectangle.Zero;
		}

		public static void MapTranslationX(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapTranslationY(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapScale(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapRotation(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapRotationX(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapRotationY(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapAnchorX(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapAnchorY(IViewHandler handler, IView view)
		{
			MapTransformation(handler, view);
		}

		public static void MapTransformation(IViewHandler handler, IView view)
		{
			if (handler is not ViewHandler viewHandler)
				return;

			CALayer? layer = viewHandler._layer;
			bool isInteractive = viewHandler._isInteractive;
			CGPoint? originalAnchor = viewHandler._originalAnchor;
			Rectangle? lastBounds = viewHandler._lastBounds;

			((NativeView?)handler.NativeView)?.UpdateTransformation(view, layer, isInteractive, originalAnchor, lastBounds);
		}
	}
}