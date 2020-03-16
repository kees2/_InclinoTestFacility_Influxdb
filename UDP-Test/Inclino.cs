using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    public class Inclino
    {
        private const int amountInclinoAtributes = 3;//InclinoA & InclinoB & InclinoTotaal
        public InclinoData[] data = new InclinoData[amountInclinoAtributes];
        public int SensorId { get; set; }
        public enums.IC_type icType;

        public Inclino(enums.IC_type type)
        {
            icType = type;
            for (int i = 0; i < amountInclinoAtributes; i++)
            {
                data[i] = new InclinoData();
            }
        }

        public void addInclinoData(int Data_type, int newData)
        {
            data[Data_type-7].addData(newData);//min 6 misschien weg
        }

        public void resetInclinoData()
        {
            for (int i = 0; i < amountInclinoAtributes; i++)
            {
                data[i].reset();
            }
        }

        public void calculateAllDifferential()
        {
            int smallestArraySize = 0;
            if (data[0].arraySize < data[1].arraySize)
            {
                smallestArraySize = data[0].arraySize;
            }
            else
            {
                smallestArraySize = data[1].arraySize;
            }

            for (int i = 0; i < smallestArraySize; i++)
            {
                data[2].AddCalculatedDifferential(data[0].dataArray[i], data[1].dataArray[i]);
            }
        }


    }


}
