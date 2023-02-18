# XML Data Provider
XML data has the ability to be strongly typed.  [XmlReader](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0)/XmlWriter supports 
[strongly typed data](https://learn.microsoft.com/en-us/dotnet/standard/data/xml/type-support-in-the-system-xml-classes).  (Also see 
[Converting to CLR types](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0#converting-to-clr-types)).  If the XML file does not
have an accompanying XSD file, then all columns should just be of type System.String.

## Requirements
- Provide the same functionality as is provided by the [JSON Provider](Json Provider.md).  Included both the "File as Database" and "Folder as Database" concepts.
- Avoid large duplications of code (when it makes sense) by creating a library project with the following location - src/Data.Common/Data.Common.csproj
- Provide the same level of test coverage as is in the [JSON Provider](Json Provider.md)
