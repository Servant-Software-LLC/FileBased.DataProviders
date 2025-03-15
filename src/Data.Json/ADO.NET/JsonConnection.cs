﻿using Data.Common.Utils.ConnectionString;
using Data.Json.JsonIO;

namespace System.Data.JsonClient;

/// <summary>
/// Represents a connection for JSON operations.
/// </summary>
public class JsonConnection : FileConnection<JsonParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConnection"/> class.
    /// </summary>
    public JsonConnection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConnection"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public JsonConnection(string connectionString)
        : this(new FileConnectionString(connectionString)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConnection"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public JsonConnection(FileConnectionString connectionString)
        : base(connectionString) { }

    /// <summary>
    /// Gets the file extension associated with JSON files.
    /// </summary>
    public override string FileExtension => "json";

    public Func<IEnumerable<JsonElement>, Type> GuessTypeFunction { get; set; }
    public int GuessTypeRows { get; set; } = 1000;

    protected override FileReader CreateFileReader => new JsonReader(this);

#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <summary>
    /// Begins a new JSON transaction with the default isolation level.
    /// </summary>
    /// <returns>A new <see cref="JsonTransaction"/> instance.</returns>
    public override JsonTransaction BeginTransaction() => new(this, default);

    /// <summary>
    /// Begins a new JSON transaction with the specified isolation level.
    /// </summary>
    /// <param name="il">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="JsonTransaction"/> instance.</returns>
    public override JsonTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    /// <summary>
    /// Begins a new database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="DbTransaction"/> instance.</returns>
    protected override JsonTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    /// <summary>
    /// Creates a new <see cref="JsonDataAdapter"/> with the specified query.
    /// </summary>
    /// <param name="query">The query string used by the data adapter.</param>
    /// <returns>A new <see cref="JsonDataAdapter"/> instance.</returns>
    public override JsonDataAdapter CreateDataAdapter(string query) => new(query, this);

    /// <summary>
    /// Creates a new <see cref="JsonCommand"/> associated with the connection.
    /// </summary>
    /// <returns>A new <see cref="JsonCommand"/> instance.</returns>
    public override JsonCommand CreateCommand() => new(this);

    /// <summary>
    /// Creates a new <see cref="JsonCommand"/> with the specified command text.
    /// </summary>
    /// <param name="cmdText">The command text for the new command.</param>
    /// <returns>A new <see cref="JsonCommand"/> instance.</returns>
    public override JsonCommand CreateCommand(string cmdText) => new(cmdText, this);

#else       // .NET Standard 2.0 compliant implementation.

    /// <summary>
    /// Begins a new JSON transaction with the default isolation level.
    /// </summary>
    /// <returns>A new <see cref="JsonTransaction"/> instance.</returns>
    public override FileTransaction<JsonParameter> BeginTransaction() => new JsonTransaction(this, default);

    /// <summary>
    /// Begins a new JSON transaction with the specified isolation level.
    /// </summary>
    /// <param name="il">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="JsonTransaction"/> instance.</returns>
    public override FileTransaction<JsonParameter> BeginTransaction(IsolationLevel il) => BeginTransaction();

    /// <summary>
    /// Begins a new database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="DbTransaction"/> instance.</returns>
    protected override Common.DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    /// <summary>
    /// Creates a new <see cref="JsonDataAdapter"/> with the specified query.
    /// </summary>
    /// <param name="query">The query string used by the data adapter.</param>
    /// <returns>A new <see cref="JsonDataAdapter"/> instance.</returns>
    public override IFileDataAdapter CreateDataAdapter(string query) => new JsonDataAdapter(query, this);

    /// <summary>
    /// Creates a new <see cref="JsonCommand"/> associated with the connection.
    /// </summary>
    /// <returns>A new <see cref="JsonCommand"/> instance.</returns>
    public override FileCommand<JsonParameter> CreateCommand() => new JsonCommand(this);

    /// <summary>
    /// Creates a new <see cref="JsonCommand"/> with the specified command text.
    /// </summary>
    /// <param name="cmdText">The command text for the new command.</param>
    /// <returns>A new <see cref="JsonCommand"/> instance.</returns>
    public override FileCommand<JsonParameter> CreateCommand(string cmdText) => new JsonCommand(cmdText, this);

#endif
    /// <inheritdoc/>
    protected override void CreateFileAsDatabase(string databaseFileName) =>
        File.WriteAllText(databaseFileName, "{}");

    /// <inheritdoc/>
    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => fileStatement => new JsonDataSetWriter(this, fileStatement);
}
