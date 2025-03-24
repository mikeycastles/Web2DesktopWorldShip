using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
// Add a reference to the ADOX COM library (Microsoft ADO Ext. for DDL and Security)
using ADOX;
using System.IO;

public static class AccessHelper
{
    // Adjust the connection string as needed. For .mdb files you might use Microsoft.Jet.OLEDB.4.0.
    private static readonly string _connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shipping.mdb;Persist Security Info=False;";

    #region Public Methods (Open New Connections)

    /// <summary>
    /// Checks if the database file exists and creates it if it doesn't.
    /// </summary>
    public static void EnsureDatabaseExists()
    {
        try
        {
            if (!File.Exists("shipping.mdb"))
            {
                Console.WriteLine($"Database file 'shipping.mdb' not found. Creating database from template...");
                File.Copy("_blank.accdb", "shipping.mdb");
            }
            else
            {
                Console.WriteLine($"Database file 'shipping.mdb' already exists.");
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    /// <summary>
    /// Checks if a record exists in the specified table.
    /// </summary>
    public static bool RecordExists(string tableName, string columnName, object value)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                if (!TableExists(conn, tableName))
                {
                    Console.WriteLine($"Error: Table '{tableName}' does not exist.");
                    return false;
                }
                // OleDb uses positional parameter markers (?)
                string sql = $"SELECT COUNT(1) FROM [{tableName}] WHERE [{columnName}] = ?";
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("?", value);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return false;
        }
    }

    /// <summary>
    /// Checks if a table exists in the database.
    /// </summary>
    public static bool TableExists(string tableName)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                return TableExists(conn, tableName);
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return false;
        }
    }

    /// <summary>
    /// Inserts data into a table within a transaction.
    /// </summary>
    public static void InsertData(string tableName, (string Column, object Value)[] values)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    InsertDataInternal(conn, transaction, tableName, values);
                    transaction.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    /// <summary>
    /// Inserts or updates a record in a single transaction.
    /// </summary>
    public static void InsertOrUpdate(string tableName, string keyColumn, object keyValue, (string Column, object Value)[] values)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    bool exists = RecordExistsInternal(conn, transaction, tableName, keyColumn, keyValue);
                    if (exists)
                    {
                        UpdateRecordInternal(conn, transaction, tableName, keyColumn, keyValue, values);
                    }
                    else
                    {
                        // Only append the key column/value if it's not already provided.
                        var finalValues = values.Any(v => v.Column.Equals(keyColumn, StringComparison.OrdinalIgnoreCase))
                            ? values
                            : values.Append((keyColumn, keyValue)).ToArray();

                        InsertDataInternal(conn, transaction, tableName, finalValues);
                    }
                    transaction.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    /// <summary>
    /// Executes a non-query command (INSERT, UPDATE, DELETE).
    /// </summary>
    public static int ExecuteNonQuery(string sql, params (string Param, object Value)[] parameters)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    foreach (var (param, value) in parameters)
                    {
                        // Parameters are added in order for OleDb.
                        cmd.Parameters.AddWithValue("?", value);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return -1;
        }
    }

    /// <summary>
    /// Executes a scalar query and returns a single value.
    /// </summary>
    public static object ExecuteScalar(string sql, params (string Param, object Value)[] parameters)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    foreach (var (param, value) in parameters)
                    {
                        cmd.Parameters.AddWithValue("?", value);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return null;
        }
    }

    /// <summary>
    /// Executes a query and returns a DataTable.
    /// </summary>
    public static DataTable ExecuteQuery(string sql, params (string Param, object Value)[] parameters)
    {
        try
        {
            EnsureDatabaseExists();
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    foreach (var (param, value) in parameters)
                    {
                        cmd.Parameters.AddWithValue("?", value);
                    }
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return null;
        }
    }

    /// <summary>
    /// Creates a table if it does not already exist.
    /// </summary>
    public static void CreateTableIfNotExists(string tableName, string createTableSql)
    {
        try
        {
            using (var conn = new OleDbConnection(_connectionString))
            {
                conn.Open();
                if (!TableExists(conn, tableName))
                {
                    Console.WriteLine($"Table '{tableName}' does not exist. Creating table...");
                    using (var cmd = new OleDbCommand(createTableSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    Console.WriteLine($"Table '{tableName}' already exists.");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    #endregion

    #region Internal Methods (Using Existing Connection/Transaction)

    /// <summary>
    /// Checks for table existence using the given connection.
    /// </summary>
    private static bool TableExists(OleDbConnection conn, string tableName)
    {
        try
        {
            // Get the schema info for tables matching the provided table name.
            DataTable schema = conn.GetSchema("Tables", new string[] { null, null, tableName, "TABLE" });
            return schema.Rows.Count > 0;
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return false;
        }
    }

    /// <summary>
    /// Checks if a record exists using the current connection and transaction.
    /// </summary>
    private static bool RecordExistsInternal(OleDbConnection conn, OleDbTransaction transaction,
                                               string tableName, string columnName, object value)
    {
        try
        {
            if (!TableExists(conn, tableName))
            {
                Console.WriteLine($"Error: Table '{tableName}' does not exist.");
                return false;
            }
            string sql = $"SELECT COUNT(1) FROM [{tableName}] WHERE [{columnName}] = ?";
            using (var cmd = new OleDbCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("?", value);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return false;
        }
    }

    /// <summary>
    /// Inserts data using the current connection and transaction.
    /// </summary>
    private static void InsertDataInternal(OleDbConnection conn, OleDbTransaction transaction,
                                           string tableName, (string Column, object Value)[] values)
    {
        try
        {
            if (!TableExists(conn, tableName))
            {
                Console.WriteLine($"Error: Table '{tableName}' does not exist and no create logic provided.");
                return;
            }
            string columns = string.Join(", ", values.Select(v => $"[{v.Column}]"));
            // Use "?" as a positional parameter placeholder for each column.
            string parameters = string.Join(", ", values.Select(v => "?"));
            string sql = $"INSERT INTO [{tableName}] ({columns}) VALUES ({parameters})";
            using (var cmd = new OleDbCommand(sql, conn, transaction))
            {
                foreach (var (column, value) in values)
                {
                    cmd.Parameters.AddWithValue("?", value);
                }
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    /// <summary>
    /// Updates a record using the current connection and transaction.
    /// </summary>
    private static void UpdateRecordInternal(OleDbConnection conn, OleDbTransaction transaction,
                                             string tableName, string keyColumn, object keyValue,
                                             (string Column, object Value)[] values)
    {
        try
        {
            if (!TableExists(conn, tableName))
            {
                Console.WriteLine($"Error: Table '{tableName}' does not exist.");
                return;
            }
            string setClause = string.Join(", ", values.Select(v => $"[{v.Column}] = ?"));
            string sql = $"UPDATE [{tableName}] SET {setClause} WHERE [{keyColumn}] = ?";
            using (var cmd = new OleDbCommand(sql, conn, transaction))
            {
                foreach (var (column, value) in values)
                {
                    cmd.Parameters.AddWithValue("?", value);
                }
                cmd.Parameters.AddWithValue("?", keyValue);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    #endregion
}
