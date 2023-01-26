using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Properties;


namespace RingPipeLine
{
    
    class SQLiteConnection
    {
        public string connectionString = "Data Source=Hydro.db";
        public string connectionStringExt = "Data Source=usersdata3.db;Cache=Shared;Mode=ReadOnly;";

        public bool CreateConnection()
        {
            //SQLiteConnection
            //SQLitePCL

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                return true;
            }

            return false;
        }
    }
    
    static class Program
    {
        public static FormMain formMain;
        public static FormTools formTools;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        
        [STAThread]
        static void Main()
        {
            bool con = new SQLiteConnection().CreateConnection();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formMain = new FormMain();
            formTools = new FormTools();
            formTools.Show();
            Application.Run(formMain);
        }
    }
}
