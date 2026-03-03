using System;
using System.Collections.Generic;
using System.Text;
using MyQueueLibrary;
using MyStackLibrary;

namespace ADT_Code_Portale
{
    internal class Dinosaur
    {
        public string Name { get; set; }

        SemaphoreSlim s_Build;
        SemaphoreSlim s_TakePieces;
        public MyQueue<Component> Queue { get; set; }
        MyStack<Component> stack = new();

        SemaphoreSlim mutexQueue;
        SemaphoreSlim mutexStack = new(1, 1);

        int MAX_HEIGHT;
        int N_ELEMENT = 0;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Dinosaur(string name, SemaphoreSlim s, SemaphoreSlim s2, MyQueue<Component> queue, SemaphoreSlim mutex, int max_height = 5)
        {
            Name = name;
            s_Build = s;
            s_TakePieces = s2;
            Queue = queue;
            mutexQueue = mutex;
            MAX_HEIGHT = max_height;
        }


        public async Task BuildComponentAsync()
        {
            while (true)
            {
                await s_Build.WaitAsync();

                await mutexQueue.WaitAsync();
                Component c = Queue.Dequeue();
                mutexQueue.Release();

                Console.WriteLine($"{this.Name} ha preso il pezzo {c.Name} dalla coda");
                Console.WriteLine($"{this.Name} sta assemblando il pezzo . . . ");
                await Task.Delay(300);

                await mutexStack.WaitAsync();
                if (N_ELEMENT == MAX_HEIGHT)
                {
                    stack.Clear();

                    Console.WriteLine($"{this.Name} ha concluso il pezzo e svuota lo STACK");

                    Interlocked.Exchange(ref N_ELEMENT, 0);
                }

                stack.Push(c);

                Interlocked.Increment(ref N_ELEMENT);
                mutexStack.Release();

                Console.WriteLine($"{this.Name} ha posizionato il pezzo {c.Name} nello stack");

                s_TakePieces.Release();
            }
        }
    }
}
