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

        public void SendArrayToDatabase(int ic_number, enums.Data_type data_type, int data, int atribute)
        {

            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                // Write by LineProtocol
                string[] queryStrings = new String[amountPoints]; ;
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

        private enums.Data_type determineSensorType(int inputId)
        {
            const int amount_BMI55 = 8;
            const int amount_SCA103T = 8;
            const int amount_BMI085 = 1;
            const int amount_LMS6DSO = 1;
            const int amount_MS5611 = 1;
            int count = 0;
            //enums.Data_type type = 0;

 
            if (inputId >= 0 & inputId < amount_BMI55)
            {
                return (enums.Data_type)0;
            }
            count += amount_BMI55;
            if (inputId >= count & inputId < (count + amount_SCA103T))
            {
                return (enums.Data_type)1;
            }
            count += amount_SCA103T;
            if (inputId >= count & inputId < (count + amount_BMI085))
            {
                return (enums.Data_type)2;
            }
            count += amount_BMI085;
            if (inputId >= count & inputId < (count + amount_LMS6DSO))
            {
                return (enums.Data_type)3;
            }
            count += amount_LMS6DSO;
            if (inputId >= count & inputId < (count + amount_MS5611))
            {
                return (enums.Data_type)4;
            }

            return  (enums.Data_type)5;

        }
    }
}
