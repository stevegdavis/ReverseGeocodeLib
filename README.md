# ReverseGeocodeLib
## Usage

Below is a basic example showing how to load the embedded country and city data and use the library to find the country and city for a given latitude/longitude. You must call the `Load...Async` methods before calling any `Find...` methods.

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
    }
}
```

Other available lookups:
- `ReverseGeocodeService.LoadUSAStatesAsync()` and `FindUSAState(latitude, longitude)`
- `ReverseGeocodeService.LoadFranceDepartmentsAsync()` and `FindFranceDepartment(latitude, longitude)`
- `ReverseGeocodeService.LoadFranceRegionsAsync()` and `FindFranceRegion(latitude, longitude)`
- `ReverseGeocodeService.LoadUKCountiesAsync()` and `FindUKCounty(latitude, longitude)`
- `ReverseGeocodeService.LoadNetherlandsProvincesAsync()` and `FindNetherlandsProvince(latitude, longitude)`

**Notes:**
- If no area matches the provided location, you’ll get an empty list.
- The library uses a point-in-polygon algorithm to test if the point is inside an area’s polygon(s).
- Embedded data files are expected as assembly resources with names such as `{AssemblyName}.Data.countries.bin`, `{AssemblyName}.Data.cities.bin`, etc.
  - Confirm your `.csproj` includes:
    ```xml
    <ItemGroup>
      <EmbeddedResource Include="Data\countries.bin" />
      <EmbeddedResource Include="Data\cities.bin" />
      <!-- etc -->
    </ItemGroup>

## Acknowledgements to
https://github.com/InquisitorJax/Wibci.CountryReverseGeocode for the orignal project 
https://simplemaps.com for map data
