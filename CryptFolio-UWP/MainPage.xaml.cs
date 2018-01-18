using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using System.Numerics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CryptFolio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Dictionary<string, string> nameDictionary;
        //private CoinMarketCapAPI apiObj;
        private string selectedCurrency;
        private double addedAmount, totalInvestmentValue;
        private TickerJSONResult result;

        public MainPage()
        {
            this.InitializeComponent();
            this.nameDictionary = App.apiObj.GetNameDictionary();
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
            var fireAndForget = GetSingleCurrencyDataAndDisplay();
            // TODO add error handler
        }

        private Task GetSingleCurrencyDataAndDisplay()
        {
            string selectedItem = selectedCurrency;
            string ticker = nameDictionary[selectedItem];

            var task = App.apiObj.RequestTickerAsync(ticker);
            task.ContinueWith((a) =>
            {
                var u = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
               {
                   this.result = a.Result;

                   if (this.result != null)
                   {
                       DisplayCurrencyStats(ticker, selectedItem);
                   }
                   else
                   {
                       DisplayAPIError();
                   }
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

        private void DisplayCurrencyStats(string ticker, string displayName)
        {
            TextBlock currencyLabel, usdLabel, btcLabel, userAmountLabel, userUSDValueLabel;
            CustomStackPanel stackPanelToUpdate;

            string
                stackPanelStr = ticker + "StackPanel",
                labelStr = ticker + "Label",
                labelContentStr = displayName + " Stats:",
                labelUSDStr = ticker + "USD",
                labelUSDContentStr = "USD Price: $" + result.price_usd,
                labelBTCStr = ticker + "BTC",
                labelBTCContentStr = "BTC Price: " + result.price_btc,
                labelUserAmountStr = ticker + "UserAmount",
                labelUserAmountContentStr = "Amount you own: ",
                labelUserUSDValueStr = ticker + "UserUSDVal",
                labelUserUSDValueContentStr = "Your estimated USD value: ";

            // add label if currency not already displayed
            stackPanelToUpdate = GetChildOfStackPanel(stackPanelRight, stackPanelStr) as CustomStackPanel;

            if (stackPanelToUpdate == null)
            {
                // create new StackPanel to hold labels
                stackPanelToUpdate = new CustomStackPanel
                {
                    Name = stackPanelStr,
                    Margin = new Thickness(5, 0, 5, 5)
                };

                // create and add currency label
                currencyLabel = new TextBlock
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 32,
                    Text = labelContentStr,
                    Name = labelStr
                };
                stackPanelToUpdate.Children.Add(currencyLabel);

                // create and add usdLabel
                usdLabel = new TextBlock
                {
                    Name = labelUSDStr,
                    FontSize = 22,
                    Text = labelUSDContentStr
                };
                stackPanelToUpdate.Children.Add(usdLabel);

                // create and add btcLabel
                btcLabel = new TextBlock
                {
                    Name = labelBTCStr,
                    FontSize = 22,
                    Text = labelBTCContentStr
                };
                stackPanelToUpdate.Children.Add(btcLabel);

                // create and add user holding amount label
                userAmountLabel = new TextBlock
                {
                    Name = labelUserAmountStr,
                    FontSize = 22,
                    Text = labelUserAmountContentStr + addedAmount.ToString()
                };
                stackPanelToUpdate.Children.Add(userAmountLabel);

                // create and add userUSDValue label
                userUSDValueLabel = new TextBlock
                {
                    Name = labelUserUSDValueStr,
                    FontSize = 22,
                    Text = UpdateUserUSDValueContentStr(ref stackPanelToUpdate, this.result.price_usd, labelUserUSDValueContentStr),
                    Foreground = new SolidColorBrush(Colors.ForestGreen)
                };
                stackPanelToUpdate.Children.Add(userUSDValueLabel);

                stackPanelRight.Children.Add(stackPanelToUpdate);
            }
            else
            {
                // update user value
                stackPanelToUpdate.UserValue = addedAmount * Convert.ToDouble(this.result.price_usd);

                // update price labels
                usdLabel = GetChildOfStackPanel(stackPanelToUpdate, labelUSDStr) as TextBlock;
                usdLabel.Text = labelUSDContentStr;
                btcLabel = GetChildOfStackPanel(stackPanelToUpdate, labelBTCStr) as TextBlock;
                btcLabel.Text = labelBTCContentStr;
                userUSDValueLabel = GetChildOfStackPanel(stackPanelToUpdate, labelUserUSDValueStr) as TextBlock;
                userUSDValueLabel.Text = UpdateUserUSDValueContentStr(ref stackPanelToUpdate, this.result.price_usd, labelUserUSDValueContentStr);
                userAmountLabel = GetChildOfStackPanel(stackPanelToUpdate, labelUserAmountStr) as TextBlock;
                userAmountLabel.Text = labelUserAmountContentStr + addedAmount.ToString();
            }

            // update total values
            UpdateTotalValue();
        }

        private void UpdateTotalValue()
        {
            totalInvestmentValue = 0.0;
            foreach (CustomStackPanel child in stackPanelRight.Children)
            {
                totalInvestmentValue += child.UserValue;
            }

            textBlockTotalValue.Text = "Total investment value: $" + totalInvestmentValue.ToString();
        }

        private FrameworkElement GetChildOfStackPanel(StackPanel parentPanel, string name)
        {
            foreach (FrameworkElement child in parentPanel.Children)
            {
                if (child.Name == name)
                {
                    return child;
                }
            }

            return null;
        }

        /*
        private void AutoSuggestBoxAddUserHolding_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxAddUserHolding.Text != null && textBoxAddUserHolding.Text != "")
            {
                addedAmount = Convert.ToDouble(textBoxAddUserHolding.Text);
            }
            else
            {
                addedAmount = 0.0;
            }
        }*/

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
                if (amountOwnedBox.Text != null && amountOwnedBox.Text != "")
                    addedAmount = Convert.ToDouble(amountOwnedBox.Text);

                else
                    addedAmount = 0.0;
            }

            catch
            {
                Console.WriteLine("Invalid input! Please enter numbers only");
                amountOwnedBox.Text = "";
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddCurrenciesToMarketView();
        }

        private string UpdateUserUSDValueContentStr(ref CustomStackPanel sP, string priceStr, string contentStr)
        {
            // update user value
            sP.UserValue = addedAmount * Convert.ToDouble(priceStr);

            if (addedAmount == 0.0)
            {
                contentStr += "N/A";
            }
            else
            {
                contentStr += "$" + sP.UserValue.ToString();
            }

            return contentStr;
        }
    }
}
