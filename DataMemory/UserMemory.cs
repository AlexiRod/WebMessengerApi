using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClassLibrary;
using Newtonsoft.Json;

namespace DataMemory
{
    public class UserMemory
    {
        private readonly string path = "Data/users.json";


        /// <summary>
        /// Сохранение пользователей.
        /// </summary>
        /// <param name="users">Список пользователей.</param>
        public void Save(List<User> users)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            users.Sort((x, y) => x.Email.CompareTo(y.Email));
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(JsonConvert.SerializeObject(users));
            }
        }



        /// <summary>
        /// Добавление пользователя в список.
        /// </summary>
        /// <param name="user">Новый пользователь.</param>
        public void AddUser(User user)
        {
            List<User> users = GetAllUsers();
            if (users.Any(x => x.Id == user.Id))
                throw new ArgumentException("Error. User with this Id already exists");

            users.Add(user);
            Save(users);
        }



        /// <summary>
        /// Поиск пользователя по Id.
        /// </summary>
        /// <param name="id">Id пользователя.</param>
        public User GetUserById(int id)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<User> users = JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
                return users.Single(x => x.Id == id);
            }
        }



        /// <summary>
        /// Поиск пользователя по E-mail.
        /// </summary>
        /// <param name="email">E-mail пользователя.</param>
        public User GetUserByEmail(string email)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<User> users = JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
                return users.Single(x => x.Email == email);
            }
        }



        /// <summary>
        /// Получение всех пользователей.
        /// </summary>
        public List<User> GetAllUsers()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
            }
        }



        /// <summary>
        /// Получение пользователей с параметрами Limit и Offset.
        /// </summary>
        public List<User> GetAllUsersParams(int limit, int offset)
        {
            if (limit <= 0 || offset < 0)
                throw new ArgumentException("Wrong parametrs limit and offset");

            using (StreamReader sr = new StreamReader(path))
            {
                //List<User> uers = JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
                //List<User> d = uers.Skip(offset).Take(limit).ToList();
                //return d;
                return JsonConvert.DeserializeObject<List<User>>(sr.ReadLine()).Skip(offset).Take(limit).ToList();
            }
        }


        /// <summary>
        /// Проврека на существование пользователя с указанным id.
        /// </summary>
        /// <param name="id">Id пользователя.</param>
        public bool CheckId(int id)
        {
            List<User> users = GetAllUsers();
            return users.Any(x => x.Id == id);
        }
    }
}
