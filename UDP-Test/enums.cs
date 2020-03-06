using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class enums
    {

        public enum IC_type
        {
            BMI55 = 0,
            SCA103T = 1,
            BMI085 = 2,
            LMS6DSO = 3,
            MS5611 = 4,
            undefined = 5
        };

        public enum Data_type
        {
            GYRO_X = 0,
            GYRO_Y = 1,
            GYRO_Z = 2,
            ACC_X = 3,
            ACC_Y = 4,
            ACC_Z = 5,
            TEMP = 6,
            INCL_A = 7,
            INCL_B = 8,
            INCL = 9,
            BARO = 10
        };

        public enum Atribute_type
        {
            Raw = 0,
            Min_value = 1,
            Max_value = 2,
            AVG = 3,
            ERROR_AVG = 4
        };

        public enum Sensor_Id
        {
            BMI055_0 = 0,
            BMI055_1 = 1,
            BMI055_2 = 2,
            BMI055_3 = 3,
            BMI055_4 = 4,
            BMI055_5 = 5,
            BMI055_6 = 6,
            BMI055_7 = 7,
            SCA103T_0 = 8,
            SCA103T_1 = 9,
            SCA103T_2 = 10,
            SCA103T_3 = 11,
            SCA103T_4 = 12,
            SCA103T_5 = 13,
            SCA103T_6 = 14,
            SCA103T_7 = 15,
            BMI085 = 16,
            LMS6DSO = 17,
            MS5611_01BA03 = 18
        };


    }
}
