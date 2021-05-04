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
	}
}
