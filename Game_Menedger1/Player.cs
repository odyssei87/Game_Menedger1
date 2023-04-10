using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic;


namespace Game_Maneger
{
    internal class Player
    {
        public bool older;
        public bool bankrupt;
        public static int playerN = 0;
        Bank bank = new Bank();
        public double Balance { get; set; }
        public List<Factory> Factory { get; set; }
        public List<AutomaticFactory> AutomaticFactory { get; set; }
        public double ECM { get; set; }
        public double EGP { get; set; }
        public double RequestECM { get; set; }
        public double RequestEGP { get; set; }
        public double PriceECM { get; set; }
        public int PriceEGP { get; set; }
        public int Loan { get; set; }
        public int RepaymentAmount { get; set; }
        public int LoanMonth { get; set; }
        public int RequestFactory { get; set; }
        public int RequestAutomaticFactory { get; set; }
        public int RequestImprovementFactory { get; set; }
        public Player()
        {
            RequestFactory = 0;
            RequestAutomaticFactory = 0;
            RequestImprovementFactory = 0;
            Balance = 10000;
            ECM = 4;
            EGP = 2;
            Factory = new List<Factory>() {new Factory(), new Factory()};
            AutomaticFactory = new List<AutomaticFactory>();
            PriceECM = 0;
            PriceEGP = 0;
            playerN++;
            older= false;
            bankrupt= false;
            Loan = 0;
            LoanMonth = 0;
        }
        public static void Older(Player [] player)        // старший игрок
        {
            if (Bank.month==1)
            {
                Console.WriteLine($"В Месяце {Bank.month} старший игрок 1");
                player[0].older = true;
            }
            else
            {
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i].older==true)
                    {
                        player[i].older = false;
                    }
                }
                int a = (((Bank.month - 1) % player.Length)+1);
                if (a > player.Length+1)
                {
                    player[0].older = true;
                }
                else
                {
                    player[a-1].older = true;
                }
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i].older == true)
                    {
                        Console.WriteLine($"В Месяце {Bank.month} старший игрок {i+1}");
                    }
                }
            }
        }
        public static void ConstantCosts(Player [] player)         // постоянные издержки
        {
            for (int i = 0; i < player.Length; i++)
            {
                double Costs = (player[i].ECM * 300) + (player[i].EGP * 500) + (player[i].Factory.Count * 1000) + (player[i].AutomaticFactory.Count * 1500);
                player[i].Balance -= Convert.ToInt32(Costs);
                if (player[i].Balance<=0)
                {
                    Console.WriteLine($"Игрок {player[i]} банкрот");
                    Console.WriteLine($"Ваш баланс {player[i].Balance}");
                    int index = i;

                    for (int j = index; j < player.Length - 1; j++)
                    {
                        player[i] = player[i + 1];
                    }
                    Array.Resize(ref player, player.Length - 1);
                }
            }
        }
        public void ApplicationsECM(Player[] player)             //Заявка на сырье для игрока
        {
            for (int i = 0; i < player.Length; i++)
            {
                Console.Clear();
                Console.WriteLine($"Ход игрока {i + 1}\n");
                if (bank.PriceECM(bank.level) > player[i].Balance)
                {
                    Console.WriteLine("У Вас не достаточно средств для участия в торгах");
                }
                else
                {
                    string? temp;
                    double requestECM = 0;
                    int requestPrice = 0;
                    Console.WriteLine("Заявка на ECM\n");
                    Console.WriteLine("Укажите требуемое количество ECM");
                    Console.WriteLine($"На данном уровне доступно {bank.AmountECM(bank.level)}");
                    temp = Console.ReadLine();
                    Console.Clear();
                    while (!Double.TryParse(temp, out requestECM))
                    {
                        Console.WriteLine("Заявка на ECM");
                        Console.WriteLine("Укажите требуемое количество ECM");
                        Console.WriteLine($"На данном уровне доступно {bank.AmountECM(bank.level)}");
                        Console.WriteLine("\nНужно ввести целое положительное число");
                        temp = Console.ReadLine();
                    }
                    while (Convert.ToDouble(requestECM) > bank.AmountECM(bank.level))
                    {
                        Console.Clear();
                        Console.WriteLine("Банк не может предоставить данное количество ECM");
                        Console.WriteLine($"На данном уровне доступно {bank.AmountECM(bank.level)}\n");
                        Console.WriteLine("Укажите требуемое количество ECM");
                        temp = Console.ReadLine();
                        while (!Double.TryParse(temp, out requestECM))
                        {
                            Console.WriteLine("Заявка на ECM");
                            Console.WriteLine("Укажите требуемое количество ECM");
                            Console.WriteLine($"На данном уровне доступно {bank.AmountECM(bank.level)}");
                            Console.WriteLine("\nНужно ввести целое положительное число");
                            temp = Console.ReadLine();
                        }
                    }
                    while (true)
                    {
                        if ((requestECM * bank.PriceECM(bank.level)) > player[i].Balance)
                        {
                            Console.WriteLine("Не хватает средств на заданное количество ECM\nВыберите другое количество ECM");
                            temp = Console.ReadLine();
                            while (!Double.TryParse(temp, out requestECM))
                            {
                                Console.WriteLine("Заявка на ECM");
                                Console.WriteLine("Укажите требуемое количество ECM");
                                Console.WriteLine($"На данном уровне доступно {bank.AmountECM(bank.level)}");
                                Console.WriteLine("\nНужно ввести целое положительное число");
                                temp = Console.ReadLine();
                            }
                        }
                        else { break; }

                    }
                    if (requestECM == 0)
                    {
                        Console.WriteLine("Вы исключены из торгов");
                    }
                    else
                    {
                        player[i].RequestECM = requestECM;
                        Console.Clear();
                        Console.WriteLine("Укажите желаемую цену");
                        Console.WriteLine("Минимальная цена в этом месяце " + bank.PriceECM(bank.level));
                        temp = Console.ReadLine();
                        Console.Clear();
                        while (!Int32.TryParse(temp, out requestPrice))
                        {
                            Console.WriteLine("Укажите желаемую цену");
                            Console.WriteLine("Минимальная цена в этом месяце " + bank.PriceECM(bank.level));
                            Console.WriteLine("\nНужно ввести целое положительное число");
                            temp = Console.ReadLine();
                        }
                        if (requestPrice < bank.PriceECM(bank.level))
                        {
                            Console.WriteLine("Цена ниже минимальной. Вы исключены из торгов в этом месяце");
                        }
                        else { player[i].PriceECM = requestPrice; }
                    }
                }
                
                
            }
        }
        public void DistributionOfApplicationsByTheBankECM(Player[] players)            // Распределение заявок на ECM банком
        {
            bool brecker = false;
            Dictionary<int, double> requestEcm = new Dictionary<int, double>();
            Dictionary<int, double> temp = new Dictionary<int, double>();
            for (int i = 0; i < players.Length; i++)
            {
                requestEcm.Add(i, players[i].RequestECM);
                temp.Add(i, players[i].PriceECM);
            }
            var sortedDict = from entry in temp orderby entry.Value descending select entry;
            int older = 0;
            foreach (var item in sortedDict)
            {
                if (players[item.Key].older)
                {
                    older = item.Key;
                }
            }
            List<int> received = new List<int>();
            int player = 0;
            double count = bank.AmountECM(bank.level);
            while (count == 0|| brecker==true)
            {
                for (int i = 0; i < sortedDict.Count(); i++)
                {
                    if (sortedDict.ElementAt(i).Key == older)
                    {
                        if ((count -players[i].RequestECM)<0)
                        {
                            double a = players[i].RequestECM;
                            while (count!=a)
                            {
                                a--;
                            }
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                players[i].ECM += a;
                                Console.WriteLine($"Игрок {player+1} получает {a}");
                                count -= a;
                                received.Add(i);
                            }
                        }
                        else
                        {
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                Console.WriteLine($"Игрок {player+1} получает {requestEcm.ElementAt(i).Value}");
                                players[i].ECM += requestEcm.ElementAt(i).Value;
                                players[i].Balance -= players[i].PriceECM * (int)players[i].RequestECM;
                                count -= (int)players[i].RequestECM;
                                older = -1;
                                received.Add(i);
                            }
                        }
                    }
                    else
                    {
                        for (int j = i + 1; j < sortedDict.Count(); j++)
                        {
                            if (sortedDict.ElementAt(i).Value == sortedDict.ElementAt(j).Value)
                            {
                                if (sortedDict.ElementAt(j).Key == older)
                                {
                                    if ((count - (int)players[i].RequestECM) < 0)
                                    {
                                        int a = (int)players[i].RequestECM;
                                        while (count != a)
                                        {
                                            a--;
                                        }
                                        if (received.Contains(i))
                                        {
                                        }
                                        else
                                        {
                                            player = sortedDict.ElementAt(i).Key;
                                            players[i].ECM += a;
                                            Console.WriteLine($"Игрок {player + 1} получает {a}");
                                            count -= a;
                                            received.Add(i);
                                        }
                                    }
                                    else
                                    {
                                        if (received.Contains(i))
                                        {
                                        }
                                        else
                                        {
                                            player = sortedDict.ElementAt(i).Key;
                                            Console.WriteLine($"Игрок {player+1} получает {requestEcm.ElementAt(j).Value}");
                                            players[j].ECM += requestEcm.ElementAt(j).Value;
                                            players[j].Balance -= players[j].PriceECM * (int)players[j].RequestECM;
                                            count -= (int)players[i].RequestECM;
                                            older = -1;
                                            received.Add(i);
                                        }
                                    }
                                }
                            }
                        }
                        if ((count - (int)players[i].RequestECM) < 0)
                        {
                            int a = (int)players[i].RequestECM;
                            while (count != a)
                            {
                                a--;
                            }
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                players[i].ECM += a;
                                Console.WriteLine($"Игрок {player+1} получает {a}");
                                count -= a;
                                received.Add(i);
                                
                            }
                        }
                        else
                        {
                            if (received.Contains(i))
                            { 
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                Console.WriteLine($"Игрок {player+1} получает {requestEcm.ElementAt(i).Value}");
                                players[i].ECM += requestEcm.ElementAt(i).Value;
                                players[i].Balance -= players[i].PriceECM * (int)players[i].RequestECM;
                                count -= (int)players[i].RequestECM;
                                received.Add(i);
                                
                            }
                        }
                    }
                    if ((i+1)> sortedDict.Count())
                    {
                        brecker = true;
                    }
                }
            }
        }
        public void ZeroingApplications(Player[] players )             // обнуление заявок 
        {
            foreach (var item in players)
            {
                item.RequestECM = 0;
                item.RequestEGP = 0;
                item.PriceECM = 0;
                item.PriceEGP = 0;
            }
        }
        public static void ShowingApplicationsECM(Player[] players)       // вывод заявок на покупку игроком ECM
        {
            int i = 1;
            foreach (var player in players)
            {
                Console.WriteLine($"Заявка игрока {i} :\n количество ECM = {player.RequestECM} , Цена = {player.PriceECM}");
                Console.WriteLine("\n");
                i++;
            }
        }
        public static void ShowingApplicationsEGP(Player[] players)  // вывод заявок на продажу игроком EGP
        {
            int i = 1;
            foreach (var player in players)
            {
                Console.WriteLine($"Заявка игрока {i} :\n количество EGP = {player.RequestEGP} , Цена = {player.PriceEGP}");
                Console.WriteLine("\n");
                i++;
            }
        }
        public void ProductionOfProducts(Player[] player)             // Производство продукции
        {
            bool brecker = false;
            double ecm;
            int factory = 0;
            int ansver =0;
            string? temp;
            int involvedAutomaticFactory = 0;
            int involvedFactory = 0;
            for (int i = 0; i < player.Length; i++)
            {
                Console.WriteLine($"Ход игрока {i + 1}");
                while (!brecker)
                {
                    Console.WriteLine($"У Вас в наличие  :\nЕдениц ЕСМ = {player[i].ECM}\nСвободных простых фабрик = {player[i].Factory.Count - involvedFactory}\nСвободных автоматизированных фабрик = {player[i].AutomaticFactory.Count - involvedAutomaticFactory}\n");
                    Console.WriteLine("Сколько ECM Вы хотите переработать в EGP?\nЕсли хотите пропустить введите - 0");
                    temp = Console.ReadLine();
                    while (!double.TryParse(temp, out ecm))
                    {
                        Console.Clear();
                        Console.WriteLine("Сколько ECM Вы хотите переработать в EGP?\nЕсли хотите пропустить введите - 0");
                        Console.WriteLine("\nНужно ввести целое положительное число");
                        temp = Console.ReadLine();
                    }
                    Console.Clear();
                    ecm = Math.Abs(ecm);
                    if (ecm == 0)
                    {
                        return;
                    }
                    while (true)
                    {
                        Console.WriteLine("На какой фабрике : \n1 - Обычная фабрика\n2 - Автоматизированная фабрика");
                        temp = Console.ReadLine();
                        while (!Int32.TryParse(temp, out factory))
                        {
                            Console.Clear();
                            Console.WriteLine("На какой фабрике : \n1 - Обычная фабрика\n2 - Автоматизированная фабрика");
                            Console.WriteLine("\nНужно ввести целое положительное число");
                            temp = Console.ReadLine();
                        }
                        if (factory > 0 && factory < 3)
                        {
                            break;
                        }
                        else { Console.WriteLine("Не верный выбор"); }
                    }
                    factory = Math.Abs(factory);
                    Console.Clear();
                    if (factory == 2)
                    {
                        while (true)
                        {
                            Console.WriteLine("1 - Переработать 1 ЕСМ за 2000$\n2 - переработать 2 ЕСМ за 3000$");
                            temp = Console.ReadLine();

                            while (!Int32.TryParse(temp, out ansver))
                            {
                                Console.Clear();
                                Console.WriteLine("1 - Переработать 1 ЕСМ за 2000$\n2 - переработать 2 ЕСМ за 3000$");
                                Console.WriteLine("\nНужно ввести целое положительное число");
                                temp = Console.ReadLine();
                            }
                            if (ansver > 0 && ansver < 3)
                            {
                                break;
                            }
                            else { Console.Clear(); Console.WriteLine("Не верный выбор"); }
                        }
                    }
                    Console.Clear();
                    if (player[i].ECM < ecm)
                    {
                        Console.WriteLine("У Вас не достаточно ЕСМ");
                        Console.WriteLine($"В наличии {player[i].ECM} ecm");
                    }
                    else if (ecm > player[i].Factory.Count && factory == 1)
                    {
                        Console.WriteLine("У Вас не достаточно фабрик для переработки данного количества ЕСМ");
                    }
                    else if (ecm * 2000 > player[i].Balance && factory == 1)
                    {
                        Console.WriteLine("У Вас не достаточно средств");
                        Console.WriteLine($"Ваш баланс {player[i].Balance}$");
                    }
                    else if (factory == 2 && ecm > player[i].AutomaticFactory.Count && ecm * 2000 > player[i].Balance)
                    {
                        Console.WriteLine("У Вас нет автоматизированных фабрик или не хватает средств на переработку данного количества ЕСМ");
                        Console.WriteLine($"Ваш баланс {player[i].Balance}$");
                        Console.WriteLine($"Количество автоматизированных фабрик равно {player[i].AutomaticFactory.Count}\n\n");
                    }
                    else if (factory == 2 && ecm > player[i].AutomaticFactory.Count * 2 && ecm * 3000 > player[i].Balance)
                    {
                        Console.WriteLine("У Вас нет автоматизированных фабрик или не хватает средств на переработку данного количества ЕСМ");
                        Console.WriteLine($"Ваш баланс {player[i].Balance}$");
                        Console.WriteLine($"Количество автоматизированных фабрик равно {player[i].AutomaticFactory.Count}\n\n");
                    }
                    else
                    {
                        switch (factory)
                        {
                            case 1:
                                {
                                    player[i].ECM -= ecm;
                                    player[i].Balance -= ecm * 2000;
                                    involvedFactory++;
                                }
                                break;
                            case 2:
                                {
                                    if (ansver == 2)
                                    {
                                        player[i].ECM -= ecm;
                                        player[i].Balance -= ecm * 3000;
                                        involvedAutomaticFactory++;
                                    }
                                    else
                                    {
                                        player[i].ECM -= ecm;
                                        player[i].Balance -= ecm * 2000;
                                        involvedAutomaticFactory++;
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                        if (player[i].ECM > 0 && (player[i].Factory.Count - involvedFactory > 0 || player[i].AutomaticFactory.Count - involvedAutomaticFactory > 0))
                        {
                            while (true)
                            {
                                Console.WriteLine($"У Вас осталось :\nЕдениц ЕСМ = {player[i].ECM}\nСвободных простых фабрик = {player[i].Factory.Count - involvedFactory}\nСвободных автоматизированных фабрик = {player[i].AutomaticFactory.Count - involvedAutomaticFactory}\n");
                                Console.WriteLine("Желаете переработать ещё ЕСМ?\n1- Да\n2 - Нет");
                                temp = Console.ReadLine();
                                Console.Clear();
                                if (Convert.ToInt32(temp) == 2)
                                {
                                    return;
                                }
                                while (!Int32.TryParse(temp, out ansver))
                                {
                                    Console.Clear();
                                    Console.WriteLine($"У Вас осталось :\nЕдениц ЕСМ = {player[i].EGP}\nСвободных простых фабрик = {player[i].Factory.Count - involvedFactory}\nСвободных автоматизированных фабрик = {player[i].AutomaticFactory.Count - involvedAutomaticFactory}\n");
                                    Console.WriteLine("Желаете переработать ещё ЕСМ?\n1- Да\n2 - Нет");
                                    Console.WriteLine("\nНужно ввести целое положительное число");
                                    temp = Console.ReadLine();
                                }
                                if (ansver > 0 && ansver < 3)
                                {
                                    break;
                                }
                                else { Console.Clear(); Console.WriteLine("Не верный выбор"); }
                            }
                        }
                        else { brecker = true; }
                    }
                }
                Thread.Sleep(3000);
                Console.Clear();
            }
        }
        public void SaleOfProducts(Player[] player)    //  Продажа продукции
        {
            string? temp;
            int requestEGP = 0;
            int requestPrice = 0;
            for (int i = 0; i < player.Length; i++)
            {
                Console.WriteLine($"Ход игрка {i + 1}\n");
                while (true)
                {
                    Console.WriteLine("Сколько EGP Вы хотите продать? Для выхода введите 0");
                    temp = Console.ReadLine();
                    while (!Int32.TryParse(temp, out requestEGP))
                    {
                        Console.WriteLine("Сколько EGP Вы хотите продать? Для выхода введите 0");
                        Console.WriteLine("Нужно ввести целое положительное число");
                        temp = Console.ReadLine();
                    }
                    if (requestEGP == 0)
                    {
                        break;
                    }
                    if (requestEGP > player[i].EGP)
                    {
                        Console.WriteLine("У Вас не достаточно EGP");
                    }
                    else
                    {
                        player[i].RequestEGP = requestEGP;
                        break;
                    }
                }
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Максимальная цена предлагаемая банком за еденицу EGP на данном уровне = {bank.PriceEGP(bank.level)}");
                    Console.WriteLine($"На данном уровне банк может купить {bank.AmountEGP(bank.level)} EGP ");
                    Console.WriteLine("\nПредложите Вашу цену");
                    temp = Console.ReadLine();
                    while (!Int32.TryParse(temp, out requestPrice))
                    {
                        Console.Clear();
                        Console.WriteLine($"Максимальная цена предлагаемая банком за еденицу EGP на данном уровне = {bank.PriceEGP(bank.level)}");
                        Console.WriteLine($"На данном уровне банк может купить {bank.AmountEGP(bank.level)} EGP ");
                        Console.WriteLine("\nНужно ввести целое положительное число");
                        temp = Console.ReadLine();
                    }
                    Console.Clear();
                    player[i].PriceEGP = requestPrice;
                    break;
                }
                Thread.Sleep(3000);
                Console.Clear();
            }
        }
        public void DistributionOfApplicationsByTheBankEGP(Player[] players)            // Распределение заявок на EGP банком
        {
            bool brecker = false;
            Dictionary<int, double> requestEGP = new Dictionary<int, double>();
            Dictionary<int, int> temp = new Dictionary<int, int>();
            for (int i = 0; i < players.Length; i++)
            {
                requestEGP.Add(i, players[i].RequestEGP);
                temp.Add(i, players[i].PriceEGP);
            }
            //foreach (var item in requestEGP)
            //{
            //    Console.WriteLine($"Игрок {item.Key} заявка на {item.Value}");
            //}
            var sortedDict = from entry in temp orderby entry.Value ascending select entry;
            //foreach (var item in sortedDict)
            //{
            //    Console.WriteLine($"{item.Key}-----{item.Value}");
            //}
            int older = 0;
            foreach (var item in sortedDict)
            {
                if (players[item.Key].older)
                {
                    older = item.Key;
                }
            }
            List<int> received = new List<int>();
            int player = 0;
            int count = (int)(bank.AmountEGP(bank.level));
            while (count != 0 || brecker == true)
            {
                for (int i = 0; i < sortedDict.Count(); i++)
                {
                    if (sortedDict.ElementAt(i).Key == older)
                    {
                        if ((count - (int)players[i].RequestEGP) < 0)
                        {
                            int a = (int)players[i].RequestEGP;
                            while (count != a)
                            {
                                a--;
                            }
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                players[i].EGP -= a;
                                Console.WriteLine($"Игрок {player + 1} продает {a}");
                                count -= a;
                                received.Add(i);
                            }
                        }
                        else
                        {
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                Console.WriteLine($"Игрок {player + 1} продает {requestEGP.ElementAt(i).Value}");
                                players[i].EGP -= requestEGP.ElementAt(i).Value;
                                players[i].Balance += players[i].PriceEGP * (int)players[i].RequestECM;
                                count -= (int)players[i].RequestEGP;
                                older = -1;
                                received.Add(i);
                            }
                        }
                    }
                    else
                    {
                        for (int j = i + 1; j < sortedDict.Count(); j++)
                        {
                            if (sortedDict.ElementAt(i).Value == sortedDict.ElementAt(j).Value)
                            {
                                if (sortedDict.ElementAt(j).Key == older)
                                {
                                    if ((count - (int)players[i].RequestEGP) < 0)
                                    {
                                        int a = (int)players[i].RequestEGP;
                                        while (count != a)
                                        {
                                            a--;
                                        }
                                        if (received.Contains(i))
                                        {
                                        }
                                        else
                                        {
                                            player = sortedDict.ElementAt(i).Key;
                                            players[i].EGP -= a;
                                            Console.WriteLine($"Игрок {player + 1} продает {a}");
                                            count -= a;
                                            received.Add(i);
                                        }
                                    }
                                    else
                                    {
                                        if (received.Contains(i))
                                        {
                                        }
                                        else
                                        {
                                            player = sortedDict.ElementAt(i).Key;
                                            Console.WriteLine($"Игрок {player + 1} продает {requestEGP.ElementAt(j).Value}");
                                            players[j].EGP -= requestEGP.ElementAt(j).Value;
                                            players[j].Balance += players[j].PriceEGP * (int)players[j].RequestEGP;
                                            count -= (int)players[i].RequestEGP;
                                            older = -1;
                                            received.Add(i);
                                        }
                                    }
                                }
                            }
                        }
                        if ((count - (int)players[i].RequestEGP) < 0)
                        {
                            int a = (int)players[i].RequestEGP;
                            while (count != a)
                            {
                                a--;
                            }
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                players[i].EGP -= a;
                                Console.WriteLine($"Игрок {i + 1} продает {a}");
                                count -= a;
                                received.Add(i);
                            }
                        }
                        else
                        {
                            if (received.Contains(i))
                            {
                            }
                            else
                            {
                                player = sortedDict.ElementAt(i).Key;
                                Console.WriteLine($"Игрок {player + 1} продает {requestEGP.ElementAt(i).Value}");
                                players[i].EGP -= requestEGP.ElementAt(i).Value;
                                players[i].Balance += players[i].PriceEGP * (int)players[i].RequestEGP;
                                count -= (int)players[i].RequestEGP;
                                received.Add(i);
                            }
                        }
                    }
                    if ((i + 1) > sortedDict.Count())
                    {
                        brecker = true;
                    }
                }
            }
        }
        public void InterestPayment(Player[] player)    //   	Выплата ссудного процента
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Loan != 0)
                {
                    int deduction = Loan / 100;
                    player[i].Balance -= deduction;
                }
                else
                {

                }
            }
            
        }
        public void LoanRepayment(Player[] player)               // Погашение ссуд
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Loan>0)
                {
                    if (player[i].LoanMonth + 12 == Bank.month)
                    {
                        player[i].Balance -= player[i].Loan;
                        player[i].Loan = 0;
                    }
                    if (player[i].Balance <= 0)
                    {
                        Console.WriteLine($"Игрок {i + 1} банкрот");
                        Thread.Sleep(3000);
                        Console.Clear();
                    }
                }
            }
        }
        public void GettingLoans(Player[] player)                // Получение ссуд
        {
            for (int i = 0; i < player.Length; i++)
            {
                Console.WriteLine($"Ход игрока {player[i]}\n");
                if (player[i].Loan != 0)
                {
                    Console.WriteLine("У Вас имеется непогашенная ссуда. Для взятия новой необходимо погасить предыдущую");
                }
                else
                {
                    string? ansver;
                    int playerLoan = 0;
                    double fund = 0;
                    fund = (player[i].Factory.Count * 5000) + (player[i].AutomaticFactory.Count * 10000) + (player[i].ECM * bank.PriceECM(bank.level)) + (player[i].EGP * bank.PriceEGP(bank.level)) + player[i].Balance;   // капитал игрока
                    Console.WriteLine("\tСсуды обеспечиваются имеющимися у игрока фабриками;\n\tПод обычную фабрику дается ссуда 5000 долл., под автоматизированную — 10000 долл.\n\tОбщая сумма непогашенных ссуд не может превышать половины гарантированного капитала.\n\tСрок погашения ссуды истекает через 12 месяцев. Нельзя погашать ссуды раньше срока.");
                    Console.WriteLine($"\n\nВаш капитал составляет {fund}\nСколько хотите взять? Для отмены нажмите 0");
                    ansver = Console.ReadLine();
                    while (!Int32.TryParse(ansver, out playerLoan) || Convert.ToInt32(ansver) > fund)
                    {
                        Console.WriteLine($"Нужно ввести целое положительное число не превышающее вашего капитала ({fund})");
                        ansver = Console.ReadLine();
                    }
                    if (playerLoan != 0)
                    {
                        player[i].Loan = playerLoan;
                        player[i].LoanMonth = Bank.month;
                    }
                    else
                    {
                        Console.WriteLine("Отмена");
                    }
                }
                Thread.Sleep(3000);
                Console.Clear();
            }
        }
        public void BuildingApplications(Player[] player)                  // заявки на строительство фабрик
        {
            string? temp;
            int factory = 0;
            for (int i = 0; i < player.Length; i++)
            {
                while (true)
                {
                   Console.WriteLine("\t\tОбычная фабрика стоит 5000 долл. и начинает давать продукцию на 5-й месяц после начала строительства");
                   Console.WriteLine("\t\tАвтоматизированная фабрика стоит 10 000 долл и дает продукцию на 7-й месяц после начала строительства");
                   Console.WriteLine("\t\tОбычную фабрику можно автоматизировать за 7000 долл., реконструкция продолжается 9 месяцев, все это время фабрика может работать как обычная");
                   Console.WriteLine($"Ход игрока {i+1}\n");
                   Console.WriteLine("Если хотите построить обычную фабрику нажмите 1");
                   Console.WriteLine("Если хотите построить автоматизированную фабрику нажмите 2");
                   Console.WriteLine("Если хотите улучшить существующую обычную фабрику нажмите 3");
                   Console.WriteLine("Если хотите выйти нажмите 0");
                   temp = Console.ReadLine();
                    Console.Clear();
                    while (!Int32.TryParse(temp, out factory))
                    {
                        Console.WriteLine("Если хотите построить обычную фабрику нажмите 1");
                        Console.WriteLine("Если хотите построить автоматизированную фабрику нажмите 2");
                        Console.WriteLine("Если хотите улучшить существующую обычную фабрику нажмите 3");
                        Console.WriteLine("Если хотите выйти нажмите 0");
                        temp = Console.ReadLine();
                    }

                    if (factory == 0)
                    {
                        break;
                    }
                        if (factory>3&& factory<0)
                        {
                            Console.WriteLine("Не верный выбор");
                        }
                    else
                    {
                        if (factory == 1 && player[i].Balance>=5000)
                        {
                            if (player[i].Factory.Count+1>6)
                            {
                                Console.WriteLine(". Общее число имеющихся и строящихся фабрик у каждого игрока не должно превышать шести");
                                break;
                            }
                            else
                            {
                                Factory factory1 = new Factory();
                                player[i].Factory.Add(factory1);
                                break;
                            }
                           
                        }
                        else { Console.WriteLine("Не достаточно средств"); }
                        if (factory == 2 && player[i].Balance >= 10000)
                        {
                            if (player[i].AutomaticFactory.Count+1>6)
                            {
                                Console.WriteLine(". Общее число имеющихся и строящихся фабрик у каждого игрока не должно превышать шести");
                                break;
                            }
                            else
                            {
                                AutomaticFactory factory1 = new AutomaticFactory();
                                player[i].AutomaticFactory.Add(factory1);
                                break;
                            }
                            
                        }
                        else { Console.WriteLine("Не достаточно средств"); }
                        if (factory == 3 && player[i].Balance >= 7000)
                        {
                            player[i].Factory.RemoveAt(Factory.Count - 1);
                            AutomaticFactory factory1 = new AutomaticFactory();
                            player[i].AutomaticFactory.Add(factory1);
                            break;
                        }
                        else { Console.WriteLine("Не достаточно средств"); }
                    }
                }
            }

        }
    }
}
