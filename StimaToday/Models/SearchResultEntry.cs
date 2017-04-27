using SimpleFeedReader;

namespace StimaToday.Models
{
    public class SearchResultEntry : FeedItem
    {
        public string OriginalSource { get; set; }
        public string ImgHtmlRaw { get; set; }

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