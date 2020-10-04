using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;


namespace SModr
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelsPage : Page
    {
        private static readonly List<String> CHANNELS = new List<string>(2) 
        {
            "https://feeds.feedburner.com/SModcasts",
            "http://feeds.feedburner.com/TellEmSteveDave"
        };
        private static readonly List<Channel> Items = new List<Channel>(CHANNELS.Count);

        static ChannelsPage()
        {
            foreach (var feedUrl in CHANNELS)
            {
                var channel = new Channel(feedUrl);

                Items.Add(channel);
            }
        }

        public ChannelsPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = this;
            ContentGridView.ItemsSource = Items;
        }

        private void ContentGridView_ItemClick(object _, ItemClickEventArgs e)
        {
            var selectedItem = e.ClickedItem as Channel;

            Frame.Navigate(typeof(MainPage), selectedItem);
        }
    }
}
