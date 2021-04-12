using System.Threading.Tasks;
using AndroidX.AppCompat.Widget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Xunit;
using AColor = Android.Graphics.Color;

namespace Microsoft.Maui.DeviceTests
{
	public partial class ButtonHandlerTests
	{
		[Fact(DisplayName = "CharacterSpacing Initializes Correctly")]
		public async Task CharacterSpacingInitializesCorrectly()
		{
			var xplatCharacterSpacing = 4;

			var button = new ButtonStub()
			{
				CharacterSpacing = xplatCharacterSpacing,
				Text = "Test"
			};

			float expectedValue = button.CharacterSpacing.ToEm();

			var values = await GetValueAsync(button, (handler) =>
			{
				return new
				{
					ViewValue = button.CharacterSpacing,
					NativeViewValue = GetNativeCharacterSpacing(handler)
				};
			});

			Assert.Equal(xplatCharacterSpacing, values.ViewValue);
			Assert.Equal(expectedValue, values.NativeViewValue, EmCoefficientPrecision);
		}

		[Theory(DisplayName = "Font Family Initializes Correctly")]
		[InlineData(null)]
		[InlineData("monospace")]
		[InlineData("Dokdo")]
		public async Task FontFamilyInitializesCorrectly(string family)
		{
			var button = new ButtonStub
			{
				Text = "Test",
				Font = Font.OfSize(family, 10)
			};

			var handler = await CreateHandlerAsync(button);
			var nativeButton = GetNativeButton(handler);

			var fontManager = handler.Services.GetRequiredService<IFontManager>();

			var nativeFont = fontManager.GetTypeface(Font.OfSize(family, 0.0));

			Assert.Equal(nativeFont, nativeButton.Typeface);

			if (string.IsNullOrEmpty(family))
				Assert.Equal(fontManager.DefaultTypeface, nativeButton.Typeface);
			else
				Assert.NotEqual(fontManager.DefaultTypeface, nativeButton.Typeface);
		}

		[Fact(DisplayName = "Button Padding Initializing")]
		public async Task PaddingInitializesCorrectly()
		{
			var button = new ButtonStub()
			{
				Text = "Test",
				Padding = new Thickness(5, 10, 15, 20)
			};

			var handler = await CreateHandlerAsync(button);
			var appCompatButton = (AppCompatButton)handler.NativeView;
			var (left, top, right, bottom) = (appCompatButton.PaddingLeft, appCompatButton.PaddingTop, appCompatButton.PaddingRight, appCompatButton.PaddingBottom);

			var context = handler.NativeView.Context;

			var expectedLeft = context.ToPixels(5);
			var expectedTop = context.ToPixels(10);
			var expectedRight = context.ToPixels(15);
			var expectedBottom = context.ToPixels(20);

			Assert.Equal(expectedLeft, left);
			Assert.Equal(expectedTop, top);
			Assert.Equal(expectedRight, right);
			Assert.Equal(expectedBottom, bottom);
		}

		[Theory(DisplayName = "TranslationX Initialize Correctly")]
		[InlineData(10)]
		[InlineData(50)]
		[InlineData(100)]
		public async Task TranslationXInitializeCorrectly(double translationX)
		{
			var button = new ButtonStub()
			{
				Text = "TranslationX",
				TranslationX = translationX
			};

			await ValidatePropertyInitValue(button, () => button.TranslationX, GetNativeTranslationX, button.TranslationX);
		}

		[Theory(DisplayName = "TranslationY Initialize Correctly")]
		[InlineData(10)]
		[InlineData(50)]
		[InlineData(100)]
		public async Task TranslationYInitializeCorrectly(double translationY)
		{
			var button = new ButtonStub()
			{
				Text = "TranslationY",
				TranslationY = translationY
			};

			await ValidatePropertyInitValue(button, () => button.TranslationY, GetNativeTranslationY, button.TranslationY);
		}

		[Theory(DisplayName = "ScaleX Initialize Correctly")]
		[InlineData(1.1)]
		[InlineData(1.5)]
		[InlineData(2.0)]
		public async Task ScaleXInitializeCorrectly(double scaleX)
		{
			var button = new ButtonStub()
			{
				Text = "ScaleX",
				ScaleX = scaleX
			};

			await ValidatePropertyInitValue(button, () => button.ScaleX, GetNativeScaleX, button.ScaleX);
		}

