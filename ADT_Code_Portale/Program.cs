using MyQueueLibrary;
using MyStackLibrary;

namespace ADT_Code_Portale
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cts = new();

            List<Component> components = new();

            MyQueue<Component> queue = new();

            List<Task> dinosaurs = new();
            List<Task> robots = new();

            int N_DINOSAURS = 5;
            int N_ROBOTS = 5;
            int N_COMPONENTS = 20;

            SemaphoreSlim s_TakePieces = new(0, N_ROBOTS);
            SemaphoreSlim mutex = new(1, 1);
            SemaphoreSlim s_Build = new(0, N_DINOSAURS);

//////////////////////////////////////////////////////////////////////////////////////////////////////////////

            for(int i = 0; i < N_COMPONENTS; i++)
            {
                Component c = new Component($"COMPONENTE -N{i}-");
                components.Add(c);
            }

            for (int i = 0; i < N_ROBOTS; i++)
            {
                Robot r = new Robot(i * System.Random.Shared.Next(1000, 9999), s_TakePieces, cts.Token, queue, mutex);
                robots.Add(r.TakePiecesAsync());
            }

            for (int i = 0; i < N_DINOSAURS; i++)
            {
                //creo dinosauro - ha come altezza impostata di default a 5
                Dinosaur d = new Dinosaur($"Dinosauro - {i}", s_Build, cts.Token, queue, mutex);
                dinosaurs.Add(d.BuildComponentAsync());
            }

            await Task.WhenAll(robots);

            while (!queue.IsEmpty())
            {
                await Task.Delay(10);
            }

            cts.Cancel();
            await Task.WhenAll(dinosaurs); //tutti finiscono di costruire

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Tutti i componenti sono stati assemblati");
            Console.ForegroundColor = ConsoleColor.White;

            cts.Cancel();
        }
    }
}
