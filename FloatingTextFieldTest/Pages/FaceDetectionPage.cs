using System;
using Xamarin.Forms;
using Alex.Controls.Forms;

namespace FloatingTextFieldTest.Pages
{
	public class FaceDetectionPage:ContentPage
	{
		public FaceDetectionPage ()
		{
			Content = new FaceDetectionView {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
		}
	}
}

