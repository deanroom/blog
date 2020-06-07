1. [IDbConnection](https://docs.microsoft.com/en-us/dotnet/api/system.data.idbconnection?view=netcore-3.1)
   
   Represents an open connection to a data source, and is implemented by .NET Framework data providers that access relational databases.
2. System.Data.Common.DbConnection
3. System.Data.SqlClient.SqlConnection
   1. Open()/OpenAsync()
   2. Close()/CloseAsync()/Dispose()
      You can also use the Close or Dispose methods of the connection object for the provider that you are using. Connections that are not explicitly closed might not be added or returned to the pool. For example, a connection that has gone out of scope but that has not been explicitly closed will only be returned to the connection pool if the maximum pool size has been reached and the connection is still valid
      
      > :warning: **Do not call Close or Dispose on a Connection, a DataReader, or any other managed object in the Finalize method of your class. In a finalizer, only release unmanaged resources that your class owns directly. If your class does not own any unmanaged resources, do not include a Finalize method in your class definition. For more information, see Garbage Collection.**

   3. BeginTransaction()/BeginTransactionAsync()
   4. Connection Events
      ```csharp
      connection.InfoMessage += new SqlInfoMessageEventHandler(OnInfoMessage);  
  
      protected static void OnInfoMessage(object sender, SqlInfoMessageEventArgs args)  
      {  
         foreach (SqlError err in args.Errors)  
         {  
            Console.WriteLine(  
         "The {0} has received a severity {1}, state {2} error number {3}\n" +  
         "on line {4} of procedure {5} on server {6}:\n{7}",  
            err.Source, err.Class, err.State, err.Number, err.LineNumber,
            err.Procedure, err.Server, err.Message);  
         }  
      }  
      //状态改变
      connection.StateChange  += new StateChangeEventHandler(OnStateChange);  
  
      protected static void OnStateChange(object sender,
      StateChangeEventArgs args)  
      {  
      Console.WriteLine(  
         "The current Connection state has changed from {0} to {1}.",  
            args.OriginalState, args.CurrentState);  
      }  
      ```
4. System.Data.Common.DbTransaction
   ```csharp
      public abstract class DbTransaction   
   ```
5. System.Data.SqlClient.DbTransaction
   ```csharp
      public sealed class SqlTransaction
   ```
6. [Connection Pooling-数据库连接池](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-connection-pooling)
        Connection pooling reduces the number of times that new connections must be opened. The pooler maintains ownership of the physical connection. It manages connections by keeping alive a set of active connections for each given connection configuration. Whenever a user calls Open on a connection, the pooler looks for an available connection in the pool. If a pooled connection is available, it returns it to the caller instead of opening a new connection. When the application calls Close on the connection, the pooler returns it to the pooled set of active connections instead of closing it. Once the connection is returned to the pool, it is ready to be reused on the next Open call.
    
    1. Connections are pooled per process, per application domain, per connection string and when integrated security is used, per Windows identity. Connection strings must also be an exact match; keywords supplied in a different order for the same connection will be pooled separately.\
    2. > :warning: **We strongly recommend that you always close the connection when you are finished using it so that the connection will be returned to the pool. You can do this using either the Close or Dispose methods of the Connection object, or by opening all connections inside a using statement in C#, or a Using statement in Visual Basic. Connections that are not explicitly closed might not be added or returned to the pool. For more information, see using Statement or How to: Dispose of a System Resource for Visual Basic.**