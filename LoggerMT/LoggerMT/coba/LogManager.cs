using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coba
{
  public class LogManager :IDisposable
  {

    public static LogManager Instance = new LogManager();
    private Logger _logger = new Logger("mngr");

    Queue<System.Threading.Thread> _activeThreads = new Queue<System.Threading.Thread>();
    List<Thread> _running_threads = new List<Thread>();

    int maxRunningThreads = 1000;
    int _runningThreadsCount = 0;
    private object _locker = new object();
    public LogManager()
    {
      _logger.Log("Log manager constructor.");
    }
    ~LogManager()
    {
      _logger.Log("Log manager destructor.");
    }
    public void RegisterThread(Thread thread)
    {
      _free_running_threads_list();

      lock (_locker)
      {
        _runningThreadsCount++;
        thread.IsBackground = true;
        if (string.IsNullOrEmpty(thread.Name))
        {
          thread.Name = string.Format("T{0}", _runningThreadsCount);
        }
        _running_threads.Add(thread);
        _activeThreads.Enqueue(thread);
        _logger.Log("Register thread {0}", thread.Name);
      }
     // LaunchWaitingThreads();
     // while (!done) Thread.Sleep(200);
    }
 private void _free_running_threads_list()
    {
      lock (_locker)
      {
        int i = 0;
        while(i < _running_threads.Count)
        {
          if (_running_threads[i].IsAlive)
          {
            i++;
          }
          else
          {
            _logger.Log("Thread {0} is alive {1}. REMOVE" ,_running_threads[i].Name,_running_threads[i].IsAlive);
            _running_threads.RemoveAt(i);
          }
        }
      }
    }
    public void StopAllThreads()
    {
      lock (_locker)
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
          _logger.Log("Thread {0} terminated.", thread.Name);
         // Console.WriteLine("Thread " + thread.Name + " after " + thread.IsAlive);
        }
      }
    }

    public void Dispose()
    {
      _logger.Log("Log manager Dispose");
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
