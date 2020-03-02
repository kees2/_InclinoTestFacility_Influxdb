using System;
using System.Diagnostics;

namespace UDP_Test
{
    class Program
    {


        static void Main(string[] args)
        {
            //DataHandler handler = new DataHandler();
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
                        dataProcessor.addData(i, j, (i * j * k));
                    }
                }
            }

            Receive.dataMessage erwin;
            Stopwatch stopwatch;

            int count = 0;
            while (true)
            {            
                stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
                System.Threading.Thread.Sleep(700);
                database.sendIMUData(dataProcessor.IMUS, 10, 6, 4);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
        }
    }
}


/*
stopwatch = Stopwatch.StartNew();
erwin = receiver.receiveData();
Console.WriteLine(erwin.data.ToString("X"));
stopwatch.Stop();
count++;
if(count == 12)
{
    count = 0;
    Console.WriteLine("***********************************************");
}*/
