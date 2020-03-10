using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Test
{
    class Inclino
    {
        private const int amountinclinoAtributes = 3;//InclinoA & InclinoB & InclinoTotaal
        public InclinoData[] data = new InclinoData[amountinclinoAtributes];
        public int SensorId { get; set; }

        public Inclino()
        {
            for (int i = 0; i < amountinclinoAtributes; i++)
            {
                data[i] = new InclinoData();
            }
        }

        public void addInclinoData(int Data_type, int newData)
        {
            data[Data_type-6].addData(newData);
        }

        public void resetInclinoData()
        {
            for (int i = 0; i < amountinclinoAtributes; i++)
            {
                data[i].reset();
            }
        }

        public void calculateDifferential()
        {

        }


    }


}
