using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Text.RegularExpressions;
using CoreAnimation;
using PureLayoutSharp;
using System.Threading.Tasks;
using Alex.Controls.Forms;

namespace Alex.Controls.iOS.Controls
{
	public class EGFloatingTextEntryContainer : UIView
	{
		public EGFloatingTextEntry MainControl;
		public UILabel ErrorLabel;

		public EGFloatingTextEntryContainer(CGRect frame):base(frame)
		{
			ErrorLabel = new UILabel (new CGRect (0, 45, 320, 15));
			MainControl = new EGFloatingTextEntry (new CGRect (0, 0, 320, 40));

			ErrorLabel.TextColor = MainControl.ErrorColor;
			ErrorLabel.TextAlignment = UITextAlignment.Left;
			ErrorLabel.Lines = 1;
			ErrorLabel.Font = UIFont.FromName ("HelveticaNeue",10);


			this.Add (MainControl);
			this.Add (ErrorLabel);
			MainControl.errorLabel = ErrorLabel;
		}

	}

	public class EGFloatingTextEntry:UITextField
	{

		public FloatingTextEntryValidator Validator { get; set; }

		public string ErrorMessage { get; set; }
		
		UIColor kDefaultInactiveColor = UIColor.FromWhiteAlpha(0, 0.54f);
		UIColor kDefaultActiveColor = UIColor.Blue;
		UIColor kDefaultErrorColor = UIColor.Red;
		float kDefaultLineHeight = 22;
		UIColor kDefaultLabelTextColor = UIColor.FromWhiteAlpha(0, 0.54f);


		public bool floatingLabel;
		UILabel label;

		public UILabel errorLabel;

		UIFont labelFont;
		UIColor labelTextColor;
		UIView activeBorder;
		bool floating;
		public bool active;
		bool hasError;
		string errorMessage;

		public EGFloatingTextEntry (NSCoder coder):base(coder)
		{
			this.commonInit ();
		}

		public EGFloatingTextEntry (CGRect frame):base(frame)
		{
			this.commonInit ();
		}

		void commonInit(){
			this.floating = false;
			this.hasError = false;

			this.labelTextColor = kDefaultLabelTextColor;
			this.label = new UILabel (CGRect.Empty);
			if(this.labelFont != null)
				this.label.Font = this.labelFont;
			this.label.TextColor = this.labelTextColor;
			this.label.TextAlignment = UITextAlignment.Left;
			this.label.Lines = 1;
			this.label.Layer.MasksToBounds = false;
			this.AddSubview (this.label);

			this.activeBorder = new UIView (CGRect.Empty);
			this.activeBorder.BackgroundColor = kDefaultActiveColor;
			this.activeBorder.Layer.Opacity = 0;
			this.AddSubview (this.activeBorder);

			this.label.AutoAlignAxis (ALAxis.Horizontal, this);
			this.label.AutoPinEdge (ALEdge.Left, ALEdge.Left, this);
			this.label.AutoMatchDimension (ALDimension.Width, ALDimension.Width, this);
			this.label.AutoMatchDimension (ALDimension.Height, ALDimension.Height, this);

			this.activeBorder.AutoPinEdge (ALEdge.Bottom, ALEdge.Bottom, this);
			this.activeBorder.AutoPinEdge (ALEdge.Left, ALEdge.Left, this);
			this.activeBorder.AutoPinEdge (ALEdge.Right, ALEdge.Right, this);
			this.activeBorder.AutoSetDimension (ALDimension.Height, 2);

			NSNotificationCenter.DefaultCenter.AddObserver (
				UITextField.TextFieldTextDidChangeNotification,
				(notification) => {
					this.validate();
				}
			);
		}

		public UIColor ErrorColor {
			get{
				return kDefaultErrorColor;
			}set{
				kDefaultErrorColor = value;
			}
		}

