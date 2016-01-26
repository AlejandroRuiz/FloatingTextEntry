using System;
using Xamarin.Forms;

namespace Alex.Controls.Forms
{
	public class FloatingTextEntry:Entry
	{
		public FloatingTextEntry ()
		{
			if (Device.OS == TargetPlatform.iOS)
				this.HeightRequest = 35;
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
	}
}

