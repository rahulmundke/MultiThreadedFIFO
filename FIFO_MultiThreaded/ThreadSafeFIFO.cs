using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FIFO_MultiThreaded
{
   
   class ThreadSafeFIFO <T> 
   {
      public ThreadSafeFIFO(int capacity)
      {
         m_limit = capacity;
         m_currentCapacity = 0;
         m_data = new List<T>(m_limit);
         
         for (int i = 0; i < m_limit; i++)
            m_data.Insert(i,default(T));

         m_readIndex = 0;
         m_writeIndex = 0;

         m_readerCount = 0;

         m_Signal = new Mutex();
         m_ResourceLock = new Mutex();
         m_QueueEmpty = new AutoResetEvent(true);
         m_QueueFull = new AutoResetEvent(false);
      }

      void dumpData()
      {
         Console.WriteLine("m_data items are:");
         foreach( object item in m_data)
            Console.Write(item + " ");
         Console.WriteLine("");
      }

      public bool AddItemToFIFO(T item, string requestor)
      {
         try
         {
            m_Signal.WaitOne();
            m_ResourceLock.WaitOne();

            if (m_currentCapacity >= m_limit)
            {
               Console.WriteLine("{0} : FIFO full, cant add item, will wait till space is created!", requestor);
               m_QueueFull.Reset();
               return false;
            }

            if (m_writeIndex == m_limit)
               m_writeIndex = 0;
            m_data[m_writeIndex++] = item;
            Console.WriteLine("{1} Item {0} added to FIFO", item, requestor);
            m_currentCapacity++;
            dumpData();
            return true;
         }
         catch (Exception e)
         {
            Console.WriteLine(e.Message);
            return false;
         }
         finally
         {
            m_Signal.ReleaseMutex();
            m_ResourceLock.ReleaseMutex();
         }
      }

      public T GetItem(string requestor)
      {
         try
         {
            m_Signal.WaitOne();
            
            T item;
            if (m_currentCapacity == 0)
            {
               Console.WriteLine("{0} : No items in queue currently, will wait till get one", requestor);
               m_QueueEmpty.Reset();
               return default(T);
            }

            m_readerCount++;
            if (m_readerCount == 1)
            {
               m_ResourceLock.WaitOne();
            }
            

            if (m_readIndex == m_limit)
               m_readIndex = 0;

            item = m_data[m_readIndex++];
            Console.WriteLine("{1} Item read is {0}", item, requestor);
            m_currentCapacity--;
            m_data[m_readIndex - 1] = default(T);

            dumpData();

            m_readerCount--;
            if (m_readerCount == 0)
            {
               m_ResourceLock.ReleaseMutex();
            }
            return item;
         }
         catch (Exception e)
         {
            Console.WriteLine(e.Message);
            return default(T);
         }
         finally
         {
            m_Signal.ReleaseMutex();
         }
      }

      private List<T> m_data;
      private Mutex m_Signal;
      private Mutex m_ResourceLock;
      private AutoResetEvent m_QueueFull;
      private AutoResetEvent m_QueueEmpty;

      private int m_limit;
      private int m_currentCapacity;
      private int m_writeIndex;
      private int m_readIndex;
      private int m_readerCount;
   }
}
