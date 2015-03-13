using System;
using Xamarin.Forms;

namespace XSTTLA.Shared
{
    public class App : Application
    {
        public App()
        {
            MainPage = new ContentPage {
                Content = new Label {
                    Text = "Hello, Forms !",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                }	
            }; 
        }
    }
}