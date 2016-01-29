using System;
using Xamarin.Forms;

namespace Alex.Controls.Forms
{
	public class StatesButton:Button
	{
		public StatesButton ()
		{
			
		}

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
	}
}

