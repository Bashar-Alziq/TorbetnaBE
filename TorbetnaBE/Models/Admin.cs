namespace TorbetnaBE.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
    }
}
