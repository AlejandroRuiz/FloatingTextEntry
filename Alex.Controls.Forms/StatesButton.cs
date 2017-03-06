using System;
using Xamarin.Forms;

namespace Alex.Controls.Forms
{
	public class StatesButton:Button
	{
		public StatesButton ()
		{
			
		}

		#region Color Impl

		public static readonly BindableProperty DisableBackgroundColorProperty =
			BindableProperty.Create<StatesButton, Color> (
				p => p.DisableBackgroundColor, Color.Default);

		public Color DisableBackgroundColor
		{
			get {
				return (Color)GetValue(DisableBackgroundColorProperty);
			}
			set {
				SetValue(DisableBackgroundColorProperty, value);
			}
		}

		public static readonly BindableProperty PressedBackgroundColorProperty =
			BindableProperty.Create<StatesButton, Color> (
				p => p.PressedBackgroundColor, Color.Default);

		public Color PressedBackgroundColor
		{
			get {
				return (Color)GetValue(PressedBackgroundColorProperty);
			}
			set {
				SetValue(PressedBackgroundColorProperty, value);
			}
		}


		#endregion

		#region Image Impl
		public static readonly BindableProperty NormalImageProperty =
			BindableProperty.Create<StatesButton, ImageSource> (
				p => p.NormalImage, null);
		
		public ImageSource NormalImage
		{
			get {
				return (ImageSource)GetValue(NormalImageProperty);
			}
			set {
				SetValue(NormalImageProperty, value);
			}
		}

		public static readonly BindableProperty DisableImageProperty =
			BindableProperty.Create<StatesButton, ImageSource> (
				p => p.DisableImage, null);

		public ImageSource DisableImage
		{
			get {
				return (ImageSource)GetValue(DisableImageProperty);
			}
			set {
				SetValue(DisableImageProperty, value);
			}
		}

		public static readonly BindableProperty PressedImageProperty =
			BindableProperty.Create<StatesButton, ImageSource> (
				p => p.PressedImage, null);

		public ImageSource PressedImage
		{
			get {
				return (ImageSource)GetValue(PressedImageProperty);
			}
			set {
				SetValue(PressedImageProperty, value);
			}
		}
		#endregion
	}
}

