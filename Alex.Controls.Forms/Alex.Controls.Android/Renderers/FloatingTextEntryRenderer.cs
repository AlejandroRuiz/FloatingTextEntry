using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Support.Design.Widget;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Text;
using Android.Runtime;
using Android.Views.InputMethods;
using Android.Util;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;
using Alex.Controls.Android.Renderers;
using Alex.Controls.Forms;
using Alex.Controls.Shared.Extentions;

[assembly: ExportRenderer (typeof(FloatingTextEntry), typeof(FloatingTextEntryRenderer))]
namespace Alex.Controls.Android.Renderers
{
	[Preserve(AllMembers = true)]
	public class FloatingTextEntryRenderer:ViewRenderer<FloatingTextEntry, TextInputLayout>, ITextWatcher, TextView.IOnEditorActionListener
	{

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public async static void Init()
		{
			var hack = DateTime.Now;
		}

		#region TextView.IOnEditorActionListener
		public bool OnEditorAction (TextView v, ImeAction actionId, KeyEvent e)
		{
			if (actionId == ImeAction.Done || (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter)) {
				this.targetEditor.ClearFocus ();
				InputMethodManager imm = (InputMethodManager)base.Context.GetSystemService(Context.InputMethodService);
				imm.HideSoftInputFromWindow(v.WindowToken, 0);
				base.Element.Call ("SendCompleted", null);
			}
			return true;
		}
		#endregion

		#region ITextWatcher

		public void AfterTextChanged (IEditable s)
		{
			//throw new NotImplementedException ();
		}
		public void BeforeTextChanged (Java.Lang.ICharSequence s, int start, int count, int after)
		{
			//throw new NotImplementedException ();
		}
		public void OnTextChanged (Java.Lang.ICharSequence s, int start, int before, int count)
		{
			if (string.IsNullOrEmpty (base.Element.Text) && s.Length () == 0) {
				return;
			}
			(base.Element as IElementController).SetValueFromRenderer (FloatingTextEntry.TextProperty, s.ToString ());
			SetAccentColor ();
		}

		#endregion

		public FloatingTextEntryRenderer ()
		{
		}

		EditText targetEditor; 

		protected override void OnElementChanged (ElementChangedEventArgs<FloatingTextEntry> e)
		{
			base.OnElementChanged (e);
			if (e.OldElement == null) {
				TextInputLayout ncontrol;
				ncontrol = new TextInputLayout (this.Context);
				targetEditor = new EditText(this.Context);
				ncontrol.AddView(targetEditor);
				SetNativeControl (ncontrol);

				this.targetEditor.ImeOptions = ImeAction.Done;
				this.targetEditor.AddTextChangedListener (this);
				this.targetEditor.SetOnEditorActionListener (this);
			}
			if (e.NewElement != null) {
				SetAccentColor ();
				SetPlaceholder ();
				SetIsPassword ();
				SetTextColor ();
				SetInactiveAccentColor ();
			}
		}


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

		#region Set/Update Values

		void SetAccentColor()
		{
			//TODO: Check if can be done on Android without using theme colors
		}

		void SetInactiveAccentColor()
		{
			//TODO: Check if can be done on Android without using theme colors
		}

		void SetPlaceholder()
		{
			base.Control.Hint = base.Element.Placeholder;
		}

		void SetIsPassword()
		{
			if (base.Element.IsPassword) {
				targetEditor.InputType = InputTypes.ClassText | InputTypes.TextVariationPassword;
			} else {
				targetEditor.InputType = InputTypes.ClassText | InputTypes.DatetimeVariationDate;
			}
		}

		void SetText()
		{
			if (targetEditor.IsFocused)
				return;
			targetEditor.Text = base.Element.Text;
		}

		void SetTextColor()
		{
			targetEditor.SetTextColor (base.Element.TextColor.ToAndroid ());
		}

		#endregion
	}
}

