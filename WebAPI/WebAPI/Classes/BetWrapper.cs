using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Classes
{
    public class BetWrapper
    {
        public int Stake { get; set; }

        public int Win { get; set; }

        public DateTime TimeStamp { get; set; }

        public BetWrapper(Bet bet)
        {
            this.Stake = bet.Stake;
            this.Win = bet.Win;
            this.TimeStamp = bet.TimeStamp;
        }

        public BetWrapper()
        {

        }
    }
}