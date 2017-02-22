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
				Validate (this.targetEditor.Text);
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
			Validate (s.ToString ());
				
			(base.Element as IElementController).SetValueFromRenderer (FloatingTextEntry.TextProperty, s.ToString ());
		}

		void Validate(string text)
		{
			if (Validator != null) {
				var isValid = Validator (text);;
				if (isValid) {
					this.Control.Error = null;
				} else {
					if (this.Control.Error != this.ErrorMessage) {
						this.Control.Error = this.ErrorMessage;
					}
				}
			}
		}

		#endregion

		public FloatingTextEntryRenderer ()
		{
		}

		EditText targetEditor; 

		FloatingTextEntryValidator Validator {get;set;}

		string ErrorMessage = "Error";

		protected override void OnElementChanged (ElementChangedEventArgs<FloatingTextEntry> e)
		{
			base.OnElementChanged (e);
			if (e.OldElement == null) {
				TextInputLayout ncontrol;
				ncontrol = new TextInputLayout (this.Context);
				ncontrol.ErrorEnabled = true;
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
				SetErrorColor ();
				SetValidator ();
				SetErrorText ();
				UpdateKeyboard();
			}
		}


		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == FloatingTextEntry.AccentColorProperty.PropertyName)
			{
				SetAccentColor();
			}
			else if (e.PropertyName == FloatingTextEntry.InactiveAccentColorProperty.PropertyName)
			{
				SetInactiveAccentColor();
			}
			else if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
			{
				SetPlaceholder();
			}
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
			{
				SetIsPassword();
			}
			else if (e.PropertyName == Entry.TextProperty.PropertyName)
			{
				SetText();
			}
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
			{
				SetTextColor();
			}
			else if (e.PropertyName == FloatingTextEntry.ErrorColorProperty.PropertyName)
			{
				SetErrorColor();
			}
			else if (e.PropertyName == FloatingTextEntry.ValidatorProperty.PropertyName)
			{
				SetValidator();
			}
			else if (e.PropertyName == FloatingTextEntry.ErrorTextProperty.PropertyName)
			{
				SetErrorText();
			}
			else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
			{
				UpdateKeyboard();
			}

			base.OnElementPropertyChanged (sender, e);
		}

		#region Set/Update Values

		void UpdateKeyboard()
		{
			Entry model = Element;
			targetEditor.InputType = model.Keyboard.ToInputType();
			if (model.IsPassword && ((targetEditor.InputType & InputTypes.ClassText) == InputTypes.ClassText))
				targetEditor.InputType = targetEditor.InputType | InputTypes.TextVariationPassword;
			if (model.IsPassword && ((targetEditor.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber))
				targetEditor.InputType = targetEditor.InputType | InputTypes.NumberVariationPassword;
		}

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

		void SetErrorColor()
		{
			//TODO: Check if can be done on Android without using theme colors
			//base.Control.ErrorColor = base.Element.ErrorColor.ToUIColor ();
		}

		void SetValidator()
		{
			this.Validator = base.Element.Validator;
		}

		void SetErrorText()
		{
			this.ErrorMessage = base.Element.ErrorText;
		}

		#endregion
	}

	public static class KeyboardExtensions
	{
		public static InputTypes ToInputType(this Keyboard self)
		{
			var result = new InputTypes();

			// ClassText:																						!autocaps, spellcheck, suggestions 
			// TextFlagNoSuggestions:																			!autocaps, !spellcheck, !suggestions
			// InputTypes.ClassText | InputTypes.TextFlagCapSentences											 autocaps,	spellcheck,  suggestions
			// InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagNoSuggestions;		 autocaps, !spellcheck, !suggestions

			if (self == Keyboard.Default)
				result = InputTypes.ClassText | InputTypes.TextVariationNormal;
			else if (self == Keyboard.Chat)
				result = InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagNoSuggestions;
			else if (self == Keyboard.Email)
				result = InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
			else if (self == Keyboard.Numeric)
				result = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal | InputTypes.NumberFlagSigned;
			else if (self == Keyboard.Telephone)
				result = InputTypes.ClassPhone;
			else if (self == Keyboard.Text)
				result = InputTypes.ClassText | InputTypes.TextFlagCapSentences;
			else if (self == Keyboard.Url)
				result = InputTypes.ClassText | InputTypes.TextVariationUri;
			else
			{
				// Should never happens
				result = InputTypes.TextVariationNormal;
			}
			return result;
		}
	}
}

