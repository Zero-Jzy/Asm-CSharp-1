namespace Interface_demo.entity
{
    public class SHBAccount
    {
        public string AccountNumber{ get; set; }
        public string Username{ get; set; }
        public string Password{ get; set; }
        public double Balance{ get; set; }
        public long CreatedAtMls{ get; set; }
        public long UpdatedAtMls{ get; set; }

        public SHBAccount()
        {
        }
    }
}