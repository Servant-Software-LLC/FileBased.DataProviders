namespace Data.Json.Grammer;

/// <summary>
/// A static class containing all the SQL keywords used in the grammar.
/// </summary>
internal static class SqlKeywords
{


    /// <summary>
    /// Represents the 'CREATE' keyword used in a SQL CREATE statement.
    /// </summary>
    public const string Create = "CREATE";

    /// <summary>
    /// Represents the 'NULL' keyword used in a SQL statement to indicate a null value.
    /// </summary>
    public const string Null = "NULL";

    /// <summary>
    /// Represents the 'NOT' keyword used in a SQL statement to negate a condition.
    /// </summary>
    public const string Not = "NOT";

    /// <summary>
    /// Represents the 'UNIQUE' keyword used in a SQL statement to create a unique constraint.
    /// </summary>
    public const string Unique = "UNIQUE";

    /// <summary>
    /// Represents the 'WITH' keyword used in a SQL statement to specify additional options.
    /// </summary>
    public const string With = "WITH";

    /// <summary>
    /// Represents the 'TABLE' keyword used in a SQL statement to indicate a table is being created or modified.
    /// </summary>
    public const string Table = "TABLE";

    /// <summary>
    /// Represents the 'ALTER' keyword used in a SQL statement to modify an existing table.
    /// </summary>
    public const string Alter = "ALTER";

    /// <summary>
    /// Represents the 'ADD' keyword used in a SQL statement to add a column or constraint to a table.
    /// </summary>
    public const string Add = "ADD";

    /// <summary>
    /// Represents the 'COLUMN' keyword used in a SQL statement to indicate a column is being created or modified.
    /// </summary>
    public const string Column = "COLUMN";

    /// <summary>
    /// Represents the 'DROP' keyword used in a SQL statement to remove an element from a table.
    /// </summary>
    public const string Drop = "DROP";

    /// <summary>
    /// Represents the 'CONSTRAINT' keyword used in a SQL statement to indicate a constraint is being created or modified.
    /// </summary>
    public const string Constraint = "CONSTRAINT";

    /// <summary>
    /// Represents the 'INDEX' keyword used in a SQL statement to indicate an index is being created or modified.
    /// </summary>
    public const string Index = "INDEX";

    /// <summary>
    /// Represents the 'ON' keyword used in a SQL statement to indicate a table or column a constraint or index applies to.
    /// </summary>
    public const string On = "ON";

    /// <summary>
    /// Represents the 'KEY' keyword used in a SQL statement to indicate a key constraint is being created or modified.
    /// </summary>
    public const string Key = "KEY";

    /// <summary>
    /// Represents the 'PRIMARY' keyword used in a SQL statement to indicate a primary key constraint is being created.
    /// </summary>
    public const string Primary = "PRIMARY";

    /// <summary>
    /// Represents the 'INSERT' keyword used in a SQL statement to insert data into a table.
    /// </summary>
    public const string Insert = "INSERT";

    /// <summary>
    /// Represents the 'INTO' keyword used in a SQL statement to specify the table data is being inserted into.
    /// </summary>
    public const string Into = "INTO";

    /// <summary>
    /// Represents the 'UPDATE' keyword used in a SQL statement to update data in a table.
    /// </summary>
    public const string Update = "UPDATE";

    /// <summary>
    /// Represents the 'SET' keyword used in a SQL statement to specify the new values for the updated data.
    /// </summary>
    public const string Set = "SET";

    /// <summary>
    /// Represents the 'VALUES' keyword used in a SQL statement to specify the values being inserted into a table.
    /// </summary>
    public const string Values = "VALUES";

    /// <summary>
    /// Represents the 'DELETE' keyword used in a SQL statement to delete data from a table.
    /// </summary>
    public const string Delete = "DELETE";

    /// <summary>
    /// Represents the 'SELECT' keyword used in a SQL statement to retrieve data from a table.
    /// </summary>
    public const string Select = "SELECT";

    /// <summary>
    /// Represents the 'FROM' keyword used in a SQL statement to specify the table or tables data is being retrieved from.
    /// </summary>
    public const string From = "FROM";

    /// <summary>
    /// Represents the 'AS' keyword used in a SQL statement to rename a column or table.
    /// </summary>
    public const string As = "AS";

    /// <summary>
    /// Represents the 'COUNT' keyword used in a SQL statement to count the number of rows matching a condition.
    /// </summary>
    public const string Count = "COUNT";

    /// <summary>
    /// Represents the 'JOIN' keyword used in a SQL statement to combine rows from two or more tables based on a related column between them.
    /// </summary>
    public const string Join = "JOIN";

    /// <summary>
    /// Represents the 'BY' keyword used in a SQL statement to specify the column used to combine rows in a JOIN statement.
    /// </summary>
    public const string By = "BY";


    //Non-TERMINALS


    /// <summary>
    /// Represents a non-terminal for an identifier in the SQL grammar.
    /// </summary>
    public const string Id = "Id";

    /// <summary>
    /// Represents a non-terminal for a statement in the SQL grammar.
    /// </summary>
    public const string Statement = "stmt";

    /// <summary>
    /// Represents a non-terminal for a CREATE TABLE statement in the SQL grammar.
    /// </summary>
    public const string CreateTableStmt = "createTableStmt";

