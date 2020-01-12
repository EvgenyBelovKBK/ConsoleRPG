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

                public static void ShowConsoleBoxedInfo(Dictionary<string, string> data)
        {
            int maxValueLength = 0;
            foreach (var item in data)
            {
                if (item.Value.ToString().Length > maxValueLength)
                    maxValueLength = item.Value.ToString().Length;
            }

            var sumOfLengths = maxValueLength + 24;
            for (int i = 0; i <= sumOfLengths + 16; i++)
            {
                if (i == 0)
                    Console.Write("╔");
                else if (i < sumOfLengths)
                    Console.Write("=");
                else if (i == sumOfLengths)
                    Console.Write("╗");

            }

            Console.WriteLine();
            foreach (var item in data)
            {
                var side = "║";
                Console.Write("║ ");
                Console.Write($"{item.Key.PadRight(20)}: {item.Value}");
                for (int i = 0; i < maxValueLength - item.Value.ToString().Length; i++)
                {
                    Console.Write(" ");
                }

                Console.Write(side);
                Console.WriteLine();

            }

            for (int i = 0; i <= sumOfLengths; i++)
            {
                if (i == 0)
                    Console.Write("╚");
                else if (i < sumOfLengths)
                    Console.Write("=");
                else if (i == sumOfLengths)
                    Console.Write("╝");

            }

            Console.WriteLine();
        }
    }
}
