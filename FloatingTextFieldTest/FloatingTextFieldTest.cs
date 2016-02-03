using System;
using Xamarin.Forms;
using Alex.Controls.Forms;
using System.Threading.Tasks;

namespace FloatingTextFieldTest
{
	public class App : Application
	{
		FloatingTextEntry EmailEntry {
			get;
			set;
		}

		FloatingTextEntry PassEntry {
			get;
			set;
		}

		StatesButton Button {
			get;
			set;
		}

		public App ()
		{
			EmailEntry = new FloatingTextEntry ();
			EmailEntry.Placeholder = "Email";
			EmailEntry.AccentColor = Color.FromHex("#FFC107");
			EmailEntry.InactiveAccentColor = Color.FromHex ("#1976D2");
			EmailEntry.TextColor = Color.Purple;
			EmailEntry.ErrorColor = Color.Red;
			EmailEntry.ErrorText = "Bad Email";
			EmailEntry.Validator = FloatingTextEntry.EmailValidator;

			PassEntry = new FloatingTextEntry ();
			PassEntry.Placeholder = "Password";
			PassEntry.AccentColor = Color.FromHex("#FFC107");
			PassEntry.InactiveAccentColor = Color.FromHex ("#1976D2");
			PassEntry.TextColor = Color.Purple;
			PassEntry.IsPassword = true;
			PassEntry.ErrorColor = Color.Red;
			PassEntry.ErrorText = "Bad Password";
			PassEntry.Validator = (string input) => {
				return !string.IsNullOrWhiteSpace (input);
			};

			Button = new StatesButton () {
				Text = "Hello",
				NormalImage = "boton",
				DisableImage = "boton_disabled",
				PressedImage = "boton_press",
				TextColor = Color.White
			};

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(20,0),
					Children = {
						/*EmailEntry,
						PassEntry,*/
						Button
					}
				}
			};
		}

		protected async override void OnStart ()
		{
			await Task.Delay (5000);
			Button.IsEnabled = false;
			await Task.Delay (2000);
			Button.IsEnabled = true;
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

