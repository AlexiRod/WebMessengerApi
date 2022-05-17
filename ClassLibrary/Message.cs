using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Message
    {
        public int SellerId { get; set; }
        public int RecieverId { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }

        public Message(int sellerid, int recieverid, string subject, string messagetext) 
        {
            SellerId = sellerid;
            RecieverId = recieverid;
            Subject = subject;
            MessageText = messagetext;
        }
    }
}
