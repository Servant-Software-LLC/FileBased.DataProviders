# ADO.NET.FileBased.DataProviders
ADO.NET Data Providers for common serializable formats stored to disk.

## Requirements
- You will need to sign an Assignment of Intellectual Property Rights agreement that Servant Software LLC provides.
- All interfaces in [the table on this page](https://learn.microsoft.com/en-us/previous-versions/aa720599(v=vs.71)) must have an implementation.  Here are instructions on [Implementing a .NET Framework Data Provider](https://learn.microsoft.com/en-us/previous-versions/aa720164(v=vs.71))
- The Definition of Done will be:
  - when all requested features in this document are implemented
  - the provided set of unit tests pass
  - code coverage is at least 75%. (The GitHub Actions build of this repo provides code coverage statistics)
  - all PRs have been approved and merged into the master branch
- Create a JSON ADO.NET data provider that provides CRUD operations.
- As is typically expected in the software industry, appropriate commenting of the code should be in place.

### JSON Data Provider
- Only use classes from the [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) namespace to [serialize/deserialize JSON](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0).  No dependencies on Newtonsoft allowed.
- Using non-Microsoft libraries requires approval.  Minimal dependencies is ideal and if we want/need to bring in other dependencies, those dependencies need to be platform agnostic, royality-free and lean toward unrestrictive licensing such as a MIT license. 
- The connection string is used mainly to determine the data source for the provider.  The data source can be either a file path to a folder or to a json file.   
  - If a path to a folder, then the folder represents the json database and each json file in the folder represents a table.  The file name then becomes the table's name and you can expect that data within the file be a JSON array of objects.  For example:
```
[  
    { "name":"Joe", "email":"Joe@gmail.com", "salary":56000, "married":true },  
    { "name":"Bob", "email":"bob32@gmail.com", "salary":95000 },  
]
```
In the above example, this table would have 4 columns, one for each unique name/value pair.  Note: If is valid that there is not a name/value pair as is seen for the "married" property in the second object.  Internally, this object will be represented as having a "married" property of null.
  
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

  - For determining data type in this flattened structure, if the value of a name/value pair is a JSON object, then assume a data type of string and the value of that column is just the JSON string of the object.  A future task (but is not part of the bid on this task) may be to extend this data provider to determine tables based on nested objects within the JSON data structure.

- Internally for the [JsonCommand](https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/blob/main/src/Data.Json/JsonCommand.cs) class, parsing of the [IDbCommand.CommandText](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbcommand.commandtext?view=net-7.0#system-data-idbcommand-commandtext) property should use the [Irony](https://www.nuget.org/packages/Irony) library with the [SQL Grammar](https://github.com/IronyProject/Irony/blob/master/Irony.Samples/SQL/SqlGrammar.cs) that they have provided. 
- JSON comments is to have some support in this data provider.  When the [Caption](https://learn.microsoft.com/en-us/dotnet/api/system.data.datacolumn.caption?view=net-7.0) property of a DataColumn is set in a DataSet and then the JsonDataAdapter's Update() method is called, that Caption property, if it has been set, (Note:  According to the docs, its value will be the name of the Column if it hasn't been set.  So ignore that case where Caption == Name.) it will be written within the JSON stored to disk.  For example:
```
{
"key": "value" // comment for this key here.
}
```
System.Text.Json has some support for this [JsonCommentHandling Enum](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsoncommenthandling?view=net-7.0) & [JsonTokenType.Comment Enum](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsontokentype?view=net-7.0), but no research has been done to determine if it supports writting these comments to disk.
When reading the JSON from disk, if multiple rows of a table for a particular column have a comment, it is okay to use just the first occurence of a comment if/when setting the DataColumn's Caption property.
