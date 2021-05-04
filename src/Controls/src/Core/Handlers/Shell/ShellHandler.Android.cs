using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Handlers
{
	public partial class ShellHandler : ViewHandler<Shell, ShellFlyoutView>
	{
		ShellView _shellView;
		protected override ShellFlyoutView CreateNativeView()
		{
			_shellView = new ShellView(ContextWithValidation());
			return (ShellFlyoutView)(_shellView as IShellContext).CurrentDrawerLayout;
		}


		//#region IVisualElementRenderer

		//event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
		//{
		//	add { _elementChanged += value; }
		//	remove { _elementChanged -= value; }
		//}

		//event EventHandler<PropertyChangedEventArgs> IVisualElementRenderer.ElementPropertyChanged
		//{
		//	add { _elementPropertyChanged += value; }
		//	remove { _elementPropertyChanged -= value; }
		//}

		//VisualElement IVisualElementRenderer.Element => Element;

		//VisualElementTracker IVisualElementRenderer.Tracker => null;

		//AView IVisualElementRenderer.View => _flyoutRenderer.AndroidView;

		//// Used by Previewer
		//[EditorBrowsable(EditorBrowsableState.Never)]
		//public ViewGroup ViewGroup => _flyoutRenderer.AndroidView as ViewGroup;

		//SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		//{
		//	return new SizeRequest(new Size(100, 100));
		//}

		//void IVisualElementRenderer.SetElement(VisualElement element)
		//{
		//	if (Element != null)
		//		throw new NotSupportedException("Reuse of the Shell Renderer is not supported");
		//	Element = (Shell)element;
		//	Element.SizeChanged += OnElementSizeChanged;
		//	OnElementSet(Element);

		//	Element.PropertyChanged += OnElementPropertyChanged;
		//	_elementChanged?.Invoke(this, new VisualElementChangedEventArgs(null, Element));
		//}

		//void IVisualElementRenderer.SetLabelFor(int? id)
		//{
		//}

		//// Used by Previewer
		//[EditorBrowsable(EditorBrowsableState.Never)]
		//public void UpdateLayout()
		//{
		//	var width = (int)AndroidContext.ToPixels(Element.Width);
		//	var height = (int)AndroidContext.ToPixels(Element.Height);
		//	_flyoutRenderer.AndroidView.Layout(0, 0, width, height);
		//}

		//#endregion IVisualElementRenderer



		//#region IDisposable

		//void IDisposable.Dispose()
		//{
		//	Dispose(true);
		//}

		//protected virtual void Dispose(bool disposing)
		//{
		//	if (_disposed)
		//		return;

		//	_disposed = true;

		//	if (disposing)
		//	{
		//		if (_currentView != null && _currentView.Fragment.IsAlive())
		//		{
		//			FragmentTransaction transaction = FragmentManager.BeginTransactionEx();
		//			transaction.RemoveEx(_currentView.Fragment);
		//			transaction.CommitAllowingStateLossEx();
		//			FragmentManager.ExecutePendingTransactionsEx();
		//		}

		//		Element.PropertyChanged -= OnElementPropertyChanged;
		//		Element.SizeChanged -= OnElementSizeChanged;
		//		((IShellController)Element).RemoveAppearanceObserver(this);

		//		// This cast is necessary because IShellFlyoutView doesn't implement IDisposable
		//		(_flyoutView as IDisposable)?.Dispose();

		//		_currentView.Dispose();
		//		_currentView = null;
		//	}

		//	Element = null;
		//	// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
		//	// TODO: set large fields to null.

		//	_disposed = true;
		//}

		//#endregion IDisposable
	}
}
