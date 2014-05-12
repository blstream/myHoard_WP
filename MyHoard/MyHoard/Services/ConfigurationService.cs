using Caliburn.Micro;
using MyHoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class ConfigurationService
    {

        public static Dictionary<string, string> Backends = new Dictionary<string, string>()
        {
            {"Python","http://78.133.154.18:8080"},
            {"Java1","http://78.133.154.39:1080"},
            {"Java2","http://78.133.154.39:2080"}
        };

        private DatabaseService databaseService;
        private Configuration configuration;

        public ConfigurationService()
        {
            databaseService = IoC.Get<DatabaseService>();
            var configs = databaseService.ListAll<Configuration>();
            if (configs.Count > 0)
            {
                configuration = configs.First();
            }
            else
            {
                Configuration = new Configuration();
                databaseService.Add(Configuration);
            }
            if(!string.IsNullOrEmpty(Configuration.Email))
            {
                ChangeUser();
            }
        }

        public async Task ChangeUser(bool copyDefault=false)
        {
            IoC.Get<MediaService>().CleanIsolatedStorage();
            await databaseService.ChangeDatabase(Configuration.Email,Configuration.Backend, copyDefault);
        }

        public Configuration Configuration
        {
            get { return configuration; }
            set { configuration = value; }
        }

        public void SaveConfig()
        {
            databaseService.Modify(Configuration);
        }

        public async void Logout(bool userRequest=false)
        {
            Configuration.IsLoggedIn = false;
            Configuration.KeepLogged = false;
            if(userRequest)
            {
                Configuration.Email = null;
                await ChangeUser();
            }
            SaveConfig();
        }

    }
}
