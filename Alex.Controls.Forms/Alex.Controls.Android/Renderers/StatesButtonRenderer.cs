using System;
using Android.Runtime;
using Xamarin.Forms;
using Alex.Controls.Forms;
using Alex.Controls.Android.Renderers;
using Xamarin.Forms.Platform.Android;
using Alex.Controls.Shared.Extentions;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.Util;
using System.Linq;
using Android.Graphics;

[assembly: ExportRenderer (typeof(StatesButton), typeof(StatesButtonRenderer))]
namespace Alex.Controls.Android.Renderers
{
	[Preserve(AllMembers = true)]
	public class StatesButtonRenderer:ButtonRenderer
	{
		public async static void Init()
		{
			var hack = DateTime.Now;
		}

		public StatesButtonRenderer ()
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (base.Control != null) {
					base.Control.Background.Dispose ();
				}
			}
			base.Dispose (disposing);
		}

		StatesButton BaseElement
		{
			get{
				return Element as StatesButton;
			}
		}

		protected async override void OnElementChanged (ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				await BuildBackground ();

				var statesButton = e.NewElement as StatesButton;
				if (statesButton.BackgroundColor != Xamarin.Forms.Color.Default &&
				    statesButton.PressedBackgroundColor != Xamarin.Forms.Color.Default &&
				    statesButton.DisableBackgroundColor != Xamarin.Forms.Color.Default) {
					BuildColorBackground();
				}
			}
		}

		void BuildColorBackground()
		{
			using (var statesBackground = new StateListDrawable())
			{
				using (var imgNormal = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888))
				{
					using (var imgDisable = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888))
					{
						using (var imgPressed = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888))
						{
							imgNormal.EraseColor(BaseElement.BackgroundColor.ToAndroid().ToArgb());
							imgDisable.EraseColor(BaseElement.DisableBackgroundColor.ToAndroid().ToArgb());
							imgPressed.EraseColor(BaseElement.PressedBackgroundColor.ToAndroid().ToArgb());


							statesBackground.AddState(
								new int[]{
									-global::Android.Resource.Attribute.StatePressed,
									global::Android.Resource.Attribute.StateEnabled
								},
								new BitmapDrawable(imgNormal)
							);

							statesBackground.AddState (
								new int[] {
									global::Android.Resource.Attribute.StatePressed,
									global::Android.Resource.Attribute.StateEnabled
								},
								new BitmapDrawable(imgPressed)
							);

							statesBackground.AddState(
								new int[] {
									-global::Android.Resource.Attribute.StateEnabled
								},
								new BitmapDrawable(imgDisable)
							);
						}
					}
				}
			}
		}

		async Task BuildBackground()
		{
			using (var statesBackground = new StateListDrawable ()) {
				if (BaseElement.NormalImage != null) {
					var normalHandler = BaseElement.NormalImage.GetHandler ();
					using (var imgNormal = await normalHandler.LoadImageAsync (BaseElement.NormalImage, base.Context)) {
						statesBackground.AddState (
							new int[]{
								-global::Android.Resource.Attribute.StatePressed,
								global::Android.Resource.Attribute.StateEnabled
							},
							new BitmapDrawable(imgNormal)
						);
						if (BaseElement.PressedImage == null) {
							statesBackground.AddState (
								new int[] {
									global::Android.Resource.Attribute.StatePressed,
									global::Android.Resource.Attribute.StateEnabled
								},
								new BitmapDrawable(imgNormal)
							);
						}
						if (BaseElement.DisableImage == null) {
							statesBackground.AddState (
								new int[] {
									-global::Android.Resource.Attribute.StateEnabled
								},
								new BitmapDrawable(imgNormal)
							);
						}
					}
				}

				if (BaseElement.PressedImage != null) {
					var pressedHandler = BaseElement.PressedImage.GetHandler ();
					using (var imgPressed = await pressedHandler.LoadImageAsync (BaseElement.PressedImage, base.Context)) {
						statesBackground.AddState (
							new int[] {
								global::Android.Resource.Attribute.StatePressed,
								global::Android.Resource.Attribute.StateEnabled
							},
							new BitmapDrawable(imgPressed)
						);
					}
				}
				if (BaseElement.DisableImage != null) {
					var disableHandler = BaseElement.DisableImage.GetHandler ();
					using (var imgDisable = await disableHandler.LoadImageAsync (BaseElement.DisableImage, base.Context)) {
						statesBackground.AddState (
							new int[] {
								-global::Android.Resource.Attribute.StateEnabled
							},
							new BitmapDrawable(imgDisable)
						);
					}
				}
				if (Control != null)
					Control.Background = statesBackground;
			}
		}

		protected async override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName == StatesButton.NormalImageProperty.PropertyName)
			{
				await BuildBackground();
			}
			else if (e.PropertyName == StatesButton.DisableImageProperty.PropertyName)
			{
				await BuildBackground();
			}
			else if (e.PropertyName == StatesButton.PressedImageProperty.PropertyName)
			{
				await BuildBackground ();
			}
			else if (e.PropertyName == StatesButton.BackgroundColorProperty.PropertyName)
			{
				BuildColorBackground();
			}
			else if (e.PropertyName == StatesButton.DisableBackgroundColorProperty.PropertyName)
			{
				BuildColorBackground();
			}
			else if (e.PropertyName == StatesButton.PressedBackgroundColorProperty.PropertyName)
			{
				BuildColorBackground();
			}
		}
	}
}

