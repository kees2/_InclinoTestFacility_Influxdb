using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Net;


namespace UDP_Test
{
    public class Receive
    {
        const int UDPBytes = 3;
        Socket s;
        EndPoint senderRemote;

        public struct dataMessage
        {
            public int data;
            public byte Data_type;
            public byte Sensor_Id;            
        };

        UdpClient udpServer;
        IPEndPoint remoteEP;
        //private dataMessage receiveBuffer;

        public void initUDP()
        {
            //receiveBuffer = new dataMessage();
            udpServer = new UdpClient(52256);
            remoteEP = new IPEndPoint(IPAddress.Any, 52256);

            byte[] ipbyte = new byte[] { 192, 168, 0, 9 };
            System.Net.IPAddress ip = new IPAddress(ipbyte);

            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint endPoint = new IPEndPoint(ip, 52256);

            //IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[4], 52256);
            
            s = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            // Creates an IpEndPoint to capture the identity of the sending host.
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            senderRemote = (EndPoint)sender;

            // Binding is required with ReceiveFrom calls.
            s.Bind(endPoint);
        }

        public dataMessage receiveDataWithResponse()
        {
            var data = udpServer.Receive(ref remoteEP);
            dataMessage test = fromBytesToMessage(data);
            udpServer.Send(new byte[] { 4, 2, 0 }, 3, remoteEP); // if data is received reply letting the client know that we got his data  \
            return test;
        }

        public dataMessage receiveData()
        {
            var data = udpServer.Receive(ref remoteEP);

            dataMessage test = fromBytesToMessage(data);
            return test;  
        }

        public dataMessage[] receiveDataArray()
        {
            //var data = udpServer.Receive(ref remoteEP);
            byte[] data = new Byte[852];
            s.ReceiveBufferSize = 131072;
            //Console.WriteLine("Buffersize = {0}", s.ReceiveBufferSize);
            s.ReceiveFrom(data, 0, data.Length, SocketFlags.None, ref senderRemote);

            dataMessage[] test = fromBytesToMessageArray(data);

            return test;
        }

        private dataMessage fromBytesToMessage(byte[] arr)
        {
            dataMessage str = new dataMessage();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            byte[] dst = new byte[arr.Length - UDPBytes];//Min de eerste 3 UDP bytes
    
            Array.Copy(arr, UDPBytes, dst, 0, dst.Length);

            Marshal.Copy(dst, 0, ptr, size);

            str = (dataMessage)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        private dataMessage[] fromBytesToMessageArray(byte[] arr)
        {

            int messageArraySize = arr[0];

            dataMessage[] str = new dataMessage[messageArraySize];

            int size = Marshal.SizeOf(str[0]) * str.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            byte[] dst = new byte[arr.Length - UDPBytes];//Min de eerste 3 UDP bytes

            Array.Copy(arr, UDPBytes, dst, 0, dst.Length);

            Marshal.Copy(dst, 0, ptr, size);

            for(int i = 0; i < str.Length; i++)
            {
                IntPtr p = new IntPtr((ptr.ToInt32() + i * Marshal.SizeOf(str[0])));
                str[i] = (dataMessage)Marshal.PtrToStructure(p, str[i].GetType());
            }
            
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        private byte[] getBytes(dataMessage str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }
    }
}
