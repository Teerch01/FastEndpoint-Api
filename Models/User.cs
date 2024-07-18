namespace WebApi.Models;

    public class User : Time
    {
        public int Id { get; set; }
        public string FirstName {set; get;}
        public string LastName { set; get; }
        public string UserName { set; get; }
        public string Email { set; get; }
        public byte[] Password { set; get; }
        public byte[] PasswordKey { set; get; }
    }
