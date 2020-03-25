﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


namespace UDP_Test
{
    class AsyncReceive
    {

        const int UDPBytes = 3;

        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }


        public static bool messageReceived = false;
        public static int messageCounter = 0;
        public static Receive.dataMessage[][] messageBuffer = new Receive.dataMessage[1000][];
        public static int dataMessageCounter = 0;
        public static int MessageReadbuffer = 0;

        private static void InitTimer()
        {
            System.Timers.Timer timerS = new System.Timers.Timer();
            timerS.Interval = 1000;
            timerS.Elapsed += timer_ElapsedS;
            timerS.Start();
        }

        static void timer_ElapsedS(object sender, System.Timers.ElapsedEventArgs e)
        {

            Console.WriteLine("Evert");
            Console.WriteLine($"Received: {messageCounter}");
            messageCounter = 0;
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            //byte[] receiveBytes = u.Receive(ref e);

            messageBuffer[dataMessageCounter] = fromBytesToMessageArray(receiveBytes);
            if (dataMessageCounter == 999)
            {
                dataMessageCounter = 0;
            }
            dataMessageCounter++;
            messageCounter++;

            u.BeginReceive(new AsyncCallback(ReceiveCallback), (UdpState)ar.AsyncState);
            //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            //Console.WriteLine($"Received: {messageCounter}");
            messageReceived = true;
        }

        public static void ReceiveMessages()
        {
            // Receive a message and write it to the console.
            InitTimer();
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 52256);
            UdpClient u = new UdpClient(e);

            UdpState s = new UdpState();
            s.e = e;
            s.u = u;

            Console.WriteLine("listening for messages");
            u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

            // Do some work while we wait for a message. For this example, we'll just sleep
            while (!messageReceived)
            {
                Thread.Sleep(1);
            }
        }

        private static Receive.dataMessage[] fromBytesToMessageArray(byte[] arr)
        {

            int messageArraySize = arr[0];

            Receive.dataMessage[] str = new Receive.dataMessage[messageArraySize];

            int size = Marshal.SizeOf(str[0]) * str.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            byte[] dst = new byte[arr.Length - UDPBytes];//Min de eerste 3 UDP bytes

            Array.Copy(arr, UDPBytes, dst, 0, dst.Length);

            Marshal.Copy(dst, 0, ptr, size);

            for (int i = 0; i < str.Length; i++)
            {
                IntPtr p = new IntPtr((ptr.ToInt32() + i * Marshal.SizeOf(str[0])));
                str[i] = (Receive.dataMessage)Marshal.PtrToStructure(p, str[i].GetType());
            }

            Marshal.FreeHGlobal(ptr);

            return str;
        }



    }
}