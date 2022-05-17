using System;

namespace ClassLibrary
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email {   get; set; }

        public User(int id, string username, string email)
        {
            Id = id;
            UserName = username;    
            Email = email;
        }
    }
}
