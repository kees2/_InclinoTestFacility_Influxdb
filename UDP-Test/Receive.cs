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

        public class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 852;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }



        UdpClient udpServer;
        IPEndPoint remoteEP;
        //private dataMessage receiveBuffer;
        int messageCount = 0;

        /* public void ReceiveCallback(IAsyncResult ar)
         {

             // byte[] receiveBytes = u.Receive(ar);
             //byte[] receiveBytes = u.EndReceive(ar, ref e);
             //string receiveString = Encoding.ASCII.GetString(receiveBytes);

             StateObject state = (StateObject)ar.AsyncState;
             Socket client = state.workSocket;

             messageCount++;
             //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,new AsyncCallback(ReceiveCallback), state);


             //Console.WriteLine("Received{0}", messageCount);
             //u.BeginReceive(new AsyncCallback(ReceiveCallback), ar.AsyncState);
         }*/
         
            /*
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("Entered Receive Callback");
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    //  Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        //response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    //receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void initUDP()
          {
            byte[] ipbyte = new byte[] { 192, 168, 0, 10 };
            System.Net.IPAddress ip = new IPAddress(ipbyte);

            IPEndPoint endPoint = new IPEndPoint(ip, 52256);

            Socket client = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp); ;
            client.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), client);
            try
                {
                // Create the state object.  
                    StateObject state = new StateObject();
                    state.workSocket = client;

                // Begin receiving the data from the remote device.  
                //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
       }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                //connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        */
        
        //blocking
        public void initUDP()
         {
             byte[] ipbyte = new byte[] { 192, 168, 0, 9 };
             System.Net.IPAddress ip = new IPAddress(ipbyte);

             IPEndPoint endPoint = new IPEndPoint(ip, 52256);

             //IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[4], 52256);

             s = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

             // Creates an IpEndPoint to capture the identity of the sending host.
             IPEndPoint sender = new IPEndPoint(IPAddress.Any, 52256);
             senderRemote = (EndPoint)sender;

             // Binding is required with ReceiveFrom calls.
             s.Bind(endPoint);
        }

        public int resetMessageCount()
        {
            int returnValue = messageCount;
            messageCount = 0;
            return returnValue;
        }


        

        public dataMessage[] receiveDataArray()
        {
            //var data = udpServer.Receive(ref remoteEP);
            byte[] data = new Byte[852];
            s.ReceiveBufferSize = 131072;
            //Console.WriteLine("Buffersize = {0}", s.ReceiveBufferSize);
            s.ReceiveFrom(data, 0, data.Length, SocketFlags.None, ref senderRemote);

            dataMessage[] test = fromBytesToMessageArray(data);
            messageCount++;
            return test;
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
