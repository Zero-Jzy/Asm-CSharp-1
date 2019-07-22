namespace Interface_demo.entity
{
    public class BlockchainTransaction
    {
        public string Id { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public decimal Amount { get; set; }
        public long CreatedAtMls { get; set; }
        public long UpdatedAtMls { get; set; }
        public TransactionType Type { get; set; }
        public int Status { get; set; }
        
        public enum TransactionType
        {
            DEPOSIT = 1,
            WITHDRAW = 2,
            TRANSFER = 3
        }

        public BlockchainTransaction()
        {
            
        }

        public BlockchainTransaction(string id, string senderAddress, string receiverAddress, decimal amount, TransactionType type)
        {
            Id = id;
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            Amount = amount;
            Type = type;
        }
    }
}