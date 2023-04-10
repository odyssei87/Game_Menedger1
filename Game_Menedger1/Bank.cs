using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Game_Maneger
{
    internal class Bank
    {
        private static Bank? instance;
        public int[,] probability_of_transition;
        private double[] amountECM = { 1, 1.5, 2, 2.5, 3 };
        private double[] amountEGP = { 3, 2.5, 2, 1.5, 1 };
        private int[] priceECM = {800, 650, 500, 400, 300 };
        private int[] priceEGP = { 6500, 6000, 5500, 5000, 4500 };
        public int level;
        public static int month;
        public Bank()
        {
             probability_of_transition = new int[5,12]
            {
                { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4, 5 },
                { 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 5 },
                { 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 5 },
                { 1, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5 },
                { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5 },
            };
            level = 3;
            month=1;
        }
        public static Bank getInstance()
        {
            if (instance == null)
                instance = new Bank();
            return instance;
        }
        public int Transition(int level, int[,] probability_of_transition)  // генерация следующего уровня
        {
            Random random = new Random();
            return level = probability_of_transition[level - 1, random.Next(12)];
        }
        public double AmountECM(int lavel)  // количество ecm на текущем уровне
        {
            return amountECM[lavel-1]*Player.playerN;
        }
        public double AmountEGP(int lavel)   // количество egp на текущем уровне
        {
            return amountEGP[lavel-1] * Player.playerN;
        }
        public int PriceECM(int lavel)
        {
            return priceECM[lavel - 1];
        }
        public int PriceEGP(int lavel)
        {
            return priceECM[lavel-1];
        }
       
    }
}
 