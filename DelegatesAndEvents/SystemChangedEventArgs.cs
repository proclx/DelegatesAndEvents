using System;

namespace DelegatesAndEvents
{
    public class SystemChangedEventArgs
        : EventArgs
    {
        public string Message { get; }
        public ConsoleColor Color { get; }
        public SystemChangedEventArgs(string message, ConsoleColor color)
        {
            Message = message;
            Color = color;
        }
    }
    public class WarningMessage : SystemChangedEventArgs
    {
        public int Value { get; } // можливо знадобиться
        public WarningMessage(int value, string message) 
            : base(message, ConsoleColor.Yellow)
        {
            Value = value;
        }
    }
    public class CriticalMessage
        : SystemChangedEventArgs
    {
        public CriticalMessage(string message) 
            : base(message, ConsoleColor.Red)
        {
        }
    }
}
