using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EF;

namespace XCRM.Module.Dashboards {
    public static class SqlDashboardHelper {
        public static DataConnectionParametersBase GetSqlParameters(IObjectSpace objectSpace) {
            EFObjectSpace efObjectSpace = objectSpace as EFObjectSpace;
            if(efObjectSpace != null) {
                EntityConnection connection = efObjectSpace.ObjectContext.Connection as EntityConnection;
                if(connection != null) {
                    if(connection.StoreConnection is SQLiteConnection) {
                        return GetSQLiteParameters(connection);
                    }
                    else {
                        return GetMsSqlParameters(connection);
                    }
                }
            }
            return null;
        }
        private static MsSqlConnectionParameters GetMsSqlParameters(EntityConnection connection) {
            MsSqlConnectionParameters connectionParameters = new MsSqlConnectionParameters();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection.StoreConnection.ConnectionString);
            connectionParameters.ServerName = connection.DataSource;
            connectionParameters.DatabaseName = builder.InitialCatalog;
            connectionParameters.AuthorizationType = MsSqlAuthorizationType.Windows;
            if(builder.IntegratedSecurity == false) {
                connectionParameters.AuthorizationType = MsSqlAuthorizationType.SqlServer;
                connectionParameters.UserName = builder.UserID;
                connectionParameters.Password = builder.Password;
            }
            return connectionParameters;
        }
        private static SQLiteConnectionParameters GetSQLiteParameters(EntityConnection connection) {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(connection.StoreConnection.ConnectionString);
            return new SQLiteConnectionParameters(builder.DataSource, builder.Password);
        }
    }
}
