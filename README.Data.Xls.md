# XLS ADO.NET Data Provider
| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `ServantSoftware.Data.Xls`       | [![NuGet](https://img.shields.io/nuget/v/ServantSoftware.Data.Xls.svg)](https://www.nuget.org/packages/ServantSoftware.Data.Xls/)

The XLS ADO.NET Data Provider offers a set of classes that facilitate data access to XLS files using the ADO.NET framework. Currently it only provides light-weight read-only operations to interact with XLS files as if they were a database.  

## Features

- Basic ADO.NET compliant provider for XLS files.
- Implementation of ADO.NET interfaces for consistency with other data providers.
- Customizable and extendable to meet specific needs.

## Getting Started

### Prerequisites

- .NET 7.0 or later.
- An IDE that supports .NET development, such as Visual Studio.

### Installation

You can install the XLS ADO.NET Data Provider from the NuGet package manager or by using the following command in your terminal:

```bash
dotnet add package ServantSoftware.Data.Xls
```
## Usage

To use the XLS ADO.NET Data Provider, you'll need to create a `XlsConnection` instance, then create a `XlsCommand` instance, and execute it. Here's an example:

```csharp
using System.Data.XlsClient;

var connectionString = new FileConnectionString() { DataSource = "path/to/your/folder" };
using var connection = new XlsConnection(connectionString);
connection.Open();

var commandText = "SELECT * FROM your_table_name_without_extension";
using var command = new XlsCommand(commandText, connection);
using var reader = command.ExecuteReader();

while (reader.Read())
{
    // Process data
}

connection.Close();
```

### Connection String

The connection string is used mainly to determine the data source for the provider. The data source will always be a path to a folder.

#### File As Database
The path must be to an XLS file and this XLS file represents a database. 


## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Thanks to the .NET Foundation for the ADO.NET framework.
- Special gratitude to those who have provided invaluable feedback and contributions to this project.
