using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Interfaces
{
    public interface IMessageService
    {
        Action<string> ShowMessageAction { get; set; }
        Action ClearTextAction { get; set; }
        Func<string> ReadInputAction { get; set; }
        void ShowMessage(Message message);
        string ReadPlayerInput();
        void ClearTextField();
    }
}
