using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF;

namespace Demos.Data {
    public static class DemoEFDatabaseHelper {
        public static string SQLiteUsageMessage = "Since SQLite does not support database schema update operations, XAF’s Demo application cannot save any changes you make to business classes. ";
        public static string AlternativeName = "SQLite with EntityFramework";
        public static EFObjectSpaceProvider CreateObjectSpaceProvider(Type dbContextType, string efConnectionString, string sqliteConnectionString) {
            string patchedEFConnectionString = DemoDbEngineDetectorHelper.PatchSQLConnectionString(efConnectionString);
            if((patchedEFConnectionString == DemoDbEngineDetectorHelper.AlternativeConnectionString) || !DemoDbEngineDetectorHelper.IsSqlServerAccessible(patchedEFConnectionString)) {
                UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.GetIssueMessage(patchedEFConnectionString), AlternativeName, SQLiteUsageMessage);
                return new EFObjectSpaceProvider(dbContextType, new SQLiteConnection(sqliteConnectionString));
            }
            else {
                return new EFObjectSpaceProvider(dbContextType, patchedEFConnectionString);
            }
        }
        public static EFObjectSpaceProvider CreateSQLiteObjectSpaceProvider(Type dbContextType, ITypesInfo typesInfo, string sqliteConnectionString) {
            return new EFObjectSpaceProvider(dbContextType, typesInfo, null, new SQLiteConnection(sqliteConnectionString));
        }
        public static EFObjectSpaceProvider CreateObjectSpaceProvider(Type dbContextType, ITypesInfo typesInfo, string efConnectionString, string sqliteConnectionString) {
            EFObjectSpaceProvider result = null;
            if(UseSQLAlternativeInfoSingleton.Instance.UseAlternative || efConnectionString == sqliteConnectionString) {
                result = new EFObjectSpaceProvider(dbContextType, typesInfo, null, new SQLiteConnection(sqliteConnectionString));
            }
            else {
                if(DemoDbEngineDetectorHelper.IsSqlServerAccessible(efConnectionString)) {
                    result = new EFObjectSpaceProvider(dbContextType, typesInfo, null, efConnectionString);
                }
                else {
                    UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.DBServerIsNotAccessibleMessage, AlternativeName, SQLiteUsageMessage);
                    result = new EFObjectSpaceProvider(dbContextType, typesInfo, null, new SQLiteConnection(sqliteConnectionString));
                }
            }
            return result;
        }
    }
}
