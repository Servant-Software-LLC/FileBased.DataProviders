namespace Data.Common.DataSource;

/// <summary>
/// Represents the method that will handle an event when the data source changes.
/// </summary>
/// <param name="sender">The source of the event, typically the data source provider.</param>
/// <param name="e">An <see cref="DataSourceEventArgs"/> that contains the event data, such as the table that was changed.</param>
public delegate void DataSourceEventHandler(object sender, DataSourceEventArgs e);
