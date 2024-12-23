namespace lb8
{
    using System;
    using System.Threading;

    class Program
    {
        static void Main()
        {
            var account = new Account();
            const decimal requiredAmount = 100m;
            var depositThread = new Thread(() =>
            {
                var random = new Random();
                for (int i = 0; i < 3; i++)
                {
                    {
                        account.Deposit(random.Next(10, 60));
                        Thread.Sleep(1000);
                    }
                }
            });

            depositThread.Start();

            account.WaitForBalance(requiredAmount);

            if (account.TryWithdraw(requiredAmount)) 
            {
                Console.WriteLine($"Остаток на счете: " + account.balance);
            }
        }
    }

    class Account
    {
        public decimal balance; 
        private readonly object balanceLock = new(); 

        public Account()
        {
            this.balance = 0;
        }

        public void Deposit(decimal amount)
        {
            lock (balanceLock)
            {
                balance += amount;
                Console.WriteLine($"Пополнение: {amount:C}. Баланс: {balance:C}");
            }
        }

        public void WaitForBalance(decimal requiredAmount)
        {
            while (true)
            {
                lock (balanceLock)
                {
                    if (balance >= requiredAmount)
                    {
                        Console.WriteLine("Достаточно средств для снятия.");
                        return;
                    }
                }
                Thread.Sleep(500);
            }
        }

        public bool TryWithdraw(decimal amount)
        {
            lock (balanceLock)
            {
                if (balance >= amount)
                {
                    balance -= amount;
                    Console.WriteLine($"Снятие: {amount:C}. Баланс: {balance:C}");
                    return true;
                }
                Console.WriteLine("Недостаточно средств.");
                return false;
            }
        }
    }
}

