using SimpleFeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StimaToday.Models
{
    public class SearchResultEntry : FeedItem
    {
        public string OriginalSource { get; set; }

        public SearchResultEntry(FeedItem item)
        {
            this.Content = item.Content;
            this.Date = item.Date;
            this.Summary = item.Summary;
            this.Title = item.Title;
            this.Uri = item.Uri;
        }
    }
}