using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using CryptFolio.Helper_Classes;
using Windows.UI.Xaml.Input;

namespace CryptFolio
{
    class PortfolioList
    {
        internal TickerJSONResult result;
        internal double addedAmount;
        internal StackPanel rightStackPanelRef;

        public PortfolioList() { }

        private FrameworkElement GetChildOfStackPanel(StackPanel parentPanel, string name)
        {
            foreach (FrameworkElement child in parentPanel.Children)
                if (child.Name == name)
                    return child;
            return null;
        }

        internal void DisplayCurrencyStats(string ticker, string displayName, ref StackPanel stackPanelRight, ref ScrollViewer scrollViewer, bool initialLoad = false)
        {
            rightStackPanelRef = stackPanelRight;

            //PrepareAndAddCurrencyStats(ticker, displayName, ref stackPanelRight, ref scrollViewer);
            PrepareAndAddCurrencyStats(ticker, displayName, ref scrollViewer);
            if (!initialLoad)
                AddToDB(ticker, displayName);
        }

        private void AddToDB(string ticker, string displayName)
        {
            //Add Value into the Portfolio_Main Table
            DataAccessLibrary.DataAccess.AddDataEntries(ticker, displayName, addedAmount);
        }

        //internal void InitialLoad_DisplayCurrencyStats(string ticker, string displayName, ref StackPanel stackPanelRight)
        //{
        //    TextBlock currencyLabel, usdLabel, btcLabel, userAmountLabel, userUSDValueLabel;
        //    CustomStackPanel stackPanelToUpdate;

        //    string
        //        stackPanelStr = ticker + "StackPanel",
        //        labelStr = ticker + "Label",
        //        labelContentStr = displayName + " Stats:",
        //        labelUSDStr = ticker + "USD",
        //        labelUSDContentStr = "USD Price: $" + result.price_usd,
        //        labelBTCStr = ticker + "BTC",
        //        labelBTCContentStr = "BTC Price: " + result.price_btc,
        //        labelUserAmountStr = ticker + "UserAmount",
        //        labelUserAmountContentStr = "Amount you own: ",
        //        labelUserUSDValueStr = ticker + "UserUSDVal",
        //        labelUserUSDValueContentStr = "Your estimated USD value: ";

        //    // add label if currency not already displayed
        //    stackPanelToUpdate = GetChildOfStackPanel(stackPanelRight, stackPanelStr) as CustomStackPanel;

        //    // create new StackPanel to hold labels
        //    stackPanelToUpdate = new CustomStackPanel
        //    {
        //        Name = stackPanelStr,
        //        Margin = new Thickness(5, 0, 5, 5)
        //    };

        //    // create and add currency label
        //    currencyLabel = new TextBlock
        //    {
        //        FontWeight = FontWeights.Bold,
        //        FontSize = 32,
        //        Text = labelContentStr,
        //        Name = labelStr
        //    };
        //    stackPanelToUpdate.Children.Add(currencyLabel);

        //    // create and add usdLabel
        //    usdLabel = new TextBlock
        //    {
        //        Name = labelUSDStr,
        //        FontSize = 22,
        //        Text = labelUSDContentStr
        //    };
        //    stackPanelToUpdate.Children.Add(usdLabel);

        //    // create and add btcLabel
        //    btcLabel = new TextBlock
        //    {
        //        Name = labelBTCStr,
        //        FontSize = 22,
        //        Text = labelBTCContentStr
        //    };
        //    stackPanelToUpdate.Children.Add(btcLabel);

        //    // create and add user holding amount label
        //    userAmountLabel = new TextBlock
        //    {
        //        Name = labelUserAmountStr,
        //        FontSize = 22,
        //        Text = labelUserAmountContentStr + addedAmount.ToString()
        //    };
        //    stackPanelToUpdate.Children.Add(userAmountLabel);

        //    // create and add userUSDValue label
        //    userUSDValueLabel = new TextBlock
        //    {
        //        Name = labelUserUSDValueStr,
        //        FontSize = 22,
        //        Text = UpdateUserUSDValueContentStr(ref stackPanelToUpdate, this.result.price_usd, labelUserUSDValueContentStr),
        //        Foreground = new SolidColorBrush(Colors.ForestGreen)
        //    };
        //    stackPanelToUpdate.Children.Add(userUSDValueLabel);

        //    stackPanelRight.Children.Add(stackPanelToUpdate);
        //}

