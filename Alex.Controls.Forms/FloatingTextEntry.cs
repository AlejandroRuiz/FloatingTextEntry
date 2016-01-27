using System;
using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace Alex.Controls.Forms
{
	public delegate bool FloatingTextEntryValidator(string input);

	public class FloatingTextEntry:Entry
	{
		/// <summary>
		/// Gets the default email validator.
		/// </summary>
		/// <value>The email validator.</value>
		public static FloatingTextEntryValidator EmailValidator
		{
			get{
				return (input) => {
					var emailRegex = "[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,6}";
					var isValid = Regex.Match (input, emailRegex).Success;
					return isValid;
				};
			}
		}

		/// <summary>
		/// Gets the default numeric validator.
		/// </summary>
		/// <value>The numeric validator.</value>
		public static FloatingTextEntryValidator NumericValidator
		{
			get{
				return (input) => {
					var numRegex = "[0-9.+-]+";
					var isValid = Regex.Match (input, numRegex).Success;
					return isValid;
				};
			}
		}

		public FloatingTextEntry ()
		{
			this.TextColor = Color.Black;
		}

		/// <summary>
		/// The accent color property.
		/// </summary>
		public static readonly BindableProperty AccentColorProperty =
			BindableProperty.Create<FloatingTextEntry, Color>(
				p => p.AccentColor, Color.Blue);

		/// <summary>
		/// The inactive accent color property.
		/// </summary>
		public static readonly BindableProperty InactiveAccentColorProperty =
			BindableProperty.Create<FloatingTextEntry, Color> (
				p => p.InactiveAccentColor, Color.Gray.MultiplyAlpha (0.54));

		/// <summary>
		/// Gets or sets the color of the accent.
		/// Only works for iOS for Android set "colorAccent" property on your theme 
		/// <item name="colorAccent">@color/accent</item>
		/// Or set Alex.Controls.Android.Renderers.FloatingTextEntryRenderer.FloatingTextEntryThemeResource Custom Theme
		/// </summary>
		/// <value>The color of the accent.</value>
		public Color AccentColor
		{
			get {
				return (Color)GetValue(AccentColorProperty);
			}
			set {
				SetValue(AccentColorProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the color of the inactive accent.
		/// Only works for iOS for Android set "colorControlNormal" and "android:textColorHint" property on your theme
		/// <item name="colorControlNormal">@color/greyLight</item>
		/// <item name="android:textColorHint">@color/primaryDark</item>
		/// Or set Alex.Controls.Android.Renderers.FloatingTextEntryRenderer.FloatingTextEntryThemeResource Custom Theme
		/// </summary>
		/// <value>The color of the inactive accent.</value>
		public Color InactiveAccentColor
		{
			get {
				return (Color)GetValue(InactiveAccentColorProperty);
			}
			set {
				SetValue(InactiveAccentColorProperty, value);
			}
		}

		/// <summary>
		/// The error color property.
		/// </summary>
		public static readonly BindableProperty ErrorColorProperty =
			BindableProperty.Create<FloatingTextEntry, Color>(
				p => p.ErrorColor, Color.Red);

		/// <summary>
		/// Gets or sets the color of the error label.
		/// </summary>
		/// <value>The color of the error.</value>
		public Color ErrorColor
		{
			get {
				return (Color)GetValue(ErrorColorProperty);
			}
			set {
				SetValue(ErrorColorProperty, value);
			}
		}

		/// <summary>
		/// The validator property.
		/// </summary>
		public static readonly BindableProperty ValidatorProperty =
			BindableProperty.Create<FloatingTextEntry, FloatingTextEntryValidator>(
				p => p.Validator, null);

		/// <summary>
		/// Gets or sets the validator.
		/// </summary>
		/// <value>The validator.</value>
		public FloatingTextEntryValidator Validator
		{
			get {
				return (FloatingTextEntryValidator)GetValue(ValidatorProperty);
			}
			set {
				SetValue(ValidatorProperty, value);
			}
		}

		/// <summary>
		/// The error text property.
		/// </summary>
		public static readonly BindableProperty ErrorTextProperty =
			BindableProperty.Create<FloatingTextEntry, string>(
				p => p.ErrorText, "Error");

		/// <summary>
		/// Gets or sets the error text.
		/// </summary>
		/// <value>The error text.</value>
		public string ErrorText
		{
			get {
				return (string)GetValue(ErrorTextProperty);
			}
			set {
				SetValue(ErrorTextProperty, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this text pass the validator.
		/// </summary>
		/// <value><c>true</c> if text pass validator; otherwise, <c>false</c>.</value>
		public bool IsValid
		{
			get {
				if (this.Validator == null)
					return true;
				return (this.Validator (this.Text));
			}
		}
	}
}

