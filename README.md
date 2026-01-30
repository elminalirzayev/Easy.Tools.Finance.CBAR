[![Build & Test](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/build.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/build.yml)
[![Build & Release](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/release.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/release.yml)
[![Build & Nuget Publish](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/nuget.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/actions/workflows/nuget.yml)
[![Release](https://img.shields.io/github/v/release/elminalirzayev/Easy.Tools.Finance.CBAR)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/releases)
[![License](https://img.shields.io/github/license/elminalirzayev/Easy.Tools.Finance.CBAR)](https://github.com/elminalirzayev/Easy.Tools.Finance.CBAR/blob/master/LICENSE.txt)
[![NuGet](https://img.shields.io/nuget/v/Easy.Tools.Finance.CBAR.svg)](https://www.nuget.org/packages/Easy.Tools.Finance.CBAR)

# Easy.Tools.Finance.CBAR

**Easy.Tools.Finance.CBAR** is a high-performance, enterprise-ready .NET library designed to fetch official daily exchange rates from the **Central Bank of Azerbaijan (CBAR/AMB)**.

It handles XML parsing, network resilience, and data flattening efficiently, allowing developers to focus on business logic.


## Features

- ** High Performance:** Uses **Static XML Serializer** caching and **Zero-Allocation** `Span<T>` parsing logic to minimize memory pressure.
- ** Resilience:** Built-in **Retry Policy** with exponential backoff for handling network glitches.
- ** Culture Safe:** Parsing logic is strictly **Invariant Culture**, ensuring stability regardless of the server's regional settings.
- ** Easy Integration:** Single-line integration with `IServiceCollection` (Dependency Injection).
- ** Async & Cancellable:** Full support for `async/await` and `CancellationToken` to handle request timeouts properly.
- ** Multi-Target:** Supports `.NET 10`, `.NET 8`, `.NET 6`, `.NET Standard 2.0`, and `.NET Framework 4.7.2+`.


## Installation

Install via NuGet Package Manager:

```bash
Install-Package Easy.Tools.Finance.CBAR
```

Or via .NET CLI:

```bash
dotnet add package Easy.Tools.Finance.CBAR
```

## Usage

### 1. Service Registration (Program.cs)

Register the client in your `Program.cs`. The library provides a fluent extension method.

```csharp
using Easy.Tools.Finance.CBAR;

var builder = WebApplication.CreateBuilder(args);

// 1. Standard Registration
builder.Services.AddCbarClient();

// 2. OR: Advanced Configuration
builder.Services.AddCbarClient(options => 
{
    options.RetryCount = 3;             // Retry 3 times on failure
    options.RetryDelaySeconds = 2;      // Wait 2 seconds between retries
    // options.BaseUrl = "...";         // Optional: Use a proxy URL if needed
});

var app = builder.Build();
```


### 2. Fetching Rates (Controller Example)

Inject `ICbarClient` into your controllers or services. Ensure you pass the `CancellationToken` for best practices.

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

    [HttpGet("today")]
    public async Task<IActionResult> GetRates(CancellationToken cancellationToken)
    {
        // Fetch all rates efficiently
        var rates = await _cbarClient.GetTodayRatesAsync(cancellationToken);

        // Example 1: Filter Foreign Currencies (USD, EUR, etc.)
        var currencies = rates.Where(x => x.CurrencyType == "Xarici valyutalar").ToList();

        // Example 2: Filter Precious Metals (Gold, Silver, etc.)
        var metals = rates.Where(x => x.CurrencyType == "Bank metalları").ToList();

        // Get Specific Rates
        var usd = currencies.FirstOrDefault(x => x.Code == "USD");
        var gold = metals.FirstOrDefault(x => x.Code == "XAU"); // XAU = Gold

        if (usd != null)
            Console.WriteLine($"1 USD = {usd.Value} AZN");

        if (gold != null)
            Console.WriteLine($"1 Ounce Gold = {gold.Value} AZN");

        return Ok(rates);
    }
}
```


##  Models

The package returns a list of `CbarCurrency` objects. The data structure is flattened for ease of use.

| Property | Type | Description |
| --- | --- | --- |
| `Code` | `string` | The ISO code (e.g., `USD`, `EUR`, `XAU`). |
| `Name` | `string` | The localized name (e.g., `1 ABŞ dolları`, `Qızıl`). |
| `Value` | `decimal` | The official exchange rate. Parsed safely using `decimal`. |
| `Nominal` | `int` | The unit amount (e.g., `1`, `100`). Useful for currencies like JPY (100). |
| `CurrencyType` | `string` | The category (e.g., `Xarici valyutalar` or `Bank metalları`). |


---

## Contributing

Contributions and suggestions are welcome. Please open an issue or submit a pull request.

---

## License

This project is licensed under the MIT License.

---

© 2025 Elmin Alirzayev / Easy Code Tools