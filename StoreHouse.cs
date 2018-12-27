using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab16
{
    class Customer
    {
        public async Task TakeFromStore(BlockingCollection<string> bc)
        {
            Random rand = new Random();
            if (bc.Count == 0)
            {
                Console.WriteLine("Склад пуст");
                Thread.Sleep(rand.Next(1000));
            }
            else
            {
                bc.Take();
                Console.WriteLine("Товар взят со склада.");
                Thread.Sleep(rand.Next(1000));
            }
        }
    }

    class Provider
    {
        public string thing;
        public int timing;

        public Provider(string s, int i)
        {
            thing = s;
            timing = i;
        }

        public async Task AddToStore(BlockingCollection<string> bc)//добавление нового товара
        {
            await Task.Run(() =>//запуск добавления асинхронно
            {
                bc.Add(thing);
                Console.WriteLine("Добавлен товар: " + thing);
                Thread.Sleep(timing);
            });
        }
    }
}
