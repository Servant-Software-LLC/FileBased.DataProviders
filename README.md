# EFCore FileBased Providers
EF Core data providers for common serializable formats stored to disk.

## Requirements
- Instructions on [Writing a Database Provider](https://learn.microsoft.com/en-us/ef/core/providers/writing-a-provider) and [So you want to write an EF Core provider](https://blog.oneunicorn.com/2016/11/11/so-you-want-to-write-an-ef-core-provider/)(This documentation is grossly outdated.  Written in EFCore 1.1) 
- Must have database-first support (i.e. [scaffolding](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli) with a class that inherits IDesignTimeServices).


- Data providers for each of the following serializable formats (json, csv, xml)
- Each data provider to live in its own project and have its own set of unit tests which should have at least 60% coverage.  For examples of unit tests, you may want to refer to the [unit tests provided for Microsoft's EF Core providers](https://github.com/dotnet/efcore/tree/main/test).
- Provider's source code, for the classes needed to achieve the definition of done for this task, should follow a similar class naming/signature and folder structure as the [Sqlite data provider](https://github.com/dotnet/efcore/tree/main/src/EFCore.Sqlite.Core).
- Extension methods on [DbContextOptionsBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder?view=efcore-7.0) for use on each of the different data providers will follow the models (FolderAsDatabase and FileAsDatabase) used as the DataSource for the [ADO.NET.FileBased.DataProviders](https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders).  
- Per recommendations of the EFCore contributors start with the [Npgsql](https://github.com/npgsql/efcore.pg) or [Pomelo MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) providers as the basis for our providers.

### CSV Data Provider
- The CSV format has no data typing.  If a code-first approach is taken by a consumer of this library, then using the [CsvHelper](https://joshclose.github.io/CsvHelper/) library, the serialization of each column of CSV will map to the proper data types of CSV.  When scaffolding a CSV file, you can require that the first line of the file be a list of column names and assume that all properties in the generated model class be of type System.String.

### XML Data Provider
- Should be strongly data typed when writing to file using an [EF Model](https://learn.microsoft.com/en-us/ef/core/#the-model) - XmlReader/XmlWriter supports [strongly typed data](https://learn.microsoft.com/en-us/dotnet/standard/data/xml/type-support-in-the-system-xml-classes).
- If XML on disk also contains an XSD file, then [scaffolding](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli) that file should result in a model with the properly data typed properties (See [Converting to CLR types](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0#converting-to-clr-types)).  Otherwise, the scaffolded model should just have properties of type System.String.


### JSON Data Provider
- Only use classes from the [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) namespace to [serialize/deserialize JSON](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0).  No dependencies on Newtonsoft allowed.
- When scaffolding a file, you can enforce that it contains only one json name/value pair whose name is the table/model name and whose value is an array of objects all of the same object type.  Therefore, when constucting the signature of the model class, only the first object in the array need be interogated to determine the property names of the model class.  Although, if the value of any of the properties in the first object of the array is **null** then further rows will need to be interrogated in order to determine the proper data type of the model to be created. 
- For determination of the data type of a property of a model, it is fine to limit the valid data types to string, number (System.Int64), and boolean ([See JSON Data Types](https://www.w3schools.com/js/js_json_datatypes.asp))  In other words, scaffolding of objects or arrays for property types is unnecessary.  **But** for values of type Int64 and bool, you must determine if any of the properties in the array of objects contains a null value.  If so, then the property within the model must be [nullable](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types).
