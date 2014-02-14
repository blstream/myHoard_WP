using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoardTest
{
    [TestClass]
    public class DatabaseServiceTests
    {
        [TestMethod]
        public void AddingItemTest()
        {
            DatabaseService dbService = new DatabaseService("testdb.sqlite");

            Collection expected = dbService.Add<Collection>(new Collection { Name = "Collection1", Description = "Sample Collection" });

            Collection actual = dbService.Get<Collection>(expected.Id);

            Assert.AreEqual(expected.Id,actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Description, actual.Description);
            dbService.CloseConnection();
            
        }

        [TestMethod]
        public void DeletingItemTest()
        {
            DatabaseService dbService = new DatabaseService("testdb.sqlite");

            Collection item = dbService.Add<Collection>(new Collection { Name = "Collection1", Description = "Sample Collection" });

            dbService.Delete<Collection>(item);
            Assert.ThrowsException<System.InvalidOperationException>(() => dbService.Get<Collection>(item.Id));
            dbService.CloseConnection();
        }
    }
}
