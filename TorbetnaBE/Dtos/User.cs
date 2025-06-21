using TorbetnaBE.Models;

namespace TorbetnaBE.Dtos
{
    public class User
    {
        public class SignInModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UpdateModel
        {
            public string Name { get; set; }
            public string Phone { get; set; }
        }

        public class CreateModel
        {
            public string Name { get; set; }

            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
        }
    }
}
