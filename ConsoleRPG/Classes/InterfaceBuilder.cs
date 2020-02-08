using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class InterfaceBuilder
    {
        private Interface Interface { get;}

        public InterfaceBuilder()
        {
            Interface = new Interface();
        }

        public Interface BuildInterface()
        {
            return Interface;
        }

        public InterfaceBuilder AddPart(InterfacePartType interfacePart)
        {
            Interface.Parts.Add(interfacePart);
            return this;
        }
    }
}
