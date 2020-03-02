using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


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


        private static Mutex mut = new Mutex();
        private const int numIterations = 8;
        private const int numThreads = 1;
        private const int maxDataId = 6;
        
        private static Receive.dataMessage[] messageBuffer = new Receive.dataMessage[5000];
        static int writePointer = 0;
        static int readPointer = 0;

        
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
            dataProcessor.IMUS[0].addData(0, 0);
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
            fillDummyData();
            influx.sendIMUData(dataProcessor.IMUS, amountIMUs, amountDataTypes, amountAttributes);
            dataProcessor.resetIMUs();
        }


        private databaseMessage makePackage(int data, byte Data_type, byte Sensor_Id, byte attribute)
        {
            databaseMessage packet = new databaseMessage();

            packet.data = data;
            packet.Data_type = Data_type;
            packet.Sensor_Id = Sensor_Id;
            packet.Attribute = attribute;

            return packet;
        }

        private void makeThreads()
        {
            for (int i = 0; i < numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProcReceive));
                newThread.Name = String.Format("ThreadReceive{0}", i + 1);
                newThread.Start();
            }

            /*for (int i = 0; i < (numThreads + 5); i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProcSend));
                newThread.Name = String.Format("ThreadSend{0}", (i + 1));
                newThread.Start();
            }*/
        }
                              
        private static void ThreadProcReceive()
        {
            while (true)
            {
                dataReceiver();
            }
        }

        private static void ThreadProcSend()
        {
            while (true) { 
                databaseSend();
            }
                
        }

        private static void dataReceiver()
        {
            Receive.dataMessage message = receiver.receiveData();

            Influxdb influx = new Influxdb();
            influx.initDB();

            // Wait until it is safe to enter.
            mut.WaitOne();
            if(message.Data_type < maxDataId)
            {
                //Process acc and gyrodata so that average error can be calculated
                //The data is not putten in the message buffer because this data comes in at 1000 samples per second and needs to be send at 1 sample per second
                dataProcessor.addData(message.Sensor_Id, message.Data_type, message.data);
            }
            else
            {
                messageBuffer[writePointer] = message;
                writePointer++;
            }

            mut.ReleaseMutex();
            influx.SendToDatabase(message.Sensor_Id, (enums.Data_type)message.Data_type, message.data, 0);//Nummers verranderen naar enums
        }

        private static void databaseSend()
        {
            while(readPointer >= writePointer)
            {
            }
            Receive.dataMessage message;
            Influxdb influx = new Influxdb();
            influx.initDB();
            mut.WaitOne();

            // Place code to access non-reentrant resources here.
            message = messageBuffer[readPointer];
            readPointer += 1;

            // Release the Mutex.
            mut.ReleaseMutex();

            influx.SendToDatabase(message.Sensor_Id, (enums.Data_type)message.Data_type, message.data, 0);//Nummers verranderen naar enums
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


