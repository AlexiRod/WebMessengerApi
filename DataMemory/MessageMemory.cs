using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClassLibrary;
using Newtonsoft.Json;


namespace DataMemory
{
    public class MessageMemory
    {
        private readonly string path = "Data/messages.json";


        /// <summary>
        /// Сохранение сообщений.
        /// </summary>
        /// <param name="messages">Список сообщений.</param>
        public void Save(List<Message> messages)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(JsonConvert.SerializeObject(messages));
            }
        }



        /// <summary>
        /// Добавление нового сообщения.
        /// </summary>
        public void AddMessage(Message message)
        {
            UserMemory userMemory = new UserMemory();
            if (!userMemory.CheckId(message.RecieverId) || !userMemory.CheckId(message.SellerId))
                throw new ArgumentException("Error. Seller or Reciever Id-s isn't correct.");

            List<Message> mes = null;
            using (StreamReader sr = new StreamReader(path))
            {
                mes = JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine());
            }
            mes.Add(message);
            Save(mes);
        }



        /// <summary>
        /// Получение всех сообщений.
        /// </summary>
        public List<Message> GetMessages()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine());
            }
        }



        /// <summary>
        /// Поиск сообщений по Id отправителя и получателя.
        /// </summary>
        /// <param name="idSeller">Id отправителя.</param>
        /// <param name="idReciever">Id получателя</param>
        /// <returns></returns>
        public List<Message> GetMessagesByIds(int idSeller, int idReciever)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<Message> mes = JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine());
                if (!new UserMemory().CheckId(idReciever) || !new UserMemory().CheckId(idSeller))
                    throw new InvalidOperationException();
                return mes.Where(x => x.SellerId == idSeller && x.RecieverId == idReciever).ToList();
            }
        }



        /// <summary>
        /// Поиск сообщений по Id получателя.
        /// </summary>
        /// <param name="idReciever">Id получателя</param>
        /// <returns></returns>
        public List<Message> GetMessagesByIdRes(int idReciever)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<Message> mes = JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine());
                if (!new UserMemory().CheckId(idReciever))
                    throw new InvalidOperationException();
                return mes.Where(x => x.RecieverId == idReciever).ToList();
            }
        }



        /// <summary>
        /// Поиск сообщений по Id отправителя.
        /// </summary>
        /// <param name="idSeller">Id отправителя</param>
        /// <returns></returns>
        public List<Message> GetMessagesByIdSel(int idSeller)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<Message> mes = JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine());
                if (!new UserMemory().CheckId(idSeller))
                    throw new InvalidOperationException();
                return mes.Where(x => x.SellerId == idSeller).ToList();
            }
        }
    }
}
