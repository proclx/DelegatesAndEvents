using System;
using System.Threading;
using System.Threading.Tasks;

namespace DelegatesAndEvents
{
    public class OxygenGenerator
        : IRepair
    {
        // асинхронність зламала цей делегат, прийшлося переробляти
        //public event EventHandler<SystemChangedEventArgs> LowOxygenLevel;

        public event EventHandler<SystemChangedEventArgs> ZeroOxygenLevel; // подія - нульовий рівень кисню, вже нічого не допоможе
        public event Func<object, EventArgs, Task> LowOxygenLevel;         // подія - низький рівень кисню, потребує ремонту 
        public string Name;
        private int oxygenLevel;
        private bool turnedOn;                                             // чи підключено до генератора
        private CancellationTokenSource cts;                               // для переривання ремонту
        public int OxygenLevel
        {
            get
            {
                return oxygenLevel;
            }
            set
            {
                if (value > 30)
                {
                    oxygenLevel = Math.Min(MaxOxygen(), value); // задумано, що кисень не може бути більше 50
                }
                else if (value <= 30 && value > 0)
                {
                    oxygenLevel = value;
                    LowOxygenLevel?.Invoke(this, new WarningMessage(value, $"Низький рiвень кисню - {value}"));
                }
                else
                {
                    oxygenLevel = 0;
                    ZeroOxygenLevel?.Invoke(this, new CriticalMessage("Кисень закiнчився...")); // кінець
                }
            }
        }
        public int MaxOxygen()
        {
            return 40;
        }
        public OxygenGenerator()
        {
            OxygenLevel = MaxOxygen();   // з самого початку рівень кисню максимальний
            turnedOn = true;     // вважаємо, що з самого початку енергія є
            Name = "Генератор кисню";
        }
        public async Task OnEnergyCutOff(object sender, EventArgs eventArgs)
        {
            turnedOn = false;
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                MessageFormatter.Print(Name, "Немає доступу до електроенергiї, кисень втрачається ще швидше.", ConsoleColor.Red);
            }
            await Task.CompletedTask;
        }
        public void OnEnergyRepaired(object sender, EventArgs eventArgs)
        {
            turnedOn = true;
            MessageFormatter.Print(Name, "Вiдновлено доступ до електроенергiї, розхiд кисню стандартний.", ConsoleColor.Green);
        }
        public async Task StartAsync(CancellationToken token)   // симуляція роботи генератора кисню, рівень кисню пасивно падає, його треба час від часу поновлювати
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(3000, token);
                if (turnedOn) // кисень втрачається вдвічі швидше, якщо немає доступу до генератора електроенергії 
                {
                    OxygenLevel -= 3;
                }
                else
                {
                    OxygenLevel -= 6; 
                }
            }
        }
        public async Task Repair()
        {
            if (!turnedOn)
            {
                MessageFormatter.AsyncPrint(Name, "Неможливо почати ремонт, бо немає енергiї", ConsoleColor.Red);
                throw new RepearingFailed();
            }
            cts = new CancellationTokenSource(); // для моєї задумки, що коли генератор ремонтується і пропадає електроенергія, щоб ремонт перервався і команда пішла ремонтувати генератор
            try
            {
                MessageFormatter.AsyncPrint(Name, "Розпочато ремонт", ConsoleColor.Green);
                await Task.Delay(8000, cts.Token);
                OxygenLevel = MaxOxygen();
                MessageFormatter.AsyncPrint(Name, "Вiдремонтовано", ConsoleColor.Green);
            }
            catch (OperationCanceledException)
            {
                MessageFormatter.AsyncPrint(Name, "Ремонт скасовано", ConsoleColor.Red); // невдача, виключився генератор
                cts = null;
                throw new RepearingFailed();
            }
            cts = null;
        }
    }
}
