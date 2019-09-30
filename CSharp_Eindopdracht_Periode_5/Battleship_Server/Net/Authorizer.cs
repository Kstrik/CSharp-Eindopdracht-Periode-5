using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Server.Net
{
    public class Authorizer
    {
        public static bool CheckAuthorization(string username, string password)
        {
            return true;
        }

        public static bool AddNewAuthorization(string username, string password)
        {
            if (!Authorizer.CheckAuthorization(username, password))
            {
                return true;
            }
            return false;
        }
    }
}
