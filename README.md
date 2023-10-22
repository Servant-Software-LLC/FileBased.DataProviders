# FileBased.DataProviders
[![DataProviders Build](https://github.com/Servant-Software-LLC/FileBased.DataProviders/actions/workflows/main.yml/badge.svg)](https://github.com/Servant-Software-LLC/FileBased.DataProviders/actions/workflows/main.yml)

FileBased.DataProviders is a collection of ADO.NET & EF Core Data Providers designed to facilitate CRUD operations on common serializable formats stored on disk, including JSON, XML, and CSV.

## Features
- ADO.NET providers for JSON, XML, and CSV formats.
- CRUD operations support for each format.
- Compatibility with EF Core.
- Extensive unit tests to ensure functionality.

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
- .NET 7.0 or later

### Installation
Clone the repository to your local machine.
```sh
git clone https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders.git
```

Navigate to the project directory.
```sh
cd ADO.NET.FileBased.DataProviders
```

Build the solution.
```sh
dotnet build
```

## Usage
Each data provider (JSON, XML, and CSV) comes with its own set of specific operations. Please refer to the README of each provider for detailed instructions on how to use them.

- [JSON Provider](README.Data.Json.md)
- [XML Provider](README.Data.Xml.md)
- [CSV Provider](README.Data.Csv.md)

## Testing
The project comes with a set of unit tests to ensure that all features are working as expected. To run the tests, use the following command:

```sh
dotnet test
```

## Contributing
Contributions are welcome. Please refer to our [contributing guidelines](CONTRIBUTING.md) for more information.

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments
- The .NET Foundation for the ADO.NET and EF Core frameworks.
