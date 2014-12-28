using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace Bit.Ly_URL_Extractor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;
        }
        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                landscapeContent.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                snapViewContent.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Filled || Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape)
            {
                landscapeContent.Visibility = Windows.UI.Xaml.Visibility.Visible;
                snapViewContent.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
        public static void AddSettingsCommands(SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            SettingsCommand privacyPref = new SettingsCommand("privacyPref", "Privacy Policy", (uiCommand) =>
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri("http://apoorvkupadhyay.blogspot.in"));
            });

            args.Request.ApplicationCommands.Add(privacyPref);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += GroupedItemsPage_CommandsRequested;
        }
        void GroupedItemsPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            MainPage.AddSettingsCommands(args);
        }
        public async Task<string> GetPhotosStream(string url1)
        {
            HttpClient client = new HttpClient();
            string url = string.Format("https://api-ssl.bitly.com/v3/expand?login=apoorv001123456&apiKey=R_96c5153c6b9719031bbaf5503268f9b1&format=json&shortUrl={0}", url1);
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        public class Expand
        {
            public string short_url { get; set; }
            public string long_url { get; set; }
            public string user_hash { get; set; }
            public string global_hash { get; set; }
        }

        public class Data
        {
            public List<Expand> expand { get; set; }
        }

        public class RootObject
        {
            public int status_code { get; set; }
            public string status_txt { get; set; }
            public Data data { get; set; }
        }
   

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageDialog mb = new MessageDialog("Don't Leave The Field Empty", "Bit.Ly URL Extractor");
                mb.ShowAsync();
            }
            else if (textBox1.Text.Contains("http://bit.ly/") || textBox1.Text.Contains("http://j.mp/")||(textBox1.Text.Contains("HTTP://J"))||(textBox1.Text.Contains("HTTP://B")))
            {
                try
                {
                    string url = WebUtility.UrlEncode(textBox1.Text.Trim());
                    string responseText = await GetPhotosStream(url)
                        ;
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (RootObject));
                    RootObject root;
                    using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(responseText)))
                    {
                        root = serializer.ReadObject(stream) as RootObject;
                        foreach (var data in root.data.expand)

                        {
                            textBox2.Text ="The URL You Entered Was:"+ data.short_url + "\n" +"The Extracted URL is "+ data.long_url;
                        }
                    }

                }
                catch
                {
                 MessageDialog mb=new MessageDialog("Are You entering Valid Stuff? Check Again Please","Bit.Ly URL Extractor");
                      mb.ShowAsync();
                  }
            }
            else
            {
                MessageDialog mb = new MessageDialog("Please Enter Valid URLs only staring with http://bit.ly/ or HTTP://j.mp/ Only",
                                                     "Bit.Ly URL Extractor");
                mb.ShowAsync();
            }
            
            }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageDialog mb = new MessageDialog("Dont Leave This Field Blank","Bit.Ly URL Extractor");
                 mb.ShowAsync();
                textBox2.Text = string.Empty;
            }
        }


    }
}
