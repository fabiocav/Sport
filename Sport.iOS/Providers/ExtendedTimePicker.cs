using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Sport.Shared;
using Sport.iOS;

[assembly: ExportRenderer(typeof(Sport.Shared.ExtendedTimePicker), typeof(ExtendedTimePickerRenderer))]
namespace Sport.iOS
{
	/// <summary>
	/// A renderer for the ExtendedTimePicker control.
	/// </summary>
	public class ExtendedTimePickerRenderer : TimePickerRenderer
	{
		/// <summary>
		/// The on element changed callback.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			var view = (ExtendedTimePicker)Element;

			if(view != null)
			{
				SetFont(view);
				SetTextAlignment(view);
				SetBorder(view);
				SetTextColor(view);
				SetMaxLength(view);
				ResizeHeight();
			}
		}

		/// <summary>
		/// The on element property changed callback
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			var view = (ExtendedTimePicker)Element;

			if(e.PropertyName == ExtendedTimePicker.FontProperty.PropertyName)
				SetFont(view);

			if(e.PropertyName == ExtendedTimePicker.XAlignProperty.PropertyName)
				SetTextAlignment(view);
			
			if(e.PropertyName == ExtendedTimePicker.HasBorderProperty.PropertyName)
				SetBorder(view);

			if(e.PropertyName == ExtendedTimePicker.TextColorProperty.PropertyName)
				SetTextColor(view);

			ResizeHeight();
		}

		/// <summary>
		/// Sets the text alignment.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetTextAlignment(ExtendedTimePicker view)
		{
			switch(view.XAlign)
			{
				case TextAlignment.Center:
					Control.TextAlignment = UITextAlignment.Center;
					break;
				case TextAlignment.End:
					Control.TextAlignment = UITextAlignment.Right;
					break;
				case TextAlignment.Start:
					Control.TextAlignment = UITextAlignment.Left;
					break;
			}
		}

		/// <summary>
		/// Sets the font.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetFont(ExtendedTimePicker view)
		{
			UIFont uiFont;
			if(view.Font != Font.Default && (uiFont = view.Font.ToUIFont()) != null)
				Control.Font = uiFont;
			else if(view.Font == Font.Default)
				Control.Font = UIFont.SystemFontOfSize(17f);
		}

		/// <summary>
		/// Sets the border.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetBorder(ExtendedTimePicker view)
		{
			Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
		}

		/// <summary>
		/// Sets the maxLength.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetMaxLength(ExtendedTimePicker view)
		{
			Control.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= view.MaxLength;
			};
		}

		/// <summary>
		/// Resizes the height.
		/// </summary>
		private void ResizeHeight()
		{
			if(Element.HeightRequest >= 0)
				return;

			var height = Math.Max(Bounds.Height, new UITextField {
				Font = Control.Font
			}.IntrinsicContentSize.Height);

			Control.Frame = new CGRect(0.0f, 0.0f, (nfloat)Element.Width, (nfloat)height);

			Element.HeightRequest = height;
		}

		void SetTextColor(ExtendedTimePicker view)
		{
			if(view.TextColor == null)
				return;
			
			Control.TextColor = view.TextColor.ToUIColor();
		}
	}
}