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
        private const int numThreads = 3;
        private const int maxDataId = 6;
        
        private static Receive.dataMessage[] messageBuffer = new Receive.dataMessage[300];
        static int writePointer = 0;
        static int readPointer = 0;

        
        static Receive receiver = new Receive();
        private static DataProcessor dataProcessor = new DataProcessor();

        private const int amountIMUs = 10;
        private const int amountDataTypes = 6;

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
            Influxdb influx = new Influxdb();
            for (int i = 0; i < amountIMUs; i++)
            {
                for (int j = 0; j < amountDataTypes; j++)
                {
                    
                    //influx.SendToDatabase(dataProcessor.IMUS[i].data[j].determineMin(), (enums.Data_type)j, (byte)dataProcessor.IMUS[i].SensorId, 1);//Nummers verranderen naar enums
                    //influx.SendToDatabase(dataProcessor.IMUS[i].data[j].determineMax(), (enums.Data_type)j, (byte)dataProcessor.IMUS[i].SensorId, 2);
                    //influx.SendToDatabase(dataProcessor.IMUS[i].data[j].calcAverage(), (enums.Data_type)j, (byte)dataProcessor.IMUS[i].SensorId, 3);
                    //influx.SendToDatabase(dataProcessor.IMUS[i].data[j].calcAverageError(), (enums.Data_type)j, (byte)dataProcessor.IMUS[i].SensorId, 4);
                    
                }
            }
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
            for (int i = 0; i < 1; i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProcSend));
                newThread.Name = String.Format("ThreadSend{0}", i + (1 + 1));
                newThread.Start();
            }
        }
                              
        private static void ThreadProcReceive()
        {
            for (int i = 0; i < numIterations; i++)
            {
                dataReceiver();
            }
        }

        private static void ThreadProcSend()
        {
            for (int i = 0; i < numIterations; i++)
            {
                databaseSend();
            }
        }

        private static void dataReceiver()
        {
            Receive.dataMessage message = receiver.receiveData();

            // Wait until it is safe to enter.
            Console.WriteLine("{0} is requesting the mutex",
                              Thread.CurrentThread.Name);
            mut.WaitOne();

            Console.WriteLine("{0} has entered the protected area",
                              Thread.CurrentThread.Name);

            // Place code to access non-reentrant resources here.
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
            Console.WriteLine("{0} is leaving the protected area",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mut.ReleaseMutex();
            Console.WriteLine("{0} has released the mutex",
                Thread.CurrentThread.Name);
        }

        private static void databaseSend()
        {
            while(readPointer >= writePointer)
            {
            }
            Receive.dataMessage message;
            Influxdb influx = new Influxdb();

            // Wait until it is safe to enter.
            Console.WriteLine("{0} is requesting the mutex",
                              Thread.CurrentThread.Name);
            mut.WaitOne();

            Console.WriteLine("{0} has entered the protected area",
                              Thread.CurrentThread.Name);

            // Place code to access non-reentrant resources here.
            message = messageBuffer[readPointer];
            readPointer += 1;

            Console.WriteLine("{0} is leaving the protected area",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mut.ReleaseMutex();
            Console.WriteLine("{0} has released the mutex",
                Thread.CurrentThread.Name);

            influx.SendToDatabase(message.Sensor_Id, (enums.Data_type)message.Data_type, message.data, 0);//Nummers verranderen naar enums
        }
    }
}


