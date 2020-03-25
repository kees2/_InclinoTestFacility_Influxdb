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
        int tempRead = 0;
        int testcounterAmountMessages = 0;
        int tempCounter = 0;
        bool Baroread = false;

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
            //receiver.initUDP();
            AsyncReceive.ReceiveMessages();
            influx17.initDB();

            makeThreads();
            InitTimer();
        }

        //Initialize timer which interrupts every second
        private void InitTimer()
        {
            System.Timers.Timer timerS = new System.Timers.Timer();
            timerS.Interval = 1000;
            timerS.Elapsed += timer_ElapsedS;
            timerS.Start();


        }

        //TODO
        //Move all functions in this class to dataprocessor

        //Send all buffered data to the influx database
        void timer_ElapsedS(object sender, System.Timers.ElapsedEventArgs e)
        {

            influx17.addIMUs(dataProcessor.IMUS, dataProcessor.AmountIMU);
            influx17.addInclinos(dataProcessor.Inclinos, dataProcessor.AmountInclino);          

            dataProcessor.resetIMUs();
            dataProcessor.resetInclinos();

            //stopwatch.Stop();
            //Console.WriteLine("Time{0}",stopwatch.ElapsedMilliseconds);
            
            Console.WriteLine("amount packages{0}", testcounterAmountMessages);
            //Console.WriteLine("amount packages{0}", testcounterAmountMessages);
            Console.WriteLine("amount temperature{0}", tempCounter);
            tempCounter = 0;
            tempRead = 0;
            testcounterAmountMessages = 0;
            Baroread = false;


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

        //void dataReceiver(object sender, System.Timers.ElapsedEventArgs e)
        void dataReceiver()
        {
            while(AsyncReceive.MessageReadbuffer == AsyncReceive.dataMessageCounter)
            {
                Thread.Sleep(1);
            }
            Receive.dataMessage[] messages = AsyncReceive.messageBuffer[AsyncReceive.MessageReadbuffer];
           
            if (AsyncReceive.MessageReadbuffer == 999)
            {
                AsyncReceive.MessageReadbuffer = 0;
            }
            AsyncReceive.MessageReadbuffer++;

            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i].Sensor_Id == 0 || messages[i].Data_type == 0)
                {
                    break;
                }
                else if ((enums.Data_type)messages[i].Data_type == enums.Data_type.TEMP)
                {
                    if ((tempRead & (1 << messages[i].Sensor_Id)) == 0)
                    {
                        tempCounter++;
                        enums.IC_type ic_type = determineSensorType(messages[i].Sensor_Id);
                        influx17.addData(messages[i].Sensor_Id, (enums.Data_type)messages[i].Data_type, dataProcessor.calculateTempDegrees(messages[i].data, ic_type), ic_type); 
                        tempRead |= (1 << (messages[i].Sensor_Id));

                    }

                }
                else if ((enums.Data_type)messages[i].Data_type == enums.Data_type.BARO)
                {
                    if(!Baroread)
                    {
                        Baroread = true;
                        influx17.addData(messages[i].Sensor_Id, (enums.Data_type)messages[i].Data_type, messages[i].data, determineSensorType(messages[i].Sensor_Id));
                    }
                }
                else if((messages[i].Sensor_Id > 0) & (messages[i].Sensor_Id < 20)){
                    dataProcessor.addData(messages[i].Sensor_Id, messages[i].Data_type, messages[i].data);
                }
                else{
                    Console.WriteLine("Data Invalid: Sensor_Id {0}, Data type {1}, data {2}", messages[i].Sensor_Id, messages[i].Data_type, messages[i].data);
                }
            }

            testcounterAmountMessages++;
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
            if(Sensor_id >=1 && Sensor_id <= 8)
            {
                return enums.IC_type.BMI55;
            }
            else if (Sensor_id >= 9 && Sensor_id <= 16)
            {
                return enums.IC_type.SCA103T;
            }
            else if((enums.Sensor_Id)Sensor_id == enums.Sensor_Id.BMI085)
            {
                return enums.IC_type.BMI085;
            }
            else if((enums.Sensor_Id)Sensor_id == enums.Sensor_Id.LMS6DSO)
            {
                return enums.IC_type.LMS6DSO;
            }
            else
            {
                return 0;
            }
            
        }

        private void resetTempRead()
        {
            tempRead = 0;
        }

    }
}


