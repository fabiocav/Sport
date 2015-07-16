using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: 
	InternalsVisibleTo("Sport.Android"),
	InternalsVisibleTo("Sport.iOS")]

namespace Sport.Shared
{
	/// <summary>
	/// An extended entry control that allows the Font and text X alignment to be set
	/// </summary>
	public class ExtendedDatePicker : DatePicker
	{
		/// <summary>
		/// The font property
		/// </summary>
		public static readonly BindableProperty FontProperty =
			BindableProperty.Create("Font", typeof(Font), typeof(ExtendedDatePicker), new Font());

		/// <summary>
		/// The XAlign property
		/// </summary>
		public static readonly BindableProperty XAlignProperty =
			BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(ExtendedDatePicker), TextAlignment.Start);

		/// <summary>
		/// The HasBorder property
		/// </summary>
		public static readonly BindableProperty HasBorderProperty =
			BindableProperty.Create("HasBorder", typeof(bool), typeof(ExtendedDatePicker), true);

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create("TextColor", typeof(Color), typeof(ExtendedDatePicker), Color.Black);
		
		/// <summary>
		/// The MaxLength property
		/// </summary>
		public static readonly BindableProperty MaxLengthProperty =
			BindableProperty.Create("MaxLength", typeof(int), typeof(ExtendedDatePicker), int.MaxValue);

		/// <summary>
		/// Gets or sets the MaxLength
		/// </summary>
		public int MaxLength
		{
			get
			{
				return (int)this.GetValue(MaxLengthProperty);
			}
			set
			{
				this.SetValue(MaxLengthProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the Font
		/// </summary>
		public Font Font
		{
			get
			{
				return (Font)GetValue(FontProperty);
			}
			set
			{
				SetValue(FontProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the X alignment of the text
		/// </summary>
		public TextAlignment XAlign
		{
			get
			{
				return (TextAlignment)GetValue(XAlignProperty);
			}
			set
			{
				SetValue(XAlignProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets if the border should be shown or not
		/// </summary>
		public bool HasBorder
		{
			get
			{
				return (bool)GetValue(HasBorderProperty);
			}
			set
			{
				SetValue(HasBorderProperty, value);
			}
		}

		public Color TextColor
		{
			get
			{
				return (Color)GetValue(TextColorProperty);
			}
			set
			{
				SetValue(TextColorProperty, value);
			}
		}

		public static readonly BindableProperty FontFamilyProperty =
			BindableProperty.Create("FontFamily", typeof(string), typeof(ExtendedDatePicker), null);

		/// <summary>
		/// Gets or sets the MaxLength
		/// </summary>
		public string FontFamily
		{
			get
			{
				return (string)this.GetValue(FontFamilyProperty);
			}
			set
			{
				this.SetValue(FontFamilyProperty, value);
			}
		}

	}
}