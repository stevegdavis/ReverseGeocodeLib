using ReverseGeocodeLib.Interface;
using ReverseGeocodeLib.Models;
using System.Reflection;

namespace ReverseGeocodeLib;

public interface IReverseGeocodeService
{
    static abstract LocationInfo FindCountry(GeoLocation location);
    static abstract LocationInfo FindCity(GeoLocation location);

    //LocationInfo FindUSAState(GeoLocation location);

    static abstract LocationInfo FindAreaData(GeoLocation location, List<AreaData> areaDataList);
}

public class ReverseGeocodeService : IReverseGeocodeService
{
    private static List<AreaData>? countries;
    private static List<AreaData>? cities;
    public static async Task LoadCountriesAsync()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.countries.bin")!;
        countries = await Deserializer.DeserializeAsync(stream);
    }
    public static async Task LoadCitiesAsync()
    {
        
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Data.cities.bin")!;
        cities = await Deserializer.DeserializeAsync(stream);
    }
    public IReverseGeocodeDataProvider? CountryDataProvider { get; set; }
    public IReverseGeocodeDataProvider? CityDataProvider { get; set; }
    public IReverseGeocodeDataProvider? USAStateDataProvider { get; set; }

    public static LocationInfo FindCountry(GeoLocation location)
    {
        if (countries == null) throw new Exception("No country data provider set. Set via 'CountryDataProvider' property.");
        return FindAreaData(location, countries);// CountryDataProvider.Data);
    }
    public static LocationInfo FindCity(GeoLocation location)
    {
        if (cities == null) throw new Exception("No city data provider set. Set via 'CityDataProvider' property.");
        return FindAreaData(location, cities);// CountryDataProvider.Data);
    }

    //public LocationInfo FindUSAState(GeoLocation location)
    //{
    //    if (CountryDataProvider == null) throw new Exception("No usa state data provider set. Set via 'USAStateDataProvider' property.");
    //    return FindAreaData(location, USAStateDataProvider.Data);
    //}

    public static LocationInfo FindAreaData(GeoLocation location, List<AreaData> areaDataList)
    {
        var matchedAreaData = areaDataList.Find(areaData => IsLocationInArea(location, areaData));
        return matchedAreaData != null ? LocationInfo.FromAreaData(matchedAreaData) : new LocationInfo("", "");// null;
    }

    private static bool IsLocationInArea(GeoLocation location, AreaData data)
    {
        return data.coordinates.Any(polygon =>
        {
            List<GeoLocation> locations = polygon.Select(point => new GeoLocation { Latitude = point[1], Longitude = point[0] }).ToList();
            return location.IsInPolygon(locations);
        });
    }
}
