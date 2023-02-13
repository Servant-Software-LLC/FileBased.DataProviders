using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Json.Interfaces;

public interface IConnectionStringProperties
{
    string ConnectionString { get; }

    public string? DataSource { get; }
    public bool Formatted { get; }
}
