using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lab16
{
    class Program
    {
        static void Algorithm(int a,int b)
        {
            while (b != 0)
            { 
                b = a % (a = b);
                Console.WriteLine(b);
                Thread.Sleep(1000);
            }

            Console.WriteLine("НОД:"+a);
        }

        static void Algorithm2(int a, int b, CancellationToken token) //метод поддерживающий отмену через токен отмены
        {
            while (b != 0)
            {
                if (token.IsCancellationRequested) //проверка флага требования завершения задачи
                {
                    Console.WriteLine("Операция прервана токеном");

                    return;
                }
                b = a % (a = b);
                Console.WriteLine(b);
                Thread.Sleep(1000);
            }

            Console.WriteLine("НОД:" + a);
        }


        static void ForAsyncMethod()
        {
            for(int i = 1; i < 11; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(300);
            }
        }

        static async void AsyncMethod()
        {
            await Task.Run(() => ForAsyncMethod());// асинхронный запуск задачи
        }

        static void Display(Task t)
        {
            Console.WriteLine("Id задачи: {0}", Task.CurrentId);//вывод информации о задаче
            Console.WriteLine("Id предыдущей задачи: {0}", t.Id);
        }

        public static void CreateBigArr(int x)
        {
            Random rand = new Random();
            int[] arr = new int[1000000];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rand.Next(10);
            }

            Console.WriteLine("Выполнена задача номер " + x);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Первое задание:");
            Stopwatch stopwatch = new Stopwatch(); // создание обекта для замера времени работы
            Task algorithmTask = new Task(()=>Algorithm(2500,24354)); // создание новой задачи из метода
            stopwatch.Start();// начало отсчета времени выполнения
            algorithmTask.Start(); // старт задачи
            
            Thread.Sleep(500);
            Console.WriteLine("Статус задачи: " + algorithmTask.Status); // вывод состояния задачи
            Console.WriteLine("Завершена ли задача: "+algorithmTask.IsCompleted);// вывод состояния задачи
            algorithmTask.Wait();//ожидание завершения задачи
            stopwatch.Stop();//остановка счетчика
            TimeSpan timeSpan = stopwatch.Elapsed; //время за которое выполнилась задача
            Console.WriteLine("Время выполнения: "+timeSpan); //вывод затраченого времени в консоль
            Console.ReadKey();

            Console.WriteLine("\nВторое задание:");
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();// создание токена отмены(позволяет принудительно завершать задачи при наличии проверки флага отмены в теле самой задачи)
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task algorithmTask2 = new Task(() => Algorithm2(2500, 24354, cancellationToken));//создание задачи и передача токена отмены
            algorithmTask2.Start(); // старт задачи
            Thread.Sleep(2500);
            cancellationTokenSource.Cancel();//завершение задачи через токен отмены
            Console.ReadKey();

            int z = 0;
            Func<int> func = () => ++z;

            Console.WriteLine("\nТретье задание:");
            //создание трех задач
            Task<int> returnOne = new Task<int>(func);
            Task<int> returnTwo = new Task<int>(func);
            Task<int> returnThree = new Task<int>(func);
            //старт этих задач
            returnOne.Start();
            returnTwo.Start();
            returnThree.Start();

            int Factorial() => returnOne.Result * returnTwo.Result * returnThree.Result; //создание локальной функции которая использует результаты задач

            Task<int> resultTask = new Task<int>(Factorial); //создание задачи на основе локальной вункции

            resultTask.Start();//старт задачи

            Console.WriteLine("Result = " + resultTask.Result); //вывод результата
            Console.ReadKey();

            Console.WriteLine("\nЧетвертое задание(1): ");
            Task taskContOne = new Task(() =>  //создание основной задачи
            {
                Console.WriteLine("Id задачи: {0}", Task.CurrentId);
            });

            Task taskContTwo = taskContOne.ContinueWith(Display); //установка метода или задачи продолжения, он будет вызван как только завершится основная задача
            taskContOne.Start();
            Console.ReadKey();
            //тоже самое но ручками
            Console.WriteLine("\nЧетвертое задание(2): ");
            Random rand = new Random();
            Task<int> what = Task.Run(() => rand.Next(10) * rand.Next(10)); //создаем и сразу же запускаем задачу

            TaskAwaiter<int> awaiter = what.GetAwaiter(); // получение объекта ожидания задачи

            awaiter.OnCompleted(() => //назначение метода на событие завершения задачи
            {
                Console.WriteLine("Result: " + awaiter.GetResult());//получение результата выполнения задачи и его вывод на консоль
            });
            Console.ReadKey();

            Console.WriteLine("\nПятое задание:");

            stopwatch.Restart(); // сброс и перезапуск счетчика времени выполнения
            Parallel.For(0, 5, CreateBigArr); // создание параллельного цикла от 0 до 5 с установкой метода который будет выполнятся 
            stopwatch.Stop();
            Console.WriteLine("Время при Parallel.For:  " + stopwatch.Elapsed + '\n'); //вывод затраченного времени

            stopwatch.Restart(); //перезапуск счетчика времени выполнения
            for (int j = 0; j < 5; j++) // та же работа но уже в синхронном варианте
            {
                int[] arr = new int[1000000];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = rand.Next(10);
                }

                Console.WriteLine("Выполнена задача номер " + j);
            }

            stopwatch.Stop();
            Console.WriteLine("Время при For: " + stopwatch.Elapsed + '\n'); // вывод времени затраченного на синхронное выполнение
            Console.WriteLine();

            List<int> list = new List<int>() { 1, 2, 3, 4, 5 };

            stopwatch.Restart();//перезапуск счетчика времени выполнения
            ParallelLoopResult result = Parallel.ForEach<int>(list, CreateBigArr); // теперь параллельный foreach
            stopwatch.Stop();
            Console.WriteLine("Время при Parallel.Foreach: " + stopwatch.Elapsed + '\n');

            stopwatch.Restart();
            foreach (int x in list)// а темерь обычный foreach
            {
                int[] arr = new int[1000000];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = rand.Next(10);
                }
                Console.WriteLine("Выполнена задача номер " + x);
            }
            stopwatch.Stop();
            Console.WriteLine("Время при Foreach: " + stopwatch.Elapsed + '\n');
            Console.ReadKey();

            Console.WriteLine("\nШестое задание:");
            Parallel.Invoke(() => Algorithm(20,255), () => Display(algorithmTask)); // паралельный запуск двух методов
            Console.ReadKey();

            Console.WriteLine("\nСедимое задание:");//не всегда может завершится нормально, коментим и показываем отдельно
            BlockingCollection<string> bc = new BlockingCollection<string>(5); //создание объекта коллекции
            Provider p1 = new Provider("Мыло", 10);//создание элементов коллекции которые мы будем добавлять
            Provider p2 = new Provider("Скраб", 300);
            Provider p3 = new Provider("Шампунь", 500);
            Provider p4 = new Provider("Гель", 700);
            Provider p5 = new Provider("Мочалка", 900);
            Customer c1 = new Customer(); Customer c2 = new Customer(); Customer c3 = new Customer(); Customer c4 = new Customer(); Customer c5 = new Customer();
            Customer c6 = new Customer(); Customer c7 = new Customer(); Customer c8 = new Customer(); Customer c9 = new Customer(); Customer c10 = new Customer();
            Customer[] carr = { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10 };// массив покупетелей
            Task bcTask = Task.Run(async () => // старт задачи с циклом в котором добавляются и покупаются товары пока склад не заполнится
            {
                while (bc.Count != bc.BoundedCapacity)
                {
                    Parallel.Invoke( //параллельный запуск методов добавления и удаления из коллекции 
                        () => p1.AddToStore(bc),
                        () => p2.AddToStore(bc),
                        () => p3.AddToStore(bc),
                        () => p4.AddToStore(bc),
                        () => p5.AddToStore(bc),
                        () => c1.TakeFromStore(bc),
                        () => c2.TakeFromStore(bc),
                        () => c3.TakeFromStore(bc),
                        () => c4.TakeFromStore(bc),
                        () => c5.TakeFromStore(bc),
                        () => c6.TakeFromStore(bc),
                        () => c7.TakeFromStore(bc),
                        () => c8.TakeFromStore(bc),
                        () => c9.TakeFromStore(bc),
                        () => c10.TakeFromStore(bc)
                        );
                }
                Console.WriteLine("Склад полон");
            });
            Console.Write("\n\n");
            Console.ReadKey();
            

            Console.WriteLine("\nВосьмое задание:");
            AsyncMethod(); // запуск асинхронного метода
            for(int i = 11; i < 21; i++)//синхронный цикл
            {
                Console.WriteLine(i);
                Thread.Sleep(500);
            }
            Console.ReadKey();
        }
    }
}
