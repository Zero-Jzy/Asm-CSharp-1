using System;

namespace Interface_demo.entity
{
    public class SHBTransaction
    {

        public string Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
        public TransactionType Type { get; set; }
        public long CreatedAtMls { get; set; }
        public long UpdatedAtMls { get; set; }
        
        public enum TransactionType
        {
            DEPOSIT = 1,
            WITHDRAW = 2,
            TRANSFER = 3
        }
        
        public int Status { get; set; }


        public SHBTransaction()
        {
        }

        public SHBTransaction(string id, string senderId, string receiverId, double amount, string messages, TransactionType type)
        {
            Id = id;
            SenderId = senderId;
            ReceiverId = receiverId;
            Amount = amount;
            Message = messages;
            Type = type;
        }
    }
}