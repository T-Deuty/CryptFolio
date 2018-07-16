using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptFolio
{
    public class MarketViewTemplate
    {
        public string currencyName { get; set; }
        public string currencyUSDPrice { get; set; }
        public string currencyBTCPrice { get; set; }
        public string currencyMarketCap { get; set; }
        public string currencyCirculatingSupply { get; set; }
        public string currencyTotalSupply { get; set; }
        public string currencyPercentChange1hr { get; set; }
        public string currencyPercentChange24hr { get; set; }
        public string currencyPercentChange1wk { get; set; }

        public MarketViewTemplate(
            string currencyName, 
            string currencyUSDPrice,
            string currencyBTCPrice,
            string currencyMarketCap,
            string currencyCirculatingSupply,
            string currencyTotalSupply,
            string currencyPercentChange1hr,
            string currencyPercentChange24hr,
            string currencyPercentChange1wk
            )
        {
            this.currencyName = currencyName;
            this.currencyUSDPrice = "USD Price: $" + currencyUSDPrice;
            this.currencyBTCPrice = "BTC Price: " + currencyBTCPrice;
            this.currencyMarketCap = "Market Cap: $" + currencyMarketCap;
            this.currencyCirculatingSupply = "Circulating supply: " + currencyCirculatingSupply;
            this.currencyTotalSupply = "Total supply: " + currencyTotalSupply;
            this.currencyPercentChange1hr = "Hour Percent Change: " + currencyPercentChange1hr + "%";
            this.currencyPercentChange24hr = "Day Percent Change: " + currencyPercentChange24hr + "%";
            this.currencyPercentChange1wk = "Week Percent Change: " + currencyPercentChange1wk + "%";
        }
    }

    public class MarketViewTemplates : ObservableCollection<MarketViewTemplate>
    {
        public MarketViewTemplates() : base()
        {
            foreach (TickerJSONResult result in App.jsonList)
            {
                Add(new MarketViewTemplate
                        (
                            result.name, 
                            result.price_usd,
                            result.price_btc,
                            result.market_cap_usd,
                            result.available_supply,
                            result.max_supply,
                            result.percent_change_1h,
                            result.percent_change_24h,
                            result.percent_change_7d
                        )
                    );
            }
        }
    }
}
