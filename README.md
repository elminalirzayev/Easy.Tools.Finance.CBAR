[![Build & Test](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/build.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/build.yml)
[![Build & Release](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/release.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/release.yml)
[![Build & Nuget Publish](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/nuget.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/nuget.yml)
[![Release](https://img.shields.io/github/v/release/elminalirzayev/Easy.Tools.Finance.CBAR)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/releases)
[![License](https://img.shields.io/github/license/elminalirzayev/Easy.Tools.Finance.CBAR)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/blob/master/LICENSE.txt)
[![NuGet](https://img.shields.io/nuget/v/Easy.Tools.Finance.CBAR.svg)](https://www.nuget.org/packages/Easy.Tools.Finance.CBAR)

# Easy.Tools.Finance.CBAR

Easy.Tools.Finance.CBAR is a lightweight .NET library designed to fetch daily exchange rates from the **Central Bank of Azerbaijan (CBAR/AMB)**.

##  Features

* **Multi-Target:** Supports `.NET Standard 2.0`, `.NET 8.0`, and `.NET 9.0`.
* **Resilient:** Built-in retry logic for handling network glitches.
* **Easy Integration:** Seamless integration with `IServiceCollection` (Dependency Injection).
* **Flattened Data:** Automatically handles CBAR's nested XML structure and returns a clean list of currencies.


## Installation

Install via NuGet:

```
dotnet add package Easy.Tools.Finance.CBAR
```

Or via NuGet Package Manager:

```
Install-Package Easy.Tools.Finance.CBAR
```

---

## Features

* **Easy Integration:** Fully compatible with .NET Dependency Injection (DI).
* **Resilience (Retry Logic):** Includes built-in retry mechanisms to handle temporary network glitches or CBAR server timeouts.
* **Type-Safe:** Automatically handles XML parsing and returns clean C# objects with `decimal` properties.
* **Configurable:** Retry counts and delay durations are fully customizable via options.

---

## Usage

### 1. Service Registration (Program.cs)

Register the service in your `Program.cs`:

```csharp
using Easy.Tools.Finance.CBAR.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Standard registration:
builder.Services.AddCbarClient();

// OR: Registration with custom options:
builder.Services.AddCbarClient(options => 
{
    options.RetryCount = 5;         // Retry 5 times on failure
    options.RetryDelaySeconds = 2;  // Wait 2 seconds between retries
});
//OR: Registration with custom CBAR Base URL (e.g., using a proxy or mirror):
builder.Services.AddCBARClient(options =>
{
    options.BaseUrl = "https://my-proxy-server.com/cbar-mirror/";
});

var app = builder.Build();
```


### 2. Fetching Rates (Controller Example)

Inject `ICbarClient` into your controllers or services.

```csharp
using Easy.Tools.Finance.CBAR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICbarClient _cbarClient;

    public CurrencyController(ICbarClient cbarClient)
    {
        _cbarClient = cbarClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetRates()
    {
        // Fetch all rates        
            var rates =  await _cbarClient.GetTodayRatesAsync();

        // 1. Sadecə Valyutaları Al (USD, EUR vs.)
            var currencies = rates.Where(x => x.CurrencyType == "Xarici valyutalar").ToList();

        // 2. Sadecə Qiymətli Metallerı Al (Qızıl, Gümüş vs.)
           // "Bank metalları"
            var metals = rates.Where(x => x.CurrencyType == "Bank metalları").ToList();


            var gold = metals.FirstOrDefault(x => x.Code == "XAU");
            if (gold != null)
            {
                Console.WriteLine($"1 Unsiya Qızıl: {gold.Value} AZN");
            }
            var usd = currencies.FirstOrDefault(x => x.Code == "USD");
            if (usd != null)
            {
                Console.WriteLine($"1 ABŞ Dolları: {usd.Value} AZN");
            }

        return Ok(rates);)
    }
}
```

---

##  Models

The package returns a list of `CbarCurrency` objects. Key properties include:

* `Code`: Currency or Metal code (e.g., `USD`, `EUR`, `XAU`).
* `Name`: Name of the currency in Azerbaijani (e.g., `1 ABŞ dolları`, `Qızıl`).
* `Value`: The exchange rate (Decimal). *This is the official rate provided by CBAR.*
* `Nominal`: The unit amount (e.g., `1`, `100`). *Example: For JPY, Nominal is 100.*
* `CurrencyType`: The category of the currency (e.g., `Xarici valyutalar` for currencies, `Bank metalları` for metals).


---

## License

MIT License.

---

© 2025 Elmin Alirzayev / Easy Code Tools
