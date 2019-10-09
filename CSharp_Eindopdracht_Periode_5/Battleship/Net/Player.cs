using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Net
{
    public class Player
    {
        public string Username { get; set; }
        public string UserId { get; set; }

        public Player(string username, string userId)
        {
            this.Username = username;
            this.UserId = userId;
        }
    }
}