		[Theory(DisplayName = "ScaleY Initialize Correctly")]
		[InlineData(1.1)]
		[InlineData(1.5)]
		[InlineData(2.0)]
		public async Task ScaleYInitializeCorrectly(double scaleY)
		{
			var button = new ButtonStub()
			{
				Text = "ScaleY",
				ScaleY = scaleY
			};

			await ValidatePropertyInitValue(button, () => button.ScaleY, GetNativeScaleY, button.ScaleY);
		}

		[Theory(DisplayName = "Rotation Initialize Correctly")]
		[InlineData(0)]
		[InlineData(90)]
		[InlineData(180)]
		[InlineData(270)]
		[InlineData(360)]
		public async Task RotationInitializeCorrectly(double rotation)
		{
			var button = new ButtonStub()
			{
				Text = "Rotation",
				Rotation = rotation
			};

			await ValidatePropertyInitValue(button, () => button.Rotation, GetNativeRotation, button.Rotation);
		}

		[Theory(DisplayName = "RotationX Initialize Correctly")]
		[InlineData(0)]
		[InlineData(90)]
		[InlineData(180)]
		[InlineData(270)]
		[InlineData(360)]
		public async Task RotationXInitializeCorrectly(double rotationX)
		{
			var button = new ButtonStub()
			{
				Text = "RotationX",
				RotationX = rotationX
			};

			await ValidatePropertyInitValue(button, () => button.RotationX, GetNativeRotationX, button.RotationX);
		}

		[Theory(DisplayName = "RotationY Initialize Correctly")]
		[InlineData(0)]
		[InlineData(90)]
		[InlineData(180)]
		[InlineData(270)]
		[InlineData(360)]
		public async Task RotationYInitializeCorrectly(double rotationY)
		{
			var button = new ButtonStub()
			{
				Text = "RotationY",
				RotationY = rotationY
			};

			await ValidatePropertyInitValue(button, () => button.RotationY, GetNativeRotationY, button.RotationY);
		}

		AppCompatButton GetNativeButton(ButtonHandler buttonHandler) =>
			buttonHandler.NativeView;

		string GetNativeText(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).Text;

		Color GetNativeTextColor(ButtonHandler buttonHandler)
		{
			int currentTextColorInt = GetNativeButton(buttonHandler).CurrentTextColor;
			AColor currentTextColor = new AColor(currentTextColorInt);

			return currentTextColor.ToColor();
		}

		Thickness GetNativePadding(ButtonHandler buttonHandler)
		{
			var appCompatButton = GetNativeButton(buttonHandler);
			return ToThicknees(appCompatButton);

			static Thickness ToThicknees(AppCompatButton appCompatButton)
			{
				return new Thickness(appCompatButton.PaddingLeft,
					appCompatButton.PaddingTop, appCompatButton.PaddingRight, appCompatButton.PaddingBottom);
			}
		}

		Task PerformClick(IButton button)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeButton(CreateHandler(button)).PerformClick();
			});
		}

		double GetNativeUnscaledFontSize(ButtonHandler buttonHandler)
		{
			var textView = GetNativeButton(buttonHandler);
			return textView.TextSize / textView.Resources.DisplayMetrics.Density;
		}

		bool GetNativeIsBold(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).Typeface.IsBold;

		bool GetNativeIsItalic(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).Typeface.IsItalic;

		double GetNativeCharacterSpacing(ButtonHandler buttonHandler)
		{
			var button = GetNativeButton(buttonHandler);

			if (button != null)
			{
				return button.LetterSpacing;
			}

			return -1;
		}

		double GetNativeTranslationX(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).TranslationX;
		}

		double GetNativeTranslationY(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).TranslationY;
		}

		double GetNativeScaleX(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).ScaleX;
		}

		double GetNativeScaleY(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).ScaleY;
		}

		double GetNativeRotation(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).Rotation;
		}

		double GetNativeRotationX(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).RotationX;
		}

		double GetNativeRotationY(ButtonHandler buttonHandler)
		{
			return GetNativeButton(buttonHandler).RotationY;
		}
	}
}