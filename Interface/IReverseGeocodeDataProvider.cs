using ReverseGeocodeLib.Models;

namespace ReverseGeocodeLib.Interface;

public interface IReverseGeocodeDataProvider
{
    List<AreaData> Data { get; }
}
