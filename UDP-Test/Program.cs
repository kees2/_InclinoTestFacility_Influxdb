using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace UDP_Test
{
    class Program
    {
        


        static void Main(string[] args)
        {

            
            Receive receiver = new Receive();
            Influxdb database = new Influxdb();
            receiver.initUDP();
            database.initDB();

            for (int i = 0; i < 10; i++)
            {
            database.SendToDatabase(0, 0, (UDP_Test.Influxdb.Data_type)0, 260);
            }

            while (true)
            {
                //receiver.receiveData();   
            }
        }
    }
}
