using System.Net;
using System.Net.Sockets;

namespace WakeOnLan
{
    public class WakeOnLanClient
    {
        private const string BroadcastingAddress = "255.255.255.255";
        private const int BroadcastingPort = 0;
        private readonly IPAddress localEndPointAddress;
        
        public WakeOnLanClient(IPAddress localEndPointAddress)
        {
            this.localEndPointAddress = localEndPointAddress;
        }

        public void WakeUp(string macAddress)
        {
            var magicPacket = CreateMagicPacket(macAddress);

            SendPacket(magicPacket);
        }

        private byte[] CreateMagicPacket(string macAddress)
        {
            int packetIndex = 0;
            byte[] magicPacket = new byte[102];

            for (int index = 0; index < 6; index++)
            {
                magicPacket[index] = 0xff;
                packetIndex++;
            }

            for (int index = 0; index < 16; index++)
            {
                for (int group = 0; group < 6; group++)
                {
                    magicPacket[packetIndex++] = byte.Parse(macAddress.Substring(group * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }

            return magicPacket;
        }

        private void SendPacket(byte[] magicPacket)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(localEndPointAddress, 0));
                socket.EnableBroadcast = true;
                socket.SendTo(magicPacket, new IPEndPoint(IPAddress.Parse(BroadcastingAddress), BroadcastingPort));
            }
        }
    }
}
