using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Message
    {
        public string Text { get; set; }
        public MessageType Type { get; set; }

        public Message(string text, MessageType type)
        {
            Text = text;
            Type = type;
        }
    }
}
