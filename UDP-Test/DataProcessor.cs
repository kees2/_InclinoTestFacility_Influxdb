using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace UDP_Test
{
    public class DataProcessor
    {
        private const int sensorOffset = 1;


        private const int amountBMI055 = 8;
        private const int amountBMI085 = 1;
        private const int amountLMS6DSO = 1;

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
            int j = 0;
            imus = new IMU[amountIMU];
            inclinos = new Inclino[amountInclino];
            for (int i = 0; i < amountBMI055; i++)
            {
                imus[i] = new IMU(enums.IC_type.BMI55);
                imus[i].SensorId = i + 1;
            }
            for (int i = 0; i < amountBMI085; i++)
            {
                imus[amountBMI055 + i] = new IMU(enums.IC_type.BMI085);
                imus[amountBMI055 + i].SensorId = (int)enums.Sensor_Id.BMI085;
            }
            for (int i = 0; i < amountLMS6DSO; i++)
            {
                imus[amountBMI055 + amountBMI085 + i] = new IMU(enums.IC_type.LMS6DSO);
                imus[amountBMI055 + amountBMI085 + i].SensorId = (int)enums.Sensor_Id.LMS6DSO;
            }
            for (int i = (int)enums.Sensor_Id.SCA103T_0; i < (int)enums.Sensor_Id.SCA103T_0 + amountInclino; i++)
            {
                inclinos[j] = new Inclino(enums.IC_type.SCA103T);
                inclinos[j].SensorId = i;
                j++;
            }
        }

        public void addData(int Sensor_Id, int Data_type, int data)
        {
            //work in progress
            //for loop waarbij alle imu sensoren afgegaan worden
            //Deze if statements kunnen misschien beter in de DataHandler
            int index = Sensor_Id;
            if (
                (enums.Data_type)Data_type >= enums.Data_type.GYRO_X &&
                (enums.Data_type)Data_type <= enums.Data_type.ACC_Z)
            {
                imus[determineIndexIMU(Sensor_Id)].addIMUData(Data_type, data);
            }
            else if (
                (enums.Data_type)Data_type == enums.Data_type.INCL_A ||
                (enums.Data_type)Data_type == enums.Data_type.INCL_B)
            {
                inclinos[determineIndexInclino(Sensor_Id)].addInclinoData(Data_type, data);
                //stopwatch.Stop();
                //Console.WriteLine("Ms{0}", stopwatch.Elapsed.TotalMillisecondsMilliseconds);

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
            for (int i = 0; i < amountInclino; i++)
            {
                inclinos[i].resetInclinoData();
            }
        }

        //Convert a sensor id to an index for the inclinosensor
        public int determineIndexInclino(int Sensor_Id)
        {
            return Sensor_Id - amountBMI055 - sensorOffset;
        }

        //Convert a sensor id to an index for the IMUs
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

        public double calculateTempDegrees(int temp, enums.IC_type ic_type)
        {
            // BMI055
            //The slope of the temperature sensor is 0.5K/LSB, it's center temperature is 23 degrees Celcius when temp = 0x00
            //Temp value is 8 bits 
            if (ic_type == enums.IC_type.BMI55)
            {
                sbyte BMI055Temp = (sbyte)temp;
                return 23 + 0.5 * BMI055Temp;

            }
            else if (ic_type == enums.IC_type.BMI085)
            {
                // Temperature Sensor slope
                // typ = 0.125
                // Units K/LSB

                byte msb = (byte)(temp >> 8);
                byte lsb = (byte)(temp);

                int BMI085Temp_int11 = (msb * 8) + (lsb / 32);

                if (BMI085Temp_int11 > 1023)
                {
                    BMI085Temp_int11 -= 2048;
                }
                double test = BMI085Temp_int11 * 0.125 + 23;
                return BMI085Temp_int11 * 0.125 + 23;

            }
            else if (ic_type == enums.IC_type.LMS6DSO)
            {
                //16bit Resolution
                //Temperature sensitivity 256 LSB/°C
                //The output of the temperature sensor is 0 LSB (typ.) at 25 °C
                short LMS6DSOTemp_int16 = (short)temp;
                double Evert = 25 + (LMS6DSOTemp_int16 / 256);
                return 25 + (LMS6DSOTemp_int16 / 256);
            }

                return 1;
        }







    } 
    
}
