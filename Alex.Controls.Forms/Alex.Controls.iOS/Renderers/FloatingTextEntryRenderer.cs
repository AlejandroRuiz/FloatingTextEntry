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

[assembly: ExportRenderer (typeof(FloatingTextEntry), typeof(FloatingTextEntryRenderer))]
namespace Alex.Controls.iOS.Renderers
{
	[Preserve(AllMembers = true)]
	public class FloatingTextEntryRenderer:ViewRenderer<FloatingTextEntry, EGFloatingTextEntry>
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
			this.Frame = new CGRect (0, 20, 320, 40);
		}

		#region Handlers

		private void EditingChanged (object sender, EventArgs eventArgs)
		{
			(base.Element as IElementController).SetValueFromRenderer (FloatingTextEntry.TextProperty, base.Control.Text);
		}

		private bool ShouldReturn (UITextField view)
		{
			base.Control.ResignFirstResponder ();
			base.Element.Call ("SendCompleted", null);
			return true;
		}

		#endregion

		#region Set/Update Values

		void SetAccentColor()
		{
			base.Control.AccentColor = base.Element.AccentColor.ToUIColor ();
		}

		void SetInactiveAccentColor()
		{
			base.Control.InactiveAccentColor = base.Element.InactiveAccentColor.ToUIColor ();
		}

		void SetPlaceholder()
		{
			base.Control.PlaceHolder = base.Element.Placeholder;
		}

		void SetIsPassword()
		{
			base.Control.SecureTextEntry = base.Element.IsPassword;
		}

		void SetText()
		{
			base.Control.SetText (base.Element.Text);
		}

		void SetTextColor()
		{
			base.Control.TextColor = base.Element.TextColor.ToUIColor ();
		}

		#endregion


		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == FloatingTextEntry.AccentColorProperty.PropertyName) {
				SetAccentColor ();
			} else if (e.PropertyName == FloatingTextEntry.InactiveAccentColorProperty.PropertyName) {
				SetInactiveAccentColor();
			} else if (e.PropertyName == Entry.PlaceholderProperty.PropertyName) {
				SetPlaceholder ();
			} else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName) {
				SetIsPassword ();
			} else if (e.PropertyName == Entry.TextProperty.PropertyName) {
				SetText ();
			} else if (e.PropertyName == Entry.TextColorProperty.PropertyName) {
				SetTextColor ();
			}
			base.OnElementPropertyChanged (sender, e);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<FloatingTextEntry> e)
		{
			base.OnElementChanged (e);
			if(e.OldElement == null){
				SetNativeControl (new EGFloatingTextEntry (this.Frame){
					floatingLabel = true
				});
				MessagingCenter.Subscribe<IVisualElementRenderer> (this, "Xamarin.ResignFirstResponder", HideKeyBoard, null);
				base.Control.EditingChanged += this.EditingChanged;
				base.Control.ShouldReturn = this.ShouldReturn;
			}

			if (e.NewElement != null) {
				SetAccentColor ();
				SetPlaceholder ();
				SetIsPassword ();
				SetTextColor ();
				SetInactiveAccentColor ();
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
					base.Control.EditingChanged -= this.EditingChanged;
				}
				MessagingCenter.Unsubscribe<IVisualElementRenderer> (this, "Xamarin.ResignFirstResponder");
			}
			base.Dispose (disposing);
		}
	}
}

