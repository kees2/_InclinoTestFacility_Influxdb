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
        public int[] testBuffer = new int[100000];
        int testcounter = 0;
        int max = 0;
        int min = 0;
        int testcounterAmountMessages = 0;
        
        public struct databaseMessage
        {
            public int data;
            public byte Data_type;
            public byte Sensor_Id;
            public byte Attribute;
        };

        private const int numThreads = 1;



        static Receive receiver = new Receive();
        private static DataProcessor dataProcessor = new DataProcessor();
        private Influxdb1_7 influx17 = new Influxdb1_7();

        public DataHandler()
        {
            initDataHandler();
        }

        public void initDataHandler()
        {
            receiver.initUDP();
            influx17.initDB();

            makeThreads();
            InitTimer();
        }

        private void InitTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        //TODO
        //Move all functions in this class to dataprocessor
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            //fillDummyDataInclino();
            influx17.addIMUs(dataProcessor.IMUS, dataProcessor.AmountIMU);
            influx17.addInclinos(dataProcessor.Inclinos, dataProcessor.AmountInclino);

            for(int i = 0; i < dataProcessor.AmountIMU; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    Console.WriteLine("amount data {0}", dataProcessor.IMUS[i].data[j].arraySize);
                }
            }
            Console.WriteLine("amount of messages received {0}", testcounterAmountMessages);
            testcounterAmountMessages = 0;
            dataProcessor.resetIMUs();
            dataProcessor.resetInclinos();
            influx17.sendData();
            stopwatch.Stop();
            Console.WriteLine("Time{0}",stopwatch.ElapsedMilliseconds);
            
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            Receive.dataMessage message = receiver.receiveData();
            //receiveMessageTest(message);
            
            //Console.WriteLine("id: {0}, Data_type: {1}, data: {2}", message.Sensor_Id, (enums.Data_type)message.Data_type, message.data.ToString("X"));
           if (
                (enums.Data_type)message.Data_type == enums.Data_type.TEMP  ||
                (enums.Data_type)message.Data_type == enums.Data_type.BARO  )
            {
                 //Hier data toevoegen voor temperatuur, barometer etc.
                influx17.addData(message.Sensor_Id, (enums.Data_type)message.Data_type, message.data, determineSensorType(message.Sensor_Id));
                //Console.WriteLine("id: {0}, Data_type: {1}, data: {2}", message.Sensor_Id, (enums.Data_type)message.Data_type, message.data.ToString("X"));
            }
            else
            {
                //Process acc, inclino and gyrodata so that average error can be calculated
                testcounterAmountMessages++;
                dataProcessor.addData(message.Sensor_Id, message.Data_type, message.data);  
            }
            stopwatch.Stop();
            Console.WriteLine("ReadTime{0}", stopwatch.ElapsedTicks);
        }

        public void fillDummyDataIMU()
        {
            int amountIMUS = 8;
            int amountDataTypes = 6;

            for (int i = 0; i < amountIMUS; i++)
            {
                for (int j = 0; j < amountDataTypes; j++)
                {
                     dataProcessor.addData(i, j, 333);
                    dataProcessor.addData(i, j, 667);
                }
            }
        }

        public void fillDummyDataInclino()
        {
            int amountIMUS = 8;
            int amountInclinos = 8;
            int amountDataTypes = 2;

            for (int i = 0; i < amountInclinos; i++)
            {
                for (int j = 7; j < 9; j++)
                {
                    if(j == 7)
                    {
                        dataProcessor.addData(i + amountIMUS, j, 333);
                        dataProcessor.addData(i + amountIMUS, j, 667);
                    }
                    else
                    {
                        dataProcessor.addData(i + amountIMUS, j, 1000);
                        dataProcessor.addData(i + amountIMUS, j, 1000);
                    }
                    
                }
            }
        }

        public void receiveMessageTest(Receive.dataMessage message)
        {
            if(message.Data_type == 0)
            {
                int difference = testcounter - message.data;
                if (difference != 0)
                {
                    if (difference > max)
                    {
                        max = difference;
                    }
                    if (difference < min)
                    {
                        min = difference;
                    }
                }
                if (testcounter == 75999)
                {
                    testcounter = 0;
                }
                testcounter++;
                Console.WriteLine("Min = {0}, Max = {1}", min, max);
                testBuffer[testcounter] = message.data;
            }
        }

        private enums.IC_type determineSensorType(int Sensor_id)
        {
            if(Sensor_id >=0 && Sensor_id < 8)
            {
                return enums.IC_type.BMI55;
            }
            else if (Sensor_id >= 8 && Sensor_id < 16)
            {
                return enums.IC_type.BMI085;
            }
            return (enums.IC_type)Sensor_id;
        }

    }
}


