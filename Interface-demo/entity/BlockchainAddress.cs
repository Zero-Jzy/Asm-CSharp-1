namespace Interface_demo.entity
{
    public class BlockchainAddress
    {
        
        public string Address{ get; set; }
        public string PrivateKey{ get; set; }
        public decimal Balance{ get; set; }
        public string CreatedAtMls{ get; set; }
        public string UpdatedAtMls{ get; set; }


        public BlockchainAddress()
        {
        }

        public BlockchainAddress(string address, string privateKey, decimal balance)
        {
            Address = address;
            PrivateKey = privateKey;
            Balance = balance;
        }
    }
}