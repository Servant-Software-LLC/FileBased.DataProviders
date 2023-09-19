# CSV Data Provider
The CSV format (as specified in [RFC 4180](https://datatracker.ietf.org/doc/html/rfc4180) has no data typing.  There are variants to the format.  The [CsvHelper](https://github.com/JoshClose/CsvHelper) 
library ([NuGet package](https://www.nuget.org/packages/CsvHelper/) looks to take in account many of those variants.  It is unclear to me though if this library has any ability to determine data types
of the data therein.  If no data type can be determined through this library, then assume all data are strings.  

## Requirements
- Provide the same functionality as is provided by the [JSON Provider](Json Provider.md)
  - Major exception is that there is no equivalent "File as Database" concept.
- Avoid large duplications of code (when it makes sense) by creating a library project with the following location - src/Data.Common/Data.Common.csproj
- Provide the same level of test coverage as is in the [JSON Provider](Json Provider.md)
- Assume that the first line of the CSV are the list of column names.
