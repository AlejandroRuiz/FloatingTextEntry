using System;
using Xamarin.Forms;
using Alex.Controls.Forms;

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

		public App ()
		{
			EmailEntry = new FloatingTextEntry ();
			EmailEntry.Placeholder = "Email";
			EmailEntry.AccentColor = Color.FromHex("#FFC107");
			EmailEntry.InactiveAccentColor = Color.FromHex ("#1976D2");
			EmailEntry.TextColor = Color.Purple;
			EmailEntry.Completed += MaterialEntry_Completed;

			PassEntry = new FloatingTextEntry ();
			PassEntry.Placeholder = "Password";
			PassEntry.AccentColor = Color.FromHex("#FFC107");
			PassEntry.InactiveAccentColor = Color.FromHex ("#1976D2");
			PassEntry.TextColor = Color.Purple;
			PassEntry.Completed += MaterialEntry_Completed;
			PassEntry.IsPassword = true;

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(20,0),
					Children = {
						EmailEntry,
						PassEntry
					}
				}
			};
		}

		void MaterialEntry_Completed (object sender, EventArgs e)
		{
			Console.WriteLine (EmailEntry.Text);
		}

		protected override void OnStart ()
		{
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

