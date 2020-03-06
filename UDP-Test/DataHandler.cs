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

        private const int amountIMUs = 8;
        private const int amountDataTypes = 6;
        private const int amountAttributes = 4;

        Influxdb influx = new Influxdb();
        Influxdb1_7 influx17 = new Influxdb1_7();

        public DataHandler()
        {
            initDataHandler();
        }

        public void initDataHandler()
        {
            receiver.initUDP();
            makeThreads();
            influx17.initDB();
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
            //fillDummyData();
            influx17.addIMUs(dataProcessor.IMUS, amountIMUs);
            dataProcessor.resetIMUs();
            influx17.sendData();
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
                              
        private  void ThreadProcReceive()
        {
            while (true)
            {
                dataReceiver();
            }
        }

        private  void dataReceiver()
        {
            Receive.dataMessage message = receiver.receiveData();

            if(message.Data_type < maxDataId)
            {
                //Process acc and gyrodata so that average error can be calculated
                dataProcessor.addData(message.Sensor_Id, message.Data_type, message.data);
            }
            else
            {
                //Hier data toevoegen voor temperatuur, barometer etc.
                influx17.addData(message.Sensor_Id, (enums.Data_type)message.Data_type, message.data);
                Console.WriteLine("id: {0}, Data_type: {1}, data: {2}", message.Sensor_Id, (enums.Data_type)message.Data_type, message.data.ToString("X"));
            }       
        }

        public void fillDummyData()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                     dataProcessor.addData(i, j, 333);
                    dataProcessor.addData(i, j, 667);
                }
            }
        }
    }
}


