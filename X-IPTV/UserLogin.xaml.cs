using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            busy_ind.IsBusy = true;

            await Connect(usrTxt.Text, passTxt.Text, serverTxt.Text, portTxt.Text);

            busy_ind.BusyContent = "Loading channels list...";

            await LoadChannels(usrTxt.Text, passTxt.Text, serverTxt.Text, portTxt.Text);

            var channelWindow = new ChannelList();

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
    }
}
