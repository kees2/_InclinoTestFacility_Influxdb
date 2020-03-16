using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class DataProcessor
    {
        private const int sensorOffset = 1;


        private const int amountBMI055 = 8;
        private const int amountBMI085 = 0;
        private const int amountLMS6DSO = 0;

        const int amountIMU = amountBMI055 + amountBMI085 + amountLMS6DSO;

        private const int amountInclino = 8;

        public int AmountInclino
        {
            get { return amountInclino; }
        }
        public int AmountIMU
        {
            get { return amountIMU; }
        }

        private IMU[] imus;
        private Inclino[] inclinos;

        public Inclino[] Inclinos
        {
            get { return inclinos; }
            set { inclinos = Inclinos; }
        }

        public IMU[] IMUS
        {
            get { return imus; }
            set { imus = IMUS; }
        }



        public DataProcessor()
        {
            //Initialize the amount of IMU that are needed 

            imus = new IMU[amountIMU];
            inclinos = new Inclino[amountInclino];
            for (int i = 0; i < amountBMI055; i++)
            {
                imus[i] = new IMU(enums.IC_type.BMI55);
                imus[i].SensorId = i;
            }
            for (int i = 0; i < amountBMI085; i++)
            {
                imus[i] = new IMU(enums.IC_type.BMI085);
                imus[i].SensorId = i;
            }
            for (int i = 0; i < amountLMS6DSO; i++)
            {
                imus[i] = new IMU(enums.IC_type.LMS6DSO);
                imus[i].SensorId = i;
            }
            for (int i = 0; i < amountInclino; i++)
            {
                inclinos[i] = new Inclino(enums.IC_type.SCA103T);
                inclinos[i].SensorId = i + amountIMU;
            }
        }

        public void addData(int Sensor_Id, int Data_type, int data)
        {
            //work in progress
            //for loop waarbij alle imu sensoren afgegaan worden
            //Deze if statements kunnen misschien beter in de DataHandler
            int index = Sensor_Id;
            if (
                (enums.Data_type)Data_type == enums.Data_type.GYRO_X ||
                (enums.Data_type)Data_type == enums.Data_type.GYRO_Y ||
                (enums.Data_type)Data_type == enums.Data_type.GYRO_Z ||
                (enums.Data_type)Data_type == enums.Data_type.ACC_X ||
                (enums.Data_type)Data_type == enums.Data_type.ACC_Y ||
                (enums.Data_type)Data_type == enums.Data_type.ACC_Z)
            {
                imus[determineIndexIMU(Sensor_Id)].addIMUData(Data_type, data);
            }
            else if (
                (enums.Data_type)Data_type == enums.Data_type.INCL_A ||
                (enums.Data_type)Data_type == enums.Data_type.INCL_B)
            {

                inclinos[determineIndexInclino(Sensor_Id)].addInclinoData(Data_type, data);
            }
            else
            {
                Console.WriteLine("Error Sensor with this Id does not exist");
            }

        }

        public void resetIMUs()
        {
            for (int i = 0; i < amountIMU; i++)
            {
                imus[i].resetIMUData();
            }
        }

        public void resetInclinos()
        {
            for (int i = 0; i < amountIMU; i++)
            {
                inclinos[i].resetInclinoData();
            }
        }

        public int determineIndexInclino(int Sensor_Id)
        {
            return Sensor_Id - amountBMI055 - sensorOffset;
        }

        public int determineIndexIMU(int Sensor_Id)
        {
            int index = 0;


            if ((enums.Sensor_Id)Sensor_Id == enums.Sensor_Id.BMI085)
            {
                index = amountBMI055;
            }
            else if ((enums.Sensor_Id)Sensor_Id == enums.Sensor_Id.LMS6DSO)
            {
                index = amountBMI055 + amountBMI085;
            }
            else
            {
                index = Sensor_Id - sensorOffset;
            }
            return index;
        }








    } 
    
}
