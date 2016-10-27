using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kasp2
{
    class Program
    {
        static MySaveQueue<int> q;
        static void Main(string[] args)
        {
            q = new MySaveQueue<int>();

            //Вызываем в произвольном порядке
            q.Pop();
            q.Pop();
            q.Push(3);
            q.Push(4);
            q.Pop();
            q.Pop();
            q.Push(2);
            q.Pop();
            q.Pop();
            q.Push(1);
            q.Push(5);
            q.Push(6);

            Console.ReadLine();
        }

         

    }

    class MySaveQueue<T>
    {
        private Queue<T> _myQueue;
        private  Object _myQueueLockObj = new object();
        private object _changeLocker = new object();

        protected Queue<T> MyQueue
        {
            get
            {
                if (_myQueue == null)
                {
                    lock (_myQueueLockObj)
                    {
                        if (_myQueue == null)
                        {
                            _myQueue = new Queue<T>();

                        }
                    }
                }
                return _myQueue;
            }
        }

        public async void Push(T obj)
        {
            await Task.Run(() =>
            {
                lock (_changeLocker)
                {
                    Console.WriteLine("Push {0}", obj);

                    MyQueue.Enqueue(obj);

                    Monitor.Pulse(_changeLocker);
                }
            });
        }


        public async Task<T> Pop()
        {
            T item = default(T);
            await Task.Run(() =>
            {
                lock (_changeLocker)
                {
                    var i = 0;
                    do
                    {
                        try
                        {
                            item = MyQueue.Dequeue();
                            i++;
                            Console.WriteLine("Pop {0}, {1}", item, MyQueue.Count);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Wait(_changeLocker);
                        }
                    } while (i == 0);
                }
            });

            return item;
        }
    }
}
