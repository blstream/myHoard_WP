using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class DatabaseService
    {

        
        private SQLiteConnection dbConnection;

        public DatabaseService(String databaseName)
        {

            dbConnection = new SQLiteConnection(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, databaseName));

            if (!FileExists(databaseName).Result)
            {
                
            }
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
        public T Get<T>(int id)where T : new()
        {
            return dbConnection.Get<T>(id);
        }

        public void CloseConnection()
        {
            dbConnection.Close();
        }

        private async Task<bool> FileExists(string fileName)
        {
            var result = false;
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                result = true;
            }
            catch
            {
            }

            return result;

        }
    }
}
