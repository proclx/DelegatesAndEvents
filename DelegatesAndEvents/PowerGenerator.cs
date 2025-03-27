using System;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;

namespace DelegatesAndEvents
{
    public class PowerGenerator
        : IRepair
    {
        public event EventHandler<SystemChangedEventArgs> EnergyRepaired;
        public event Func<object, EventArgs, Task> LowEnergyLevel;
        public event Func<object, EventArgs, Task> ZeroEnergyLevel;
        private int powerLevel;
        public string Name;
        public int PowerLevel
        {
            get
            {
                return powerLevel;
            }
            set
            {
                if (value > 30)
                {
                    powerLevel = Math.Min(value, MaxPower()); // задумано, що енергія не повинна бути більше 100 
                }
                else if (value <= 30 && value > 0)
                {
                    powerLevel = value;
                    LowEnergyLevel?.Invoke(this, new WarningMessage(value, $"Низький рiвень енергiї - {value}"));
                }
                else
                {
                    if (powerLevel == 0) // немає потреби сповіщати про 0 енергію більше одного разу
                    {
                        return;
                    }
                    powerLevel = 0;
                    ZeroEnergyLevel?.Invoke(this, new CriticalMessage("Електроенергiя не виробляється")); // навідміну від кисню це ще не кінець, також треба сповістити генератор кисню про це
                }
            }
        }
        public int MaxPower()
        {
            return 40;
        }
        public PowerGenerator()
        {
            powerLevel = MaxPower(); // на початку максимальний рівень енергії
            Name = "Генератор енергiї";
        }
        public async Task StartAsync(CancellationToken token) 
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2000, token);
                PowerLevel -= 8;
            }
        }
        public async Task Repair()
        {
            await Task.Delay(6000);
            if(PowerLevel == 0) // унікальна подія - корабель повністю виключився, але люди його полагодили
            {
                EnergyRepaired?.Invoke(this, null); // нічого не треба передавати
            }
            PowerLevel = MaxPower();
            MessageFormatter.AsyncPrint(Name, "Вiдремонтовано", ConsoleColor.Green);
        }
    }
}
