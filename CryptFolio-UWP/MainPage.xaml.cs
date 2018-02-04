using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CryptFolio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Dictionary<string, string> nameDictionary;
        private string selectedCurrency;
        private PortfolioList portfolioList;

        public MainPage()
        {
            this.InitializeComponent();
            this.nameDictionary = App.apiObj.GetNameDictionary();
            this.portfolioList = new PortfolioList();
            this.LoadPreviousCoins();

        }
        private void AddCurrenciesToMarketView()
        {
            CurrencyStackPanelBuilder spBuilder = new CurrencyStackPanelBuilder();
            foreach (TickerJSONResult result in App.jsonList)
            {
                mainStackTrackerPage.Children.Add(spBuilder.BuildGrid(result));
            }
        }
        
        // BUTTON CLICK HANDLER
        private void ButtonAddAmount_Click(object sender, RoutedEventArgs e)
        {
            // --- only send API request if not called within last API_RECOMMENDED_TIME seconds ---
            const Int64 API_RECOMMENDED_LIMIT = 6;

            // timeSinceLastUpdated calculated here
            try
            {
                var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                string selectedItem = selectedCurrency;
                string id = nameDictionary[selectedItem];

                // get result from list if it exists
                var timeSinceLastUpdated = API_RECOMMENDED_LIMIT + 1; // initialize to higher than API limit
                portfolioList.result = App.apiObj.RetrieveJSONById(id);

                if (App.jsonList != null && portfolioList.result != null)
                    timeSinceLastUpdated = currentTime - Convert.ToInt64(portfolioList.result.last_updated);

                if (timeSinceLastUpdated > API_RECOMMENDED_LIMIT)
                {
                    var fireAndForget = GetSingleCurrencyDataAndDisplay(id, selectedItem, currentTime.ToString());
                    // TODO add error handler
                }
                else
                {
                    portfolioList.DisplayCurrencyStats(id, selectedItem, ref stackPanelRight, ref PortfolioListScrollViewer);
                    UpdateTotalValue();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        private Task GetSingleCurrencyDataAndDisplay(string id, string selectedItem, string currentTime)
        {
            var task = App.apiObj.RequestTickerAsync(id);
            task.ContinueWith((a) =>
            {
               var u = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
               {
                   portfolioList.result = a.Result;

                   if (a.Result != null)
                   {
                       // update last_updated value of the currency
                       var index = App.jsonList.FindIndex(x => x.id == id);
                       App.jsonList[index].last_updated = currentTime;
                       portfolioList.DisplayCurrencyStats(id, selectedItem, ref stackPanelRight, ref PortfolioListScrollViewer);
                       UpdateTotalValue();
                   }
                   else
                       DisplayAPIError();
               });
            });
            return task;
        }

        private void DisplayAPIError()
        {
            CustomStackPanel stackPanelForError = new CustomStackPanel();
            TextBlock errorTextBlock = new TextBlock
            {
                Text = "API Error: ID Not Found",
                FontSize = 20
            };

            stackPanelForError.Children.Add(errorTextBlock);

            stackPanelRight.Children.Add(stackPanelForError);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset

                var tempKeys = new List<string>();

                foreach (string key in nameDictionary.Keys)
                {
                    if (key.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tempKeys.Add(key);
                    }
                }

                sender.ItemsSource = tempKeys;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = args.SelectedItem as string;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                selectedCurrency = args.ChosenSuggestion as string;
                buttonAddAmount.IsEnabled = true;
            }
            else
            {
                // Use args.QueryText to determine what to do.
                var suggestionList = sender.ItemsSource as List<string>;

                // Error handling if user tries to enter text with no suggestionList
                try
                {
                    sender.Text = suggestionList[0];
                    selectedCurrency = suggestionList[0];
                    buttonAddAmount.IsEnabled = true;
                }

                catch
                {
                    sender.Text = "";
                }
            }
        }

        private void AmountOwnedBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Error handling if user does not enter numeric input
            try
            {
                portfolioList.addedAmount = Convert.ToDouble(amountOwnedBox.Text);
                portfolioList.addedAmount = Math.Round(portfolioList.addedAmount, 6);
            }
            catch (Exception)
            {
                amountOwnedBox.Text = portfolioList.addedAmount.ToString();
                FlyoutBase.ShowAttachedFlyout(amountOwnedBox);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddCurrenciesToMarketView();
        }

        private void UpdateTotalValue()
        {
            textBlockTotalValue.Text = "Total investment value: $" + portfolioList.CalculateTotalInvestmentValue(ref stackPanelRight).ToString();
        }

        public void LoadPreviousCoins()
        {
            //Gets all coins previously stored
            List<DataAccessLibrary.DataAccess.CoinInfoClass> checkCoins = DataAccessLibrary.DataAccess.GetDataEntries();

            try
            {
                //Traverse SQLite table and add all coins to view
                for (int i = 0; i < checkCoins.Count; i++)
                {
                    var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                    string selectedItem = checkCoins[i].theCoinsFullName;
                    string id = checkCoins[i].theCoinsName;
                    portfolioList.addedAmount = checkCoins[i].theCoinsAmount;
  
                    portfolioList.result = App.apiObj.RetrieveJSONById(id);
                    //InitialLoad_DisplayCurrencyStats(id, selectedItem); 
                    portfolioList.DisplayCurrencyStats(id, selectedItem, ref stackPanelRight, ref PortfolioListScrollViewer, true);
                    // TODO add error handler 
                    UpdateTotalValue();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
