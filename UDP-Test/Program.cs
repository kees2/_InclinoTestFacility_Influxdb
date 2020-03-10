using System;
using System.Diagnostics;

namespace UDP_Test
{
    class Program
    {


        static void Main(string[] args)
        {
            /*
            Influxdb1_7 database = new Influxdb1_7();
            IMU imu = new IMU();
            imu.SensorId = 1;
            imu.data[0].dataArray[0] = 420;
            database.initDB();
            database.sendData(imu);
            */

            DataHandler datahandler = new DataHandler();
            //datahandler.initDataHandler();
            //= new DataHandler();
            Console.WriteLine("Schrijfactie voltooid");
            while (true)
            {

            }

            /*
            int count = 0;
            Stopwatch stopwatch;
            Receive receiver = new Receive();
            receiver.initUDP();
            Receive.dataMessage erwin;
            while (true)
            {
                stopwatch = Stopwatch.StartNew();
                erwin = receiver.receiveData();
                Console.WriteLine(erwin.data.ToString("X"));
                stopwatch.Stop();
                count++;
                if (count == 12)
                {
                    count = 0;
                    Console.WriteLine("***********************************************");
                }
            }*/
        }
    }   
}




//Dit is voor het uitlezen van Erwin zijn data

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
