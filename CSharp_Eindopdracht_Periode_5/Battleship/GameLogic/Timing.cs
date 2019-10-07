using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.GameLogic
{
    public class Timing
    {
        private static Timing instance;

        private DateTime time1;
        private DateTime time2;

        public float DeltaTime { get; set; }

        private Timing()
        {
            this.time1 = DateTime.Now;
            this.time2 = DateTime.Now;
        }

        public void Update()
        {
            this.time2 = DateTime.Now;
            this.DeltaTime = (this.time2.Ticks - this.time1.Ticks) / 10000000f;
            this.time1 = this.time2;
        }

        public void Reset()
        {
            this.time1 = DateTime.Now;
            this.time2 = DateTime.Now;
        }

        public static Timing GetInstance()
        {
            if(instance == null)
            {
                instance = new Timing();
            }
            return instance;
        }
    }
}
