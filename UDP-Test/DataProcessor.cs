using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class DataProcessor
    {
        private const int BMI085Id = 16;
        private const int LMS6DSOId = 17;

        //dit aanmaken in de constructor
        private const int amountIMU = 10;
        private IMU[] imus = new IMU[amountIMU];

        private const int highestDataId = 6;
        
        public IMU[] IMUS
        {
            get { return imus; }
            set { imus = IMUS; }
        }

        public DataProcessor()
        {
            
            for(int i = 0; i < amountIMU; i++)
            {
                imus[i] = new IMU();
            }
            imus[0].addData(1, 1);
            for (int i = 0; i < 8; i++)
            {
                imus[i].SensorId = i;
            }
            imus[8].SensorId = BMI085Id;
            imus[9].SensorId = LMS6DSOId;
        }

        public void calcDataIMUs()
        {
            for(int i = 0; i < amountIMU; i++)
            {
                imus[i].calcData();
            }
        }
        
        public void addData(int Sensor_Id, int Data_type, int data)
        {
            //0-7 = BMI055
            //8 = BMI085
            //9 = LMS6DSO

            if(0 < Sensor_Id & Sensor_Id < 8)
            {
                imus[Sensor_Id].addData(Data_type, data);
            }
            else if (Sensor_Id == 16)
            {
                imus[8].addData(Data_type, data);
            }
            else if (Sensor_Id == 17)
            {
                imus[9].addData(Data_type, data);
            }
            else
            {
                Console.WriteLine("SensorId does not point to an IMU");
            }

        }
    }
}
