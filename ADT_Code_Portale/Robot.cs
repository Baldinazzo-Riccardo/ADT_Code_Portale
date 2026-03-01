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
        SemaphoreSlim s_Build;
        public MyQueue<Component> Queue { get; set; }

        SemaphoreSlim mutexQueue;

        int N_PRODUCED_COMPONENTS = 0;
        int MAX_COUNT;


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Robot(int id, SemaphoreSlim s, SemaphoreSlim s2, MyQueue<Component> queue, SemaphoreSlim mutex, int max_count)
        {
            Id = id;
            s_TakePieces = s;
            s_Build = s2;
            Queue = queue;
            mutexQueue = mutex;
            MAX_COUNT = max_count;
        }

        public async Task TakePiecesAsync()
        {
            while(true)
            {
                await s_TakePieces.WaitAsync();

                int current = Interlocked.Increment(ref N_PRODUCED_COMPONENTS);

                if (current > MAX_COUNT)
                {
                    s_TakePieces.Release();
                    break;
                }

                Component c = new Component($"COMPONENTE -N{Id}-");

                Console.WriteLine($"ROBOT#{this.Id} ha preso il pezzo {c.Name}");
                Console.WriteLine($"ROBOT#{this.Id} sta spostando il pezzo . . . ");
                await Task.Delay(200);

                await mutexQueue.WaitAsync();
                Queue.Enqueue(c);
                mutexQueue.Release();

                Console.WriteLine($"ROBOT#{this.Id} ha posizionato il pezzo {c.Name} nella coda");

                s_Build.Release();
            }
        }
    }
}
