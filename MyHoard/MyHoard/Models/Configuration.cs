using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models
{
    public class Configuration
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsLoggedIn { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
