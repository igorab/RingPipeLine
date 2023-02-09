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
        //public static string connectionStringExt = "Data Source=usersdata3.db;Cache=Shared;Mode=ReadOnly;";
        
        public void InitConnection()
        {
            string connstr = "Data Source=usersdata3.db;Cache=Shared;Mode=ReadOnly;";
        }

        public bool CreateConnection()
        {
            //SQLiteConnection
            //SQLitePCL
            InitConnection();

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
            // работа с БД
            //SQLiteConnection sqLiteConnection = new SQLiteConnection();
            //sqLiteConnection.InitConnection();
            //bool con = sqLiteConnection.CreateConnection();


            // Граф
            //CalcModule_QGraph qGraph = new CalcModule_QGraph();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formMain = new FormMain();
            formTools = new FormTools();
            formTools.Show();
            Application.Run(formMain);
        }
    }
}
