namespace Data.Common.Utils;

/// <summary>
/// This class will get more complex when it must support more tracking than just INSERT'd rows.  Currently the EF Core providers built on this 
/// library do not make transactional calls requiring support to track UPDATE and DELETE.
/// </summary>
public class TransactionScopedRows : Dictionary<string, List<DataRow>>    //The key to this dictionary is the name of the table that the DataRow belongs to.
{

}
