using System;
using System.Data.Common;
using System.Transactions;
using Interface_demo.entity;
using MySql.Data.MySqlClient;

namespace Interface_demo.model
{
    public class ShbAccountModel
    {
        public SHBAccount FindByUsernameAndPassword(string username, string password)
        {
            
            var conn = ConnectionHelper.GetConnection();
            const string sql = "Select * from ShbAccount WHERE username = @username and password = @password";
            var command = new MySqlCommand(sql, conn);

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            var account = new SHBAccount();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            account.AccountNumber = reader.GetString("account_number");
            account.Username = reader.GetString("username");
            account.Password = reader.GetString("password");
            account.Balance = reader.GetDouble("balance");
            reader.Close();
            conn.Close();
            ConnectionHelper.CloseConnection();
            return account;
        }

        public bool UpdateBalance(SHBAccount account, SHBTransaction shbTransaction)
        {
            var transaction = ConnectionHelper.GetConnection().BeginTransaction();
            
            try
            {

//            1.Check money.
                var command1 = new MySqlCommand("Select balance from `ShbAccount` WHERE account_number = @account_number", ConnectionHelper.GetConnection());
                command1.Parameters.AddWithValue("@account_number", account.AccountNumber);
                var reader = command1.ExecuteReader();
                double currentBalance = 0;
                if (reader.Read())
                {
                    currentBalance = reader.GetDouble("balance");
                }

                reader.Close();
                
                switch (shbTransaction.Type)
                {
                    case SHBTransaction.TransactionType.WITHDRAW when currentBalance < shbTransaction.Amount:
                        Console.WriteLine("ko du tien");
                        return false;
                    case SHBTransaction.TransactionType.WITHDRAW:
                        currentBalance -= shbTransaction.Amount;    
                        break;
                    case SHBTransaction.TransactionType.DEPOSIT:
                        currentBalance += shbTransaction.Amount;
                        break;
                }


//            2.Update Balance.
                var updateAccCmd = new MySqlCommand(
                    "update ShbAccount set balance = @balance WHERE account_number = @account_number", ConnectionHelper.GetConnection());
                updateAccCmd.Parameters.AddWithValue("@account_number", account.AccountNumber);
                updateAccCmd.Parameters.AddWithValue("@balance", currentBalance);
               updateAccCmd.ExecuteNonQuery();

                //            2.Update Balance.

//           3. Insert transaction history.
                var insertTransCmd = new MySqlCommand(
                    "insert into `ShbTransaction` (id,sender_id,receiver_id,amount,Message,type) " +
                    "values (@id, @sender_id, @receiver_id, @amount, @Message, @type)", ConnectionHelper.GetConnection());
                insertTransCmd.Parameters.AddWithValue("@id", shbTransaction.Id);
                insertTransCmd.Parameters.AddWithValue("@amount", shbTransaction.Amount);
                insertTransCmd.Parameters.AddWithValue("@type", shbTransaction.Type);
                insertTransCmd.Parameters.AddWithValue("@Message", shbTransaction.Message);
                insertTransCmd.Parameters.AddWithValue("@sender_id", shbTransaction.SenderId);
                insertTransCmd.Parameters.AddWithValue("@receiver_id", shbTransaction.ReceiverId);
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
        
        public bool Transfer(SHBAccount currentLoggedInAccount, SHBTransaction shbTransaction)
        {
            var mySqlTransaction = ConnectionHelper.GetConnection().BeginTransaction();
            
            try
            {
                // Kiểm tra số dư tài khoản.
                var selectBalance =
                    "select balance from `ShbAccount` where account_number = @account_number";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@account_number", currentLoggedInAccount.AccountNumber);
                var reader = cmdSelect.ExecuteReader();
                double currentAccountBalance = 0;
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("balance");
                }

                reader.Close(); // important.
                if (currentAccountBalance < shbTransaction.Amount)
                {
                    throw new Exception("Not enough money.");
                }

                currentAccountBalance -= shbTransaction.Amount;

                // Update tài khoản.
                var updateQuery =
                    "update `ShbAccount` set balance = @balance where account_number = @account_number";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@account_number", currentLoggedInAccount.AccountNumber);
                var updateResult = sqlCmd.ExecuteNonQuery();
                Console.WriteLine(currentLoggedInAccount.AccountNumber);
                Console.WriteLine(shbTransaction.ReceiverId);


                // Kiểm tra số dư tài khoản.
                var selectBalanceReceiver =
                    "select balance from `ShbAccount` where account_number = @account_number";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@account_number", shbTransaction.ReceiverId);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                double receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close(); // important.                
                receiverBalance += shbTransaction.Amount;

                // Update tài khoản.
                var updateQueryReceiver =
                    "update `ShbAccount` set `balance` = @balance where account_number = @account_number";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@account_number", shbTransaction.ReceiverId);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                // Lưu lịch sử giao dịch.
                var insertTransCmd = new MySqlCommand(
                    "insert into `ShbTransaction` (id,sender_id,receiver_id,amount,Message,type) " +
                    "values (@id, @sender_id, @receiver_id, @amount, @Message, @type)", ConnectionHelper.GetConnection());
                insertTransCmd.Parameters.AddWithValue("@id", shbTransaction.Id);
                insertTransCmd.Parameters.AddWithValue("@amount", shbTransaction.Amount);
                insertTransCmd.Parameters.AddWithValue("@type", shbTransaction.Type);
                insertTransCmd.Parameters.AddWithValue("@Message", shbTransaction.Message);
                insertTransCmd.Parameters.AddWithValue("@sender_id", shbTransaction.SenderId);
                insertTransCmd.Parameters.AddWithValue("@receiver_id", shbTransaction.ReceiverId);
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