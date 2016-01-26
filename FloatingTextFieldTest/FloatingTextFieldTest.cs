using System;
using Xamarin.Forms;
using Alex.Controls.Forms;

namespace FloatingTextFieldTest
{
	public class App : Application
	{
		FloatingTextEntry MaterialEntry {
			get;
			set;
		}

		public App ()
		{
			MaterialEntry = new FloatingTextEntry ();
			MaterialEntry.Placeholder = "Email";
			MaterialEntry.AccentColor = Color.FromHex("#FFC107");
			MaterialEntry.InactiveAccentColor = Color.FromHex ("#1976D2");
			MaterialEntry.TextColor = Color.Purple;
			MaterialEntry.Completed += MaterialEntry_Completed;

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(20,0),
					Children = {
						MaterialEntry
					}
				}
			};
		}

		void MaterialEntry_Completed (object sender, EventArgs e)
		{
			Console.WriteLine (MaterialEntry.Text);
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

