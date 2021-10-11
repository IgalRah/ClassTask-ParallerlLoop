using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ParallerlLoop
{
    class Ex1
    {
        public static ConcurrentDictionary<int, BankAccount> dict = new ConcurrentDictionary<int, BankAccount>();

        public static void Main(string[] args)
        {
            try
            {
                int sum = 0;

                for (int i = 1; i <= 10000; i++)
                {
                    var bankAccount = new BankAccount(100);
                    dict.TryAdd(i, bankAccount);
                }

                Random rnd = new Random();
                var po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 3;

                Parallel.ForEach(dict, x =>
                {
                    x.Value.Balance = (x.Value.Balance + 25) / rnd.Next(100);
                }
                );


                Parallel.For(0, 101,po, 
                () => 0,
                (x, TotalIncrease, TotalBalance) =>
                {
                    TotalBalance += x;
                    Console.WriteLine($"Task {Task.CurrentId} has sum {TotalBalance}");
                    return TotalBalance;
                },
                partialSum =>
                {
                    Thread.Sleep(100);
                    Console.WriteLine($"Partial value of task {Task.CurrentId} is {partialSum}");
                    Interlocked.Add(ref sum, partialSum);
                }
                );
                Console.WriteLine($"\nTotal balance: {sum}");
                Console.WriteLine($"Total increase: ");

            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    public class BankAccount
    {
        object padlock = new();

        public double Balance { get; set; }

        public BankAccount(double balance)
        {
            Balance = balance;
        }

        public void Withdraw(int amount)
        {
            lock (padlock)
            {
                Balance -= amount;
            }
        }
        public void Deposit(int amount)
        {
            lock (padlock)
            {
                Balance += amount;
            }
        }
    }
}
