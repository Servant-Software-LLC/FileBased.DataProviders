# JSON ADO.NET Data Provider
| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `ServantSoftware.Data.Json`       | [![NuGet](https://img.shields.io/nuget/v/ServantSoftware.Data.Json.svg)](https://www.nuget.org/packages/ServantSoftware.Data.Json/)

The JSON ADO.NET Data Provider offers a set of classes that facilitate data access to JSON files using the ADO.NET framework. It provides CRUD (Create, Read, Update, Delete) operations to interact with JSON files as if they were a database.

## Features

- ADO.NET compliant provider for JSON files.
- Support for CRUD operations.
- Implementation of ADO.NET interfaces for consistency with other data providers.
- Customizable and extendable to meet specific needs.
- Comprehensive XML comments to assist with usage.

## Getting Started

### Prerequisites

- .NET 7.0 or later.
- An IDE that supports .NET development, such as Visual Studio.

### Installation

You can install the JSON ADO.NET Data Provider from the NuGet package manager or by using the following command in your terminal:

```bash
dotnet add package ServantSoftware.Data.Json
```

## Usage

To use the JSON ADO.NET Data Provider, you will need to create a `JsonConnection` instance, then create a `JsonCommand` instance, and execute it. Here's an example:

```csharp
using System.Data.JsonClient;

var connectionString = new FileConnectionString() { DataSource = "path/to/your/file.json" };
using var connection = new JsonConnection(connectionString);
connection.Open();

var commandText = "SELECT * FROM your_table";
using var command = new JsonCommand(commandText, connection);
using var reader = command.ExecuteReader();

while (reader.Read())
{
    // Process data
}

connection.Close();
```

### Connection String

The connection string is used mainly to determine the data source for the provider. The data source can be either a file path to a folder or a JSON file.

#### Folder As Database
If a path to a folder, then the folder represents the JSON database, and each JSON file in the folder represents a table. The file name then becomes the table's name, and you can expect the data within the file to be a JSON array of objects.

For example:

```json
[  
    { "name": "Joe", "email": "Joe@gmail.com", "salary": 56000, "married": true },  
    { "name": "Bob", "email": "bob32@gmail.com", "salary": 95000 },  
]
```

In the above example, this table would have 4 columns, one for each unique name/value pair. Note: It is valid that there is not a name/value pair, as is seen for the "married" property in the second object. Internally, this object will be represented as having a "married" property of null.

#### File As Database
If a path to a JSON file, then the JSON file represents a database. In that case, you can expect the format of the file to be an object containing name/value pairs. Each name/value pair represents a table, and the value of that pair will be a JSON array of objects.

```json
{
  "employees": 
  [  
    { "name": "Joe", "email": "Joe@gmail.com", "salary": 56000, "married": true },  
    { "name": "Bob", "email": "bob32@gmail.com", "salary": 95000 },  
  ],
  "locations":
  [
    { "city": "Houston", "state": "Texas", "zip": 77846 },
    { "city": "New Braunfels", "state": "Texas", "zip": 78132 },
  ]
}
```
The above example would represent a database containing 2 tables. The first table would be named "employees" containing 4 columns. The second table would be named "locations" and contain 3 columns.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Thanks to the .NET Foundation for the ADO.NET framework.
- Thanks to everyone who has contributed to this project.
