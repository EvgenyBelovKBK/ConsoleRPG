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
        public Action<string> ShowMessageAction { get; set; }
        public Action ClearTextAction { get; set; }
        public Func<string> ReadInputAction { get; set; }

        public ConsoleMessageService(Action<string> showMessageAction, Action clearTextAction, Func<string> readInputAction)
        {
            ShowMessageAction = showMessageAction;
            ClearTextAction = clearTextAction;
            ReadInputAction = readInputAction;
        }

        public void ShowMessage(Message message)
        {
            Console.ForegroundColor = message.Color;
            ShowMessageAction(message.Text);
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
