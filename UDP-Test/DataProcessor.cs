using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class DataProcessor
    {

        private int amountIMU;
        private int amountInclino;
        private IMU[] imus;
        private Inclino[] inclinos;

        private const int highestDataId = 6;
        
        public IMU[] IMUS
        {
            get { return imus; }
            set { imus = IMUS; }
        }

        public DataProcessor(int amountIMUSetValue, int amountInclinoSetValue)
        {
            //Initialize the amount of IMU that are needed 
            amountIMU = amountIMUSetValue;
            amountInclino = amountInclinoSetValue;
            imus = new IMU[amountIMU];
            inclinos = new Inclino[amountInclino];
            for (int i = 0; i < amountIMU; i++)
            {
                imus[i] = new IMU();
                imus[i].SensorId = i;
            }
            for (int i = 0; i < amountInclinoSetValue; i++)
            {
                inclinos[i] = new Inclino();
                inclinos[i].SensorId = i + amountIMU;
            }
        }
        
        public void addData(int Sensor_Id, int Data_type, int data)
        {
            //work in progress
            //for loop waarbij alle imu sensoren afgegaan worden
            //Deze if statements kunnen misschien beter in de DataHandler
            if (
                (enums.Data_type)Data_type == enums.Data_type.GYRO_X ||
                (enums.Data_type)Data_type == enums.Data_type.GYRO_Y ||
                (enums.Data_type)Data_type == enums.Data_type.GYRO_Z ||
                (enums.Data_type)Data_type == enums.Data_type.ACC_X ||
                (enums.Data_type)Data_type == enums.Data_type.ACC_Y ||
                (enums.Data_type)Data_type == enums.Data_type.ACC_Z )
            {
                imus[Sensor_Id].addIMUData(Data_type, data);
            }
            else if(
                (enums.Data_type)Data_type == enums.Data_type.INCL_A ||
                (enums.Data_type)Data_type == enums.Data_type.INCL_B)
            {
                inclinos[Sensor_Id - amountIMU].addInclinoData(Data_type, data);
            }
            else
            {
                Console.WriteLine("Error SensorId is to high");
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
