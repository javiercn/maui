using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Primitives;

namespace Microsoft.Maui.UnitTests
{
	class ViewStub : IViewStub
	{
		public bool IsEnabled { get; set; }

		public Color BackgroundColor { get; set; }

		public Rectangle Frame { get; set; }

		public IViewHandler Handler { get; set; }

		public IFrameworkElement Parent { get; set; }

		public Size DesiredSize { get; set; }

		public bool IsMeasureValid { get; set; }

		public bool IsArrangeValid { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public Thickness Margin { get; set; }

		public string AutomationId { get; set; }

		public FlowDirection FlowDirection { get; set; }

		public LayoutAlignment HorizontalLayoutAlignment { get; set; }

		public LayoutAlignment VerticalLayoutAlignment { get; set; }

		public Semantics Semantics { get; set; } = new Semantics();

		public double TranslationX { get; set; }

		public double TranslationY { get; set; }

		public double Scale { get; set; }

		public double ScaleX { get; set; }

		public double ScaleY { get; set; }

		public double Rotation { get; set; }

		public double RotationX { get; set; }

		public double RotationY { get; set; }

		public double AnchorX { get; set; }

		public double AnchorY { get; set; }

		public void Arrange(Rectangle bounds) { }

		public void InvalidateArrange() { }

		public void InvalidateMeasure() { }

		public Size Measure(double widthConstraint, double heightConstraint) =>
			Size.Zero;
		public Size Arrange(Rectangle bounds)
		{
			throw new NotImplementedException();
		}

		public void InvalidateArrange()
		{
			throw new NotImplementedException();
		}

		public void InvalidateMeasure()
		{
			throw new NotImplementedException();
		}
	}
}
