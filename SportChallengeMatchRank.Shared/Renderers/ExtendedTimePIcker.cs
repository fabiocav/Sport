using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: 
	InternalsVisibleTo("SportChallengeMatchRank.Android"),
	InternalsVisibleTo("SportChallengeMatchRank.iOS")]

namespace SportChallengeMatchRank.Shared
{
	/// <summary>
	/// An extended entry control that allows the Font and text X alignment to be set
	/// </summary>
	public class ExtendedTimePicker : TimePicker
	{
		/// <summary>
		/// The font property
		/// </summary>
		public static readonly BindableProperty FontProperty =
			BindableProperty.Create("Font", typeof(Font), typeof(ExtendedTimePicker), new Font());

		/// <summary>
		/// The XAlign property
		/// </summary>
		public static readonly BindableProperty XAlignProperty =
			BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(ExtendedTimePicker), TextAlignment.Start);

		/// <summary>
		/// The HasBorder property
		/// </summary>
		public static readonly BindableProperty HasBorderProperty =
			BindableProperty.Create("HasBorder", typeof(bool), typeof(ExtendedTimePicker), true);

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create("TextColor", typeof(Color), typeof(ExtendedTimePicker), Color.Black);
		
		/// <summary>
		/// The MaxLength property
		/// </summary>
		public static readonly BindableProperty MaxLengthProperty =
			BindableProperty.Create("MaxLength", typeof(int), typeof(ExtendedTimePicker), int.MaxValue);

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
	}
}