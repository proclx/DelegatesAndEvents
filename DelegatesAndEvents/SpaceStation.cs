using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DelegatesAndEvents
{
    public static class MessageFormatter // клас для красивого друку на консоль
    {
        public static void Print(string role, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{role}: {message}");
            Console.ResetColor();
        }
        public async static void AsyncPrint(string role, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            await Console.Out.WriteAsync($"{role}: {message}\n");
            Console.ResetColor();
        }
    }
    public class SpaceStation
    {
        public string Name { get; }
        private Crew crew; // екіпаж корабля
        public Crew Crew
        {
            get
            {
                return crew;
            }
            set
            {
                crew = value;
                MessageFormatter.Print(Name, "Новий екiпаж приєднався на станцiю", ConsoleColor.Green);
            }
        }
        public SpaceStation(Crew crew)
        {
            Name = "Космiчна станцiя";
            Crew = crew;
            MessageFormatter.Print(Name, "Вiтаємо на космiчнiй станцiї!\nВаше завдання вчасно обслуговувати станцiю, слiдкувати за роботою генератора енергiї та кисню.\nЯкщо якийсь прилад потребуватиме ремонту, ви отримаєте повiдомлення про це.\nВаше головне завдання - не допустити, щоб рiвень кисню був менше нуля =).\nПам'ятайте, що генератор кисню очищає повiтря лише коли працює генератор електроенергії.",
                ConsoleColor.Green);
        }
        public async Task OnLow(object sender, EventArgs eventArgs) // алгоритм ремонту обладнання, що близьке до поломки однаковий і для генератора і для кисню
        {
            if (eventArgs is SystemChangedEventArgs args)
            {
                MessageFormatter.AsyncPrint(Name, args.Message, args.Color);
            }
            if (sender is IRepair powerGenerator)
            {
                await crew.Repair(powerGenerator);
            }
        }
        public async Task OnZeroEnergy(object sender, EventArgs eventArgs)
        {
            if (eventArgs is SystemChangedEventArgs args)
            {
                MessageFormatter.AsyncPrint(Name, args.Message, args.Color);
            }
            MessageFormatter.Print(Name, "Всi системи корабля не працють", ConsoleColor.Red); // система кисню вимикається, про це її сповістить генератор
            if (sender is IRepair powerGenerator) // без зволікань ремонтуємо генератор електрики, це остання надія для екіпажу
            {
                await crew.Repair(powerGenerator);
            }
        }
        public void OnEnergyRepaired(object sender, EventArgs eventArgs) // спеціальний метод, який викликається коли енергія була на 0, але її відремонтували
        {
            MessageFormatter.Print(Name, "Я знову з вами", ConsoleColor.Blue);
        }
        public void OnZeroOxygen(object sender, EventArgs eventArgs)
        {
            if (eventArgs is SystemChangedEventArgs args)
            {
                MessageFormatter.Print(Name, args.Message, args.Color);
            }
            MessageFormatter.Print(Name, "Очiкую на наступний екiпаж", ConsoleColor.Green); // кінець...
            Environment.Exit(0); 
        }
        public void AfterRepair(object sender, EventArgs eventArgs) // загальний алгоритм, коли генератори сповіщають про завершення ремонту
        {
            if (eventArgs is SystemChangedEventArgs args)
            {
                MessageFormatter.Print(Name, args.Message, args.Color);
            }
        }
    }
}
