using System;
using System.Net.Mime;
using Interface_demo.entity;
using Interface_demo.model;

namespace Interface_demo
{
    public class GiaoDichShb: GiaoDich
    {
        private ShbAccountModel modelAccount = new ShbAccountModel();
        

        public void Login()
        {
                
            Console.WriteLine("Enter your username");
            var username = Console.ReadLine();
            Console.WriteLine("Enter your password");
            var password = Console.ReadLine();

            while ((Program.CurrentAccount = modelAccount.FindByUsernameAndPassword(username, password)) == null)
            {
                Console.Clear();
                Console.WriteLine("Your username or password is wrong! Please re-enter:");
                Console.WriteLine("Re-enter your username");
                username = Console.ReadLine();
                Console.WriteLine("Re-enter your password");
                password = Console.ReadLine();
            }
            Console.Clear();
            Console.WriteLine("Wellcome " + Program.CurrentAccount.Username);
            Console.WriteLine("Your balance: " + Program.CurrentAccount.Balance);
        }

        public void Deposit()
        {
            Console.WriteLine("Enter money:");
            var money = double.Parse(Console.ReadLine());
            
            var transaction = new SHBTransaction
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = Program.CurrentAccount.AccountNumber,
                ReceiverId = Program.CurrentAccount.AccountNumber,
                Message = "This is message.",
                Amount = money,
                Type = SHBTransaction.TransactionType.DEPOSIT
            };

            if (modelAccount.UpdateBalance(Program.CurrentAccount, transaction))
            {
                Console.Clear();
                Console.WriteLine("Deposit success!");
                Console.WriteLine("Continue with SHB-Bank?(y/n)");
                if (Console.ReadLine().ToLower().Equals("n"))
                {
                    Program.CurrentAccount = null;
                }
            }
            else
            {
                Console.WriteLine("Transaction failed. Please try again later!\nEnter to continue!");
                Console.ReadLine();
                
            }
        }

        public void Withdraw()
        {
            Console.WriteLine("Enter money:");
            var money = double.Parse(Console.ReadLine());
            if (Program.CurrentAccount.Balance <= money)
            {
                Console.WriteLine("You have enough money!");
            }
            if (money <= 0)
            {
                Console.WriteLine("Invalid money!");
            }

            var transaction = new SHBTransaction
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = Program.CurrentAccount.AccountNumber,
                ReceiverId = Program.CurrentAccount.AccountNumber,
                Message = "This is message.",
                Amount = money,
                Type = SHBTransaction.TransactionType.WITHDRAW
            };

            if (modelAccount.UpdateBalance(Program.CurrentAccount, transaction))
            {
                Console.Clear();
                Console.WriteLine("Withdraw success!");
                Console.WriteLine("Continue with SHB-Bank? (y/n)");
                if (Console.ReadLine().ToLower().Equals("n"))
                {
                    Program.CurrentAccount = null;
                }
            }
            else
            {
                Console.WriteLine("Transaction failed. Please try again later!\nEnter to continue!");
                Console.ReadLine();
                
            }
        }

        public void Transfers()
        {
            Console.WriteLine("Enter money:");
            var money = double.Parse(Console.ReadLine());
            Console.WriteLine("Enter ReceiverId:");
            var receiverId = Console.ReadLine();
            
            var transaction = new SHBTransaction
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = Program.CurrentAccount.AccountNumber,
                ReceiverId = receiverId,
                Message = "This is message.",
                Amount = money,
                Type = SHBTransaction.TransactionType.TRANSFER
            };

            if (modelAccount.Transfer(Program.CurrentAccount, transaction))
            {
                Console.WriteLine("Transfers success!");
                Console.WriteLine("Continue with SHB-Bank? (y/n)");
                if (Console.ReadLine().ToLower().Equals("n"))
                {
                    Program.CurrentAccount = null;
                }
            }
            else
            {
                Console.WriteLine("Transaction failed. Please try again later!\nEnter to continue!");
                Console.ReadLine();
                
            }
        }
    }
}