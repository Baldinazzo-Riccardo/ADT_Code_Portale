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
        public MyQueue<Component> Queue { get; set; }
        MyStack<Component> stack = new();

        SemaphoreSlim mutexQueue;
        SemaphoreSlim mutexStack = new(1, 1);

        int MAX_HEIGHT;

        CancellationToken Ct;

        public Dinosaur(string name, SemaphoreSlim s, CancellationToken ct, MyQueue<Component> queue, SemaphoreSlim mutex, int max_height = 5)
        {
            Name = name;
            s_Build = s;
            Queue = queue;
            mutexQueue = mutex;
            MAX_HEIGHT = max_height;
            Ct = ct;
        }

        public async Task BuildComponentAsync()
        {
            while (!Ct.IsCancellationRequested)
            {
                await s_Build.WaitAsync();

                if (!Queue.IsEmpty())
                {
                    await mutexQueue.WaitAsync();
                    Component c = Queue.Dequeue();
                    mutexQueue.Release();

                    Console.WriteLine($"{this.Name} ha preso il pezzo {c.Name} dalla coda");
                    Console.WriteLine($"{this.Name} sta assemblando il pezzo . . . ");
                    await Task.Delay(300);

                    await mutexStack.WaitAsync();
                    stack.Push(c);
                    mutexStack.Release();

                    Console.WriteLine($"{this.Name} ha posizionato il pezzo {c.Name} nello stack");
                }
            }
        }
    }
}
