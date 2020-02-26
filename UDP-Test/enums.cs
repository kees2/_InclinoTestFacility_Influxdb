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
            INCL_B = 8
        };

        public enum Sensor_type
        {
            GYRO_X = 0,
            GYRO_Y = 1,
            GYRO_Z = 2,
            ACC_X = 3,
            ACC_Y = 4,
            ACC_Z = 5,
            TEMP = 6,
            INCL_A = 7,
            INCL_B = 8
        };


    }
}
