using System;
using Xamarin.Forms.Platform.iOS;
using Alex.Controls.Forms;
using UIKit;
using Xamarin.Forms;
using Alex.Controls.iOS.Renderers;
using AVFoundation;
using CoreFoundation;
using Foundation;
using CoreImage;
using CoreGraphics;
using CoreVideo;
using CoreAnimation;

[assembly: ExportRenderer (typeof(FaceDetectionView), typeof(FaceDetectionRenderer))]
namespace Alex.Controls.iOS.Renderers
{
	enum ExifKind{
		TOP_0COL_LEFT			= 1, //   1  =  0th row is at the top, and 0th column is on the left (THE DEFAULT).
		TOP_0COL_RIGHT			= 2, //   2  =  0th row is at the top, and 0th column is on the right.  
		BOTTOM_0COL_RIGHT      = 3, //   3  =  0th row is at the bottom, and 0th column is on the right.  
		BOTTOM_0COL_LEFT       = 4, //   4  =  0th row is at the bottom, and 0th column is on the left.  
		LEFT_0COL_TOP          = 5, //   5  =  0th row is on the left, and 0th column is the top.  
		RIGHT_0COL_TOP         = 6, //   6  =  0th row is on the right, and 0th column is the top.  
		RIGHT_0COL_BOTTOM      = 7, //   7  =  0th row is on the right, and 0th column is the bottom.  
		LEFT_0COL_BOTTOM       = 8  //   8  =  0th row is on the left, and 0th column is the bottom.  
	}

	public class FaceDetectionRenderer:ViewRenderer<FaceDetectionView, UIView>, IUIGestureRecognizerDelegate
	{
		class CustomAVCaptureVideoDataOutputSampleBufferDelegate:AVCaptureVideoDataOutputSampleBufferDelegate
		{
			FaceDetectionRenderer Handler;
			public CustomAVCaptureVideoDataOutputSampleBufferDelegate(FaceDetectionRenderer handler)
			{
				Handler=handler;
			}
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CoreMedia.CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
			}
		}
		static float DegreesToRadians(float degrees) {
			return (float)(degrees * Math.PI / 180);
		}

		bool isUsingFrontFacingCamera;
		AVCaptureVideoDataOutput videoDataOutput;
		DispatchQueue videoDataOutputQueue;
		AVCaptureVideoPreviewLayer previewLayer;

		UIImage borderImage;
		CIDetector faceDetector;


		UIView previewView;

		void setupAVCapture()
		{
			NSError error = null;

			AVCaptureSession session = new AVCaptureSession ();
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone){
				session.SessionPreset = AVCaptureSession.Preset640x480;
			} else {
				session.SessionPreset = AVCaptureSession.PresetPhoto;
			}

			// Select a video device, make an input
			AVCaptureDevice device = null;

			AVCaptureDevicePosition desiredPosition = AVCaptureDevicePosition.Front;

