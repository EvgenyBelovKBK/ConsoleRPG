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
        public Action<string> ShowMessageAction { get; set; } = Console.WriteLine;
        public Action ClearTextAction { get; set; } = Console.Clear;
        public Func<string> ReadInputAction { get; set; } = Console.ReadLine;

        public void ShowMessage(Message message)
        {
            Console.ForegroundColor = message.Type == MessageType.Info
                ? ConsoleColor.Cyan
                : (message.Type == MessageType.Warning ? ConsoleColor.Yellow : ConsoleColor.Red);
            ShowMessageAction.Invoke(message.Text);
        }

        public void ShowMessage(string messageText, MessageType type)
        {
            Console.ForegroundColor = type == MessageType.Info
                ? ConsoleColor.Cyan
                : (type == MessageType.Warning ? ConsoleColor.Yellow : ConsoleColor.Red);
            ShowMessageAction.Invoke(new Message(messageText, type).Text);
        }

        public string ReadPlayerInput()
        {
            return ReadInputAction();
        }

        public void ClearTextField()
        {
            ClearTextAction.Invoke();
        }
    }
}
