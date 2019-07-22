using System;
using Interface_demo.entity;
using Interface_demo.model;
using Org.BouncyCastle.Crypto.Digests;

namespace Interface_demo
{
    public class GiaoDichBlockchain: GiaoDich
    {
        private BCAddressModel model = new BCAddressModel();
        public void Login()
        {
            Console.WriteLine("Enter your address");
            var address = Console.ReadLine();
            Console.WriteLine("Enter your privateKey");
            var privateKey = Console.ReadLine();

            while ((Program.CurrentBcAddress = model.FindByAddressAndPrivateKey(address, privateKey)) == null)
            {
                Console.Clear();
                Console.WriteLine("Your address or privateKey is wrong! Please re-enter:");
                Console.WriteLine("Re-enter your address");
                address = Console.ReadLine();
                Console.WriteLine("Re-enter your privateKey");
                privateKey = Console.ReadLine();
            }
            Console.Clear();
            Console.WriteLine("Wellcome !");
            Console.WriteLine("Your balance: " + Program.CurrentBcAddress.Balance);
        }

        public void Deposit()
        {
            Console.WriteLine("Enter money:");
            decimal money;
            while ((money = decimal.Parse(Console.ReadLine())) < 0)
            {
                Console.WriteLine("Money Incorrect!\n Re-enter money");
            }
            
            var transaction = new BlockchainTransaction
            {
                Id = Guid.NewGuid().ToString(),
                SenderAddress = Program.CurrentBcAddress.Address,
                ReceiverAddress = Program.CurrentBcAddress.Address,
                Amount = money,
                Type = BlockchainTransaction.TransactionType.DEPOSIT
            };

            if (model.UpdateBalance(Program.CurrentBcAddress, transaction))
            {
                Console.Clear();
                Console.WriteLine("Deposit success!");
                Console.WriteLine("Continue with BC-Bank?(y/n)");
                if (Console.ReadLine().ToLower().Equals("n"))
                {
                    Program.CurrentBcAddress = null;
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
            decimal money;
            while ((money = decimal.Parse(Console.ReadLine())) < 0)
            {
                Console.WriteLine("Money Incorrect!\n Re-enter money");
            }
            
            var transaction = new BlockchainTransaction
            {
                Id = Guid.NewGuid().ToString(),
                SenderAddress = Program.CurrentBcAddress.Address,
                ReceiverAddress = Program.CurrentBcAddress.Address,
                Amount = money,
                Type = BlockchainTransaction.TransactionType.WITHDRAW
            };

            if (model.UpdateBalance(Program.CurrentBcAddress, transaction))
            {
                Console.Clear();
                Console.WriteLine("Deposit success!");
                Console.WriteLine("Continue with BC-Bank?(y/n)");
                if (Console.ReadLine().ToLower().Equals("n"))
                {
                    Program.CurrentBcAddress = null;
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
            Console.WriteLine("Enter receiver address:");
            var receiverAddress = Console.ReadLine();

            while (model.CheckHasAddress(receiverAddress))
            {
                Console.WriteLine("Address not found!");
                Console.WriteLine("Re-enter receiver address:");
                receiverAddress = Console.ReadLine();
            }
            
            Console.WriteLine("Enter money:");
            decimal money;
            while ((money = decimal.Parse(Console.ReadLine())) < 0)
            {
                Console.WriteLine("Money Incorrect!\n Re-enter money");
            }
            
            
            var transaction = new BlockchainTransaction()
            {
                Id = Guid.NewGuid().ToString(),
                SenderAddress = Program.CurrentBcAddress.Address,
                ReceiverAddress = receiverAddress,
                Amount = money,
                Type = BlockchainTransaction.TransactionType.TRANSFER
            };

            if (model.Transfer(Program.CurrentBcAddress, transaction))
            {
                Console.WriteLine("Transfers success!");
                Console.WriteLine("Continue with BC-Bank? (y/n)");
                if (Console.ReadLine().ToLower().Equals("n"))
                {
                    Program.CurrentBcAddress = null;
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