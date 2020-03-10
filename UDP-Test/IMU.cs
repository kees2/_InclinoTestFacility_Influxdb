using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class IMU
    {
        
        private const int amountIMUAtributes = 6;
        public IMUData[] data = new IMUData[amountIMUAtributes];
        public int SensorId{ get; set; }       

        public IMU()
        {
            for(int i = 0; i < amountIMUAtributes; i++)
            {
                data[i] = new IMUData();
            }
        }

        public void addIMUData(int Data_type, int newData)
        {
            data[Data_type].addData(newData);
        }
        
        public void resetIMUData()
        {
            for (int i = 0; i < amountIMUAtributes; i++)
            {
                data[i].reset();
            }
        }



        
    }
}

