using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Message
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; }

        public Message(string text, ConsoleColor color = default)
        {
            Text = text;
            Color = color;
        }
    }
}
