using SimpleFeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
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
                ViewData["Message"] = searchString;
                ViewData["Algo"] = searchAlgo;
                var items = feed.RetrieveFeed("http://www.antaranews.com/rss/terkini");
                ViewBag.items = items;
            }
            return View();
        }
    }
}