		public UIColor InactiveAccentColor {
			get{
				return kDefaultInactiveColor;
			}set{
				kDefaultInactiveColor = value;
				if(!this.active)
					this.label.TextColor = kDefaultInactiveColor;
			}
		}

		public UIColor AccentColor {
			get{
				return kDefaultActiveColor;
			}set{
				kDefaultActiveColor = value;
				this.activeBorder.BackgroundColor = kDefaultActiveColor;
				if(this.active)
					this.label.TextColor = kDefaultActiveColor;
			}
		}

		public string PlaceHolder {
			get{
				return this.label.Text;
			}set{
				this.label.Text = value;
			}
		}

		public void SetText(string value)
		{
			if (!floating) {
				this.Text = value;
				if (!string.IsNullOrWhiteSpace (this.Text)) {
					this.floatLabelToTop (false);
					this.floating = true;
				} else {
					this.animateLabelBack ();
					this.floating = false;
				}
			}
		}

		public override bool BecomeFirstResponder ()
		{
			var flag = base.BecomeFirstResponder ();

			if (flag) {
				if (this.floatingLabel){
					if(!this.floating || string.IsNullOrWhiteSpace(this.Text)) {
						this.floatLabelToTop ();
						this.floating = true;
					}
				} else {
					this.label.Layer.Opacity = 0;
				}
				this.label.TextColor = kDefaultActiveColor;
				this.showActiveBorder ();
			}
			this.active = flag;
			return flag;
		}

		public override bool ResignFirstResponder ()
		{
			var flag = base.ResignFirstResponder();

			if (flag) {
				if (this.floatingLabel) {
					if (this.floating && string.IsNullOrWhiteSpace (this.Text)) {
						this.animateLabelBack ();
						this.floating = false;
					}
				} else {
					if (string.IsNullOrWhiteSpace (this.Text)) {
						this.label.Layer.Opacity = 1;
					}
				}
				this.label.TextColor = kDefaultInactiveColor;
				this.showInactiveBorder ();
				this.validate();
			}
			this.active = flag;
			return flag;
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);
			var borderColor = this.hasError ? kDefaultErrorColor : kDefaultInactiveColor;

			var textRect = this.TextRect (rect);
			var context = UIGraphics.GetCurrentContext ();
			var borderlines = new CGPoint[] {
				new CGPoint (0, textRect.Height - 1),
				new CGPoint (textRect.Width, textRect.Height - 1)
			};

			if (this.Enabled) {
				context.BeginPath ();
				context.AddLines (borderlines);
				context.SetLineWidth (1);
				context.SetStrokeColor (borderColor.CGColor);
				context.StrokePath ();
			}else{
				context.BeginPath ();
				context.AddLines (borderlines);
				context.SetLineWidth (1);
				var dashPattern = new nfloat[]{
					2,
					4
				};
				context.SetLineDash (0, dashPattern, 2);
				context.SetStrokeColor (borderColor.CGColor);
				context.StrokePath ();
			}
		}

		void floatLabelToTop(bool changeColor = true) {
			CATransaction.Begin ();
			CATransaction.CompletionBlock = () => {
				if(changeColor)
					this.label.TextColor = this.kDefaultActiveColor;
			};

			var anim2 = CABasicAnimation.FromKeyPath ("transform");
			var fromTransform = CATransform3D.MakeScale (1, 1, 1);
			var toTransform = CATransform3D.MakeScale (0.5f, 0.5f, 1);
			toTransform = toTransform.Translate (-this.label.Frame.Width / 2, -this.label.Frame.Height, 0);
			anim2.From = NSValue.FromCATransform3D (fromTransform);
			anim2.To = NSValue.FromCATransform3D (toTransform);
			anim2.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			var animGroup = new CAAnimationGroup ();
			animGroup.Animations = new CAAnimation[]{ anim2 };
			animGroup.Duration = 0.3;
			animGroup.FillMode = CAFillMode.Forwards;
			animGroup.RemovedOnCompletion = false;
			this.label.Layer.AddAnimation (animGroup, "_floatingLabel");
			this.ClipsToBounds = false;
			CATransaction.Commit ();
		}

