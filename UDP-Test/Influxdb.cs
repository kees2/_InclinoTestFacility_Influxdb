using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using Task = System.Threading.Tasks.Task;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using System.Threading;
using System.Diagnostics;
using System.Globalization;


namespace UDP_Test
{
    class Influxdb
    {
        const int amount_BMI55 = 8;
        const int amount_inclino = 8;
        const int amount_BMI085 = 1;
        const int amount_LMS6DSO = 1;
        const int amount_MS5611 = 1;
        private static InfluxDBClient influxDBClient;
        private readonly string Bucket = "Bridgescout";
        private readonly string Org_id = "SensorMaritime";
        private readonly char[] Token = "gKTRy4j4gd49t8QKbtBcXTn70m-17qBDySij48T2eXWqWCjEmmMFVehBkm-0wE4fqcKO7h2i8JcXlzIQzD_HwQ==".ToCharArray();
        int dataCounter = 0;


        public void initDB()
        {
            influxDBClient = InfluxDBClientFactory.Create("http://localhost:9999", Token);
        }

        public void sendIMUData(IMU[] imus, int amountImus, int amountDataTypes, int amountAtributes)
        {
            int dataAmount = amountImus * amountDataTypes * amountAtributes;
            string[] queryStrings = new String[dataAmount];
            int index = 0;
            int indexi = 0;
            int indexj = 0;
            int data;

            for (int i = 0; i < amountDataTypes*amountImus*amountAtributes; i+=(amountDataTypes*amountAtributes))
            {
                indexj = 0;
                for (int j = 0; j < amountDataTypes* amountAtributes; j+= amountAtributes)
                {

                    for (int k = 0; k < amountAtributes; k++)
                    {
                        index = i+j+k;
                        switch (k)
                        {
                            case 0:
                                data = imus[indexi].data[indexj].determineMin();
                            break;
                            case 1:
                                data = imus[indexi].data[indexj].determineMax();
                            break;
                            case 2:
                                data = imus[indexi].data[indexj].calcAverage();
                            break;
                            case 3:
                                data = imus[indexi].data[indexj].calcAverageError();
                            break;
                            default:
                                Console.WriteLine("Not a valid attribute value");
                                data = 404;
                            break;
                        }
                        queryStrings[index] = "sensor_data,id=" + (enums.Sensor_Id)imus[indexi].SensorId + ",ic_type=" + determineSensorType(imus[indexi].SensorId) + ",data_type=" + (enums.Data_type)indexj +
                        ",attribute=" + (enums.Atribute_type)(k+1) + " data=" + data;
                    }
                    indexj++;
                }
                indexi++;
            }
            dataCounter++;
            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                writeApi.WriteRecords(Bucket, Org_id, WritePrecision.Ns, queryStrings);
            }
        }

        public void SendArrayToDatabase(int ic_number, enums.Data_type data_type, int data, int atribute, int amount)
        {

            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                // Write by LineProtocol
                string[] queryStrings = new String[amount];
                for (int i = 0; i < amount; i++)
                {
                    queryStrings[i] = "sensor_data,id=" + (enums.Sensor_Id)ic_number + ",ic_type=" + determineSensorType(ic_number) + ",data_type=" + data_type + " data=" + data;
                }
                writeApi.WriteRecords(Bucket, Org_id, WritePrecision.Ns, queryStrings);
            }
        }

        public void SendToDatabase(int ic_number, enums.Data_type data_type, int data, enums.Atribute_type atribute)
        {

            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                // Write by LineProtocol
                string queryString = "sensor_data,id=" + (enums.Sensor_Id)ic_number + ",ic_type=" + determineSensorType(ic_number) + ",data_type=" + data_type + " data=" + data;
                writeApi.WriteRecord(Bucket, Org_id, WritePrecision.Ns, queryString);
            }
        }

        private enums.IC_type determineSensorType(int inputId)
        {
            int count = 0;
 
            if (inputId < amount_BMI55)
            {
                return (enums.IC_type)0;
            }
            count += amount_BMI55;
            if (inputId >= count & inputId < (count + amount_inclino))
            {
                return (enums.IC_type)1;
            }
            count += amount_inclino;
            if (inputId >= count & inputId < (count + amount_BMI085))
            {
                return (enums.IC_type)2;
            }
            count += amount_BMI085;
            if (inputId >= count & inputId < (count + amount_LMS6DSO))
            {
                return (enums.IC_type)3;
            }
            count += amount_LMS6DSO;
            if (inputId >= count & inputId < (count + amount_MS5611))
            {
                return (enums.IC_type)4;
            }

            return  (enums.IC_type)5;

        }
    }
}
