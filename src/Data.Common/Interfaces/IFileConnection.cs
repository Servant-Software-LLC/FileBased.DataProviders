﻿using Data.Common.DataSource;
using Data.Common.Utils;

namespace Data.Common.Interfaces;

public interface IFileConnection : IDbConnection
{
    string FileExtension { get; }
    bool? Formatted { get; }
    DataSourceType DataSourceType { get; }
    IDataSourceProvider DataSourceProvider { get; }
    bool FolderAsDatabase { get; }
    bool AdminMode { get; }
    FileReader FileReader { get; }
    LoggerServices LoggerServices { get; }
    bool CaseInsensitive { get; }
    bool DataTypeAlwaysString { get; }
}
