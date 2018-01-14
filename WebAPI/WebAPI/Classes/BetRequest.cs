using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Classes
{
    public class BetRequest
    {
        public int UserId { get; set; }

        public int Sum { get; set; }

        public int Stake { get; set; }
    }
}