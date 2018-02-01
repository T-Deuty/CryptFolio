using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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
        //const double totalBytesToReceive = 800000;
        //int bytesReceived = 0;

        public Splash() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var getAllTask = GetAllCurrencyData();
            base.OnNavigatedTo(e);
            DownloadProgressRing.IsActive = true;
        }

        private Task GetAllCurrencyData()
        {
            var task = App.apiObj.RequestAllAsync(this);

            task.ContinueWith(async (a) =>
            {
                App.jsonList = a.Result;

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DownloadProgressRing.IsActive = false;
                    this.Frame.Navigate(typeof(MainPage));
                });
            });
            return task;
        }

        //internal async void HandleProgressAsync(HttpProgress progress)
        //{
        //    if (progress.BytesReceived > 0)
        //    {
        //        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //        () =>
        //        {
        //            bytesReceived += (int)progress.BytesReceived;
        //            var prog = ((double)progress.BytesReceived / totalBytesToReceive) * 100;
        //            DownloadProgressBar.Value = prog;
        //        }
        //        );
        //    }
        //}
    }
}