    /// <summary>
    /// Represents a non-terminal for a CREATE INDEX statement in the SQL grammar.
    /// </summary>
    public const string CreateIndexStmt = "createIndexStmt";

    /// <summary>
    /// Represents a non-terminal for an ALTER statement in the SQL grammar.
    /// </summary>
    public const string AlterStmt = "alterStmt";

    /// <summary>
    /// Represents a non-terminal for a DROP TABLE statement in the SQL grammar.
    /// </summary>
    public const string DropTableStmt = "dropTableStmt";

    /// <summary>
    /// Represents a non-terminal for a DROP INDEX statement in the SQL grammar.
    /// </summary>
    public const string DropIndexStmt = "dropIndexStmt";

    /// <summary>
    /// Represents a non-terminal for a SELECT statement in the SQL grammar.
    /// </summary>
    public const string SelectStmt = "selectStmt";

    /// <summary>
    /// Represents a non-terminal for an INSERT statement in the SQL grammar.
    /// </summary>
    public const string InsertStmt = "insertStmt";

    /// <summary>
    /// Represents a non-terminal for an UPDATE statement in the SQL grammar.
    /// </summary>
    public const string UpdateStmt = "updateStmt";

    /// <summary>
    /// Represents a non-terminal for a DELETE statement in the SQL grammar.
    /// </summary>
    public const string DeleteStmt = "deleteStmt";

    /// <summary>
    /// Represents a non-terminal for a field definition in the SQL grammar.
    /// </summary>
    public const string FieldDef = "fieldDef";

    /// <summary>
    /// Represents a non-terminal for a list of field definitions in the SQL grammar.
    /// </summary>
    public const string FieldDefList = "fieldDefList";

    /// <summary>
    /// Represents a non-terminal for a NULL specification in the SQL grammar.
    /// </summary>
    public const string NullSpecOpt = "nullSpecOpt";

    /// <summary>
    /// Represents a non-terminal for a type name in the SQL grammar.
    /// </summary>
    public const string TypeName = "typeName";

    /// <summary>
    /// Represents a non-terminal for a type specification in the SQL grammar.
    /// </summary>
    public const string TypeSpec = "typeSpec";

    /// <summary>
    /// Represents a non-terminal for optional type parameters in the SQL grammar.
    /// </summary>
    public const string TypeParamsOpt = "typeParams";

    /// <summary>
    /// Represents a non-terminal for a constraint definition in the SQL grammar.
    /// </summary>
    public const string ConstraintDef = "constraintDef";

    /// <summary>
    /// Represents a non-terminal for an optional list of constraints in the SQL grammar.
    /// </summary>
    public const string ConstraintListOpt = "constraintListOpt";

    /// <summary>
    /// Represents a non-terminal for an optional constraint type in the SQL grammar.
    /// </summary>
    public const string ConstraintTypeOpt = "constraintTypeOpt";

    /// <summary>
    /// Represents a non-terminal for a list of identifiers in the SQL grammar.
    /// </summary>
    public const string IdList = "idlist";

    /// <summary>
    /// Represents a non-terminal for a list of identifiers in parentheses in the SQL grammar.
    /// </summary>
    public const string IdListPar = "idlistPar";

    /// <summary>
    /// Represents a non-terminal for an optional UNIQUE constraint in the SQL grammar.
    /// </summary>
    public const string UniqueOpt = "uniqueOpt";

    /// <summary>
    /// Represents a non-terminal for a list of order members in the SQL grammar.
    /// </summary>
    public const string OrderList = "orderList";

    /// <summary>
    /// Represents a non-terminal for an order member in the SQL grammar.
    /// </summary>
    public const string OrderMember = "orderMember";

    /// <summary>
    /// Represents a non-terminal for an optional order direction in the SQL grammar.
    /// </summary>
    public const string OrderDirOpt = "orderDirOpt";

    /// <summary>
    /// Represents a non-terminal for an optional WITH clause in the SQL grammar.
    /// </summary>
    public const string WithClauseOpt = "withClauseOpt";

    /// <summary>
    /// Represents a non-terminal for an ALTER command in the SQL grammar.
    /// </summary>
    public const string AlterCmd = "alterCmd";

    /// <summary>
    /// Represents a non-terminal for data to be inserted in the SQL grammar.
    /// </summary>
    public const string InsertData = "insertData";

    /// <summary>
    /// Represents a non-terminal for an optional INTO clause in the SQL grammar.
    /// </summary>
    public const string IntoOpt = "intoOpt";

    /// <summary>
    /// Represents a non-terminal for a list of assignments in the SQL grammar.
    /// </summary>
    public const string AssignList = "assignList";

    /// <summary>
    /// Represents a non-terminal for an optional WHERE clause in the SQL grammar.
    /// </summary>
    public const string WhereClauseOpt = "whereClauseOpt";

    /// <summary>
    /// Represents a non-terminal for a FROM clause in the SQL grammar.
    /// </summary>
    public const string FromClauseOpt = "fromClauseOpt";

    /// <summary>
    /// Represents a non-terminal for an assignment in the SQL grammar.
    /// </summary>
    public const string Assignment = "assignment";

    /// <summary>
    /// Represents a non-terminal for an expression in the SQL grammar.
    /// </summary>
    public const string Expression = "expression";

}









