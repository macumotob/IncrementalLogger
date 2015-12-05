using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coba
{
  public class LogManager
  {

    public static LogManager Instance = new LogManager();
    //public delegate void WWThreadWorker(object data);

    Queue<System.Threading.Thread> _activeThreads = new Queue<System.Threading.Thread>();
   // Dictionary<int, System.Threading.Thread> _activeThreads = new Dictionary<int, System.Threading.Thread>();
    int maxRunningThreads = 1000;
    int _runningThreadsCount = 0;
    object locker = new object();
    volatile bool done;

    public void RegisterThread(Thread thread)
    {
      lock (locker)
      {
        _runningThreadsCount++;
        thread.IsBackground = true;
        thread.Name = string.Format("ww_thread_{0}", _runningThreadsCount);
        _activeThreads.Enqueue(thread);
      }
     // LaunchWaitingThreads();
     // while (!done) Thread.Sleep(200);
    }
 
    public void StopAllThreads()
    {
      lock (locker)
      {
        while (_activeThreads.Count > 0)
        {
          Thread thread = _activeThreads.Dequeue();
          //activeThreads.Add(nextThread.ManagedThreadId, nextThread);
          // nextThread.Start();
         // Console.WriteLine("Thread " + thread.Name + " before " + thread.IsAlive);

          //thread.Abort();
          Thread.Sleep(100);
          thread.Join();
          Logger.Instance.Log("Thread terminated {0}", thread.Name);
         // Console.WriteLine("Thread " + thread.Name + " after " + thread.IsAlive);
        }
      }
    }
    /*
    // this is called by each thread when it's done
    void ThreadDone(int threadIdArg)
    {
      lock (locker)
      {
        // remove thread from active pool
        activeThreads.Remove(threadIdArg);
      }
      Console.WriteLine("Thread " + threadIdArg.ToString() + " finished");
      LaunchWaitingThreads(); // this could instead be put in the wait loop at the end of Run()
    }*/
  }
}
