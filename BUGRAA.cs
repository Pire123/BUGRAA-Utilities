using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using xNetStandart;
using Newtonsoft.Json;

namespace BUGRAA_Utilities
{

    #region Update [BROKEN]
    [Obsolete("BROKEN",true)]
    public class Update
    {
        public Update()
        {
            Resolver();
            Check();
        }

        private void Resolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }

        public static string version = "1";

        public static string name = "BUGRAA Utilities.dll";

        public static bool version_checked = false;

        public static bool need_update = false;

        public bool Check()
        {
            if (!version_checked)
            {
                version_checked = true;
                Resolver();
                using (var request = new HttpRequest())
                {
                    try
                    {
                        request.UserAgent = Http.ChromeUserAgent();
                        HttpResponse response = request.Get("https://pastebinp.com/raw/TbHGeCdA");

                        string[] lines = Encoding.UTF8.GetString(response.ToBytes()).Trim().Split();

                        if (lines.Length > 0)
                        {
                            if (lines[0] != null && lines[0] != version)
                            {
                                need_update = true;
                                version_checked = true;
                                return true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        version_checked = false;
                        Console.WriteLine(e);
                        return false;
                    }
                }
            }
            return false;
        }

        public byte[] Download(string link = "https://pastebinp.com/raw/TbHGeCdA")
        {
            Resolver();
            byte[] vs = null;

            string webadress = link;
            using (var request = new HttpRequest())
            {
                try
                {
                    request.UserAgent = Http.ChromeUserAgent();
                    HttpResponse response = request.Get(webadress);

                    string[] lines = Encoding.UTF8.GetString(response.ToBytes()).Trim().Split();

                    if (lines.Length > 0)
                    {
                        if (lines[0] != null && lines[0] != version)
                        {
                            string n = lines[1];
                            HttpResponse lin = request.Get(lines[2].Trim().ToString());
                            byte[] bytes = lin.ToBytes();
                            return bytes;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return vs;
        }
    }


    #endregion

    #region Core

    public class BUGRAA : Globals
    { 
        //self
        private decimal PlayerId { get; set; }
        private string ROBLOSECURTY { get; set; }
        private string XCRSF_Token { get; set; }

        public Player PLAYER = null;

        public BUGRAA()
        {
            Resolver();
        }

        public BUGRAA(Player player)
        {
            Resolver();
            player.XCRSF_Token = GetXCRSFToken(player.ROBLOXSECURITY).GetAwaiter().GetResult();
            PLAYER = player;
        }

        private protected void Resolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }

        private protected byte[] ExtractResource(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        public async Task Init()
        {
            var config = await GetConfig();

            ROBLOSECURTY = config.ROBLOSECURITY;
            PlayerId = config.UserId;

            XCRSF_Token = await GetXCRSFToken(ROBLOSECURTY);

            BlockedPlayerIds.Clear();

            BlockedPlayerIds = new List<decimal>() { 1 };

            BlockedPlayerIds.Add(PlayerId);
        }

        public async Task ManualInit(string robloxsecurity, decimal player_id, bool use_blocked_ids = false,string blocked_list_file_name = "Blocked Players.txt")
        {
            if (robloxsecurity != null || robloxsecurity != string.Empty)
            {
                ROBLOSECURTY = robloxsecurity;
                XCRSF_Token = await GetXCRSFToken(robloxsecurity);
            }
            if (player_id > 0)
            {
                PlayerId = player_id;
            }

            BlockedPlayerIds.Clear();

            BlockedPlayerIds = new List<decimal>() { 1 };

            if (player_id > 0)
            {
                BlockedPlayerIds.Add(player_id);
            }

            if (use_blocked_ids)
            {
                if (File.Exists(blocked_list_file_name))
                {
                    try
                    {
                        foreach (string x in File.ReadAllLines(blocked_list_file_name))
                        {
                            if (x.Trim() != null && x.Trim() != string.Empty && x.Trim() != "")
                            {
                                try
                                {
                                    BlockedPlayerIds.Add(decimal.Parse(x));
                                }
                                catch { }
                            }
                        }
                    }
                    catch { };
                }
            }   
            await Task.Delay(0);
        }

        private async Task CreateProxy()
        {
            await Task.Delay(0);
            string name = "Proxy_Http.txt";
            try
            {
                if (File.Exists(name))
                {
                    if (File.GetCreationTime(name).Day != DateTime.Now.Day)
                    {
                        using (FileStream fs = File.Create(name))
                        {
                            using (HttpRequest request = new HttpRequest())
                            {
                                BuildRequest(request);
                                HttpResponse response = request.Get("https://api.proxyscrape.com/?request=getproxies&proxytype=http&timeout=5000&ssl=yes");
                                byte[] vs = response.ToBytes();
                                string clone = Encoding.UTF8.GetString(vs).Clone().ToString();
                                fs.Write(vs, 0, vs.Length);
                            }
                        }
                    }
                }
                else
                {
                    using (FileStream fs = File.Create(name))
                    {
                        using (HttpRequest request = new HttpRequest())
                        {
                            BuildRequest(request);
                            HttpResponse response = request.Get("https://api.proxyscrape.com/?request=getproxies&proxytype=http&timeout=5000&ssl=yes");
                            byte[] vs = response.ToBytes();
                            string clone = Encoding.UTF8.GetString(vs).Clone().ToString();
                            fs.Write(vs, 0, vs.Length);
                        }
                    }
                }
                string[] x = File.ReadAllLines(name);
            }
            catch { };
        }

        private String WriteError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error Occurred! Send error message to dev");
            Console.WriteLine();
            Console.WriteLine(e.Message);
            Console.WriteLine();
            Console.WriteLine(e);
            Console.ResetColor();
            return "ok";
        }

        private HttpRequest BuildRequest(HttpRequest request)
        {
            try
            {
                string xd = Http.ChromeUserAgent();

                switch (new Random().Next(0, 4))
                {
                    case 0:
                        xd = Http.ChromeUserAgent();
                        break;
                    case 1:
                        xd = Http.FirefoxUserAgent();
                        break;
                    case 2:
                        xd = Http.OperaUserAgent();
                        break;
                    case 3:
                        xd = Http.OperaMiniUserAgent();
                        break;
                    case 4:
                        xd = Http.IEUserAgent();
                        break;
                    default:
                        xd = Http.ChromeUserAgent();
                        break;
                }

                request.UserAgent = xd;
                request.CharacterSet = Encoding.UTF8;
                request.EnableEncodingContent = true;
                request.IgnoreProtocolErrors = true;
                request.AllowAutoRedirect = true;
                request.KeepAlive = true;
                request.Username = "31";
                request.Password = "31";
            }
            catch { }
            return request;
        }

        private HttpRequest PseudoHumanRequest(HttpRequest request, CookieDictionary cookies = null, Dictionary<string, string> headers = null)
        {
            try
            {

                if (headers != null)
                {
                    foreach (var x in headers)
                    {
                        request.AddHeader(x.Key, x.Value);
                    }
                }
                else
                {
                    request.AddHeader("X-CSRF-TOKEN", XCRSF_Token);
                }
                if (cookies != null)
                {
                    request.Cookies = cookies;
                }
                else
                {
                    request.Cookies = new CookieDictionary
                    {
                        { ".ROBLOSECURITY" , ROBLOSECURTY }
                    };
                }
            }
            catch { }
            return request;
        }

        public String BetweenStrings(String text, String start, String end)
        {
            int p1 = text.IndexOf(start) + start.Length;
            int p2 = text.IndexOf(end, p1);

            if (end == "") return (text.Substring(p1));
            else return text.Substring(p1, p2 - p1);
        }

        public String ToUTF8(byte[] x)
        {
            try
            {
                return Encoding.UTF8.GetString(x);
            }
            catch { return null; }
        }

        public void print(object text = null, bool line = true, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            if (line)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
        }

        public async Task<LimitedItemOwnersJSON.Root> GetLimitedItemOwners(decimal id, string next_cursor = null)
        {
            await Task.Delay(0);
            string url = "https://inventory.roblox.com/v2/assets/{0}/owners?limit=100&Cursor={1}";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = request.Get(string.Format(url, id.ToString(), next_cursor));
                    string raw = Encoding.UTF8.GetString(response.ToBytes());
                    LimitedItemOwnersJSON.Root limited_owners = JsonConvert.DeserializeObject<LimitedItemOwnersJSON.Root>(raw);
                    return limited_owners;
                }
            }
            catch (Exception e) { Console.WriteLine(e); return null; }
        }

        public async Task<ConfigJSON.Root> GetConfig()
        {
            await Task.Delay(0);

            if (!File.Exists("Config.json"))
            {
                File.WriteAllBytes("Config.json", ExtractResource("Config.json"));
            }

            try
            {
                ConfigJSON.Root x = JsonConvert.DeserializeObject<ConfigJSON.Root>(File.ReadAllText("Config.json", Encoding.UTF8));
                return x;
            }
            catch { return null; }
        }

        public async Task<AssetInfoJSON.Root> GetAssetInfo(decimal assetid)
        {
            await Task.Delay(0);
            string url = "https://api.roblox.com/Marketplace/ProductInfo?assetId={0}";
            using (var request = new HttpRequest())
            {
                BuildRequest(request);
                try
                {
                    string info = Encoding.UTF8.GetString(request.Get(string.Format(url, assetid)).ToBytes());
                    AssetInfoJSON.Root asset = JsonConvert.DeserializeObject<AssetInfoJSON.Root>(info);
                    return asset;
                }
                catch (Exception e)
                {
                    WriteError(e);
                    return null;
                }
            }
        }

        public async Task<PlayerInvJSON.Root> GetPlayerInventory(decimal player_id, string rbxsecurity = null, string XcrsfToken = null)
        {
            await Task.Delay(0);
            string sending = "https://inventory.roblox.com/v1/users/{0}/assets/collectibles?limit=100";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    if (XcrsfToken != null)
                    {
                        request.AddHeader("X-CSRF-TOKEN", XcrsfToken);
                    }
                    if (rbxsecurity != null)
                    {
                        request.Cookies = new CookieDictionary
                            {
                                { ".ROBLOSECURITY" , rbxsecurity }
                            };
                    }

                    HttpResponse response = request.Get(string.Format(sending, player_id));

                    string raw = Encoding.UTF8.GetString(response.ToBytes());

                    PlayerInvJSON.Root root = JsonConvert.DeserializeObject<PlayerInvJSON.Root>(raw);
                    return root;
                }
            }
            catch (Exception e) { print(e); return null; }
        }

        public async Task<SellersInfoJSON.Root> GetReSellersInfo(decimal assetid, string next_page = null, bool use_proxy = false)
        {
            await Task.Delay(0);
            string url = "https://economy.roblox.com/v1/assets/{0}/resellers?limit=100&Cursor={1}";

            using (var request = new HttpRequest())
            {
                try
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = null;

                    response = request.Get(string.Format(url, assetid.ToString(), next_page));

                    SellersInfoJSON.Root root = JsonConvert.DeserializeObject<SellersInfoJSON.Root>(Encoding.UTF8.GetString(response.ToBytes()));
                    return root;
                }
                catch (Exception e)
                {
                    WriteError(e);
                    return null;
                }
            }
        }

