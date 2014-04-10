using MyHoard.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MyHoard.Services
{
    public class DatabaseService
    {

        private const string DefaultName="myHoard.sqlite";
        private SQLiteConnection dbConnection;
        private const string ConfigName = "myHoard_config.sqlite";
        private SQLiteConnection configDbConnection;

        public DatabaseService()
        {

            dbConnection = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, DefaultName));
            dbConnection.CreateTable<Collection>();
            dbConnection.CreateTable<Item>();
            dbConnection.CreateTable<Media>();

            configDbConnection = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, ConfigName));
            configDbConnection.CreateTable<Configuration>();
            
        }

        public async Task ChangeDatabase(string userName, string backend, bool copyDefault=false)
        {
            CloseConnection();
            if (!String.IsNullOrWhiteSpace(userName))
            {
                if (copyDefault)
                {
                    if (!System.IO.File.Exists(ApplicationData.Current.LocalFolder.Path + "\\" + backend + "_" + userName + "_" + DefaultName))
                    {
                        StorageFile databaseFile = await ApplicationData.Current.LocalFolder.GetFileAsync(DefaultName);
                        await databaseFile.RenameAsync(backend + "_" + userName + "_" + DefaultName);
                    }
                }
                dbConnection = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, backend + "_" + userName + "_" + DefaultName));
            }
            else
            {
                dbConnection = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, DefaultName));
            }

            dbConnection.CreateTable<Collection>();
            dbConnection.CreateTable<Item>();
            dbConnection.CreateTable<Media>();
        }

        
        public T Add<T>(T item)
        {
            dbConnection.Insert(item);
            return item;
        }

        public Configuration Add(Configuration item)
        {
            configDbConnection.Insert(item);
            return item;
        }

        public T Modify<T>(T item)
        {
            dbConnection.Update(item);
            return item;
        }

        public Configuration Modify(Configuration item)
        {
            configDbConnection.Update(item);
            return item;
        }

        public void Delete<T>(T item)
        {
            dbConnection.Delete(item);
        }

        public void Delete(Configuration item)
        {
            configDbConnection.Delete(item);
        }

        public List<T> ListAll<T>() where T : new()
        {
            if(typeof(T)==typeof(Configuration))
                return configDbConnection.Table<T>().ToList<T>();
            else
                return dbConnection.Table<T>().ToList<T>();
        }

        public TableQuery<T> ListAllTable<T> () where T : new()
        {
            if (typeof(T) == typeof(Configuration))
                return configDbConnection.Table<T>();
            else
                return dbConnection.Table<T>();
        }

      
        public T Get<T>(int id)where T : new()
        {
            if (typeof(T) == typeof(Configuration))
                return configDbConnection.Get<T>(id);
            else
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
