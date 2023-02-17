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

### Specific Data Providers
- [JSON Provider](Json Provider.md)
- [XML Provider](Xml Provider.md)
- [CSV Provider](Csv Provider.md)