			// find the front facing camera
			foreach (AVCaptureDevice d in AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video)) {
				if (d.Position == desiredPosition) {
					device = d;
					this.isUsingFrontFacingCamera = true;
					break;
				}
			}
			// fall back to the default camera.
			if( device == null)
			{
				this.isUsingFrontFacingCamera = false;
				device = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			}

			// get the input device
			AVCaptureDeviceInput deviceInput = AVCaptureDeviceInput.FromDevice(device, out error);

			if( error == null) {

				// add the input to the session
				if ( session.CanAddInput(deviceInput) ){
					session.AddInput (deviceInput);
				}


				// Make a video data output
				this.videoDataOutput = new AVCaptureVideoDataOutput();

				// we want BGRA, both CoreGraphics and OpenGL work well with 'BGRA'
				NSDictionary rgbOutputSettings = new NSDictionary (
					                                 CVPixelBuffer.PixelFormatTypeKey,
					                                 CVPixelFormatType.CV32BGRA
				                                 );

				this.videoDataOutput.WeakVideoSettings = rgbOutputSettings;
				this.videoDataOutput.AlwaysDiscardsLateVideoFrames = true; // discard if the data output queue is blocked

				// create a serial dispatch queue used for the sample buffer delegate
				// a serial dispatch queue must be used to guarantee that video frames will be delivered in order
				// see the header doc for setSampleBufferDelegate:queue: for more information
				this.videoDataOutputQueue = new DispatchQueue("VideoDataOutputQueue");
				this.videoDataOutput.SetSampleBufferDelegate (new CustomAVCaptureVideoDataOutputSampleBufferDelegate(this), this.videoDataOutputQueue);

				if (session.CanAddOutput(this.videoDataOutput)){
					session.AddOutput (this.videoDataOutput);
				}

				// get the output for doing face detection.
				this.videoDataOutput.ConnectionFromMediaType(AVMediaType.Video).Enabled = true; 

				this.previewLayer = new AVCaptureVideoPreviewLayer (session);
				this.previewLayer.BackgroundColor = UIColor.Black.CGColor;
				this.previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;

				CALayer rootLayer = this.previewView.Layer;
				rootLayer.MasksToBounds = true;
				this.previewLayer.Frame = rootLayer.Bounds;
				rootLayer.AddSublayer (this.previewLayer);
				session.StartRunning ();

			}
			session = null;
			if (error != null) {
				UIAlertView alertView = new UIAlertView (
					                        "Failed with error " + (int)error.Code,
					                        error.LocalizedDescription,
					                        null,
					                        "Dismiss",
					                        null);
				alertView.Show ();
				this.teardownAVCapture ();
			}
		}

		void teardownAVCapture()
		{
			this.videoDataOutput = null;
			if (this.videoDataOutputQueue != null) {
				//this.videoDataOutputQueue;
				//dispatch_release(self.videoDataOutputQueue);
			}
			this.previewLayer.RemoveFromSuperLayer ();
			this.previewLayer = null;
		}


		// find where the video box is positioned within the preview layer based on the video size and gravity
		CGRect videoPreviewBoxForGravity(AVLayerVideoGravity gravity, CGSize frameSize, CGSize apertureSize)
		{
			var apertureRatio = apertureSize.Height / apertureSize.Width;
			var viewRatio = frameSize.Width / frameSize.Height;

			CGSize size = CGSize.Empty;
			if (gravity == AVLayerVideoGravity.ResizeAspectFill) {
				if (viewRatio > apertureRatio) {
					size.Width = frameSize.Width;
					size.Height = apertureSize.Width * (frameSize.Width / apertureSize.Height);
				} else {
					size.Width = apertureSize.Height * (frameSize.Height / apertureSize.Width);
					size.Height = frameSize.Height;
				}
			} else if (gravity == AVLayerVideoGravity.ResizeAspect) {
				if (viewRatio > apertureRatio) {
					size.Width = apertureSize.Height * (frameSize.Height / apertureSize.Width);
					size.Height = frameSize.Height;
				} else {
					size.Width = frameSize.Width;
					size.Height = apertureSize.Width * (frameSize.Width / apertureSize.Height);
				}
			} else if (gravity == AVLayerVideoGravity.Resize) {
				size.Width = frameSize.Width;
				size.Height = frameSize.Height;
			}

			CGRect videoBox = CGRect.Empty;
			videoBox.Width = size.Width;
			videoBox.Height = size.Height;
			if (size.Width < frameSize.Width)
				videoBox.X = (frameSize.Width - size.Width) / 2;
			else
				videoBox.X = (size.Width - frameSize.Width) / 2;

			if ( size.Height < frameSize.Height )
				videoBox.Y = (frameSize.Height - size.Height) / 2;
			else
				videoBox.Y = (size.Height - frameSize.Height) / 2;

			return videoBox;
		}


		void drawFaces(CIFeature[] features, CGRect clearAperture, UIDeviceOrientation orientation)
		{
			var sublayers = this.previewLayer.Sublayers;
			int sublayersCount = sublayers.Length, currentSublayer = 0;
			int featuresCount = features.Length, currentFeature = 0;

			CATransaction.Begin ();
			CATransaction.DisableActions = true;
			//CATransaction.SetValueForKey (true, CATransaction.DisableActionsKey);

			// hide all the face layers
			foreach( CALayer layer in sublayers ) {
				if (layer.Name == "FaceLayer")
					layer.Hidden = true;
			}	

			if ( featuresCount == 0 ) {
				CATransaction.Commit ();
				return; // early bail.
			}

			var parentFrameSize = this.previewView.Frame.Size;
			var gravity = this.previewLayer.VideoGravity;
			var isMirrored = this.previewLayer.Mirrored;
			CGRect previewBox = videoPreviewBoxForGravity (
				                    gravity,
				                    parentFrameSize,
				                    clearAperture.Size
			                    );

			foreach ( CIFaceFeature ff in features ) {
				// find the correct position for the square layer within the previewLayer
				// the feature box originates in the bottom left of the video frame.
				// (Bottom right if mirroring is turned on)
				CGRect faceRect = ff.Bounds;

				// flip preview width and height
				var temp = faceRect.Size.Width;
				faceRect.Width = faceRect.Size.Height;
				faceRect.Height = temp;
				temp = faceRect.Location.X;
				faceRect.X = faceRect.Location.Y;
				faceRect.Y = temp;
				// scale coordinates so they fit in the preview box, which may be scaled
				var widthScaleBy = previewBox.Size.Width / clearAperture.Size.Height;
				var heightScaleBy = previewBox.Size.Height / clearAperture.Size.Width;
				faceRect.Width *= widthScaleBy;
				faceRect.Height *= heightScaleBy;
				faceRect.X *= widthScaleBy;
				faceRect.Y *= heightScaleBy;

				if (isMirrored)
					/*faceRect = */faceRect.Offset (previewBox.Location.X + previewBox.Size.Width - faceRect.Size.Width - (faceRect.Location.X * 2), previewBox.Location.Y);
				else
					/*faceRect = */faceRect.Offset (previewBox.Location.X, previewBox.Location.Y);

				CALayer featureLayer = null;

				// re-use an existing layer if possible
				while ( featureLayer == null && (currentSublayer < sublayersCount) ) {
					var currentLayer = sublayers [currentSublayer++];
					if (currentLayer.Name == "FaceLayer") {
						featureLayer = currentLayer;
						currentLayer.Hidden = false;
					}
				}

				// create a new one if necessary
				if (featureLayer == null) {
					featureLayer = new CALayer ();
					featureLayer.Contents = this.borderImage.CGImage;
					featureLayer.Name = "FaceLayer";
					this.previewLayer.AddSublayer (featureLayer);
					featureLayer = null;
				}
				featureLayer.Frame = faceRect;

				switch (orientation) {
				case UIDeviceOrientation.Portrait:
					featureLayer.AffineTransform = CGAffineTransform.MakeRotation (DegreesToRadians (0));
					break;
				case UIDeviceOrientation.PortraitUpsideDown:
					featureLayer.AffineTransform = CGAffineTransform.MakeRotation (DegreesToRadians (180));
					break;
				case UIDeviceOrientation.LandscapeLeft:
					featureLayer.AffineTransform = CGAffineTransform.MakeRotation (DegreesToRadians (90));
					break;
				case UIDeviceOrientation.LandscapeRight:
					featureLayer.AffineTransform = CGAffineTransform.MakeRotation (DegreesToRadians (-90));
					break;
				case UIDeviceOrientation.FaceUp:
				case UIDeviceOrientation.FaceDown:
				default:
					break; // leave the layer in its last known orientation
				}
				currentFeature++;
			}

			CATransaction.Commit ();
		}

		int exifOrientation(UIDeviceOrientation orientation)
		{
			ExifKind exifOrientation;
			/* kCGImagePropertyOrientation values
     The intended display orientation of the image. If present, this key is a CFNumber value with the same value as defined
     by the TIFF and EXIF specifications -- see enumeration of integer constants. 
     The value specified where the origin (0,0) of the image is located. If not present, a value of 1 is assumed.
     
     used when calling featuresInImage: options: The value for this key is an integer NSNumber from 1..8 as found in kCGImagePropertyOrientation.
     If present, the detection will be done based on that orientation but the coordinates in the returned features will still be based on those of the image. */

			switch (orientation) {
			case UIDeviceOrientation.PortraitUpsideDown:  // Device oriented vertically, home button on the top
				exifOrientation = ExifKind.LEFT_0COL_BOTTOM;
				break;
			case UIDeviceOrientation.LandscapeLeft:       // Device oriented horizontally, home button on the right
				if (this.isUsingFrontFacingCamera)
					exifOrientation = ExifKind.BOTTOM_0COL_RIGHT;
				else
					exifOrientation = ExifKind.TOP_0COL_LEFT;
				break;
			case UIDeviceOrientation.LandscapeRight:      // Device oriented horizontally, home button on the left
				if (this.isUsingFrontFacingCamera)
					exifOrientation = ExifKind.TOP_0COL_LEFT;
				else
					exifOrientation = ExifKind.BOTTOM_0COL_RIGHT;
				break;
			case UIDeviceOrientation.Portrait:            // Device oriented vertically, home button on the bottom
			default:
				exifOrientation = ExifKind.RIGHT_0COL_TOP;
				break;
			}
			return (int)exifOrientation;
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

		void InitLayout()
		{
			
		}
	}
}

