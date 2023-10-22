# CSV ADO.NET Data Provider
| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `ServantSoftware.Data.Csv`       | [![NuGet](https://img.shields.io/nuget/v/ServantSoftware.Data.Csv.svg)](https://www.nuget.org/packages/ServantSoftware.Data.Csv/)

The CSV ADO.NET Data Provider offers a set of classes that facilitate data access to CSV files using the ADO.NET framework. It provides CRUD (Create, Read, Update, Delete) operations to interact with CSV files as if they were a database.  The CSV format (as specified in [RFC 4180](https://datatracker.ietf.org/doc/html/rfc4180) has no data typing.  As a result, all data stored in the XML files is data typed as a string.

## Features

- ADO.NET compliant provider for CSV files.
- Support for CRUD operations.
- Implementation of ADO.NET interfaces for consistency with other data providers.
- Customizable and extendable to meet specific needs.
- Comprehensive XML comments to assist with usage.

## Getting Started

### Prerequisites

- .NET 7.0 or later.
- An IDE that supports .NET development, such as Visual Studio.

### Installation

You can install the CSV ADO.NET Data Provider from the NuGet package manager or by using the following command in your terminal:

```bash
dotnet add package ServantSoftware.Data.Csv
```
## Usage

To use the CSV ADO.NET Data Provider, you'll need to create a `CsvConnection` instance, then create a `CsvCommand` instance, and execute it. Here's an example:

```csharp
using System.Data.CsvClient;

var connectionString = new FileConnectionString() { DataSource = "path/to/your/folder" };
using var connection = new CsvConnection(connectionString);
connection.Open();

var commandText = "SELECT * FROM your_table_name_without_extension";
using var command = new CsvCommand(commandText, connection);
using var reader = command.ExecuteReader();

while (reader.Read())
{
    // Process data
}

connection.Close();
```

### Connection String

The connection string is used mainly to determine the data source for the provider. The data source will always be a path to a folder.

#### Folder As Database

In this mode, the folder represents the CSV database, and each CSV file in the folder represents a table. The file name then becomes the table's name, minus the `.csv` extension, and you can expect the data within the file to be rows of comma-separated values.

For example, if there's a file named `employees.csv` with the following content:

```plaintext
name,email,salary,married
Joe,Joe@gmail.com,56000,true
Bob,bob32@gmail.com,95000,
```
In the above example, this table would have 4 columns, one for each unique header. The absence of a value, as seen for the "married" property in the second row, would be represented internally as null.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Thanks to the .NET Foundation for the ADO.NET framework.
- Special gratitude to those who have provided invaluable feedback and contributions to this project.
