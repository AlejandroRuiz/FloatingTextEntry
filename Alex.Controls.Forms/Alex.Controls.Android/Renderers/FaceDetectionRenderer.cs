using System;
using Xamarin.Forms.Platform.Android;
using Alex.Controls.Forms;
using Android.Widget;
using Xamarin.Forms;
using Alex.Controls.Android.Renderers;
using Android.Hardware;
using Android.Views;
using Android.App;

[assembly: ExportRenderer (typeof(FaceDetectionView), typeof(FaceDetectionRenderer))]
namespace Alex.Controls.Android.Renderers
{
	public class FaceDetectionRenderer:ViewRenderer<FaceDetectionView, global::Android.Widget.RelativeLayout>, ISurfaceHolderCallback, global::Android.Hardware.Camera.IFaceDetectionListener
	{
		#region IFaceDetectionListener
		public void OnFaceDetection (Camera.Face[] faces, Camera camera)
		{
			for(int i = 0 ; i < faces.Length ; i++){
				int posX = midScreenWidth - faces[0].Rect.CenterX();
				int posY = midScreenHeight + faces[0].Rect.CenterY();
				myCustomView.PosX = posX;
				myCustomView.PosY = posY;
			}
			myCustomView.Invalidate ();
		}
		#endregion

		#region ISurfaceHolderCallback
		public void SurfaceChanged (ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
		{
			mCamera.SetFaceDetectionListener (this);
			mCamera.StartPreview();
			mCamera.StartFaceDetection();
		}

		public void SurfaceCreated (ISurfaceHolder holder)
		{
			try {
				//Try to open front camera if exist...
				Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
				int cameraId = 0;
				int camerasCount = Camera.NumberOfCameras;
				for ( int camIndex = 0; camIndex < camerasCount; camIndex++ ) {
					Camera.GetCameraInfo(camIndex, cameraInfo );
					if (cameraInfo.Facing == CameraFacing.Front  ) {
						cameraId = camIndex;
						break;
					}
				}
				mCamera = Camera.Open(cameraId);
				mCamera.SetPreviewDisplay(holder);
			} catch (Exception exception) {
				Console.WriteLine ("TrackingFlow === Surface Created Exception === " + exception.Message);
				if(mCamera == null)return;
				mCamera.Release();
				mCamera = null;  
			}
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			if(mCamera == null)
				return;
			mCamera.StopPreview();
			mCamera.StopFaceDetection();
			mCamera.Release();
			mCamera = null;
		}
		#endregion

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public async static void Init()
		{
			var hack = DateTime.Now;
		}

		#region Properties

		private int midScreenWidth;
		private int midScreenHeight;
		private Camera mCamera;
		private SurfaceView cameraSurface;
		private ISurfaceHolder cameraSurfaceHolder;
		private Alex.Controls.Android.Views.CustomView myCustomView;

		#endregion

		void InitLayout()
		{
			myCustomView = new Alex.Controls.Android.Views.CustomView (
				base.Context
			);
			cameraSurface = new SurfaceView (
				base.Context
			);
			cameraSurfaceHolder = cameraSurface.Holder;
			cameraSurfaceHolder.AddCallback(this);

			cameraSurface.LayoutParameters = myCustomView.LayoutParameters = new global::Android.Widget.RelativeLayout.LayoutParams (global::Android.Widget.RelativeLayout.LayoutParams.MatchParent, global::Android.Widget.RelativeLayout.LayoutParams.MatchParent);

			//Screen sizes...
			Display display = (base.Context as Activity).WindowManager.DefaultDisplay;
			midScreenHeight = display.Height / 2;

			var mainControl = new global::Android.Widget.RelativeLayout (base.Context);
			mainControl.AddView (myCustomView);
			mainControl.AddView (cameraSurface);
			SetNativeControl (mainControl);
		}

		public FaceDetectionRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<FaceDetectionView> e)
		{
			base.OnElementChanged (e);
			if (e.OldElement == null) {
				InitLayout ();
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
		}
	}
}