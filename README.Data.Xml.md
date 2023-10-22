# XML ADO.NET Data Provider
| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `ServantSoftware.Data.Xml`       | [![NuGet](https://img.shields.io/nuget/v/ServantSoftware.Data.Xml.svg)](https://www.nuget.org/packages/ServantSoftware.Data.Xml/)

The XML ADO.NET Data Provider offers a set of classes that facilitate data access to XML files using the ADO.NET framework. It provides CRUD (Create, Read, Update, Delete) operations to interact with XML files as if they were a database.

## Features

- ADO.NET compliant provider for XML files.
- Support for CRUD operations.
- Implementation of ADO.NET interfaces for consistency with other data providers.
- Customizable and extendable to meet specific needs.
- Comprehensive XML comments to assist with usage.
- Support for strongly typed XML data via [XmlReader](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0) and XmlWriter. Learn more about [strongly typed data](https://learn.microsoft.com/en-us/dotnet/standard/data/xml/type-support-in-the-system-xml-classes) and [Converting to CLR types](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0#converting-to-clr-types). If an XML file doesn't have an accompanying XSD, all columns will default to type `System.String`.

## Getting Started

### Prerequisites

- .NET 7.0 or later.
- An IDE that supports .NET development, such as Visual Studio.

### Installation

You can install the XML ADO.NET Data Provider from the NuGet package manager or by using the following command in your terminal:

```bash
dotnet add package ServantSoftware.Data.Xml
```

## Usage

To use the XML ADO.NET Data Provider, you will need to create an `XmlConnection` instance, then create an `XmlCommand` instance, and execute it. Here's an example:

```csharp
using System.Data.XmlClient;

var connectionString = new FileConnectionString() { DataSource = "path/to/your/file.xml" };
using var connection = new XmlConnection(connectionString);
connection.Open();

var commandText = "SELECT * FROM your_element";
using var command = new XmlCommand(commandText, connection);
using var reader = command.ExecuteReader();

while (reader.Read())
{
    // Process data
}

connection.Close();
```

### Connection String
The connection string is mainly used to determine the data source for the provider. The data source can be either a file path to a folder or to an XML file.

#### Folder As Database
For example, if your XML structure is:

If a path to a folder is provided, then the folder represents the XML database, and each XML file in the folder represents a table. The file name then becomes the table's name. The XML data within each file is treated as a set of records.

For instance, consider an XML file named employees.xml with the following content:
```xml
<employees>
    <employee>
        <name>Joe</name>
        <email>Joe@gmail.com</email>
        <salary>56000</salary>
        <married>true</married>
    </employee>
    <employee>
        <name>Bob</name>
        <email>bob32@gmail.com</email>
        <salary>95000</salary>
    </employee>
</employees>
```
In the above example, this XML would represent a table named employees with four columns: name, email, salary, and married.

#### File As Database
If a path to an XML file is provided, then the XML file represents the database. The structure and organization of the XML data will dictate the representation of tables and records. Each major XML element can be seen as a table, and the sub-elements represent the records and fields.

For example:
```xml
<database>
    <employees>
        <employee>
            <name>Joe</name>
            <email>Joe@gmail.com</email>
            <salary>56000</salary>
            <married>true</married>
        </employee>
        <employee>
            <name>Bob</name>
            <email>bob32@gmail.com</email>
            <salary>95000</salary>
        </employee>
    </employees>
    <locations>
        <location>
            <city>Houston</city>
            <state>Texas</state>
            <zip>77846</zip>
        </location>
        <location>
            <city>New Braunfels</city>
            <state>Texas</state>
            <zip>78132</zip>
        </location>
    </locations>
</database>
```
The above XML would represent a database containing two tables: employees and locations, each with their respective columns and values.

You can query specific elements and attributes, and the hierarchy of XML is treated similarly to tables in relational databases.

#### Strongly Typed Data
XML data has the ability to be strongly typed. [XmlReader](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0)/XmlWriter supports [strongly typed data](https://learn.microsoft.com/en-us/dotnet/standard/data/xml/type-support-in-the-system-xml-classes). (Also see [Converting to CLR types](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0#converting-to-clr-types)). If the XML file does not have an accompanying XSD file, then all columns should just be of type System.String.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Thanks to the .NET Foundation for the ADO.NET framework.
- Thanks to everyone who has contributed to this project.
