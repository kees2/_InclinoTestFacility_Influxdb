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
            
            Receive receiver = new Receive();
            receiver.initUDP();

            Receive.dataMessage test;
            Influxdb database = new Influxdb();
            database.initDB();
            
            DataProcessor dataProcessor = new DataProcessor();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    for (int k = 1; i <= 4; i++)
                    {
                        dataProcessor.addData(i, j, (i*j*k));

                    }
                }
            }
            System.Threading.Thread.Sleep(500);

            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
            database.sendIMUData(dataProcessor.IMUS, 10, 6, 4);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            System.Threading.Thread.Sleep(2000);





            while (true)
            {

                stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
                database.SendToDatabasePoint();
                stopwatch.Stop();
                Console.WriteLine("point {0}", stopwatch.ElapsedMilliseconds);

                /*
                stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
                database.SendToDatabase(0, 0, 0, 0);
                stopwatch.Stop();
                Console.WriteLine("line {0}", stopwatch.ElapsedMilliseconds);*/

                /*stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
                test = receiver.receiveData();
                database.SendArrayToDatabase(0, 0, 0, 0);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);*/
            }








            //Receive receiver = new Receive();
            //receiver.initUDP();

            /*Influxdb database = new Influxdb();
            database.initDB();
            database.SendToDatabase(0, 0, 0, 0);
            */

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
