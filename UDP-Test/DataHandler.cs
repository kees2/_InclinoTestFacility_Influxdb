using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace UDP_Test
{
    public class DataHandler
    {
        
        public struct databaseMessage
        {
            public int data;
            public byte Data_type;
            public byte Sensor_Id;
            public byte Attribute;
        };

        private const int numThreads = 4;
        private const int maxDataId = 6;
        
        static Receive receiver = new Receive();
        private static DataProcessor dataProcessor = new DataProcessor();

        private const int amountIMUs = 10;
        private const int amountDataTypes = 6;
        private const int amountAttributes = 4;

        Influxdb influx = new Influxdb();

        public DataHandler()
        {
            initDataHandler();
        }

        public void initDataHandler()
        {
            receiver.initUDP();
            makeThreads();
            influx.initDB();
            InitTimer();
        }

        private void InitTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        //static
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            fillDummyData();
            influx.sendIMUData(dataProcessor.IMUS, amountIMUs, amountDataTypes, amountAttributes);
            dataProcessor.resetIMUs();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        private void makeThreads()
        {
            for (int i = 0; i < numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProcReceive));
                newThread.Name = String.Format("ThreadReceive{0}", i + 1);
                newThread.Start();
            }
        }
                              
        private static void ThreadProcReceive()
        {
            while (true)
            {
                dataReceiver();
            }
        }

        private static void dataReceiver()
        {
            Receive.dataMessage message = receiver.receiveData();

            Influxdb influx = new Influxdb();
            influx.initDB();

            // Wait until it is safe to enter.
            if(message.Data_type < maxDataId)
            {
                //Process acc and gyrodata so that average error can be calculated
                //The data is not putten in the message buffer because this data comes in at 1000 samples per second and needs to be send at 1 sample per second
                dataProcessor.addData(message.Sensor_Id, message.Data_type, message.data);
            }
            else
            {
                //Hier data toevoegen voor temperatuur, barometer etc.
                influx.SendToDatabase(message.Sensor_Id, (enums.Data_type)message.Data_type, message.data, 0);//Nummers verranderen naar enums
            }            
        }

        public void fillDummyData()
        {
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
        }
    }
}


