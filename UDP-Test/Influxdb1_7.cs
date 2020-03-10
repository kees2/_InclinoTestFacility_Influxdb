using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxDB.Collector;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;

namespace UDP_Test
{
    class Influxdb1_7
    {

        const int amount_BMI55 = 8;
        const int amount_inclino = 8;
        const int amount_BMI085 = 1;
        const int amount_LMS6DSO = 1;
        const int amount_MS5611 = 1;

        const int amount_IMU_datatypes = 6;
        const int amount_Inclino_datatypes = 2;

        string databaseAddress = "http://127.0.0.1:8086";
        string databaseName = "testdata";

        LineProtocolClient client;
        //LineProtocolPayload payload;

        public void initDB()
        {
            client = new LineProtocolClient(new Uri(databaseAddress), databaseName);
            //payload = new LineProtocolPayload();
        }

        public void sendData()
        {
            //client.WriteAsync(payload);
            //payload = new LineProtocolPayload();
        }

        public int addData(int sensor_id, enums.Data_type data_type, int data)
        {
            LineProtocolPayload payload = new LineProtocolPayload();
            var fields = new Dictionary<string, object>();

            fields.Add("Raw", data);

            var tags = new Dictionary<string, string>
            {
                    {"id", ((enums.Sensor_Id)sensor_id).ToString()},
                    {"ic_type", determineSensorType(sensor_id).ToString()},
                    {"data_type", data_type.ToString()},
            };
            payload.Add(new LineProtocolPoint("sensor_measurement", new ReadOnlyDictionary<string, object>(fields), tags));
            client.WriteAsync(payload);
            return 0;
        }

        public int addIMUs(IMU[] source, int amountIMUs)
        {
            LineProtocolPayload payload = new LineProtocolPayload();
            for (int i = 0; i < amountIMUs; i++)
            {
                for (int j = 0; j < amount_IMU_datatypes; j++)
                {
                    if(source[i].data[j].arraySize > 0)
                    {
                        LineProtocolPoint point = convertIMU(source[i].SensorId, (enums.Data_type)j, source[i].data[j]);
                        payload.Add(point);
                        Console.WriteLine("id: {0}, Data_type: {1}, data: {2}", source[i].SensorId, (enums.Data_type)j, source[i].data[j].determineMin().ToString("X"));
                    }
                    else
                    {
                        Console.WriteLine("Error Sensor {0} doesn't have data of type {1}", (enums.Sensor_Id)source[i].SensorId, (enums.Data_type)j);
                    }
                }
            }
            client.WriteAsync(payload);
            return 0;
        }


        public int addInclinos(Inclino[] source, int amountInclinos)
        {
            LineProtocolPayload payload = new LineProtocolPayload();
            LineProtocolPoint point;
            for (int i = 0; i < amountInclinos; i++)
            {
                if ((source[i].data[0].arraySize != 0) & (source[i].data[1].arraySize != 0)){//This should always be like this
                    point = convertIMU(source[i].SensorId, enums.Data_type.INCL_A, source[i].data[0]);
                    payload.Add(point);
                    point = convertIMU(source[i].SensorId, enums.Data_type.INCL_B, source[i].data[1]);
                    payload.Add(point);
                    for(int j = 0; j < source[i].data[0].arraySize; j++)
                    {
                        source[i].data[2].AddCalculatedDifferential(source[i].data[0].dataArray[j], source[i].data[1].dataArray[j]);
                    }
                    point = convertIMU(source[i].SensorId, enums.Data_type.INCL, source[i].data[2]);
                    payload.Add(point);
                }
            }
            client.WriteAsync(payload);
            return 0;
        }

        public static LineProtocolPoint convertIMU(int sensor_id, enums.Data_type data_type, Data data)//naam aanpassen naar iets algemeneererdere iets niet alleen IMU
        {
            var fields = new Dictionary<string, object>();

            fields.Add("Min_value", data.determineMin());
            fields.Add("Max_value", data.determineMax());
            fields.Add("AVG_value", data.calcAverage());
            fields.Add("ERROR_AVG_value", data.calcAverageError());

            if (fields.Count == 0)
            {
                return null;
            }

            var tags = new Dictionary<string, string>
            {
                    {"id", ((enums.Sensor_Id)sensor_id).ToString()},
                    {"ic_type", determineSensorType(sensor_id).ToString()},
                    {"data_type", data_type.ToString()},
            };
            return new LineProtocolPoint("IMU_measurement", new ReadOnlyDictionary<string, object>(fields), tags);
        }



        private static enums.IC_type determineSensorType(int inputId)
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

            return (enums.IC_type)5;
        }




    }
}
