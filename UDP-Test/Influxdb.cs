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


namespace UDP_Test
{
    class Influxdb
    {
        private const int amountPoints = 60;
        private static InfluxDBClient influxDBClient;
        private string Bucket = "Bridgescout";
        private string Org_id = "cda037e02dd0cf48";
        private char[] Token = "VkhsECxCQoHqFMvMmuU3Ui3s5XL1q3DANTDKuXJEvqc6smmx3G713LBeUeMiaTQzHOvOvAispoYJRfccdtxSSQ==".ToCharArray();

        public void initDB()
        {
            influxDBClient = InfluxDBClientFactory.Create("http://localhost:9999", Token);
        }

        public void sendIMUData(IMU[] imus, int amountImus, int amountDataTypes, int amountAtributes)
        {
            //influx.SendToDatabase(dataProcessor.IMUS[i].data[j].determineMin(), (enums.Data_type)j, (byte)dataProcessor.IMUS[i].SensorId, 1);//Nummers verranderen naar enums
            int dataAmount = amountImus * amountDataTypes * amountAtributes;
            string[] queryStrings = new String[dataAmount]; ;
            int index = 0;
            int indexi = 0;
            int indexj = 0;
            for (int i = 0; i < amountDataTypes*amountImus*amountAtributes; i+=(amountDataTypes*amountAtributes))
            {
                indexj = 0;
                for (int j = 0; j < amountDataTypes* amountAtributes; j+= amountAtributes)
                {
                    for (int k = 0; k < amountAtributes; k++)
                    {
                        index = i+j+k;
                        queryStrings[index] = "sensor_data,id=" + imus[indexi].SensorId + ",ic_type=" + determineSensorType(imus[indexi].SensorId) + ",data_type=" + (enums.Data_type)indexj +
                            " data=" + imus[indexi].data[indexj].determineMin() + " attribute=" + (k+1);//add one to k because value 0 is the standard value
                    }
                    indexj++;
                }
                indexi++;
            }
            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                writeApi.WriteRecords(Bucket, Org_id, WritePrecision.Ns, queryStrings);
                influxDBClient.Dispose();
            }
        }

        public void SendArrayToDatabase(int ic_number, enums.Data_type data_type, int data, int atribute)
        {

            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                // Write by LineProtocol
                string[] queryStrings = new String[amountPoints];
                for (int i = 0; i < amountPoints; i++)
                {
                    queryStrings[i] = "sensor_data,id=" + ic_number + ",ic_type=" + determineSensorType(ic_number) + ",data_type=" + data_type + " data=" + data;
                }
                writeApi.WriteRecords(Bucket, Org_id, WritePrecision.Ns, queryStrings);
            }
            influxDBClient.Dispose();
        }

        public void SendToDatabase(int ic_number, enums.Data_type data_type, int data, int atribute)
        {

            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                // Write by LineProtocol
                string queryString = "sensor_data,id=" + ic_number + ",ic_type=" + determineSensorType(ic_number) + ",data_type=" + data_type + " data=" + data;
                writeApi.WriteRecord(Bucket, Org_id, WritePrecision.Ns, queryString);
            }
            influxDBClient.Dispose();
        }


        public void SendToDatabasePoint()
        {
            using (var writeApi = influxDBClient.GetWriteApi())
            {
                var point = PointData.Measurement("temperature")
                .Tag("location", "west")
                .Field("value", 55D)
                .Timestamp(DateTime.UtcNow.AddSeconds(-10), WritePrecision.Ns);

                writeApi.WritePoint(Bucket, Org_id, point);

            }
        }   

        private enums.IC_type determineSensorType(int inputId)
        {
            const int amount_BMI55 = 8;
            const int amount_SCA103T = 8;
            const int amount_BMI085 = 1;
            const int amount_LMS6DSO = 1;
            const int amount_MS5611 = 1;
            int count = 0;
 
            if (inputId >= 0 & inputId < amount_BMI55)
            {
                return (enums.IC_type)0;
            }
            count += amount_BMI55;
            if (inputId >= count & inputId < (count + amount_SCA103T))
            {
                return (enums.IC_type)1;
            }
            count += amount_SCA103T;
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
