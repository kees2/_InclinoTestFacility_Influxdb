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
        public int[] dataArray { get; set; }//Dit is volgens https://www.w3schools.com/cs/cs_properties.asp hetzelfde als een normale getter setter
        private int arraySize;

        private int min;
        private int max;
        private int avg;
        private int avgerror;

        public int[] DataArray { get; set; }

        public int Min { get; set; }
        public int Max { get; set; }
        public int Avg { get; set; }
        public int Avgerror { get; set; }

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

        public int calcAverage()
        {
            int total = 0;
            for(int i = 0; i < arraySize; i++)
            {
                total += dataArray[i];
            }
            return total / arraySize;
        }

        public int calcAverageError()
        {
            int total = 0;
            for (int i = 0; i < arraySize; i++)
            {
                total = Math.Abs(dataArray[i]);
            }
            return total / arraySize;
        }

        public int determineMin()
        {
            int minValue = 0;
            for (int i = 0; i < arraySize; i++)
            {
                if (dataArray[i] < minValue)
                {
                    minValue = dataArray[i];
                }
            }
            return minValue;
        }
        public int determineMax()
        {
            int maxValue = 0;
            for (int i = 0; i < arraySize; i++)
            {
                if(dataArray[i] > maxValue)
                {
                    maxValue = dataArray[i];
                }
            }
            return maxValue;
        }

        public void calculateData()
        {
            min = determineMin();
            max = determineMax();
            avg = calcAverage();
            avgerror = calcAverageError();
        }

    }
}
