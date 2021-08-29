using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_IPTV
{
    public static class Instance
    {
        public static PlayerInfo PlayerInfo = null;

        public static ChannelEntry[] ChannelsArray = null;

        //public static PlaylistData[] PlaylistArray = null;

        public static Dictionary<string, PlaylistData> playlistDataMap = null;
    }
}
