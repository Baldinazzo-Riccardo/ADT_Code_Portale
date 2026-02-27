using System;
using System.Collections.Generic;
using MyQueueLibrary;
using MyStackLibrary;
using System.Text;

namespace ADT_Code_Portale
{
    class Robot
    {
        public int Id { get; set; }
        SemaphoreSlim s_TakePieces;
        public MyQueue<Component> Queue { get; set; }

        SemaphoreSlim mutexQueue;

        CancellationToken Ct;
        public Robot(int id, SemaphoreSlim s, CancellationToken ct, MyQueue<Component> queue, SemaphoreSlim mutex)
        {
            Id = id;
            s_TakePieces = s;
            Queue = queue;
            mutexQueue = mutex;
            Ct = ct;
        }

        public async Task TakePiecesAsync()
        {
            while(!Ct.IsCancellationRequested)
            {
                await s_TakePieces.WaitAsync();
                
                Component c = new Component($"COMPONENTE -N{Id}-");

                Console.WriteLine($"{this.Id} ha preso il pezzo {c.Name}");
                Console.WriteLine($"{this.Id} sta spostando il pezzo . . . ");
                await Task.Delay(200);

                await mutexQueue.WaitAsync();
                Queue.Enqueue(c);
                mutexQueue.Release();

                Console.WriteLine($"{this.Id} ha posizionato il pezzo {c.Name} nella coda");

                s_TakePieces.Release();
            }
        }
    }
}
