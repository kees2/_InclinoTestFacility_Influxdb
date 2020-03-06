using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class DataProcessor
    {
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
            for (int i = 0; i < amountIMU; i++)
            {
                imus[i].SensorId = i;
            }
        }
        
        public void addData(int Sensor_Id, int Data_type, int data)
        {
            //0-7 = BMI055
            //8 = BMI085
            //9 = LMS6DSO
            if (Sensor_Id < amountIMU)
            {
                imus[Sensor_Id].addData(Data_type, data);
            }
            else
            {
                Console.WriteLine("SensorId does not point to an IMU");
            }

        }

        public void resetIMUs()
        {
            for(int i = 0; i < amountIMU; i++)
            {
                imus[i].resetIMUData();
            }
        }
    }
}
