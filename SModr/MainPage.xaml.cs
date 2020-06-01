using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using Windows.Web.Syndication;

namespace SModr
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string FEED_URL = "https://feeds.feedburner.com/SModcasts";
        private readonly SyndicationFeed Feed;
        private readonly SyndicationClient Client = new SyndicationClient();

        private static string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;

            var task = Task.Run(async () => await GetFeedAsync(FEED_URL).ConfigureAwait(false));
            Feed = task.Result;
            var episodes = new List<FeedItem>(Feed.Items.Count);

            foreach (var item in Feed.Items)
            {
                var feedItem = new FeedItem
                {
                    Title = item.Title.Text,
                    Description = StripHtml(item.Summary.Text),
                    PubDate = item.PublishedDate.ToString()
                };

                foreach (var extension in item.ElementExtensions)
                {
                    if (extension.NodeName.Equals("origEnclosureLink", StringComparison.OrdinalIgnoreCase))
                    {
                        feedItem.EnclosureUri = extension.NodeValue.ToString().Replace("http://", "https://", StringComparison.OrdinalIgnoreCase);
                    }
                }

                episodes.Add(feedItem);
                BasicGridView.ItemsSource = episodes.ToArray();
            }
        }

        private async Task<SyndicationFeed> GetFeedAsync(string feedUri)
        {
            var uri = new Uri(feedUri);

            return await Client.RetrieveFeedAsync(uri);
        }

        private void BasicGridView_ItemClick(object _, ItemClickEventArgs e)
        {
            var item = (FeedItem) e.ClickedItem;
            SetMediaSource(item.EnclosureUri);
        }

        private void SetMediaSource(string url)
        {
            try
            {
                var pathUri = new Uri(url);
                mediaPlayer.Source = MediaSource.CreateFromUri(pathUri);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (FormatException _)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                
            }
        }
    }
}
