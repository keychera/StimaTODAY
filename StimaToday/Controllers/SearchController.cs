using HtmlAgilityPack;
using SimpleFeedReader;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using StimaToday.Models;

namespace StimaToday.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index(string searchString, string searchAlgo)
        {
            var feed = new FeedReader();
            if (!String.IsNullOrEmpty(searchString))
            {
                var items = feed.RetrieveFeed("http://www.antaranews.com/rss/terkini");
                ArrayList result = new ArrayList();
                Finder f = new Finder();
                string searchResult = "";
                //using htmlagilitypack here
                HtmlDocument htmlDoc = new HtmlDocument();
                using (var actualArticle = new HttpClient())
                {
                    foreach (var item in items)
                    {
                        var response = actualArticle.GetAsync((item as FeedItem).Uri).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = response.Content;
                            htmlDoc.LoadHtml(responseContent.ReadAsStringAsync().Result);
                            HtmlNode node = htmlDoc.DocumentNode.SelectNodes("//div[@id='content_news']").First();
                            if (searchAlgo.Equals("Booyer-Moore Algorithm"))
                            {
                                if (f.booyerMoore(node.InnerText, searchString, ref searchResult))
                                {
                                    (item as FeedItem).Content = searchResult;
                                    result.Add(item);
                                }
                            }
                            else
                            if (searchAlgo.Equals("Knuth–Morris–Pratt Algorithm"))
                            {
                                if (f.kmpMatch(node.InnerText, searchString, ref searchResult))
                                {
                                    (item as FeedItem).Content = searchResult;
                                    result.Add(item);
                                }
                            }
                            else
                            if (searchAlgo.Equals("Regex"))
                            {
                                if (f.regex(node.InnerText, searchString, ref searchResult))
                                {
                                    (item as FeedItem).Content = searchResult;
                                    result.Add(item);
                                }
                            }
                        }
                    }
                }
                ViewData["Keywords"] = searchString;
                ViewData["Algo"] = searchAlgo;
                ViewBag.items = result;
            }
            return View();
        }
    }
}