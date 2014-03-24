using MyHoard.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyHoard.Services
{
    public class DatabaseService
    {

        private const string DatabaseName="myHoard.sqlite";
        private SQLiteConnection dbConnection;

        public DatabaseService()
        {

            dbConnection = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseName));
            dbConnection.CreateTable<Collection>();
            dbConnection.CreateTable<Item>();
            dbConnection.CreateTable<Media>();
            dbConnection.CreateTable<Configuration>();
            
        }

        public T Add<T>(T item)
        {
            dbConnection.Insert(item);
            return item;
        }

        public T Modify<T>(T item)
        {
            dbConnection.Update(item);
            return item;
        }

        public void Delete<T>(T item)
        {
            dbConnection.Delete(item);
        }

        public List<T> ListAll<T>() where T : new()
        {
            return dbConnection.Table<T>().ToList<T>();
        }

        public TableQuery<T> ListAllTable<T> () where T : new()
        {
            return dbConnection.Table<T>();
        }       

        public T Get<T>(int id)where T : new()
        {
            return dbConnection.Get<T>(id);
        }

        public void CloseConnection()
        {
            dbConnection.Close();
        }

        public int DeleteAll<T>()
        {
            return dbConnection.DeleteAll<T>();
        }

    }
}
