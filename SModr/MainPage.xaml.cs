using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Syndication;

namespace SModr
{
    public sealed partial class MainPage : Page
    {
        private static readonly SyndicationClient Client = new SyndicationClient();

        private static string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var channel = e?.Parameter as Channel;
            var episodes = Fetch(channel.Url);

            BasicGridView.ItemsSource = episodes;

            base.OnNavigatedTo(e);
        }

        private static async Task<SyndicationFeed> GetFeedAsync(string feedUri)
        {
            var uri = new Uri(feedUri);

            return await Client.RetrieveFeedAsync(uri);
        }

        private static IList<FeedItem> Fetch(string uri)
        {
            var Feed = Task.Run(async () => await GetFeedAsync(uri).ConfigureAwait(false)).Result;
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
            }

            return episodes;
        }

        private void BasicGridView_ItemClick(object _, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FeedItem;

            SetMediaSource(item.EnclosureUri);
        }

        private void SetMediaSource(string url)
        {
            try
            {
                var pathUri = new Uri(url);

                mediaPlayer.Source = MediaSource.CreateFromUri(pathUri);
            }
            catch (FormatException)
            {
                
            }
        }
    }
}
