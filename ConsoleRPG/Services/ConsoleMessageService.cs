using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class ConsoleMessageService : IMessageService
    {
        public Action<Message> ShowMessageAction { get; set; } = Console.WriteLine;
        public Func<string> ReadInputAction { get; set; } = Console.ReadLine;

        public void ShowMessage(Message message)
        {
            Console.ForegroundColor = message.Type == MessageType.Info
                ? ConsoleColor.Cyan
                : (message.Type == MessageType.Warning ? ConsoleColor.Yellow : ConsoleColor.Red);
            ShowMessageAction.Invoke(message);
        }

        public void ShowMessage(string messageText, MessageType type)
        {
            Console.ForegroundColor = type == MessageType.Info
                ? ConsoleColor.Cyan
                : (type == MessageType.Warning ? ConsoleColor.Yellow : ConsoleColor.Red);
            ShowMessageAction.Invoke(new Message(messageText, type));
        }

        public string ReadPlayerInput()
        {
            return ReadInputAction();
        }
    }
}
