namespace TorbetnaBE.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool isDeleted { get; set; } = false;
        public UserStatus Status { get; set; } = UserStatus.Inactive;
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public void UpdatePassword (string paswd)
        {
            this.Password = Methods.Hash.HashPassword(paswd);
        }

        public void Update(Dtos.User.UpdateModel usr)
        {
            this.Name = usr.Name;
            this.Phone = usr.Phone;
            this.ModifiedOn = DateTime.UtcNow;
        }

        public void Create(Dtos.User.CreateModel user)
        {
            Update(new Dtos.User.UpdateModel { Phone = user.Phone, Name = user.Name });
            UpdatePassword(user.Password);
            this.Email = user.Email;
            this.CreatedOn = DateTime.UtcNow;
        }

        public void Activate()
        {
            this.Status = UserStatus.Active;
        }

        public void Delete()
        {
            this.isDeleted = true;
        }

    }

    public enum UserStatus
    {
        Active,
        Inactive,
    }

}
