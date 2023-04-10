using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Maneger
{
    internal class Game
    {
        public void Start() 
        {
            int player = 0;
            int mounth =0;
            Console.WriteLine("Добро пожаловать выберите количество игроков");
            player= Convert.ToInt32(Console.ReadLine());
            Player[] player1= new Player[player];
           for (int i = 0;i<player1.Length;i++)
            {
                player1[i]= new Player();
            }
            Console.Clear();
            Console.WriteLine("Выберите количество кругов игры");
            mounth= Convert.ToInt32(Console.ReadLine());
            Console.Clear();
            for (int i = 0; i < mounth&&player1.Length>0; i++)
            {
                for (int j = 0; j < player1.Length; j++)
                {
                    Player.Older(player1);
                    Player.ConstantCosts(player1);
                    player1[i].ApplicationsECM(player1);
                    player1[i].DistributionOfApplicationsByTheBankECM(player1);
                    Player.ShowingApplicationsECM(player1);
                    player1[i].ProductionOfProducts(player1);
                    player1[i].SaleOfProducts(player1);
                    Player.ShowingApplicationsEGP(player1);
                    player1[i].DistributionOfApplicationsByTheBankEGP(player1);
                    player1[i].InterestPayment(player1);
                    player1[i].LoanRepayment(player1);
                    player1[i].GettingLoans(player1);
                    player1[i].BuildingApplications(player1);
                    player1[i].ZeroingApplications(player1);
                    Bank.month++;
                    mounth++;
                }
                
                
            }
            Console.WriteLine("Конец");
        }

    }
}
