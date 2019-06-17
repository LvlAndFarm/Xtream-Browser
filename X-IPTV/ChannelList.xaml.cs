using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ChannelList.xaml
    /// </summary>
    public partial class ChannelList : Window
    {
        public ChannelList()
        {
            InitializeComponent();

            ChannelModel model = new ChannelModel();

            ChannelLst.DataContext = model;
        }

        private void ChannelLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 1) return;

            ChannelEntry entry = e.AddedItems[0] as ChannelEntry;

            ProcessStartInfo processStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe", $"http://iptv-pure.com:8000/live/sabihi/ek5jkfngrf/{entry.stream_id}.m3u8");

            Process.Start(processStartInfo);
        }
    }

    public class ChannelModel
    {
        public ChannelModel()
        {
            MyListBoxItems = new ObservableCollection<ChannelEntry>();
            for (int i = 0; i < Instance.ChannelsArray.Length; i++)
            {
                MyListBoxItems.Add(Instance.ChannelsArray[i]);
            }
        }

        public ObservableCollection<ChannelEntry> MyListBoxItems { get; set; }
    }

    public class MyMockClass
    {
        public MyMockClass()
        {
            MyListBoxItems = new ObservableCollection<ChannelEntry>();
            MyListBoxItems.Add(new ChannelEntry() { name = "|FR| TF1 UHD", stream_icon = "http://f.iptv-pure.com/tf14k.png" });
            MyListBoxItems.Add(new ChannelEntry() { name = "|FR| CSTAR FHD", stream_icon = "http://f.iptv-pure.com/cstar.png" });
        }
        public ObservableCollection<ChannelEntry> MyListBoxItems { get; set; }
    }
}
