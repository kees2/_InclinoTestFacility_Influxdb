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


        const int amountPoints = 60;
        static InfluxDBClient influxDBClient;
        string Bucket = "Bridgescout";
        string Org_id = "cda037e02dd0cf48";
        char[] Token = "VkhsECxCQoHqFMvMmuU3Ui3s5XL1q3DANTDKuXJEvqc6smmx3G713LBeUeMiaTQzHOvOvAispoYJRfccdtxSSQ==".ToCharArray();
        //WriteApi writeApi;

        public enum IC_type
        {
            BMI55 = 0,
            SCA103T = 1,
            BMI085 = 2,
            LMS6DSO = 3,
            MS5611 = 4
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



        public void initDB()
        {
            influxDBClient = InfluxDBClientFactory.Create("http://localhost:9999", Token);
            System.IO.File.WriteAllText(@"C:\Users\Kees\Desktop\cSharp\UDP-Test\UDP-Test\queries.txt", string.Empty);
            //writeApi = influxDBClient.GetWriteApi();
        }

        public void SendToDatabase(IC_type ic_type, int ic_number, Data_type data_type, int data)
        {

            using (WriteApi writeApi = influxDBClient.GetWriteApi())
            {
                // Write by LineProtocol
                string[] queryStrings = new String[amountPoints]; ;
                for (int i = 0; i < amountPoints; i++)
                {
                    queryStrings[i] = "sensor_data,id=" + ic_number + ",ic_type=" + ic_type + ",data_type=" + data_type + " data=" + data;
                }
                writeApi.WriteRecords(Bucket, Org_id, WritePrecision.Ns, queryStrings);
            }
            //influxDBClient.Dispose();
        }

    }
}
