using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DelegatesAndEvents
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Crew crew = new Crew();
            SpaceStation station = new SpaceStation(crew);
            PowerGenerator power = new PowerGenerator();
            OxygenGenerator oxygen = new OxygenGenerator();

            power.LowEnergyLevel += station.OnLow;
            power.ZeroEnergyLevel += oxygen.OnEnergyCutOff;
            power.ZeroEnergyLevel += station.OnZeroEnergy;
            power.EnergyRepaired += station.OnEnergyRepaired;
            power.EnergyRepaired += oxygen.OnEnergyRepaired;
            oxygen.LowOxygenLevel += station.OnLow;
            oxygen.ZeroOxygenLevel += station.OnZeroOxygen;

            var cts = new CancellationTokenSource();
            await Task.WhenAll(power.StartAsync(cts.Token), oxygen.StartAsync(cts.Token));
        }
    }
}
