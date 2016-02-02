using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace PassExt.Models
{
    public class Connector
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["ptconn"].ConnectionString;
        //private string _connectionString = "Server=192.168.189.128;Database=passtrans;Uid=root;Pwd=!?admin123;";
        public IDbConnection DatabaseConnection;
        //Подключение к базе и его статус
        public bool ConnectToDatabase(string connectionString)
        {
            bool success = false;
            DatabaseConnection = new MySqlConnection(connectionString);
            try
            {
                DatabaseConnection.Open();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        public MySqlConnection CreateOneMoreConnection(string connectionString)
        {
            MySqlConnection Con = null;
            try
            {
                Con = new MySqlConnection(connectionString);
                Con.Open();
            }
            catch (Exception)
            {
                Con = null;
            }
            return Con;
        }
        //Отключение от базы
        public void DisconnectFromDatabase()
        {
            //if (_databaseConnection.State != ConnectionState.Open)
            //{
            DatabaseConnection.Close();
            //}
        }
        // Ридэр
        public IDataReader ExecuteReadCommand(IDbCommand command)
        {
            IDataReader dataReader = null;
            if (DatabaseConnection.State == ConnectionState.Open)
            {
                dataReader = command.ExecuteReader();
            }
            return dataReader;
        }
        // ЛОГ
        public List<string> _Log = new List<string>();
        public List<string> Log { get { return _Log; } }
    }
}