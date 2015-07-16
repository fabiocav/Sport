using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Sport.iOS;
using Sport.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Sport.Shared.ExtendedDatePicker), typeof(ExtendedDatePickerRenderer))]
namespace Sport.iOS
{
	/// <summary>
	/// A renderer for the ExtendedDatePicker control.
	/// </summary>
	public class ExtendedDatePickerRenderer : DatePickerRenderer
	{
		/// <summary>
		/// The on element changed callback.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			var view = (ExtendedDatePicker)Element;

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

			var view = (ExtendedDatePicker)Element;

			if(e.PropertyName == ExtendedDatePicker.FontProperty.PropertyName)
				SetFont(view);

			if(e.PropertyName == ExtendedDatePicker.XAlignProperty.PropertyName)
				SetTextAlignment(view);
			
			if(e.PropertyName == ExtendedDatePicker.HasBorderProperty.PropertyName)
				SetBorder(view);

			if(e.PropertyName == ExtendedDatePicker.TextColorProperty.PropertyName)
				SetTextColor(view);

			if(e.PropertyName == ExtendedDatePicker.FontFamilyProperty.PropertyName)
				SetFontFamily(view);
			
			ResizeHeight();
		}

		/// <summary>
		/// Sets the text alignment.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetTextAlignment(ExtendedDatePicker view)
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
		private void SetFont(ExtendedDatePicker view)
		{
			UIFont uiFont;
			if(view.Font != Font.Default && (uiFont = view.Font.ToUIFont()) != null)
				Control.Font = uiFont;
			else if(view.Font == Font.Default)
				Control.Font = UIFont.SystemFontOfSize(17f);
		}

		private void SetFontFamily(ExtendedDatePicker view)
		{
			UIFont uiFont;

			if(!string.IsNullOrWhiteSpace(view.FontFamily) && (uiFont = view.Font.ToUIFont()) != null)
			{
				var ui = UIFont.FromName(view.FontFamily, (nfloat)(view.Font != null ? view.Font.FontSize : 17f));
				Control.Font = uiFont;
			}
		}

		/// <summary>
		/// Sets the border.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetBorder(ExtendedDatePicker view)
		{
			Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
		}

		/// <summary>
		/// Sets the maxLength.
		/// </summary>
		/// <param name="view">The view.</param>
		private void SetMaxLength(ExtendedDatePicker view)
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

		void SetTextColor(ExtendedDatePicker view)
		{
			Control.TextColor = view.TextColor.ToUIColor();
		}
	}
}