        //private void PrepareAndAddCurrencyStats(string ticker, string displayName, ref StackPanel stackPanelRight, ref ScrollViewer scrollViewer)
        //private void PrepareAndAddCurrencyStats(string ticker, string displayName, ref StackPanel stackPanelRight, ref ScrollViewer scrollViewer)
        private void PrepareAndAddCurrencyStats(string ticker, string displayName, ref ScrollViewer scrollViewer)
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
            stackPanelToUpdate = GetChildOfStackPanel(rightStackPanelRef, stackPanelStr) as CustomStackPanel;

            if (stackPanelToUpdate == null)
            {
                // create new StackPanel to hold labels
                stackPanelToUpdate = new CustomStackPanel
                {
                    Name = stackPanelStr,
                    Margin = new Thickness(5, 0, 5, 5)
                };

                stackPanelToUpdate.PointerEntered += PointerEnteredStackPanel;
                stackPanelToUpdate.PointerExited += PointerExitedStackPanel;

                ColumnDefinition currencyLabelColumnDefinitionLeft = new ColumnDefinition
                {
                    Width = new GridLength(8, GridUnitType.Star)
                };
                ColumnDefinition currencyLabelColumnDefinitionRight = new ColumnDefinition
                {
                    Width = new GridLength(2, GridUnitType.Star)
                };

                // create and add currency grid
                Grid currencyGrid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                currencyGrid.ColumnDefinitions.Add(currencyLabelColumnDefinitionLeft);
                currencyGrid.ColumnDefinitions.Add(currencyLabelColumnDefinitionRight);

                currencyLabel = new TextBlock
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 40,
                    Text = labelContentStr,
                    Name = labelStr
                };
                currencyGrid.Children.Add(currencyLabel);
                Grid.SetColumn(currencyLabel, 0);

                //Button deleteCurrencyButton = new Button
                stackPanelToUpdate.RemoveButton = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = new SymbolIcon(Symbol.Clear),
                    Tag = stackPanelStr,
                    Background = new SolidColorBrush { Opacity = 0 },
                    Opacity = 0,
                    IsEnabled = false
                };

                stackPanelToUpdate.RemoveButton.Click += RemoveCurrencyStatsForCurrency;

                currencyGrid.Children.Add(stackPanelToUpdate.RemoveButton);
                Grid.SetColumn(stackPanelToUpdate.RemoveButton, 1);

                //stackPanelToUpdate.Children.Add(currencyLabel);
                stackPanelToUpdate.Children.Add(currencyGrid);

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

                rightStackPanelRef.Children.Add(stackPanelToUpdate);

                PopInThemeAnimation popInAnimation = new PopInThemeAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    FromHorizontalOffset = 300,
                    TargetName = stackPanelToUpdate.Name
                };

                Storyboard.SetTarget(popInAnimation, stackPanelToUpdate);

                Storyboard popInAnimationStoryboard = new Storyboard();
                popInAnimationStoryboard.Children.Add(popInAnimation);
                popInAnimationStoryboard.Begin();

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

                PopInThemeAnimation popInAnimation = new PopInThemeAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    FromHorizontalOffset = 300,
                    TargetName = userUSDValueLabel.Name
                };

                Storyboard.SetTarget(popInAnimation, userUSDValueLabel);

                Storyboard popInAnimationStoryboard = new Storyboard();
                popInAnimationStoryboard.Children.Add(popInAnimation);
                popInAnimationStoryboard.Begin();
            }

            UIHelperClass.ScrollToElement(scrollViewer, stackPanelToUpdate);
        }

        private void PointerExitedStackPanel(object sender, PointerRoutedEventArgs e)
        {
            CustomStackPanel sP = sender as CustomStackPanel;
            sP.RemoveButton.Opacity = 0;
            sP.RemoveButton.IsEnabled = false;
        }

        private void PointerEnteredStackPanel(object sender, PointerRoutedEventArgs e)
        {
            CustomStackPanel sP = sender as CustomStackPanel;
            sP.RemoveButton.Opacity = 100;
            sP.RemoveButton.IsEnabled = true;
        }

        private void RemoveCurrencyStatsForCurrency(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            rightStackPanelRef.Children.Remove(GetChildOfStackPanel(rightStackPanelRef, button.Tag.ToString()));
        }

        internal double CalculateTotalInvestmentValue(ref StackPanel stackPanelRight)
        {
            double totalInvestmentValue = 0.0;
            foreach (CustomStackPanel child in stackPanelRight.Children)
                totalInvestmentValue += child.UserValue;

            return totalInvestmentValue;
        }

        private string UpdateUserUSDValueContentStr(ref CustomStackPanel sP, string priceStr, string contentStr)
        {
            // update user value
            sP.UserValue = addedAmount * Convert.ToDouble(priceStr);

            if (addedAmount == 0.0)
                contentStr += "N/A";
            else
                contentStr += "$" + sP.UserValue.ToString();

            return contentStr;
        }
    }
}
