using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegatesAndEvents
{
    public interface IRepair         // інтерфейс механізмів які можна ремонтувати
    {
        Task Repair();
    }
    public class RepearingFailed     // помилка, що сигналізує, про те що не вдалося полагодити
        : Exception
    {
        public RepearingFailed() { }
    }
    public class Crew
    {
        private bool alreadyWorking; // працівники можуть ремонтувати тільки один механізм
        public string Name { get; }
        public Crew()
        {
            alreadyWorking = false;
            Name = "Екiпаж";
        } 
        public async Task Repair(IRepair repair)
        {
            if (alreadyWorking)
            {
                return;
            }
            MessageFormatter.AsyncPrint(Name, "Приступаємо до ремонту", ConsoleColor.Green);
            alreadyWorking = true;
            try
            {
                await repair.Repair();
                MessageFormatter.AsyncPrint(Name, "Завершили ремонт, готовi до нових завдань", ConsoleColor.Green);
            }
            catch (RepearingFailed)
            {
                MessageFormatter.AsyncPrint(Name, "Куди пропало свiтло?", ConsoleColor.Yellow);
            }
            alreadyWorking = false;
        }
    }
}
