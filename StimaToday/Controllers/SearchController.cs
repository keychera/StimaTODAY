using HtmlAgilityPack;
using SimpleFeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

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
                //using htmlagilitypack here
                HtmlDocument htmlDoc = new HtmlDocument();
                using (var actualArticle = new HttpClient())
                {
                    var response = actualArticle.GetAsync(items.ElementAt(0).Uri).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;
                        htmlDoc.LoadHtml(responseContent.ReadAsStringAsync().Result);
                    }
                }
                //buat ambil artikelnya, but setiap sumber berita, idnya beda2 buat artikelnya
                //yang ini buat antaranews
                HtmlNode node = htmlDoc.DocumentNode.SelectNodes("//div[@id='content_news']").First();
                ViewData["Message"] = node.InnerText;

                ViewData["Keywords"] = searchString;
                ViewData["Algo"] = searchAlgo;
                ViewBag.items = items;
            }
            return View();
        }
    }
}