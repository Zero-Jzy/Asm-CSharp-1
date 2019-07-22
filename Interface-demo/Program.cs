using System;
using Interface_demo.entity;
using Interface_demo.model;

namespace Interface_demo
{
    class Program
    {
        public static SHBAccount CurrentAccount { get; set; }
        public static BlockchainAddress CurrentBcAddress { get; set; }
        private static GiaoDich _giaoDich = null;
        
        static void Main(string[] args)
        {
            while (true)
            {

                if (CurrentAccount == null && CurrentBcAddress == null)
                {
                    Console.Clear();
                    Console.WriteLine("Select the type of transaction.");
                    Console.WriteLine("1. Banking transaction.");
                    Console.WriteLine("2. Blockchain transaction.");
                    Console.WriteLine("0. Exit.");
                    Console.WriteLine("Your choice?");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            _giaoDich = new GiaoDichShb();
                            break;
                        case "2":
                            _giaoDich = new GiaoDichBlockchain();
                            break;
                        case "0":
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                    _giaoDich.Login();
                };
                Console.Clear();
                Console.WriteLine("Choose a trading method.");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw.");
                Console.WriteLine("3. Transfer.");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("Your choice?");

                switch (Console.ReadLine())
                {
                    case "1":
                        _giaoDich.Withdraw();
                        break;
                    case "2":
                        _giaoDich.Deposit();
                        break;
                    case "3":
                        _giaoDich.Transfers();
                        break;
                    case "0":
                        CurrentAccount = null;
                        CurrentBcAddress = null;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}