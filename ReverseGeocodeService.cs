using ReverseGeocodeLib.Models;
using System.Reflection;
using System.Text;

namespace ReverseGeocodeLib;

public class ReverseGeocodeService
{
    
    private static List<AreaData>? countries;
    private static List<AreaData>? cities;
    private static List<AreaData>? usastates;
    private static List<AreaData>? francedepartments;
    private static List<AreaData>? franceregions;
    private static List<AreaData>? ukcounties;
    private static List<AreaData>? netherlandsprovinces;
    public static async Task LoadCountriesAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.countries.bin")!;
        countries = await DeserializeAsync(stream);
    }
    public static async Task LoadCitiesAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.cities.bin")!;
        cities = await DeserializeAsync(stream);
    }
    public static async Task LoadUSAStatesAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.usa-states.bin")!;
        usastates = await DeserializeAsync(stream);
    }
    public static async Task LoadFranceDepartmentsAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.france-departments.bin")!;
        francedepartments = await DeserializeAsync(stream);
    }
    public static async Task LoadFranceRegionsAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.france-regions.bin")!;
        franceregions = await DeserializeAsync(stream);
    }
    public static async Task LoadUKCountiesAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.gb.bin")!;
        ukcounties = await DeserializeAsync(stream);
    }
    public static async Task LoadNetherlandsProvincesAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.netherlands-provinces.bin")!;
        netherlandsprovinces = await DeserializeAsync(stream);
    }
    public static List<(string, string)> FindCountry(double lat, double lon)
    {
        if (countries == null) throw new Exception("No country data provider set. Set via 'LoadCountriesAsync' async method.");
        return FindAreasByPoint(countries, lat, lon);        
    }
    public static List<(string, string)> FindUSAState(double lat, double lon)
    {
        if (usastates == null) throw new Exception("No usa-state data provider set. Set via 'LoadUSAStatesAsync' async method.");
        return FindAreasByPoint(usastates, lat, lon);
    }
    public static List<(string, string)> FindCity(double lat, double lon)
    {
        if (cities == null) throw new Exception("No city data provider set. Set via 'LoadCitiesAsync' property.");
        return FindAreasByPoint(cities, lat, lon);

    }
    public static List<(string, string)> FindFranceDepartment(double lat, double lon)
    {
        if (francedepartments == null) throw new Exception("No france department data provider set. Set via 'LoadFranceDepartmentsAsync' property.");
        return FindAreasByPoint(francedepartments, lat, lon);

    }
    public static List<(string, string)> FindFranceRegion(double lat, double lon)
    {
        if (franceregions == null) throw new Exception("No france region data provider set. Set via 'LoadFranceRegionsAsync' property.");
        return FindAreasByPoint(franceregions, lat, lon);

    }
    public static List<(string, string)> FindUKCounty(double lat, double lon)
    {
        if (ukcounties == null) throw new Exception("No uk county data provider set. Set via 'LoadUKCountiesAsync' property.");
        return FindAreasByPoint(ukcounties, lat, lon);
    }
    public static List<(string, string)> FindNetherlandsProvince(double lat, double lon)
    {
        if (netherlandsprovinces == null) throw new Exception("No netherlands province data provider set. Set via 'LoadNetherlandsProvincesAsync' property.");
        return FindAreasByPoint(netherlandsprovinces, lat, lon);
    }
    static List<(string Id, string Name)> FindAreasByPoint(List<AreaData> areaDataList, double latitude, double longitude)
    {
        var results = new List<(string Id, string Name)>();

        foreach (var area in areaDataList)
        {
            if (area.ContainsPoint(latitude, longitude))
            {
                results.Add((area.Id, area.Name));
            }
        }
        return results;
    }
    // Method to deserialize binary file to List<AreaData>
    static async Task<List<AreaData>> DeserializeAsync(string filePath)
    {
        var areaDataList = new List<AreaData>();

        using (var fs = new FileStream(filePath, FileMode.Open))
        using (var reader = new BinaryReader(fs))
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string id = reader.ReadString();
                string name = reader.ReadString();

                int coordCount = reader.ReadInt32();
                var coordinates = new List<List<List<double>>>();

                for (int j = 0; j < coordCount; j++)
                {
                    int ringCount = reader.ReadInt32();
                    var ring = new List<List<double>>();

                    for (int k = 0; k < ringCount; k++)
                    {
                        double lon = reader.ReadDouble();
                        double lat = reader.ReadDouble();
                        ring.Add(new List<double> { lon, lat });
                    }

                    coordinates.Add(ring);
                }

                areaDataList.Add(new AreaData(id, name, coordinates));
            }
        }
        return areaDataList;
    }
    public static async Task SerializeAsync(List<AreaData> areaDataList, string outputPath)
    {
        using (var fs = new FileStream(outputPath, FileMode.Create))
        using (var writer = new BinaryWriter(fs))
        {
            writer.Write(areaDataList.Count);

            foreach (var area in areaDataList)
            {
                writer.Write(area.Id);
                writer.Write(area.Name);

                var coords = area.Coordinates;
                writer.Write(coords.Count);

                foreach (var ring in coords)
                {
                    writer.Write(ring.Count);
                    foreach (var point in ring)
                    {
                        writer.Write(point[0]);
                        writer.Write(point[1]);
                    }
                }
            }
        }
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