		void showActiveBorder() {
			this.activeBorder.Layer.Transform = CATransform3D.MakeScale (0.01f, 1, 1);
			this.activeBorder.Layer.Opacity = 1;
			CATransaction.Begin ();
			this.activeBorder.Layer.Transform = CATransform3D.MakeScale (0.01f, 1, 1);
			var anim2 = CABasicAnimation.FromKeyPath ("transform");
			var fromTransform = CATransform3D.MakeScale (0.01f, 1, 1);
			var toTransform = CATransform3D.MakeScale (1, 1, 1);
			anim2.From = NSValue.FromCATransform3D (fromTransform);
			anim2.To = NSValue.FromCATransform3D (toTransform);
			anim2.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			anim2.FillMode = CAFillMode.Forwards;
			anim2.RemovedOnCompletion = false;
			this.activeBorder.Layer.AddAnimation (anim2, "_activeBorder");
			CATransaction.Commit ();
		}

		void animateLabelBack() {
			CATransaction.Begin ();
			CATransaction.CompletionBlock = () =>{
				this.label.TextColor = this.kDefaultInactiveColor;
			};

			var anim2 = CABasicAnimation.FromKeyPath ("transform");
			var fromTransform = CATransform3D.MakeScale (0.5f, 0.5f, 1);
			fromTransform = fromTransform.Translate (-this.label.Frame.Width / 2, -this.label.Frame.Height, 0);
			var toTransform = CATransform3D.MakeScale (1, 1, 1);
			anim2.From = NSValue.FromCATransform3D (fromTransform);
			anim2.To = NSValue.FromCATransform3D (toTransform);
			anim2.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			var animGroup = new CAAnimationGroup ();
			animGroup.Animations = new CAAnimation[] {
				anim2
			};
			animGroup.Duration = 0.3;
			animGroup.FillMode = CAFillMode.Forwards;
			animGroup.RemovedOnCompletion = false;

			this.label.Layer.AddAnimation (animGroup, "_animateLabelBack");
			CATransaction.Commit ();
		}

		void showInactiveBorder() {
			CATransaction.Begin ();
			CATransaction.CompletionBlock =()=>{
				this.activeBorder.Layer.Opacity = 0;
			};
			var anim2 = CABasicAnimation.FromKeyPath ("transform");
			var fromTransform = CATransform3D.MakeScale (1, 1, 1);
			var toTransform = CATransform3D.MakeScale (0.01f, 1, 1);
			anim2.From = NSValue.FromCATransform3D (fromTransform);
			anim2.To = NSValue.FromCATransform3D (toTransform);
			anim2.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			anim2.FillMode = CAFillMode.Forwards;
			anim2.RemovedOnCompletion = false;
			this.activeBorder.Layer.AddAnimation(anim2, "_activeBorder");
			CATransaction.Commit ();
		}

		void performValidation(bool isValid,string message){
			if (!isValid){
				this.hasError = true;
				this.errorMessage = message;
				this.errorLabel.Text = this.errorMessage;
				this.labelTextColor = kDefaultErrorColor;
				this.errorLabel.TextColor = this.kDefaultErrorColor;
				this.activeBorder.BackgroundColor = kDefaultErrorColor;
				this.SetNeedsDisplay ();
			}else {
				this.hasError = false;
				this.errorMessage = null;
				this.errorLabel.Text = this.errorMessage;
				this.labelTextColor = kDefaultActiveColor;
				this.activeBorder.BackgroundColor = kDefaultActiveColor;
				this.SetNeedsDisplay();
			}
		}

		void validate(){
			if (Validator != null) {
				var isValid = Validator (this.Text);
				performValidation (isValid, ErrorMessage);
			}
		}
	}
}

