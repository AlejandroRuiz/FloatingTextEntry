using System;
using Android.Views;
using Android.Content;
using Android.Graphics;

namespace Alex.Controls.Android.Views
{
	public class CustomView:View
	{
		public int PosX{
			get;
			set;
		}

		public int PosY{
			get;
			set;
		}

		private Bitmap leftEyeBmp;
		private int leftEyeBmpWidth;
		private int leftEyeBmpHeight;
		private Paint paint = new Paint();

		public CustomView (Context context):base(context)
		{
			CommonInit ();
		}

		public CustomView (Context context, global::Android.Util.IAttributeSet attrs):base(context, attrs)
		{
			CommonInit ();
		}

		public CustomView (Context context, global::Android.Util.IAttributeSet attrs, int defStyleAttr):base(context,attrs,defStyleAttr)
		{
			CommonInit ();
		}

		void CommonInit()
		{
			leftEyeBmp = BitmapFactory.DecodeResource(base.Context.Resources, Resource.Drawable.my_face_glasses);
			if(leftEyeBmp != null){
				leftEyeBmpWidth = leftEyeBmp.Width;
				leftEyeBmpHeight = leftEyeBmp.Height;
			}
		}

		protected override void OnDetachedFromWindow ()
		{
			base.OnDetachedFromWindow ();
			if (leftEyeBmp != null && !leftEyeBmp.IsRecycled) {
				leftEyeBmp.Recycle ();
			}
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);
			canvas.DrawBitmap (leftEyeBmp, PosX - leftEyeBmpWidth / 2, PosY - leftEyeBmpHeight / 2, paint);
		}
	}
}

