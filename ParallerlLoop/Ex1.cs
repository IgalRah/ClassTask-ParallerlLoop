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
            int totalIncrease = 0;
            int newBalance = 0;
            int oldBalance = 0;

            for (int i = 0; i < 10; i++)
            {
                cDict.TryAdd(i, new BankAccount(100));
                oldBalance += 100;
            }  

            Random rnd = new Random();
            //var po = new ParallelOptions(); // Optional
            //po.MaxDegreeOfParallelism = 4; // Optional

            Parallel.ForEach(cDict, x =>
            {
                var increase = Convert.ToInt32((x.Value.Balance * 0.25) / rnd.Next(1, 10));
                x.Value.Deposit(increase);

                newBalance += x.Value.Balance;

                Console.WriteLine($"[{Thread.GetCurrentProcessorId()}] Account: {x.Key}, Balance: {x.Value.Balance}");
            }
            );
            Console.WriteLine($"Old balance: {oldBalance}$ || 24-hour period in active stock: {newBalance - oldBalance}$ || New total balance: {newBalance}$");
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
