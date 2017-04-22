using HtmlAgilityPack;
using SimpleFeedReader;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using StimaToday.Models;
using System.Collections.Generic;

namespace StimaToday.Controllers
{
    public class SearchController : Controller
    {
        private class newsSource
        {
            public IEnumerable<FeedItem> Feeds { get; set; }
            public string OriginalSource { get; set; }
            public string NewsHtmlIdentifier { get; set; }

            public newsSource(IEnumerable<FeedItem> feeds, string source, string identifier)
            {
                Feeds = feeds;
                OriginalSource = source;
                NewsHtmlIdentifier = identifier;
            }
        }

        // GET: Search
        public ActionResult Index(string searchString, string searchAlgo)
        {
            var feed = new FeedReader();
            if (!String.IsNullOrEmpty(searchString))
            {
                List<newsSource> newsSources = new List<newsSource>
                {
                    new newsSource(
                        feed.RetrieveFeed("http://rss.detik.com/index.php/detikcom"),
                        "detik.com",
                        "//div[@id='detikdetailtext']"
                        ),
                    new newsSource(
                        feed.RetrieveFeed("http://tempo.co/rss/terkini"),
                        "tempo.com",
                        "//div[@class='artikel']"
                        ),
                    new newsSource(
                        feed.RetrieveFeed("http://rss.vivanews.com/get/all"),
                        "vivanews.com",
                        "//div[@id='article-content']"
                        ),
                    new newsSource(
                        feed.RetrieveFeed("http://www.antaranews.com/rss/terkini"),
                        "antaranews.com",
                        "//div[@id='content_news']"
                        )
                };
                ArrayList result = new ArrayList();
                Finder f = new Finder();
                foreach (var source in newsSources) {
                    var items = source.Feeds;
                    string htmlIdentifier = source.NewsHtmlIdentifier;
                    string searchResult = "";
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
                                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(htmlIdentifier);
                                if (nodes != null)
                                {
                                    var node = nodes.First();
                                    Boolean isSearchSuccess = false;
                                    if (searchAlgo.Equals("Booyer-Moore Algorithm"))
                                    {
                                        isSearchSuccess = f.booyerMoore(node.InnerText, searchString, ref searchResult);
                                    }
                                    else
                                    if (searchAlgo.Equals("Knuth–Morris–Pratt Algorithm"))
                                    {
                                        isSearchSuccess = f.kmpMatch(node.InnerText, searchString, ref searchResult);
                                    }
                                    else
                                    if (searchAlgo.Equals("Regex"))
                                    {
                                        isSearchSuccess = f.regex(node.InnerText, searchString, ref searchResult);
                                    }
                                    if (isSearchSuccess)
                                    {
                                        var newItem = new SearchResultEntry(item as FeedItem);
                                        (newItem as SearchResultEntry).Content = searchResult;
                                        (newItem as SearchResultEntry).OriginalSource = source.OriginalSource;
                                        result.Add(newItem);
                                    }
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