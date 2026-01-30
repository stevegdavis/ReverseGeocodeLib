using ReverseGeocodeLib.Data;
using ReverseGeocodeLib.Interface;
using ReverseGeocodeLib.Models;
using System.Reflection;

namespace ReverseGeocodeLib;

public interface IReverseGeocodeService
{
    LocationInfo? FindCountry(GeoLocation location);

    //LocationInfo FindUSAState(GeoLocation location);

    LocationInfo? FindAreaData(GeoLocation location, List<AreaData> areaDataList);
}

public class ReverseGeocodeService : IReverseGeocodeService
{
    private static List<AreaData>? _countries;
    
    public IReverseGeocodeDataProvider? CountryDataProvider { get; set; }
    public IReverseGeocodeDataProvider? USAStateDataProvider { get; set; }

    public ReverseGeocodeService(List<AreaData> countries)
    {
        _countries = countries;        
        //this.USAStateDataProvider = new USAStateDataProvider();
    }

    public LocationInfo? FindCountry(GeoLocation location)
    {
        if (_countries == null) throw new Exception("No country data provider set. Set via 'CountryDataProvider' property.");
        return FindAreaData(location, _countries);// CountryDataProvider.Data);
    }

    //public LocationInfo FindUSAState(GeoLocation location)
    //{
    //    if (CountryDataProvider == null) throw new Exception("No usa state data provider set. Set via 'USAStateDataProvider' property.");
    //    return FindAreaData(location, USAStateDataProvider.Data);
    //}

    public LocationInfo? FindAreaData(GeoLocation location, List<AreaData> areaDataList)
    {
        var matchedAreaData = areaDataList.Find(areaData => IsLocationInArea(location, areaData));
        return matchedAreaData != null ? LocationInfo.FromAreaData(matchedAreaData) : null;
    }

    private bool IsLocationInArea(GeoLocation location, AreaData data)
    {
        return data.coordinates.Any(polygon =>
        {
            List<GeoLocation> locations = polygon.Select(point => new GeoLocation { Latitude = point[1], Longitude = point[0] }).ToList();
            return location.IsInPolygon(locations);
        });
    }
}
