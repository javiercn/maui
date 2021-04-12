using CoreAnimation;
using CoreGraphics;
using NativeView = UIKit.UIView;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler
	{
		CALayer? _layer;
		bool _isInteractive;
		CGPoint? _originalAnchor;

		partial void SettingDefault(NativeView? nativeView)
		{
			_layer = nativeView?.Layer;
			_isInteractive = nativeView?.UserInteractionEnabled ?? false;
			_originalAnchor = _layer?.AnchorPoint;
		}

		public static void MapTranslationX(IViewHandler handler, IView view)
		{
			var viewHandler = handler as ViewHandler;
			((NativeView?)handler.NativeView)?.UpdateTransformation(view, viewHandler?._layer, viewHandler?._isInteractive ?? false, viewHandler?._originalAnchor);
		}

		public static void MapTranslationY(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapScale(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapRotation(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapRotationX(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapRotationY(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapAnchorX(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}

		public static void MapAnchorY(IViewHandler handler, IView view)
		{
			((NativeView?)handler.NativeView)?.UpdateTransformation(view);
		}
	}
}