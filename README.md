# ADO.NET.FileBased.DataProviders
ADO.NET Data Providers for common serializable formats stored to disk.

## Requirements
- You will need to sign an Assignment of Intellectual Property Rights agreement that Servant Software LLC provides.
- Here are instructions on [Implementing a .NET Framework Data Provider](https://learn.microsoft.com/en-us/previous-versions/aa720164(v=vs.71))
- The Definition of Done will be when all requested features in this document are implemented, the provided set of unit tests pass and code coverage is at least 70%.
- Create a JSON data provider that provides CRUD operations.

### JSON Data Provider
- Only use classes from the [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) namespace to [serialize/deserialize JSON](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0).  No dependencies on Newtonsoft allowed.
- The connection string is used mainly to determine the data source for the provider.  The data source can be either a file path to a folder or to a json file.   
  - If a path to a folder, then the folder represents the json database and each json file in the folder represents a table.  The file name then becomes the table's name and you can expect that data within the file be a JSON array of objects.  For example:
```
[  
    { "name":"Joe", "email":"Joe@gmail.com", "salary":56000, "married":true },  
    { "name":"Bob", "email":"bob32@gmail.com", "salary":95000 },  
]
```
In the above example, this table would have 4 columns, one for each unique name/value pair.  Note: If is valid that there is not a name/value pair for "married" in the second object.  Internally, this object will be represented as having a "married" property of null.
  
  - If a path to a json file, then the json file represents a database.  In that case then, you can expect the format of the file to be an object containing name\value pairs.  Each name\value pair represents a table and the value of that pair will a JSON array of objects.
```
{
  "employees": 
  [  
    { "name":"Joe", "email":"Joe@gmail.com", "salary":56000, "married":true },  
    { "name":"Bob", "email":"bob32@gmail.com", "salary":95000 },  
  ],
  "locations":
  [
    { "city":"Houston", "state":"Texas", "zip":77846 },
    { "city":"New Braunfels", "state":"Texas", "zip":78132 },
  ]
}
```
The above example would represent a database containing 2 tables.  The first table would be named "employees" containing 4 columns.  The second table would be named "locations" and contain 3 columns.
  
- For determination of the data type of a property of a model, limit the valid data types to string, number (System.Int64 or System.Double depending on the data provided), and boolean ([See JSON Data Types](https://www.w3schools.com/js/js_json_datatypes.asp)) or an IEnumerable<> of any of those data types.   Values of type Int64, double and bool, you must determine if any of the properties in the array of objects contains a null value.  If so, then the property within the model must be [nullable](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types).

