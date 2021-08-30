using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace X_IPTV
{
    /// <summary>
    /// Interaction logic for UserLogin.xaml
    /// </summary>
    public partial class UserLogin : Window
    {
        public UserLogin()
        {
            InitializeComponent();
        }

        private async void Con_btn_Click(object sender, RoutedEventArgs e)
        {
            //busy_ind.IsBusy = true;

            await Connect(usrTxt.Text, passTxt.Text, serverTxt.Text, portTxt.Text);

            //busy_ind.BusyContent = "Loading channels list...";

            await LoadChannels(usrTxt.Text, passTxt.Text, serverTxt.Text, portTxt.Text);

            var channelWindow = new ChannelList();

            //load epg. Eventually make it optional
            //busy_ind.BusyContent = "Loading playlist data...";

            await LoadPlaylistData(usrTxt.Text, passTxt.Text, serverTxt.Text, portTxt.Text);

            channelWindow.Show();

            this.Close();
        }

        private async Task Connect(string user, string pass, string server, string port)
        {
            // Create a request for the URL. 		
            WebRequest request;
            if ((bool)protocolCheckBox.IsChecked)//use the https protocol
                request = WebRequest.Create($"https://{server}:{port}/player_api.php?username={user}&password={pass}");
            else//use the http protocol
                request = WebRequest.Create($"http://{server}:{port}/player_api.php?username={user}&password={pass}");
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = await reader.ReadToEndAsync();
            // Display the content.
            Console.WriteLine(responseFromServer);

            PlayerInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerInfo>(responseFromServer);

            Instance.PlayerInfo = info;

            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
        }

        private async Task LoadChannels(string user, string pass, string server, string port)
        {
            // Create a request for the URL. 	
            WebRequest request;
            if ((bool)protocolCheckBox.IsChecked)//use the https protocol
                request = WebRequest.Create($"https://{server}:{port}/player_api.php?username={user}&password={pass}&action=get_live_streams");
            else//use the http protocol
                request = WebRequest.Create($"http://{server}:{port}/player_api.php?username={user}&password={pass}&action=get_live_streams");
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = await reader.ReadToEndAsync();
            // Display the content.
            Console.WriteLine(responseFromServer);

            ChannelEntry[] info = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelEntry[]>(responseFromServer);

            Instance.ChannelsArray = info;

            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
        }

        private static readonly HttpClient client = new HttpClient();
        private async Task LoadPlaylistData(string user, string pass, string server, string port)
        {
            //retrieve playlist data from client
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");

            var stringTask = client.GetStringAsync($"https://{server}:{port}/get.php?username={user}&password={pass}");

            var msg = await stringTask;
            //Console.Write(msg);

            //parse the m3u playlist and split into an array
            string[] playlist = msg.Split(new string[] { "#EXTINF:" }, StringSplitOptions.None);

            //needs cleaned up
            PlaylistData[] info = new PlaylistData[Instance.ChannelsArray.Length];
            int index = -1;
            Instance.playlistDataMap = new Dictionary<string, PlaylistData>();
            foreach (var channel in playlist)
            {
                //Console.WriteLine($"#EXTINF:{channel}");
                if (index > -1)
                {
                    //eventually fix this, make the split better and use all of the data in the PlaylistData class
                    string[] splitArr = channel.Split(' ');
                    string xui_id = "";
                    foreach (Match match in Regex.Matches(splitArr[1], "\"([^\"]*)\""))
                        xui_id = match.ToString().Replace("\"", "");
                    info[index] = new PlaylistData { 
                        xui_id = xui_id,
                        stream_url = channel.Substring(channel.LastIndexOf("https"))
                    };
                    Instance.playlistDataMap.Add(info[index].xui_id, info[index]);
                }
                index++;
            }
            Console.WriteLine("Done.");
        }
    }
}
