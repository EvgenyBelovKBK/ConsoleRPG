using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class LogMessageService : IMessageService
    {

        public Action<string> ShowMessageAction { get; set; }
        public Action ClearTextAction { get; set; }
        public Func<string> ReadInputAction { get; set; }

        private void WriteLogInFile(string text)
        {

        }

        public void ShowMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string messageText, MessageType type)
        {
            throw new NotImplementedException();
        }

        public string ReadPlayerInput()
        {
            throw new NotImplementedException();
        }

        public void ClearTextField()
        {
            throw new NotImplementedException();
        }
    }
}
