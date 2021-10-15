using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ParallerlLoop
{
    class Ex1
    {
        public static ConcurrentDictionary<int, BankAccount> cDict = new ConcurrentDictionary<int, BankAccount>();
        public static void Main(string[] arg)
        {
            int TotalIncrease = 0;
            int TotalBalance = 0;

            for (int i = 0; i < 10; i++)
            {
                var bankAcount = new BankAccount(100);
                cDict.TryAdd(i, bankAcount);
                TotalIncrease++;
            }  

            Random rnd = new Random();
            var po = new ParallelOptions();
            //po.MaxDegreeOfParallelism = 4;

            Parallel.ForEach(cDict, po, x =>
            {
                x.Value.Deposit(25);
                x.Value.Balance /= rnd.Next(1, 10);
                TotalBalance += x.Value.Balance;

                Console.WriteLine($"[{Thread.GetCurrentProcessorId()}] Account: {x.Key}, Balance: {x.Value.Balance}");
            }
            );
            Console.WriteLine($"Total increase: {TotalIncrease}, total balance of all accounts: {TotalBalance}");
        }
    }
    class BankAccount
    {
        object padlock = new object();
        public int Balance { get; set; }

        public BankAccount(int balance)
        {
            Balance = balance;
        }

        public void Deposit(int amount)
        {
            lock (padlock)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (padlock)
            {
                Balance -= amount;
            }
        }
    }
}