        public async Task<LimitedItemInfoJSON.Root> GetLimitedItemSalesData(decimal assetid, bool use_proxy = false)
        {
            await Task.Delay(0);
            string api = "https://economy.roblox.com/v1/assets/{0}/resale-data";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = request.Get(string.Format(api, assetid.ToString()));

                    string raw = ToUTF8(response.ToBytes());

                    LimitedItemInfoJSON.Root root = JsonConvert.DeserializeObject<LimitedItemInfoJSON.Root>(raw);
                    return root;
                }
            }
            catch (Exception e) { print(e); return null; }
        }

        public async Task<PlayerCollectiableItemsJSON.Root> GetPlayerCollectiables(decimal player_id)
        {
            await Task.Delay(0);
            string url = "https://www.roblox.com/users/profile/robloxcollections-json?userId={0}";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);

                    HttpResponse response = request.Get(string.Format(url, player_id.ToString()));

                    PlayerCollectiableItemsJSON.Root root = JsonConvert.DeserializeObject<PlayerCollectiableItemsJSON.Root>(ToUTF8(response.ToBytes()));
                    return root;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<OutboundPageJson.Root> GetOutboundRequests(string next_page = "")
        {
            await Task.Delay(0);
            string api = "https://trades.roblox.com/v1/trades/outbound?limit=100&sortOrder=Desc&Cursor=";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = request.Get(api + next_page);

                    var bounds = JsonConvert.DeserializeObject<OutboundPageJson.Root>(ToUTF8(response.ToBytes()));

                    return bounds;
                }
            }
            catch { return null; }
        }

        public async Task<InboundPageJson.Root> GetInboundRequests(string next_page = "")
        {
            await Task.Delay(0);
            string api = "https://trades.roblox.com/v1/trades/inbound?limit=100&sortOrder=Desc&Cursor=";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    var response = request.Get(api + next_page);

                    var page = JsonConvert.DeserializeObject<InboundPageJson.Root>(ToUTF8(response.ToBytes()));
                    return page;
                }
            }
            catch { return null; }
        }

        public async Task<TradeInfoJson.Root> GetTradeInfo(decimal trade_id)
        {
            string api = "https://trades.roblox.com/v1/trades/{0}";
            await Task.Delay(0);
            try
            {
                if (trade_id < 1)
                {
                    return null;
                }
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request); // showing us like chrome user xd
                    PseudoHumanRequest(request); // autoh.

                    HttpResponse response = request.Get(string.Format(api, trade_id.ToString()));

                    string raw = ToUTF8(response.ToBytes());

                    TradeInfoJson.Root root = JsonConvert.DeserializeObject<TradeInfoJson.Root>(raw);
                    return root;
                }
            }
            catch { return null; }
        }

        public async Task<UserInfoJson> GetUserInfo(decimal player_id)
        {
            await Task.Delay(0);
            string api = "https://users.roblox.com/v1/users/{0}";
            try
            {
                using (var request = new HttpRequest())
                {
                    BuildRequest(request);

                    HttpResponse response = request.Get(string.Format(api, player_id.ToString()));

                    UserInfoJson user = JsonConvert.DeserializeObject<UserInfoJson>(ToUTF8(response.ToBytes()));
                    return user;
                }
            }
            catch { return null; }
        } 

        public async Task<List<ItemDictionary>> UAIDParser(PlayerInvJSON.Root inv, List<decimal> ItemsToSend)
        {
            try
            {
                List<ItemDictionary> Items = new List<ItemDictionary>();

                if (inv == null)
                {
                    inv = await GetPlayerInventory(PlayerId, ROBLOSECURTY, XCRSF_Token);
                }

                #region ItemUserAssetId Parser
                foreach (var x in ItemsToSend)
                {
                    foreach (var data in inv.data)
                    {
                        if (data != null)
                        {
                            bool go = false;
                            if (x == data.assetId)
                            {
                                go = true;
                            }
                            if (go)
                            {
                                bool ok = true;
                                if (ok)
                                {      
                                    bool nope = true;
                                    foreach (var y in Items)
                                    {
                                        if (y.UserAssetId == data.userAssetId)
                                        {
                                            nope = false;
                                            break;
                                        }
                                    }
                                    if (nope)
                                    {
                                        ItemDictionary dictionary = new ItemDictionary();
                                        dictionary.AssetId = data.assetId;
                                        dictionary.UserAssetId = data.userAssetId;
                                        dictionary.Name = data.name;
                                        Items.Add(dictionary);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return Items;
                #endregion
            }
            catch { return null; }
        }

        public async Task<string> GetXCRSFToken(string rbxsecurity)
        {
            await Task.Delay(0);
            using (var request = new HttpRequest())
            {
                BuildRequest(request);
                request.Cookies = new CookieDictionary
                    {
                        { ".ROBLOSECURITY" , rbxsecurity }
                    };

                byte[] data = request.Get("https://www.roblox.com/home").ToBytes();

                string newdata = Encoding.UTF8.GetString(data);

                string ass = null;
                try
                {
                    ass = BetweenStrings(newdata, "Roblox.XsrfToken.setToken('", "')");
                    return ass;
                }
                catch
                {
                    try
                    {
                        ass = BetweenStrings(newdata, "Roblox.XsrfToken.setToken(\"", "\")");
                        return ass;
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("XCSRF TOKEN FAILED TO GET !");
                        Console.ResetColor();
                        return null;
                    }
                }
            }
        }

        public async Task<string> GetVerificationToken(string rbxsecurity, string XcrsfToken)
        {
            await Task.Delay(0);
            try
            {
                using (var request = new HttpRequest())
                {
                    request.UserAgent = Http.ChromeUserAgent();
                    request.AllowAutoRedirect = true;

                    request.AddHeader("X-CSRF-TOKEN", XcrsfToken);
                    request.Cookies = new CookieDictionary
                            {
                                { ".ROBLOSECURITY" , rbxsecurity }
                            };

                    var res = request.Get("https://www.roblox.com/build/upload");

                    var token = "";

                    if (res.Cookies.ToString().Contains("__RequestVerificationToken"))
                    {
                        token = BetweenStrings(res.Cookies.ToString(), "__RequestVerificationToken=", "");
                        if (token.Trim() == string.Empty)
                        {
                            token = BetweenStrings(res.Cookies.ToString(), "__RequestVerificationToken=", ";");
                        }
                        return token;
                    }
                }
            }
            catch { return ""; }
            return "";
        }

        public async Task<SendTradeResponseDictionary> SendTrade(TradingDictionary trader_dictionary, TradingDictionary your_dictonary)
        {
            await Task.Delay(0);
        il:
            try
            {
                using (var request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    #region trader
                    string trader = trader_dictionary.Id.ToString(); // trader id
                    string items_want = "[";
                    for (int i = 0; i < trader_dictionary.Items.Count; i++)
                    {
                        if (i < 4)
                        {
                            decimal x = trader_dictionary.Items[i];
                            items_want = items_want + x + ",";
                        }
                    }
                    items_want = items_want.Remove(items_want.Length - 1); // removing last ","
                    items_want = items_want + "]";
                    string rbx_want = trader_dictionary.Robux.ToString(); // how much you want
                    #endregion

                    #region self
                    string your = your_dictonary.Id.ToString();
                    string items_give = "[";
                    for (int i = 0; i < your_dictonary.Items.Count; i++)
                    {
                        if (i < 4)
                        {
                            decimal x = your_dictonary.Items[i];
                            items_give = items_give + x + ",";
                        }
                    }
                    items_give = items_give.Remove(items_give.Length - 1); // removing last ","
                    items_give = items_give + "]";
                    string rbx_give = your_dictonary.Robux.ToString();
                    #endregion

                    string json = "{\"offers\": [{\"userId\": " + trader + ",\"userAssetIds\": " + items_want + ",\"robux\": " + rbx_want + "}, {\"userId\": " + your + ",\"userAssetIds\": " + items_give + ",\"robux\": " + rbx_give + "}]}";

                    byte[] bytes = Encoding.UTF8.GetBytes(json);

                    string content_type = "application/json; charset=UTF-8";

                    HttpResponse response = request.Post("https://trades.roblox.com/v1/trades/send", bytes, content_type);

                    string clone = response.ToString().Clone().ToString();

                    print(clone.Clone().ToString());

                    print(response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        print("fixing token problem...", true, ConsoleColor.Magenta);
                        XCRSF_Token = await GetXCRSFToken(ROBLOSECURTY);
                        print("fixed.", true, ConsoleColor.Green);
                        goto il;
                    }

                    if (response.StatusCode == xNetStandart.HttpStatusCode.OK)
                    {
                        string raw = clone.Clone().ToString();
                        string data = BetweenStrings(raw, "id\":", "}");
                        decimal xd = 0;
                        try
                        {
                            decimal.TryParse(data.Trim().ToString(), out xd);
                        }
                        catch
                        {
                            try
                            {
                                xd = Convert.ToDecimal(data.Trim().ToString());
                            }
                            catch { };
                        }
                        SendTradeResponseDictionary dictionary = new SendTradeResponseDictionary();
                        dictionary.trade_id = xd;
                        dictionary.success = true;
                        dictionary.note = "success trade";
                        dictionary.HttpStatusCode = response.StatusCode;
                        return dictionary;
                    }
                    else
                    {
                        SendTradeResponseDictionary dictionary = new SendTradeResponseDictionary();
                        dictionary.trade_id = 0;
                        dictionary.success = false;
                        dictionary.note = "status code changed";
                        dictionary.HttpStatusCode = response.StatusCode;
                        return dictionary;
                    }
                }
            }
            catch (Exception e)
            {
                print(e, true, ConsoleColor.Cyan);
                SendTradeResponseDictionary dictionary = new SendTradeResponseDictionary();
                dictionary.trade_id = 0;
                dictionary.success = false;
                dictionary.note = e.ToString();
                dictionary.HttpStatusCode = HttpStatusCode.BadRequest;
                return dictionary;
            }

        }

        public async Task<bool> DeclineTrade(decimal trade_id)
        {
            await Task.Delay(0);
            string api = "https://trades.roblox.com/v1/trades/{0}/decline";
            try
            {
                if (trade_id < 1)
                {
                    return false;
                }
                using (var request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse httpResponse = request.Post(string.Format(api, trade_id.ToString()));

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception e) { print(e); return false; }
        }

        public async Task<ClearOutboundClass> ClearOutbound(bool debug_mode = false)
        {
            await Task.Delay(0);

            ClearOutboundClass outboundClass = new ClearOutboundClass(this,debug_mode);
            outboundClass.Loop();
            outboundClass.debug_mode = debug_mode;

            return outboundClass;
        }

        public async Task<bool> FollowPlayer(decimal p_id, bool follow = true)
        {
            await Task.Delay(0);
            string fa = "https://friends.roblox.com/v1/users/{0}/follow";
            string unfa = "https://friends.roblox.com/v1/users/{0}/unfollow";

            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);
                    if (follow)
                    {
                        var res = request.Post(string.Format(fa, p_id.ToString()));
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (!follow)
                    {
                        var res = request.Post(string.Format(unfa, p_id.ToString()));
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch { return false; }

        }

        public async Task<bool> FriendRequest(decimal player_id, bool friend = true)
        {
            await Task.Delay(0);
            string unfriend_ = "https://friends.roblox.com/v1/users/{0}/unfriend";
            string friend_ = "https://friends.roblox.com/v1/users/{0}/request-friendship";
            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);
                    if (friend)
                    {
                        HttpResponse response = request.Post(string.Format(friend_, player_id.ToString()));
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        HttpResponse response = request.Post(string.Format(unfriend_, player_id.ToString()));
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch { return false; }
        }

        public async Task<bool> CheckPremium(decimal id)
        {
            await Task.Delay(0);
            string sending = "https://premiumfeatures.roblox.com/v1/users/{0}/validate-membership";
            if (id <= 0)
            {
                return false;
            }
            using (var request = new HttpRequest())
            {
                try
                {

                    foreach(var x in PremiumPlayers.ToArray())
                    {
                        if (x == id)
                        {
                            return true;
                        }
                    }

                    foreach(var x in NonPremiumPlayers.ToArray())
                    {
                        if (x == id)
                        {
                            return false;
                        }
                    }

                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = request.Get(string.Format(sending, id));
                    string raw = Encoding.UTF8.GetString(response.ToBytes());
                    if (raw.Contains("true"))
                    {
                        PremiumPlayers.Add(id);
                        return true;
                    }
                    else
                    {
                        NonPremiumPlayers.Add(id);
                        return false;
                    }

                }
                catch 
                {
                    return false;
                }
            }
        }

        public async Task<bool> CheckProjected(decimal assetid)
        {
            await Task.Delay(0);

            try
            {
                using (var request = new HttpRequest())
                {
                    BuildRequest(request);

                    bool projected = false;
                    bool set = true;
                    #region Checking Item to know Projection
                    if (true)
                    {
                        foreach (var x in Projected)
                        {
                            if (x == assetid)
                            {
                                projected = true;
                                set = false;
                                print(assetid + " knew proj", true, ConsoleColor.Red);
                                return true;
                            }
                        }
                        if (!set)
                        {
                            foreach (var x in NotProjected)
                            {
                                if (x == assetid)
                                {
                                    projected = false;
                                    set = false;
                                    print(assetid + " knew not proj", true, ConsoleColor.Red);
                                    return false;
                                }
                            }
                        }
                    }
                    #endregion

                    if (!set)
                    {
                        return projected;
                    }
                    if (set)
                    {
                        HttpResponse response = request.Get(string.Format("https://www.rolimons.com/item/{0}", assetid.ToString()));

                        string data = Encoding.UTF8.GetString(response.ToBytes());

                        if (data.Contains("</g></svg> Projected</span></span>"))
                        {
                            Projected.Add(assetid);
                            print(assetid + " added to proj list", true, ConsoleColor.Red);
                            return true;
                        }
                        else
                        {
                            NotProjected.Add(assetid);
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;

        }

        public async Task<SendTradeResponseDictionary> CheckCanTrade(decimal player_id)
        {
            await Task.Delay(0);
            if (player_id < 1)
            {
                return null;
            }
        il:
            string api = "https://trades.roblox.com/v1/users/{0}/can-trade-with";
            try
            {

                foreach (var x in CanTradeWith.ToArray())
                {
                    if (x == player_id)
                    {
                        return new SendTradeResponseDictionary() { success = true, HttpStatusCode = HttpStatusCode.OK, note = "OK" };
                    }
                }

                foreach (var x in CantTradeWith.ToArray())
                {
                    if (x == player_id)
                    {
                        return new SendTradeResponseDictionary() { success = false, HttpStatusCode = HttpStatusCode.BadRequest, note = "nope" };
                    }
                }

                using (var request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = request.Get(string.Format(api, player_id.ToString()));


                    string raw = ToUTF8(response.ToBytes()).Clone().ToString();

                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        XCRSF_Token = await GetXCRSFToken(ROBLOSECURTY);
                        goto il;
                    }

                    print(raw);

                    if (raw.Contains("\"canTrade\":true"))
                    {
                        CanTradeWith.Add(player_id);
                        return new SendTradeResponseDictionary() { success = true, HttpStatusCode = response.StatusCode, note = "OK" };
                    }
                    else if (raw.Contains("\"canTrade\":false"))
                    {
                        CantTradeWith.Add(player_id);
                        return new SendTradeResponseDictionary() { success = false, HttpStatusCode = response.StatusCode, note = "nope" };
                    }
                    else if (raw.ToLower().Contains("toomanyreq"))
                    {
                        return new SendTradeResponseDictionary() { success = true, HttpStatusCode = response.StatusCode, note = "too many req" };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsset(decimal assetid)
        {
            await Task.Delay(0);
            string DeleteAssetUrl = "https://www.roblox.com/asset/delete-from-inventory";
            try
            {
                Dictionary<string, string> keys = new Dictionary<string, string>
                {
                    { "assetId", assetid.ToString() }
                };
                HttpContent pairs = new FormUrlEncodedContent(keys);
                using (var request = new HttpRequest())
                {
                    try
                    {
                        BuildRequest(request);
                        PseudoHumanRequest(request);

                        HttpResponse response = request.Post(DeleteAssetUrl, pairs);
                        byte[] x = response.ToBytes();
                        string data = Encoding.UTF8.GetString(x);
                        request.Dispose();
                        if (data.Contains("isValid\":true"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch { }
                }
            }
            catch { return false; }
            return false;
        }

        public async Task<bool> PurchaseAsset(decimal assetid)
        {
            await Task.Delay(0);
            string PurchaseAssetUrl = "https://economy.roblox.com/v1/purchases/products/{0}";
            var info = await GetAssetInfo(assetid);
            Dictionary<string, string> keys = new Dictionary<string, string>
            {
                {"expectedCurrency", "1"},
                {"expectedPrice", (info.PriceInRobux ?? 0 ).ToString() },
                {"expectedSellerId", info.Creator.CreatorTargetId.ToString() }
            };
            HttpContent pairs = new FormUrlEncodedContent(keys);
            try
            {
                using (var request = new HttpRequest())
                {
                    BuildRequest(request);
                    PseudoHumanRequest(request);

                    HttpResponse response = request.Post(string.Format(PurchaseAssetUrl, info.ProductId.ToString()), pairs);
                    byte[] x = response.ToBytes();
                    string data = Encoding.UTF8.GetString(x);
                    if (data.Contains("AlreadyOwned"))
                    {
                        return true;
                    }
                    else if (data.Contains("purchased\":true"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch { return false; }
        }

        public async Task<bool> PurchaseLimitedItem(PurchaseLimitedItemDictionary dictionary)
        {
            await Task.Delay(0);
            string url = "https://economy.roblox.com/v1/purchases/products/{0}";
            bool status = false;

            try
            {
                using (HttpRequest request = new HttpRequest())
                {
                    var info = await GetAssetInfo(Convert.ToDecimal(dictionary.assetid));
                    var token = await GetVerificationToken(ROBLOSECURTY,XCRSF_Token);
                    BuildRequest(request);

                    Dictionary<string, string> keys = new Dictionary<string, string>
                    {
                        {"expectedCurrency", "1"},
                        {"expectedPrice", dictionary.exceptedprice.ToString() },
                        {"expectedSellerId", dictionary.exceptedsellerid.ToString() },
                        {"userAssetID" , dictionary.userassetid.ToString() }
                    };

                    HttpContent content = new FormUrlEncodedContent(keys);

                    var cookies = new CookieDictionary {
                        { ".ROBLOSECURITY", ROBLOSECURTY },
                        { "__RequestVerificationToken", token }
                    };

                    var dic = new Dictionary<string, string>() {
                        {"X-CSRF-TOKEN", XCRSF_Token.ToString() }
                    };

                    PseudoHumanRequest(request, cookies , dic);

                    HttpResponse response = request.Post(string.Format(url.Clone().ToString(), info.ProductId.ToString()), content);

                    string data = Encoding.UTF8.GetString(response.ToBytes());

                    List<string> lists = new List<string>();
                    lists.Add("Price: " + dictionary.exceptedprice.ToString());
                    lists.Add("Time: " + DateTime.Now.ToString());
                    lists.Add("Response: " + data);

                    if (data.Contains("\"purchased\":true"))
                    {
                        status = true;
                        await Task.Delay(500);
                    }

                    if (data.Contains("\"purchased\":false"))
                    {
                        Console.WriteLine("limited purchase failed : " + BetweenStrings(data, "\"reason\":\"", "\","));
                        await Task.Delay(500);
                        status = false;
                    }

                    if (data.Contains("message\":\"TooManyRequests\""))
                    {
                        status = false;
                    }
                }
            }
            catch (Exception e) { WriteError(e); return false; }
            return status;
        }

        public class ClearOutboundClass
        {
            private bool set = false;
            private bool break_loop = false;
            public bool debug_mode = true;
            public int count = 0;
            public BUGRAA player = null;

            public ClearOutboundClass(BUGRAA player1, bool debug = false)
            {
                player = player1;
                debug_mode = debug;
            }

            public void Loop()
            {
                if (!set)
                {
                    set = true;
                    var methods = player;
                    string nex_ = "";
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    ix:
                    try
                    {
                        var x = methods.GetOutboundRequests(nex_).GetAwaiter().GetResult();
                        if (x.data != null)
                        {
                            Parallel.For(0, x.data.Count, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, async (i, st) =>
                            {
                                await Task.Delay(0);
                                #region break loop 
                                if (break_loop)
                                    {
                                        st.Break();
                                        return;
                                    }
                                #endregion
                                var item = x.data[i];
                                bool success = methods.DeclineTrade(item.id).GetAwaiter().GetResult();
                                if (success )
                                {
                                    if (debug_mode)
                                    {
                                        methods.print("haha xd declined", true, ConsoleColor.Green);
                                    }
                                    count++;
                                }
                            });
                        }
                        else
                        {
                            if (debug_mode)
                            {
                                methods.print("data null ");
                            }
                            set = false;
                            return;
                        }

                        if (x.nextPageCursor != null && x.nextPageCursor != string.Empty && !break_loop)
                        {
                            nex_ = x.nextPageCursor;
                            goto ix;
                        }

                        stopwatch.Stop();
                        if (debug_mode)
                        {
                            methods.print($"outbound cleared in {stopwatch.Elapsed}", true, ConsoleColor.Green);
                        }
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            public void Break()
            {
                try
                {
                    break_loop = true;
                    player = null;
                    GC.Collect();
                }
                catch { };
            }
        }
    }

    #endregion

    #region Globals

    public class Globals
    {
        public static List<decimal> Projected = new List<decimal>();

        public static List<decimal> NotProjected = new List<decimal>();

        public static List<decimal> CanTradeWith = new List<decimal>();

        public static List<decimal> CantTradeWith = new List<decimal>();

        public static List<decimal> BlockedPlayerIds = new List<decimal>() { 1 };

        public static List<decimal> PremiumPlayers = new List<decimal>();

        public static List<decimal> NonPremiumPlayers = new List<decimal>();
    }

    #endregion

    #region Player

    public class Player
    {
        public decimal PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string ROBLOXSECURITY { get; set; }
        public string XCRSF_Token { get; set; }

        private BUGRAA PlayerFunctions_ = null;

        public Player(string ROBLOSECURITY = null,decimal PLAYERID = 0)
        {
            int i = 0;
            if (ROBLOSECURITY != null && ROBLOSECURITY.Length > 15)
            {
                this.ROBLOXSECURITY = ROBLOXSECURITY;
                i++;
            }
            if (PLAYERID > 0)
            {
                this.PlayerId = PLAYERID;
                i++;
            }
            if (i == 2)
            {
                PlayerFunctions();
            }
        }

        public BUGRAA PlayerFunctions()
        {
            if (PlayerFunctions_ != null)
            {
                return PlayerFunctions_;
            }
            else
            {
                this.PlayerFunctions_ = new BUGRAA(this);
                this.PlayerFunctions_.ManualInit(this.ROBLOXSECURITY, this.PlayerId).Wait();
                this.PlayerName = ((this.PlayerFunctions_).GetUserInfo(this.PlayerId).GetAwaiter().GetResult()).name;
                return this.PlayerFunctions_;
            }
        }
    } 

    #endregion

    #region Json Klasse
    public class AssetInfoJSON
    {
        public class Creator
        {
            public decimal Id { get; set; }
            public string Name { get; set; }
            public string CreatorType { get; set; }
            public decimal CreatorTargetId { get; set; }
        }
        public class Root
        {
            public long TargetId { get; set; }
            public string ProductType { get; set; }
            public long AssetId { get; set; }
            public decimal ProductId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal AssetTypeId { get; set; }
            public Creator Creator { get; set; }
            public decimal IconImageAssetId { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
            public object PriceInRobux { get; set; }
            public object PriceInTickets { get; set; }
            public decimal Sales { get; set; }
            public bool IsNew { get; set; }
            public bool IsForSale { get; set; }
            public bool IsPublicDomain { get; set; }
            public bool IsLimited { get; set; }
            public bool IsLimitedUnique { get; set; }
            public object Remaining { get; set; }
            public int MinimumMembershipLevel { get; set; }
            public int ContentRatingTypeId { get; set; }
        }
    }
    public class SellersInfoJSON
    {
        public class Seller
        {
            public decimal id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
        }

        public class Datum
        {
            public decimal userAssetId { get; set; }
            public Seller seller { get; set; }
            public decimal? price { get; set; }
            public decimal? serialNumber { get; set; }
        }

        public class Root
        {
            public object previousPageCursor { get; set; }
            public object nextPageCursor { get; set; }
            public List<Datum> data { get; set; }
        }
    }
    public class RolimonsApiJSON
    {
        public class Root
        {
            public bool success { get; set; }
            public List<List<object>> activities { get; set; }
        }
    }
    public class ConfigJSON
    {
        public class Root
        {
            public string ROBLOSECURITY { get; set; }
            public decimal UserId { get; set; }
        }
    }
    public class PlayerInfoJSON
    {
        public class Root
        {
            public decimal Id { get; set; }
            public string Username { get; set; }
            public object AvatarUri { get; set; }
            public bool AvatarFinal { get; set; }
            public bool IsOnline { get; set; }
        }
    }
    public class LimitedItemOwnersJSON
    {
        public class Owner
        {
            public decimal id { get; set; }
            public string type { get; set; }
        }

        public class Datum
        {
            public decimal id { get; set; }
            public decimal? serialNumber { get; set; }
            public Owner owner { get; set; }
            public DateTime created { get; set; }
            public DateTime updated { get; set; }
        }

        public class Root
        {
            public string previousPageCursor { get; set; }
            public string nextPageCursor { get; set; }
            public List<Datum> data { get; set; }
        }
    }
    public class PlayerInvJSON
    {
        public class Datum
        {
            public decimal userAssetId { get; set; }
            public decimal? serialNumber { get; set; }
            public decimal assetId { get; set; }
            public string name { get; set; }
            public decimal recentAveragePrice { get; set; }
            public decimal? originalPrice { get; set; }
            public decimal? assetStock { get; set; }
            public int buildersClubMembershipType { get; set; }
        }

        public class Root
        {
            public object previousPageCursor { get; set; }
            public object nextPageCursor { get; set; }
            public List<Datum> data { get; set; }
        }
    }
    public class LimitedItemInfoJSON
    {
        public class PriceDataPoint
        {
            public decimal value { get; set; }
            public DateTime date { get; set; }
        }

        public class VolumeDataPoint
        {
            public decimal value { get; set; }
            public DateTime date { get; set; }
        }

        public class Root
        {
            public object assetStock { get; set; }
            public decimal sales { get; set; }
            public object numberRemaining { get; set; }
            public decimal recentAveragePrice { get; set; }
            public object originalPrice { get; set; }
            public List<PriceDataPoint> priceDataPoints { get; set; }
            public List<VolumeDataPoint> volumeDataPoints { get; set; }
        }


    }
    public class PlayerCollectiableItemsJSON
    {
        public class Thumbnail
        {
            public bool Final { get; set; }
            public string Url { get; set; }
            public object RetryUrl { get; set; }
            public int UserId { get; set; }
            public string EndpointType { get; set; }
        }

        public class AssetRestrictionIcon
        {
            public string TooltipText { get; set; }
            public string CssTag { get; set; }
            public bool LoadAssetRestrictionIconCss { get; set; }
            public bool HasTooltip { get; set; }
        }

        public class CollectionsItem
        {
            public string AssetSeoUrl { get; set; }
            public Thumbnail Thumbnail { get; set; }
            public string Name { get; set; }
            public object FormatName { get; set; }
            public string Description { get; set; }
            public AssetRestrictionIcon AssetRestrictionIcon { get; set; }
            public bool HasPremiumBenefit { get; set; }
        }

        public class Root
        {
            public List<CollectionsItem> CollectionsItems { get; set; }
        }
    }
    public class OutboundPageJson
    {
        public class User
        {
            public decimal id { get; set; }
            public string name { get; set; }
            public string displayName { get; set; }
        }

        public class Datum
        {
            public decimal id { get; set; }
            public User user { get; set; }
            public DateTime created { get; set; }
            public DateTime expiration { get; set; }
            public bool isActive { get; set; }
            public string status { get; set; }
        }

        public class Root
        {
            public string previousPageCursor { get; set; }
            public string nextPageCursor { get; set; }
            public List<Datum> data { get; set; }
        }
    }
    public class InboundPageJson
    {
        public class User
        {
            public decimal id { get; set; }
            public string name { get; set; }
            public string displayName { get; set; }
        }

        public class Datum
        {
            public decimal id { get; set; }
            public User user { get; set; }
            public DateTime created { get; set; }
            public DateTime expiration { get; set; }
            public bool isActive { get; set; }
            public string status { get; set; }
        }

        public class Root
        {
            public string previousPageCursor { get; set; }
            public string nextPageCursor { get; set; }
            public List<Datum> data { get; set; }
        }
    }
    public class TradeInfoJson
    {
        public class User
        {
            public int id { get; set; }
            public string name { get; set; }
            public string displayName { get; set; }
        }

        public class UserAsset
        {
            public decimal? id { get; set; }
            public decimal? serialNumber { get; set; }
            public decimal assetId { get; set; }
            public string name { get; set; }
            public decimal recentAveragePrice { get; set; }
            public decimal? originalPrice { get; set; }
            public decimal? assetStock { get; set; }
            public string membershipType { get; set; }
        }

        public class Offer
        {
            public User user { get; set; }
            public List<UserAsset> userAssets { get; set; }
            public decimal robux { get; set; }
        }

        public class Root
        {
            public List<Offer> offers { get; set; }
            public decimal id { get; set; }
            public User user { get; set; }
            public DateTime created { get; set; }
            public DateTime expiration { get; set; }
            public bool isActive { get; set; }
            public string status { get; set; }
        }
    }

    public class UserInfoJson
    {
        public string description { get; set; }
        public DateTime created { get; set; }
        public bool isBanned { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    //Dictionarys

    public class TradingDictionary
    {
        public decimal Id { get; set; }
        public decimal Robux { get; set; }
        public decimal AvgPrice { get; set; }
        public List<decimal> Items { get; set; }
    }
    public class PriceDictionary
    {
        public decimal AssetId { get; set; }
        public decimal UserAssetId { get; set; }
        public decimal AvgPrice { get; set; }
        public string Name { get; set; }
        public string AssetName { get; set; }
    }
    public class ItemDictionary
    {
        public string Name { get; set; }
        public decimal AssetId { get; set; }
        public decimal UserAssetId { get; set; }
    }
    public class SendTradeResponseDictionary
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public bool success { get; set; }
        public decimal trade_id { get; set; }
        public string note { get; set; }
    }
    public class SendTradeDataDictionary
    {
        public bool is_premium { get; set; }
        public string name { get; set; }
        public decimal id { get; set; }
        public List<decimal> inventory { get; set; }
        public List<decimal> UAIDinventory { get; set; }
    }
    public class PurchaseLimitedItemDictionary
    {
        public decimal assetid { get; set; }
        public decimal userassetid { get; set; }
        public decimal exceptedprice { get; set; }
        public decimal exceptedsellerid { get; set; }
    }

    //Misc.

    public class RobloxItemsRap
    {
        public List<decimal> k1 = new List<decimal>() { 321570512, };

        public List<decimal> k1_5 = new List<decimal>() { 67996263, 1772531407 };

        public List<decimal> k2 = new List<decimal>() { 1191162539, 71484026 };

        public List<decimal> k2_5 = new List<decimal>() { 1191125008, 1230403652, 928908332 };

        public List<decimal> k3 = new List<decimal>() { 343585127, 554663025 };

        public List<decimal> k3_5 = new List<decimal>() { 362029470, };

        public List<decimal> k4 = new List<decimal>() { 158069071, 69939573 };

        public List<decimal> k4_5 = new List<decimal>() { 128206864 };

        public List<decimal> k5 = new List<decimal>() { 244160970, 151786902, 557058485, 151327351 };

        public List<decimal> k5_5 = new List<decimal>() { 151327351 };

        public List<decimal> k6 = new List<decimal>() { 564449640, 244160970, 69344020 };

        public List<decimal> k7 = new List<decimal>() { 334656546, 66330231, 483306493 };

        public List<decimal> k8 = new List<decimal>() { 406000421, 250405532 };

        public List<decimal> k9 = new List<decimal>() { 183797762, 51243190 };

        public List<decimal> scanning_env = new List<decimal>() { 1191162013, 71484026, 835095003, 71484026, 1191125008, 321570512 };
    }
    #endregion

    #region os

    public static class os
    {
        public static int time()
        {
            //UNIX time
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static ________TIME date()
        {
            ________TIME _TIME = new ________TIME();
            var x = DateTime.Now;
            _TIME.Day = x.Day;
            _TIME.Year = x.Year;
            _TIME.Month = x.Month;
            _TIME.Hour = x.Hour;
            _TIME.Second = x.Second;
            _TIME.MiliSecond = x.Millisecond;
            _TIME.Minute = x.Minute;
            _TIME.DayOfWeek = x.DayOfWeek.ToString();
            return _TIME;
        }

      
    }

    public class ________TIME
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }
        public string DayOfWeek { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public int Second { get; set; }

        public int MiliSecond { get; set; }
    }

    #endregion } 
}