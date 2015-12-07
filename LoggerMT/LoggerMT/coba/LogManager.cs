using System;
using System.Collections.Generic;
using System.Threading;

namespace coba
{
  internal class _thread_info
  {
    internal Thread thread;
    internal object data;
    internal void Run()
    {
      thread.Start(data);
    }
  }
  public class LogManager : IDisposable
  {

    public static LogManager Instance = new LogManager();
    private Logger _logger = new Logger("mngr");

    Queue<_thread_info> _waiting_threads = new Queue<_thread_info>();
    List<Thread> _running_threads = new List<Thread>();

    int _max_running_threads = 1000;
    int _thread_id = 0;
    private object _locker = new object();
    public LogManager()
    {
      _logger.Log("Log manager constructor.");
    }
    ~LogManager()
    {
      _logger.Log("Log manager destructor.");
    }
    private void _run_thread_from_queue()
    {
      if (_waiting_threads.Count == 0) return;
      _thread_info info = _waiting_threads.Dequeue();
      if (info == null)
      {
        return;
      }
      _running_threads.Add(info.thread);
      info.thread.Start(info.data);

      _logger.Log("Thread from queue started : {0}", info.thread.Name);
      info = null;

    }
    public void RegisterThread(ParameterizedThreadStart ps, object data)
    {
      _free_running_threads_list();

      lock (_locker)
      {
        Thread thread = new Thread(ps);
        _thread_info info = new _thread_info() { thread = thread, data = data };

        thread.IsBackground = true;
        if (string.IsNullOrEmpty(thread.Name))
        {
          thread.Name = string.Format("T{0}", _thread_id++);
        }
        _waiting_threads.Enqueue(info);

        if (_running_threads.Count >= _max_running_threads)
        {
          _logger.Log("Maximum. Thread enqueue : {0}", thread.Name);
        }
        else
        {
          _run_thread_from_queue();
        }
        _logger.Log("Register thread {0}", thread.Name);
      }
    }

  public void RegisterThread(Thread thread, object data)
    {
      _free_running_threads_list();

      lock (_locker)
      {
        thread.IsBackground = true;
        if (string.IsNullOrEmpty(thread.Name))
        {
          thread.Name = string.Format("T{0}", _thread_id++);
        }
        _thread_info info = new _thread_info() { thread = thread, data = data };
        _waiting_threads.Enqueue(info);

        if (_running_threads.Count >= _max_running_threads)
        {
          _logger.Log("Maximum. Thread enqueue : {0}", thread.Name);
        }
        else
        {
          _run_thread_from_queue();
        }
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
        while (i < _running_threads.Count)
        {
          if (_running_threads[i].IsAlive)
          {
            i++;
          }
          else
          {
            _logger.Log("Thread {0} is alive {1}. REMOVE", _running_threads[i].Name, _running_threads[i].IsAlive);
            _running_threads[i] = null;
            _running_threads.RemoveAt(i);
          }
        }
        if (_running_threads.Count < _max_running_threads)
        {
          _run_thread_from_queue();
        }
      }
    }
    public void StopAllThreads()
    {
      lock (_locker)
      {
        while (_running_threads.Count > 0)
        {
          _free_running_threads_list();
          Thread.Sleep(10);
        }
        _logger.Log("Menger : all thread STOPPED!");
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
