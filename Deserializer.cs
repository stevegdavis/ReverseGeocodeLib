using ReverseGeocodeLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReverseGeocodeLib;

public class Deserializer
{
    public static async Task<List<AreaData>> DeserializeAsync(string path)
    {
        // Placeholder for deserialization logic
        // Read binary file back into data
        List<AreaData> data = new List<AreaData>();

        byte[] fileBytes = await File.ReadAllBytesAsync(path);

        using (var memoryStream = new MemoryStream(fileBytes))
        using (var binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
        {
            // Read count of countries
            int countryCount = binaryReader.ReadInt32();

            for (int i = 0; i < countryCount; i++)
            {
                // Read country code
                string id = binaryReader.ReadString();

                // Read country name
                string name = binaryReader.ReadString();

                // Read polygons
                int ringCount = binaryReader.ReadInt32();
                var coordinates = new List<List<List<double>>>();

                for (int j = 0; j < ringCount; j++)
                {
                    int coordinateCount = binaryReader.ReadInt32();
                    var ring = new List<List<double>>();

                    for (int k = 0; k < coordinateCount; k++)
                    {
                        double longitude = binaryReader.ReadDouble();
                        double latitude = binaryReader.ReadDouble();
                        ring.Add(new List<double> { longitude, latitude });
                    }

                    coordinates.Add(ring);
                }

                data.Add(new AreaData(id, name, coordinates));
            }
        }
        return data;
    }
    public static async Task<List<AreaData>> DeserializeAsync(Stream stream)
    {
        List<AreaData> data = new List<AreaData>();

        byte[] fileBytes = new byte[stream.Length];
        await stream.ReadExactlyAsync(fileBytes.AsMemory(0, (int)stream.Length));
        using (var memoryStream = new MemoryStream(fileBytes))
        using (var binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
        {
            // Read count of countries
            int countryCount = binaryReader.ReadInt32();

            for (int i = 0; i < countryCount; i++)
            {
                // Read country code
                string id = binaryReader.ReadString();

                // Read country name
                string name = binaryReader.ReadString();

                // Read polygons
                int ringCount = binaryReader.ReadInt32();
                var coordinates = new List<List<List<double>>>();

                for (int j = 0; j < ringCount; j++)
                {
                    int coordinateCount = binaryReader.ReadInt32();
                    var ring = new List<List<double>>();

                    for (int k = 0; k < coordinateCount; k++)
                    {
                        double longitude = binaryReader.ReadDouble();
                        double latitude = binaryReader.ReadDouble();
                        ring.Add(new List<double> { longitude, latitude });
                    }

                    coordinates.Add(ring);
                }

                data.Add(new AreaData(id, name, coordinates));
            }
        }
        return data;
    }
}
