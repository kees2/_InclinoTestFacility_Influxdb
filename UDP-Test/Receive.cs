﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace UDP_Test
{
    public class Receive
    {
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
        }

        public dataMessage receiveDataWithResponse()
        {
            var data = udpServer.Receive(ref remoteEP);
            dataMessage test = fromBytes(data);
            udpServer.Send(new byte[] { 4, 2, 0 }, 3, remoteEP); // if data is received reply letting the client know that we got his data  \
            return test;
        }

        public dataMessage receiveData()
        {
            var data = udpServer.Receive(ref remoteEP);

            dataMessage test = fromBytes(data);
            return test;  
        }

        private dataMessage fromBytes(byte[] arr)
        {
            dataMessage str = new dataMessage();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            byte[] dst = new byte[arr.Length - 3];
    
            Array.Copy(arr, 3, dst, 0, dst.Length);

            Marshal.Copy(dst, 0, ptr, size);

            str = (dataMessage)Marshal.PtrToStructure(ptr, str.GetType());
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
    }
}
