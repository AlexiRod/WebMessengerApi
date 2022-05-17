using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using DataMemory;
using ClassLibrary;

namespace WebMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static readonly Random random = new Random();
        private UserMemory userMemory = new UserMemory();
        private char[] glas = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        private char[] soglas = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        private string[] mails = new string[] { "@mail.com", "@gmail.com", "@mail.ru", "@yandex.ru" };



        #region Users Post

        /// <summary>
        /// Случайная генерация пользователей.
        /// </summary>
        /// <response code="200">Пользователи успешно сгенерированы.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("generateUsers")]
        public ActionResult GenerateUsers()
        {
            List<User> users = new List<User>();
            int count = random.Next(5, 16);

            for (int i = 0; i < count; i++)
                users.Add(new User(users.Count + 1, GenerateString(true, 4, 10), GenerateEmail()));

            try
            {
                userMemory.Save(users);
                new MessageMemory().Save(new List<Message>());
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return BadRequest("Unexpected exception");
            }

            return Ok($"Successfully generated {count} users. Messages list is clear");
        }



        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <response code="200">Пользователь успешно зарегестрирован.</response>
        /// <response code="400">Такой пользователь уже существует.</response>
        /// <response code="404">Неизвестная ошибка.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("registrateUser")]
        public ActionResult RegistrateUsers(User user)
        {
            try
            {
                userMemory.AddUser(user);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception)
            {
                return NotFound("Unexpected exception");
            }

            return Ok("User successfully registered");
        }

        #endregion



        #region Users Get

        /// <summary>
        /// Получение всех пользователей.
        /// </summary>
        /// <response code="200">Пользователи успешно получены.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Список пользователей пуст.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getAllUsers")]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                List<User> users = userMemory.GetAllUsers();
                if (users == null || users.Count == 0)
                    return NotFound(new { error = "Users list is empty" });
                return users;
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return BadRequest("Unexpected exception");
            }
        }



        /// <summary>
        /// Получение пользователя по ID.
        /// </summary>
        /// <response code="200">Пользователь успешно получен.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Пользователь с таким ID не найден.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getUserById")]
        public ActionResult<User> GetUserById(int? id)
        {
            if (id == null)
                return BadRequest();

            try
            {
                return userMemory.GetUserById(id.Value);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { error = "User with this id does not exists." });
            }
            catch (Exception)
            {
                return BadRequest("Unexpected exception");
            }
        }




        /// <summary>
        /// Получение пользователя по email.
        /// </summary>
        /// <response code="200">Пользователь успешно получен.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Пользователь с таким email не найден.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getUserByEmail")]
        public ActionResult<User> GetUserByEmail(string email)
        {
            if (email == null)
                return BadRequest("Email is not defined");

            try
            {
                return userMemory.GetUserByEmail(email);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { error = "User with this email does not exists." });
            }
            catch (Exception)
            {
                return BadRequest("Unexpected exception");
            }


        }



        /// <summary>
        /// Получение всех пользователей с заданными параметрами Limit и Offset.
        /// </summary>
        /// <response code="200">Пользователи успешно получены.</response>
        /// <response code="400">Неверно заданные параметры.</response>
        /// <response code="404">Список пользователей пуст.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getUsersWithParams")]
        public ActionResult<List<User>> GetUsersWithParams(int limit, int offset)
        {
            try
            {
                List<User> users = userMemory.GetAllUsersParams(limit, offset);
                if (users == null || users.Count == 0)
                    return NotFound("There is no users with such params");
                return users;
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        #endregion



        #region Support



        /// <summary>
        /// Создание случайного Email-a.
        /// </summary>
        private string GenerateEmail()
        {
            return GenerateString(false, 7, 15) + random.Next(1000, 10000) + mails[random.Next(mails.Length)];
        }


        /// <summary>
        /// Создание случайной строки.
        /// </summary>
        private string GenerateString(bool isUp, int min, int max)
        {
            string res = isUp ? Convert.ToString((char)random.Next('A', 'Z' + 1)) : "";
            int count = random.Next(min, max);
            for (int i = 0; i < count; i++)
            {
                int val = random.Next(1, 5);
                if (val < 3)
                    res += glas[random.Next(6)];
                else
                    res += soglas[random.Next(20)];
            }
            return res;
        }

        #endregion
    }
}
