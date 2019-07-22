using System;
using Interface_demo.entity;
using MySql.Data.MySqlClient;

namespace Interface_demo.model
{
    public class BCAddressModel
    {
        
        public bool CheckHasAddress(string address)
        {
            const string sql = "Select address from BcAddress WHERE address = @address";
            var command = new MySqlCommand(sql, ConnectionHelper.GetConnection());

            command.Parameters.AddWithValue("@address", address);
            var reader = command.ExecuteReader();
            var re = reader.Read();
            
            reader.Close();
            ConnectionHelper.CloseConnection();
            
            return re;
        }
        public BlockchainAddress FindByAddressAndPrivateKey(string address, string privateKey)
        {
            const string sql = "Select * from BcAddress WHERE address = @address and private_key = @private_key";
            var command = new MySqlCommand(sql, ConnectionHelper.GetConnection());

            command.Parameters.AddWithValue("@address", address);
            command.Parameters.AddWithValue("@private_key", privateKey);
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                ConnectionHelper.CloseConnection();
                return null;
            }
            
            var blockchainAddress = new BlockchainAddress()
            {
                Address = reader.GetString(0),
                PrivateKey = reader.GetString(1),
                Balance = reader.GetDecimal(2)
            };
            reader.Close();
            ConnectionHelper.CloseConnection();
            return blockchainAddress;
        }

        public bool UpdateBalance(BlockchainAddress currentBcAddress, BlockchainTransaction BcTransaction)
        {
            var transaction = ConnectionHelper.GetConnection().BeginTransaction();

            try
            {
                // 1.Check money.
                var command1 =
                    new MySqlCommand("Select balance from `BcAddress` WHERE address = @address",
                        ConnectionHelper.GetConnection());
                command1.Parameters.AddWithValue("@address", currentBcAddress.Address);
                var reader = command1.ExecuteReader();
                decimal currentBalance = 0;
                if (reader.Read())
                {
                    currentBalance = reader.GetDecimal("balance");
                }

                reader.Close();

                switch (BcTransaction.Type)
                {
                    case BlockchainTransaction.TransactionType.WITHDRAW when currentBalance < BcTransaction.Amount:
                        Console.WriteLine("ko du tien");
                        return false;
                    case BlockchainTransaction.TransactionType.WITHDRAW:
                        currentBalance -= BcTransaction.Amount;
                        break;
                    case BlockchainTransaction.TransactionType.DEPOSIT:
                        currentBalance += BcTransaction.Amount;
                        break;
                }


                //            2.Update Balance.
                var updateAccCmd = new MySqlCommand(
                    "update BcAddress set balance = @balance WHERE address = @address",
                    ConnectionHelper.GetConnection());
                updateAccCmd.Parameters.AddWithValue("@address", currentBcAddress.Address);
                updateAccCmd.Parameters.AddWithValue("@balance", currentBalance);
                updateAccCmd.ExecuteNonQuery();
                

                //           3. Insert transaction history.
                var insertTransCmd = new MySqlCommand(
                    "insert into `BcTransaction` (id,sender_address,receiver_address,amount, type) " +
                    "values (@id, @sender_address, @receiver_address, @amount, @type)",
                    ConnectionHelper.GetConnection());
                insertTransCmd.Parameters.AddWithValue("@id", BcTransaction.Id);
                insertTransCmd.Parameters.AddWithValue("@amount", BcTransaction.Amount);
                insertTransCmd.Parameters.AddWithValue("@type", BcTransaction.Type);
                insertTransCmd.Parameters.AddWithValue("@sender_address", BcTransaction.SenderAddress);
                insertTransCmd.Parameters.AddWithValue("@receiver_address", BcTransaction.ReceiverAddress);
                insertTransCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return false;
            }
            finally
            {
                ConnectionHelper.CloseConnection();
            }
        }

        public bool Transfer(BlockchainAddress currentBcAddress, BlockchainTransaction bcTransaction)
        {
            var mySqlTransaction = ConnectionHelper.GetConnection().BeginTransaction();
            
            try
            {
                // Kiểm tra số dư tài khoản.
                var selectBalance =
                    "select balance from `BcAddress` where address = @address";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@address", currentBcAddress.Address);
                var reader = cmdSelect.ExecuteReader();
                decimal currentAccountBalance = 0;
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDecimal("balance");
                }

                reader.Close(); // important.
                if (currentAccountBalance < bcTransaction.Amount)
                {
                    throw new Exception("Not enough money.");
                }

                currentAccountBalance -= bcTransaction.Amount;

                // Update tài khoản.
                var updateQuery =
                    "update `BcAddress` set balance = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentBcAddress.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();


                // Kiểm tra số dư tài khoản.
                var selectBalanceReceiver =
                    "select balance from `BcAddress` where address = @address";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@address", bcTransaction.ReceiverAddress);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                decimal receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDecimal("balance");
                }

                readerReceiver.Close(); // important.                
                receiverBalance += bcTransaction.Amount;

                // Update tài khoản.
                var updateQueryReceiver =
                    "update `BcAddress` set `balance` = @balance where address = @address";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@address", bcTransaction.ReceiverAddress);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                // Lưu lịch sử giao dịch.
                var insertTransCmd = new MySqlCommand(
                    "insert into `BcTransaction` (id,sender_address,receiver_address,amount, type) " +
                    "values (@id, @sender_address, @receiver_address, @amount, @type)",
                    ConnectionHelper.GetConnection());
                insertTransCmd.Parameters.AddWithValue("@id", bcTransaction.Id);
                insertTransCmd.Parameters.AddWithValue("@amount", bcTransaction.Amount);
                insertTransCmd.Parameters.AddWithValue("@type", bcTransaction.Type);
                insertTransCmd.Parameters.AddWithValue("@sender_address", bcTransaction.SenderAddress);
                insertTransCmd.Parameters.AddWithValue("@receiver_address", bcTransaction.ReceiverAddress);
                var historyResult = insertTransCmd.ExecuteNonQuery();
                
                if (updateResult != 1 || historyResult != 1 || updateResultReceiver != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                mySqlTransaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mySqlTransaction.Rollback();
                return false;
            }
            finally
            {                
                ConnectionHelper.CloseConnection();
            }
        }
    }
}