using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace UDP_Test
{
    class Program
    {
        


        static void Main(string[] args)
        {

            //DataHandler handler = new DataHandler();

            //Receive receiver = new Receive();
            //receiver.initUDP();

            Influxdb database = new Influxdb();
            database.initDB();
            database.SendToDatabase(0, 0, 0, 0);




            /*handler.initDataHandler();
            handler.makeThreads();
            while (true)
            {
            }*/

            //Console.SetOut(TextWriter.Null);
            //Console.SetError(TextWriter.Null);
            //receiver.initUDP();
            //database.initDB();

            /*
            Stopwatch sw;
            for (int i = 0; i < 10; i++)
            {
                sw = Stopwatch.StartNew();
                database.SendToDatabase(0, (enums.Data_type)0, 260);
                Console.WriteLine(sw.ElapsedMilliseconds);
            }*/

            while (true)
            {
                //Receive.dataMessage test = receiver.receiveData();
            }

        }
    }
}
