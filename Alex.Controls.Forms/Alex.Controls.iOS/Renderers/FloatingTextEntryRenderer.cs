using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using CoreGraphics;
using UIKit;
using Alex.Controls.iOS.Renderers;
using Alex.Controls.iOS.Controls;
using Alex.Controls.Forms;
using Alex.Controls.Shared.Extentions;
using PureLayoutSharp;

[assembly: ExportRenderer (typeof(FloatingTextEntry), typeof(FloatingTextEntryRenderer))]
namespace Alex.Controls.iOS.Renderers
{
	[Preserve(AllMembers = true)]
	public class FloatingTextEntryRenderer:ViewRenderer<FloatingTextEntry, EGFloatingTextEntryContainer>
	{

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public async static void Init()
		{
			var hack = DateTime.Now;
		}

		public FloatingTextEntryRenderer ()
		{
			this.Frame = new CGRect (0, 0, 320, 60);
		}

		#region Handlers

		private void EditingChanged (object sender, EventArgs eventArgs)
		{
			(base.Element as IElementController).SetValueFromRenderer (FloatingTextEntry.TextProperty, base.Control.MainControl.Text);
		}

		private bool ShouldReturn (UITextField view)
		{
			base.Control.ResignFirstResponder ();
			base.Element.Call ("SendCompleted", null);
			return true;
		}

		#endregion

		#region Set/Update Values

		void UpdateKeyboard()
		{
			base.Control.MainControl.ApplyKeyboard(Element.Keyboard);
		}

		void SetAccentColor()
		{
			base.Control.MainControl.AccentColor = base.Element.AccentColor.ToUIColor ();
		}

		void SetInactiveAccentColor()
		{
			base.Control.MainControl.InactiveAccentColor = base.Element.InactiveAccentColor.ToUIColor ();
		}

		void SetPlaceholder()
		{
			base.Control.MainControl.PlaceHolder = base.Element.Placeholder;
		}

		void SetIsPassword()
		{
			base.Control.MainControl.SecureTextEntry = base.Element.IsPassword;
		}

		void SetText()
		{
			base.Control.MainControl.SetText (base.Element.Text);
		}

		void SetTextColor()
		{
			base.Control.MainControl.TextColor = base.Element.TextColor.ToUIColor ();
		}

		void SetErrorColor()
		{
			base.Control.MainControl.ErrorColor = base.Element.ErrorColor.ToUIColor ();
		}

		void SetValidator()
		{
			base.Control.MainControl.Validator = base.Element.Validator;
		}

		void SetErrorText()
		{
			base.Control.MainControl.ErrorMessage = base.Element.ErrorText;
		}

		#endregion


		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == FloatingTextEntry.AccentColorProperty.PropertyName) {
				SetAccentColor ();
			} else if (e.PropertyName == FloatingTextEntry.InactiveAccentColorProperty.PropertyName) {
				SetInactiveAccentColor ();
			} else if (e.PropertyName == Entry.PlaceholderProperty.PropertyName) {
				SetPlaceholder ();
			} else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName) {
				SetIsPassword ();
			} else if (e.PropertyName == Entry.TextProperty.PropertyName) {
				SetText ();
			} else if (e.PropertyName == Entry.TextColorProperty.PropertyName) {
				SetTextColor ();
			} else if (e.PropertyName == FloatingTextEntry.ErrorColorProperty.PropertyName) {
				SetErrorColor ();
			} else if (e.PropertyName == FloatingTextEntry.ValidatorProperty.PropertyName) {
				SetValidator ();
			} else if (e.PropertyName == FloatingTextEntry.ErrorTextProperty.PropertyName) {
				SetErrorText ();
			} else if (e.PropertyName == FloatingTextEntry.WidthProperty.PropertyName) {
				var newFrame = this.Frame;
				newFrame.Width = (nfloat)base.Element.Width;
				this.Frame = base.Control.Frame = newFrame;
			} else if (e.PropertyName == FloatingTextEntry.HeightProperty.PropertyName) {
				var newFrame = this.Frame;
				newFrame.Height = (nfloat)base.Element.Height;
				this.Frame = base.Control.Frame = newFrame;
			}else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
				UpdateKeyboard();
		}

		protected override void OnElementChanged (ElementChangedEventArgs<FloatingTextEntry> e)
		{
			base.OnElementChanged (e);
			if(e.OldElement == null){
				SetNativeControl (new EGFloatingTextEntryContainer (this.Frame));

				base.Control.AutoMatchDimension (ALDimension.Width, ALDimension.Width, this);
				base.Control.AutoMatchDimension (ALDimension.Height, ALDimension.Height, this);

				Control.MainControl.floatingLabel = true;
				MessagingCenter.Subscribe<IVisualElementRenderer> (this, "Xamarin.ResignFirstResponder", HideKeyBoard, null);
				base.Control.MainControl.EditingChanged += this.EditingChanged;
				base.Control.MainControl.ShouldReturn = this.ShouldReturn;
			}

			if (e.NewElement != null) {
				SetAccentColor ();
				SetPlaceholder ();
				SetIsPassword ();
				SetTextColor ();
				SetInactiveAccentColor ();
				SetErrorColor ();
				SetValidator ();
				SetErrorText ();
				UpdateKeyboard();
			}
		}

		void HideKeyBoard(IVisualElementRenderer sender)
		{
			if (base.Control.IsFirstResponder) {
				base.Control.ResignFirstResponder ();
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (base.Control != null) {
					base.Control.MainControl.EditingChanged -= this.EditingChanged;
				}
				MessagingCenter.Unsubscribe<IVisualElementRenderer> (this, "Xamarin.ResignFirstResponder");
			}
			base.Dispose (disposing);
		}
	}
}

