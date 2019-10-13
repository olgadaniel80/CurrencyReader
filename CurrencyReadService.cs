using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyReaderService
{
    public partial class CurrencyReadService : ServiceBase
    {
        
        public CurrencyReadService()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            TimerCallback tm = new TimerCallback(WriteCurrency);
            Timer timer = new Timer(tm, 0, 0, 10800000);
            
        }

        public static void WriteCurrency(object obj)
        {
            int x = (int)obj;
            var url_GBP_EUR = @"https://finance.yahoo.com/lookup?s=GBPEUR=X/";
            var url_EUR_JPY = @"https://finance.yahoo.com/lookup?s=EURJPY=X/";
            var url_EUR_USD = @"https://finance.yahoo.com/lookup?s=EURUSD=X/";
            var url_EUR_ILS = @"https://finance.yahoo.com/lookup?s=EURILS=X/";
            try
            {

                string path = @"C:\homework\readFromSite.txt";
                if (!File.Exists(path))
                {
                    var myFile = File.Create(path);
                    myFile.Close();
                }
                List<string> lines = new List<string>();
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                    sr.Close();
                }
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    if (lines.Any())
                    {
                        foreach (var l in lines)
                        {
                            var splitLine = l.Split(' ');
                            if (splitLine.Length > 1)
                            {
                                var url = "url_" + splitLine[0].Split('/')[0] + "_" + splitLine[0].Split('/')[1];
                                var splitResult = GetCurrencyValue(url).Split(' ');
                                if (splitResult != null && splitLine[1] != splitResult[1])
                                    sw.WriteLine(GetCurrencyValue(url_GBP_EUR));
                                else
                                    sw.WriteLine(l);
                            }

                        }
                    }
                    else
                    {
                        sw.WriteLine(GetCurrencyValue(url_GBP_EUR));
                        sw.WriteLine(GetCurrencyValue(url_EUR_JPY));
                        sw.WriteLine(GetCurrencyValue(url_EUR_USD));
                        sw.WriteLine(GetCurrencyValue(url_EUR_ILS));
                    }

                    sw.Close();

                }
            }catch(Exception ex)
            {
                Thread.Sleep(1000);
            }
        }

        public static string GetCurrencyValue(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            DateTime today = DateTime.Now;
            var node = htmlDoc.DocumentNode.SelectSingleNode("//tbody");
            foreach (var n in node.ChildNodes)
            {
                if (n.NodeType == HtmlNodeType.Element)
                {
                    HtmlNode sibling = n.FirstChild.NextSibling;
                    return sibling.InnerText + " " + sibling.NextSibling.InnerText + " " + today;
                }
            }

            return string.Empty;
        }

        protected override void OnStop()
        {
           
        }
    }


}
