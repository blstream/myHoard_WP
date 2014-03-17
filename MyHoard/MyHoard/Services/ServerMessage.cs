using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class ServerMessage
    {
        private bool isSuccessfull;
        private string message;

        public ServerMessage(bool isSuccessfull, string message)
        {
            IsSuccessfull = isSuccessfull;
            Message = message;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public bool IsSuccessfull
        {
            get { return isSuccessfull; }
            set { isSuccessfull = value; }
        }
    }
}
