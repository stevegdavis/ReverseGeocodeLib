# SDSDevelopment.ReverseGeocode

Reverse geocode a country or city based on location point (latitude ; longitude). All code is offline — no API calls to map services necessary.

## Usage

Below is a basic example showing how to load the embedded country and city data and use the library to find the country and city for a given latitude/longitude. You must call the Load...Async methods before calling FindCountry / FindCity; otherwise the library will throw an exception.

```csharp
using System;
using System.Threading.Tasks;
using ReverseGeocodeLib;
using ReverseGeocodeLib.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Load embedded binary data (Data.countries.bin and Data.cities.bin are embedded resources)
        await ReverseGeocodeService.LoadCountriesAsync();
        await ReverseGeocodeService.LoadCitiesAsync();

        // Create a location (latitude, longitude)
        var location = new GeoLocation { Latitude = 51.5074, Longitude = -0.1278 }; // London

        // Find country and city
        var country = ReverseGeocodeService.FindCountry(location);
        var city = ReverseGeocodeService.FindCity(location);
        // Country can return more than one match if the point lies within multiple areas (for example, enclaves), so we take the first match here for demonstration.
        Console.WriteLine($"Country: {country[0].Name} (Id: {country[0].Id})");
        Console.WriteLine($"City: {city[0].Name} (Id: {city[0].Id})");
    }
}
```

Notes
- If no area matches the provided location, the returned LocationInfo will have empty `Id` and `Name` strings.
- The library uses a point-in-polygon algorithm to test whether the point lies inside an area's polygon(s).
- The embedded data files are expected under the assembly manifest resource path:
  `{AssemblyName}.Data.countries.bin` and `{AssemblyName}.Data.cities.bin`.
  - Ensure your .csproj embeds the binary resources (for example):
    ```xml
    <ItemGroup>
      <EmbeddedResource Include="Data\countries.bin" />
      <EmbeddedResource Include="Data\cities.bin" />
    </ItemGroup>
    ```
- The public entry points are:
  - ReverseGeocodeService.LoadCountriesAsync()
  - ReverseGeocodeService.LoadCitiesAsync()
  - ReverseGeocodeService.FindCountry(GeoLocation)
  - ReverseGeocodeService.FindCity(GeoLocation)

## Built from
https://github.com/InquisitorJax/Wibci.CountryReverseGeocode
