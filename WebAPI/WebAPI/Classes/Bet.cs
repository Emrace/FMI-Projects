using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Classes
{
    public class Bet
    {
        private static Dictionary<int, int> payouts = new Dictionary<int, int>
        {
            {2, 33},  //33 to 1
            {3, 16},  //16 to 1
            {4, 10},  //10 to 1
            {5, 8},   //8 to 1
            {6, 6},   //6 to 1
            {7, 5},   //5 to 1
            {8, 6},   //6 to 1
            {9, 8},   //8 to 1
            {10, 10}, //10 to 1
            {11, 16}, //16 to 1
            {12, 33}, //33 to 1
        };

        public static int IdCounter = 0;

        public int UserId { get; set; }

        public int BetId { get; set; }

        public int DiceSum { get; set; }

        public int Stake { get; set; }

        public int Win { get; set; }

        public int ActualRoll { get; set; }

        public DateTime TimeStamp { get; set; }

        public void RollDice()
        {
            Random rand = new Random();

            int firstDice = rand.Next(1, 7);
            int secondDice = rand.Next(1, 7);

            int result = firstDice + secondDice;

            this.ActualRoll = result;

            if (result == this.DiceSum)
                this.Win = this.Stake * payouts[result];
            else
                this.Win = 0;
        }
    }
}