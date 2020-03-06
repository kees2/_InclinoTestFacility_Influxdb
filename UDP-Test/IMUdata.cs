using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class IMUdata
    {

        private const int bufferSize = 1000;
        public int[] dataArray { get; set; }
        public int arraySize { get; set; }

        public int[] DataArray { get; set; }


        public IMUdata(int dataType)
        {
            arraySize = 0;
            dataArray = new int[bufferSize];
        }

        public void reset()
        {
            arraySize = 0;
            Array.Clear(dataArray, 0, dataArray.Length);
        }

        public void addData(int data)
        {
            dataArray[arraySize] = data;
            arraySize++;
        }

        //als arraysize 0 is afvangen
        public int calcAverage()
        {
            int total = 0;
            for(int i = 0; i < arraySize; i++)
            {
                total += dataArray[i];
            }
            if(arraySize != 0)
            {
                return total / arraySize;
            }
            else
            {
                return 0;
            }
        }

        public int calcAverageError()
        {
            int total = 0;
            for (int i = 0; i < arraySize; i++)
            {
                total += Math.Abs(dataArray[i]);
            }
            if (arraySize != 0)
            {
                return total / arraySize;
            }
            else
            {
                return 0;
            }
        }

        public int determineMin()
        {
            if(arraySize != 0)
            {
                int minValue = int.MaxValue;
                for (int i = 0; i < arraySize; i++)
                {
                    if (dataArray[i] < minValue)
                    {
                        minValue = dataArray[i];
                    }
                }
                return minValue;
            }
            else
            {
                return 0;
            }
          
        }

        public int determineMax()
        {
            int maxValue = int.MinValue;
            for (int i = 0; i < arraySize; i++)
            {
                if(dataArray[i] > maxValue)
                {
                    maxValue = dataArray[i];
                }
            }
            return maxValue;
        }

    }
}
