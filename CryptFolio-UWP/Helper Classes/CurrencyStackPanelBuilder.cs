using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;

namespace CryptFolio
{
    public class CurrencyStackPanelBuilder
    {
        public CurrencyStackPanelBuilder()
        {
            // empty initializer
        }

        public Grid BuildGrid(TickerJSONResult result)
        {
            // create main containing stack panel
            var columnDefinition1 = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            var columnDefinition2 = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            var columnDefinition3 = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            var columnDefinition4 = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };

            Grid outsideGrid = new Grid
            {
                Margin = new Thickness(10),
                Padding = new Thickness(12)
            };

            outsideGrid.ColumnDefinitions.Add(columnDefinition1);
            outsideGrid.ColumnDefinitions.Add(columnDefinition2);
            outsideGrid.ColumnDefinitions.Add(columnDefinition3);
            outsideGrid.ColumnDefinitions.Add(columnDefinition4);

            // add name block
            var subPanel = BuildNamePanel(result.name, result.symbol, result.rank);
            outsideGrid.Children.Add(subPanel);
            Grid.SetColumn(subPanel, 0);

            // price and volume block
            string
                topBlockStr = "USD Price: $" + result.price_usd,
                middleBlockStr = "Volume (24h): $" + result._24h_volume_usd,
                bottomBlockStr = "BTC Price: " + result.price_btc;

            // add to panel
            subPanel = BuildRegularPanel(topBlockStr, middleBlockStr, bottomBlockStr);
            outsideGrid.Children.Add(subPanel);
            Grid.SetColumn(subPanel, 1);

            // supply and market cap block
            topBlockStr = "Market Cap: $" + result.market_cap_usd;
            middleBlockStr = "Circulating Supply: " + result.available_supply + " " + result.symbol;
            bottomBlockStr = "Total supply: " + result.total_supply + " " + result.symbol;

            // add to panel
            subPanel = BuildRegularPanel(topBlockStr, middleBlockStr, bottomBlockStr);
            outsideGrid.Children.Add(subPanel);
            Grid.SetColumn(subPanel, 2);

            // percent change block
            topBlockStr = "Percent Change (1hr): " + result.percent_change_1h + "%";
            middleBlockStr = "Percent Change (24hr): " + result.percent_change_24h + "%";
            bottomBlockStr = "Percent Change (7d): " + result.percent_change_7d + "%";

            // add to panel
            subPanel = BuildRegularPanel(topBlockStr, middleBlockStr, bottomBlockStr);
            outsideGrid.Children.Add(subPanel);
            Grid.SetColumn(subPanel, 3);

            AcrylicBrush brush = new AcrylicBrush
            {
                TintColor = Colors.DarkBlue,
                TintOpacity = 0.7,
                Opacity = 0.2
            };
            outsideGrid.Background = brush;
            outsideGrid.CornerRadius = new CornerRadius(10);

            outsideGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            outsideGrid.MaxWidth = 1400;

            return outsideGrid;
        }

        private StackPanel BuildNamePanel(string name, string symbol, string rank)
        {
            // create name block
            TextBlock nameBlock = new TextBlock
            {
                Text = name + " (" + symbol + ")",
                FontWeight = FontWeights.ExtraBold,
                FontSize = 22
            };

            // create rank textblock
            TextBlock rankBlock = new TextBlock
            {
                Text = "Rank: " + rank,
                FontSize = 18
            };

            // add them to their own stackpanel
            StackPanel namePanel = new StackPanel();
            namePanel.Children.Add(nameBlock);
            namePanel.Children.Add(rankBlock);

            return namePanel;
        }

        private StackPanel BuildRegularPanel(string top, string middle, string bottom)
        {
            // create top block
            TextBlock topBlock = new TextBlock
            {
                Text = top,
                FontSize = 18
            };

            // create middle textblock
            TextBlock middleBlock = new TextBlock
            {
                Text = middle,
                FontSize = 18
            };

            // create bottom textblock
            TextBlock bottomBlock = new TextBlock
            {
                Text = bottom,
                FontSize = 18
            };

            // add them to their own stackpanel
            StackPanel regularPanel = new StackPanel
            {
                Margin = new Thickness(10, 0, 10, 0)
            };

            regularPanel.Children.Add(topBlock);
            regularPanel.Children.Add(middleBlock);
            regularPanel.Children.Add(bottomBlock);

            return regularPanel;
        }
    }
}
