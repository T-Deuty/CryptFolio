using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptFolio
{
    public class CoinMarketCapAPI
    {
        protected Dictionary<string, string> nameDictionary;

        internal delegate AsyncOperationWithProgressCompletedHandler<Windows.Web.Http.HttpResponseMessage, HttpProgress> progressHandler();

        private const string url = "https://api.coinmarketcap.com/v1/ticker/";
        private HttpClient client;

        public CoinMarketCapAPI() { }

        public async Task<List<TickerJSONResult>> RequestAllAsync(Splash splashPage)
        {
            client = new HttpClient();
            // adding "?limit=1400" to pull info for 1400 coins from API
            string downloadStr = url + "?limit=1400";
            Uri uri = new Uri(downloadStr);
            List<TickerJSONResult> list = null;

            try
            {
                var downloadTask = client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                downloadTask.Progress = (result, progress) =>
                {
                    Console.WriteLine(progress.BytesReceived);
                    splashPage.HandleProgressAsync(progress);
                };
                HttpResponseMessage response = await downloadTask;

                response.EnsureSuccessStatusCode();

                var createStringAsyncTask = response.Content.ReadAsStringAsync();
                string jsonStr = await createStringAsyncTask;

                list = DeserializeJSON(jsonStr, "");

                // build dictionary out of the list
                BuildNameDictionary(list);
            }
            catch (Exception ex)    
            {
                string message = ex.Message;
            }

            client.Dispose();

            if (list != null && list.Count > 0)
                return list;
            else
                return null;
        }

        public async Task<TickerJSONResult> RequestTickerAsync(string ticker)
        {
            client = new HttpClient();
            string downloadStr = url + ticker + "/";
            Uri uri = new Uri(downloadStr);
            List<TickerJSONResult> list = null;

            try
            {
                var downloadTask = client.GetAsync(uri);
                HttpResponseMessage response = await downloadTask;

                response.EnsureSuccessStatusCode();

                var createStringAsyncTask = response.Content.ReadAsStringAsync();
                string jsonStr = await createStringAsyncTask;

                list = DeserializeJSON(jsonStr, ticker);

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            client.Dispose();

            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        public TickerJSONResult RetrieveJSONById(string id)
        {
            return App.jsonList.Find(x => x.id == id);
        }

        public List<TickerJSONResult> DeserializeJSON(string jsonStr, string id)
        {
            // store currency JSONs in this list
            List<TickerJSONResult> list = new List<TickerJSONResult>();

            JsonArray jsonArray;

            try
            {
                jsonArray = JsonValue.Parse(jsonStr).GetArray();
            }
            catch (Exception)
            {
                var returnList = new List<TickerJSONResult>();
                returnList.Add(RetrieveJSONById(id));
                return returnList;
            }

            string
                _id, _name, _symbol, _rank, _price_usd, _price_btc, __24h_volume_usd, _market_cap_usd, _available_supply, _total_supply, _max_supply, _percent_change_1h, _percent_change_24h, _percent_change_7d, _last_updated;

            for (uint i = 0; i < jsonArray.Count; i++)
            {
                try
                {
                    _id = jsonArray.GetObjectAt(i).GetNamedString("id");
                }
                catch (Exception)
                {
                    _id = "null";
                }
                try
                {
                    _name = jsonArray.GetObjectAt(i).GetNamedString("name");
                }
                catch (Exception)
                {
                    _name = "null";
                }

                try
                {
                    _symbol = jsonArray.GetObjectAt(i).GetNamedString("symbol");
                }
                catch (Exception)
                {
                    _symbol = "null";
                }

                try
                {
                    _rank = jsonArray.GetObjectAt(i).GetNamedString("rank");
                }
                catch (Exception)
                {
                    _rank = "null";
                }

                try
                {
                    _price_usd = jsonArray.GetObjectAt(i).GetNamedString("price_usd");
                }
                catch (Exception)
                {
                    _price_usd = "null";
                }

                try
                {
                    _price_btc = jsonArray.GetObjectAt(i).GetNamedString("price_btc");
                }
                catch (Exception)
                {
                    _price_btc = "null";
                }

                try
                {
                    __24h_volume_usd = jsonArray.GetObjectAt(i).GetNamedString("24h_volume_usd");
                }
                catch (Exception)
                {
                    __24h_volume_usd = "null";
                }

                try
                {
                    _market_cap_usd = jsonArray.GetObjectAt(i).GetNamedString("market_cap_usd");
                }
                catch (Exception)
                {
                    _market_cap_usd = "null";
                }

                try
                {
                    _available_supply = jsonArray.GetObjectAt(i).GetNamedString("available_supply");
                }
                catch (Exception)
                {
                    _available_supply = "null";
                }

                try
                {
                    _total_supply = jsonArray.GetObjectAt(i).GetNamedString("total_supply");
                }
                catch (Exception)
                {
                    _total_supply = "null";
                }

                try
                {
                    _max_supply = jsonArray.GetObjectAt(i).GetNamedString("max_supply");
                }
                catch (Exception)
                {
                    _max_supply = "null";
                }

                try
                {
                    _percent_change_1h = jsonArray.GetObjectAt(i).GetNamedString("percent_change_1h");
                }
                catch (Exception)
                {
                    _percent_change_1h = "null";
                }

                try
                {
                    _percent_change_24h = jsonArray.GetObjectAt(i).GetNamedString("percent_change_24h");
                }
                catch (Exception)
                {
                    _percent_change_24h = "null";
                }

                try
                {
                    _percent_change_7d = jsonArray.GetObjectAt(i).GetNamedString("percent_change_7d");
                }
                catch (Exception)
                {
                    _percent_change_7d = "null";
                }

                try
                {
                    _last_updated = jsonArray.GetObjectAt(i).GetNamedString("last_updated");
                }
                catch (Exception)
                {
                    _last_updated = "null";
                }

                var currency = new TickerJSONResult
                {
                    id = _id,
                    name = _name,
                    symbol = _symbol,
                    rank = _rank,
                    price_usd = _price_usd,
                    price_btc = _price_btc,
                    _24h_volume_usd = __24h_volume_usd,
                    market_cap_usd = _market_cap_usd,
                    available_supply = _available_supply,
                    total_supply = _total_supply,
                    max_supply = _max_supply,
                    percent_change_1h = _percent_change_1h,
                    percent_change_24h = _percent_change_24h,
                    percent_change_7d = _percent_change_7d,
                    last_updated = _last_updated
                };

                list.Add(currency);
            }

            return list;
        }

        
        public void BuildNameDictionary(List<TickerJSONResult> list)
        {
            var nameList = list.Select(item => item.name).ToList();
            var idList = list.Select(item => item.id).ToList();
            var symbolList = list.Select(item => item.symbol).ToList();

            // appends ticker for display
            for (int i = 0; i < idList.Count(); i++)
            {
                nameList[i] += " (" + symbolList[i] + ")";
            }

            this.nameDictionary = Enumerable.Range(0, nameList.Count).ToDictionary(i => nameList[i], i => idList[i]);
        }
        
        public Dictionary<string, string> GetNameDictionary()
        {
            return nameDictionary;
        }
    }
}
