# ReverseGeocodeLib

A fast, offline .NET library for reverse geocoding - convert latitude and longitude coordinates into country, city, state, region, county and province names. No API calls required - all data is embedded and processed locally.

## Features

- **Offline Reverse Geocoding**: Convert latitude/longitude to geographic location names
- **Multiple Geographic Levels**: Countries, cities, USA states, French regions/departments, UK counties, Netherlands provinces
- **High Performance**: Uses optimized point-in-polygon algorithm
- **Zero Dependencies**: No external API calls or network requests
- **Embedded Data**: All geographic data included - ready to use out of the box
- **C# / .NET**: Built for modern .NET applications

## Supported Geocoding Lookups

- **Countries** - Find country by coordinates
- **Cities** - Identify city by latitude/longitude
- **USA States** - Reverse geocode US state location
- **France** - Regional (regions) and departmental divisions
- **UK** - County-level geographic lookup
- **Netherlands** - Provincial geographic identification

## Installation

Add ReverseGeocodeLib to your .NET project. Ensure your `.csproj` includes embedded resources:

```xml
<ItemGroup>
  <EmbeddedResource Include="Data\countries.bin" />
  <EmbeddedResource Include="Data\cities.bin" />
  <EmbeddedResource Include="Data\usa_states.bin" />
  <EmbeddedResource Include="Data\france_departments.bin" />
  <EmbeddedResource Include="Data\france_regions.bin" />
  <EmbeddedResource Include="Data\uk_counties.bin" />
  <EmbeddedResource Include="Data\netherlands_provinces.bin" />
</ItemGroup>
```

## Usage

Below is a basic example showing how to load the embedded country and city data and use the library to find the country and city for a given latitude/longitude. You must call the `Load...Async` methods first to initialize the geographic data.

```csharp
using System;
using System.Threading.Tasks;
using ReverseGeocodeLib;

class Program
{
    static async Task Main(string[] args)
    {
        // Load embedded binary data (these should be embedded as resources)
        await ReverseGeocodeService.LoadCountriesAsync();
        await ReverseGeocodeService.LoadCitiesAsync();

        // Specify coordinates (latitude, longitude). Example: London
        double latitude = 51.5074;
        double longitude = -0.1278;

        // Find country and city
        var countries = ReverseGeocodeService.FindCountry(latitude, longitude);
        var cities = ReverseGeocodeService.FindCity(latitude, longitude);

        // The methods return a list of tuples (Id, Name)
        if (countries.Count > 0)
            Console.WriteLine($"Country: {countries[0].Name} (Id: {countries[0].Id})");
        else
            Console.WriteLine("No country match.");

        if (cities.Count > 0)
            Console.WriteLine($"City: {cities[0].Name} (Id: {cities[0].Id})");
        else
            Console.WriteLine("No city match.");

        // With an enclave the country could have more than 1 result, no way to tell which is correct
        if (countries == null || countries.Count == 0)
        {
            Console.WriteLine("Country: Not found");
        }
        else
        {
            if (countries.Count > 1)
                Console.WriteLine($"Countries: {countries[0].Name} or {countries[1].Name}");
            else
                Console.WriteLine($"Country: {countries[0].Name}");
        }
    }
}
```

### Available API Methods

- `ReverseGeocodeService.LoadCountriesAsync()` - `FindCountry(latitude, longitude)`
- `ReverseGeocodeService.LoadCitiesAsync()` - `FindCity(latitude, longitude)`
- `ReverseGeocodeService.LoadUSAStatesAsync()` - `FindUSAState(latitude, longitude)`
- `ReverseGeocodeService.LoadFranceDepartmentsAsync()` - `FindFranceDepartment(latitude, longitude)`
- `ReverseGeocodeService.LoadFranceRegionsAsync()` - `FindFranceRegion(latitude, longitude)`
- `ReverseGeocodeService.LoadUKCountiesAsync()` - `FindUKCounty(latitude, longitude)`
- `ReverseGeocodeService.LoadNetherlandsProvincesAsync()` - `FindNetherlandsProvince(latitude, longitude)`

## How It Works

ReverseGeocodeLib uses a **point-in-polygon algorithm** to efficiently determine geographic boundaries:

1. Geographic data (countries, cities, regions) is pre-compiled into binary format
2. Each geographic area is defined by polygon coordinates
3. When you query with latitude/longitude, the library tests if the point falls within any polygon
4. Matching areas are returned as results

This approach ensures fast, offline geocoding without external dependencies.

## Notes

- If no area matches the provided location, an empty list is returned
- For enclaves (areas surrounded by another area), multiple results may be returned - the library cannot determine which is correct
- Embedded data files are expected as assembly resources with names such as `{AssemblyName}.Data.countries.bin`
- All geographic operations are performed locally - no internet connection required

## Use Cases

- **Geolocation-based Applications**: Identify user location in offline-first applications
- **Mapping Applications**: Convert GPS coordinates to readable location names
- **Mobile Apps**: Lightweight reverse geocoding without network dependency
- **IoT Devices**: Location identification on devices with limited connectivity
- **Privacy-Focused Apps**: Keep user location data local without sending to external services

## Performance

- **Fast Lookups**: Optimized point-in-polygon algorithms for quick results
- **Low Memory Footprint**: Embedded binary data format
- **Scalable**: Handle millions of coordinate lookups efficiently

## License

MIT License - see LICENSE file for details

## Acknowledgements

- Original project inspiration: https://github.com/InquisitorJax/Wibci.CountryReverseGeocode
- Geographic data: https://simplemaps.com
