using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace FIFO_MultiThreaded
{
   class Program
   {
      private static ThreadSafeFIFO<int> objFifo;

      static void Main(string[] args)
      {
         objFifo = new ThreadSafeFIFO<int>(4);

         Thread t1 = new Thread(WriteToFIFO1);
         Thread t2 = new Thread(ReadFromFIFO1);
         Thread t3 = new Thread(WriteToFIFO2);
         Thread t4 = new Thread(ReadFromFIFO2);

         t1.Start();
         t2.Start();
         t3.Start();
         t4.Start();
         Console.ReadKey();
      }

      static void WriteToFIFO1()
      {
         for (int i = 2; i < 100; i=i+2)
         {
            objFifo.AddItemToFIFO(i,"WriterThread1");
            Thread.Sleep(400);
         }
         Console.WriteLine("Write Thread1 finished!!!");
      }

      static void WriteToFIFO2()
      {
         for (int i = 1; i < 100; i=i+2)
         {
            objFifo.AddItemToFIFO(i,"WriterThread2");
            Thread.Sleep(100);
         }
         Console.WriteLine("Write Thread2 finished!!!");
      }

      static void ReadFromFIFO1()
      {
         for (int j = 1; j < 1000; j++)
         {
            int temp = objFifo.GetItem("ReadThread1");
            Thread.Sleep(600);
         }
         Console.WriteLine("Read Thread1 finished!!!");
      }

      static void ReadFromFIFO2()
      {
         for (int j = 1; j < 1000; j++)
         {
            int temp = objFifo.GetItem("ReadThread2");
            Thread.Sleep(500);
         }
         Console.WriteLine("Read Thread2 finished!!!");
      }
   }
}
