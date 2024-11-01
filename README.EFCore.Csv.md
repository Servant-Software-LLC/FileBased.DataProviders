# ServantSoftware.EFCore.Csv

![Nuget](https://img.shields.io/nuget/v/ServantSoftware.EFCore.Csv)

`ServantSoftware.EFCore.Csv` is an EF Core provider that allows you to use CSV files as a data source. It's currently in an alpha state, primarily focusing on specific "happy paths" needed for [SettingsOnEF](https://github.com/Servant-Software-LLC/SettingsOnEF) and our main product, [MockDB](https://mock-db.com/). We encourage community contributions to enhance its features and cover a broader range of scenarios.

## Features

- Seamless integration with EF Core.
- Allows CSV files to act as data sources for EF Core.
- Optimized for specific scenarios to support the main product.
- Easy setup and configuration.

## Getting Started

### Prerequisites

- .NET 7.0 or later.
- EF Core compatible version.

### Installation

Install the `ServantSoftware.EFCore.Csv` provider using NuGet:

```bash
dotnet add package ServantSoftware.EFCore.Csv
```

## Usage

After installing the package, you can configure your `DbContext` to use the CSV provider:

```csharp
public class MyDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = new FileConnectionString() { DataSource = "path/to/your/data.json" };
        optionsBuilder.UseCsv(connectionString);
    }
}
```

This allows you to perform standard EF Core operations on your CSV data source.  This provider is based on the [CSV ADO.NET Provider](README.Data.Csv.md).  Refer to it for details connection string options.


## Contributing
We're open to contributions! Please read [contributing guidelines](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## Limitations

Being in its alpha state, the `ServantSoftware.EFCore.Csv` provider primarily addresses specific scenarios vital for our product [MockDB](https://mock-db.com/). Some advanced EF Core features might not be fully supported yet. We are eagerly awaiting community contributions to improve and extend its capabilities.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Thanks to the .NET Foundation and EF Core teams for providing an exceptional framework.
- Huge thanks to all who are considering contributing to this nascent project!
