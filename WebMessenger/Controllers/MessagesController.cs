using ClassLibrary;
using DataMemory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private MessageMemory messageMemory = new MessageMemory();
        private static readonly Random random = new Random();
        private char[] glas = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        private char[] soglas = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };



        #region Messages Post


        /// <summary>
        /// Случайная генерация сообщений.
        /// </summary>
        /// <response code="200">Сообщения успешно сгенерированы.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Список пользователей пуст.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("generateMessages")]
        public ActionResult GenerateMessages()
        {
            List<Message> messages = new List<Message>();
            List<User> users = new UserMemory().GetAllUsers();
            if (users == null || users.Count == 0)
                return NotFound(new { error = "Users list is empty" });

            int count = random.Next(5, 16);
            for (int i = 0; i < count; i++)
                messages.Add(new Message(users[random.Next(users.Count)].Id, users[random.Next(users.Count)].Id, GenerateString(true, 5, 15), GenerateMessage(10, 51)));

            try
            {
                messageMemory.Save(messages);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return BadRequest("Unexpected exception");
            }

            return Ok($"Successfully generated {count} messages");
        }



        /// <summary>
        /// Отправка сообщения.
        /// </summary>
        /// <response code="200">Сообщение успешно отправлено.</response>
        /// <response code="400">Параметры Id для отправителя и получателя заданы неверно.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("sendMessage")]
        public ActionResult SendMessage(Message message)
        {
            try
            {
                messageMemory.AddMessage(message);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }

            return Ok("Message successfully sent");
        }


        #endregion



        #region Messages Get


        /// <summary>
        /// Получение всех сообщений.
        /// </summary>
        /// <response code="200">Сообщения успешно получены.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Список сообщений пуст.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getAllMessages")]
        public ActionResult<List<Message>> GetAllMessages()
        {
            try
            {
                List<Message> messages = messageMemory.GetMessages();
                if (messages == null || messages.Count == 0)
                    return NotFound(new { error = "Messages list is empty" });
                return messages;
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
        /// Получение сообщений по ID отправителя и получателя.
        /// </summary>
        /// <response code="200">Сообщения успешно получены.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Сообщений с такими ID отправителя и получателя не найдены.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getMessagesByTwoIds")]
        public ActionResult<List<Message>> GetMessagesIds(int? ids, int? idr)
        {
            if (ids == null || idr == null)
                return BadRequest();

            try
            {
                List<Message> messages = messageMemory.GetMessagesByIds(ids.Value, idr.Value);
                if (messages == null || messages.Count == 0)
                    return NotFound(new { error = "Messages with those reciever and sender ids does not exists" });
                return messages;
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { error = "Users with those ids does not exist." });
            }
            catch (Exception)
            {
                return BadRequest("Unexpected exception");
            }
        }




        /// <summary>
        /// Получение сообщений по ID получателя.
        /// </summary>
        /// <response code="200">Сообщения успешно получены.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Сообщений с таким ID получателя не найдены.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getMessagesByRecieverId")]
        public ActionResult<List<Message>> GetMessagesIdr(int? idr)
        {
            if (idr == null)
                return BadRequest();

            try
            {
                List<Message> messages = messageMemory.GetMessagesByIdRes(idr.Value);
                if (messages == null || messages.Count == 0)
                    return NotFound();
                return messages;
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
        /// Получение сообщений по ID отправителя.
        /// </summary>
        /// <response code="200">Сообщения успешно получены.</response>
        /// <response code="400">Неизвестная ошибка.</response>
        /// <response code="404">Сообщений с таким ID отправителя не найдены.</response>
        /// <response code="500">Ошибка при работе с файлом.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getMessagesBySellerId")]
        public ActionResult<List<Message>> GetMessagesIds(int? ids)
        {
            if (ids == null)
                return BadRequest();

            try
            {
                List<Message> messages = messageMemory.GetMessagesByIdSel(ids.Value);
                if (messages == null || messages.Count == 0)
                    return NotFound();
                return messages;
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


        #endregion



        #region Support


        /// <summary>
        /// Создание случайного Сообщения.
        /// </summary>
        private string GenerateMessage(int min, int max)
        {
            string res = GenerateString(true, 4, 8);
            int count = random.Next(min, max);
            for (int i = 0; i < count; i++)
                res += " " + GenerateString(false, random.Next(3, 6), random.Next(7, 11));
            return res;
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
