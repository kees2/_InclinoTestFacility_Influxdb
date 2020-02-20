using System;
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
        const int AMOUNTBMI055 = 2;

        private struct imuData
        {
            Byte imu_nr;
            public ushort gyro_x;
            public ushort gyro_y;
            public ushort gyro_z;
            public ushort acc_x;
            public ushort acc_y;
            public ushort acc_z;
            public ushort temp;

            public imuData(byte p1, ushort p2, ushort p3, ushort p4, ushort p5, ushort p6, ushort p7, ushort p8)
            {
                imu_nr = p1;
                gyro_x = p2;
                gyro_y = p3;
                gyro_z = p4;
                acc_x = p5;
                acc_y = p6;
                acc_z = p7;
                temp = p8;
            }
        };

        private struct inclinoData{
            public Byte inclino_nr;
            public ushort inclino_a;
            public ushort inclino_b;
            public ushort U_rev;
            public inclinoData(byte p1, ushort p2, ushort p3, ushort p4)
            {
                inclino_nr = p1;
                inclino_a = p2;
                inclino_b = p3;
                U_rev = p4;
            }   
        };

        private struct dataMessage{
            public int temp;
            public int baro;
            public int timestamp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AMOUNTBMI055, ArraySubType = UnmanagedType.U1)]
            public imuData[] BMI055_imu;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AMOUNTBMI055, ArraySubType = UnmanagedType.U1)]
            public inclinoData[] BMI055_incl;
            public imuData imuDataBMI085;
            public imuData imuDataLMS6DSO;
        };

        UdpClient udpServer;
        IPEndPoint remoteEP;
        private dataMessage receiveBuffer;

        public void initUDP()
        {
            receiveBuffer = new dataMessage();
            udpServer = new UdpClient(52256);
            remoteEP = new IPEndPoint(IPAddress.Any, 11000);

            receiveBuffer.BMI055_imu = new imuData[AMOUNTBMI055];
            receiveBuffer.BMI055_incl = new inclinoData[AMOUNTBMI055];
            receiveBuffer.imuDataBMI085 = new imuData();
            receiveBuffer.imuDataLMS6DSO = new imuData();
        }

        public void receiveData()
        {
            var data = udpServer.Receive(ref remoteEP);

            dataMessage test = fromBytes(data);
            udpServer.Send(new byte[] { 4, 2, 0 }, 3, remoteEP); // if data is received reply letting the client know that we got his data  
        }
                          
        dataMessage fromBytes(byte[] arr)
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

        byte[] getBytes(dataMessage str)
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
