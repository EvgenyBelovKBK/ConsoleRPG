using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Interfaces
{
    public interface IMessageService
    { 
        Action<Message> ShowMessageAction { get; set; }
        Func<string> ReadInputAction { get; set; }
        void ShowMessage(Message message);
        void ShowMessage(string messageText, MessageType type);
        string ReadPlayerInput();
    }
}
