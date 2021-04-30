using UIKit;

namespace Microsoft.Maui
{
	public interface INativeViewHandler : IViewHandler
	{
		new UIView? NativeView { get; }
		UIViewController? ViewController { get; }
	}
}