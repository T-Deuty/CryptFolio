using System;
using System.Net;
using System.Net.Http;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptFolio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class Splash : Page
    {
        //internal Frame rootFrame;

        public Splash() => this.InitializeComponent();

        public void test()
        {
            
        }

        //delegate method
        internal static void HandleProgress(HttpProgress progress)
        {
            
        }
    }
}
