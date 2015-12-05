using System;
using System.Threading;
using coba;
namespace LoggerMT
{
  class LoggerTest
  {
    class dt
    {
      public int sleep;
      public int index;
      public string name;
      public Thread thread;
      public int times;
    }
    public static void Run()
    {
      int thread_count = 1500;
      int max_steps_in_thread = 10;
      int max_sleep_time = 1500;

      Random rnd = new Random();
      for (int i = 0; i < thread_count; i++)
      {
        Thread t = new Thread(_worker);
     
        dt d = new dt();
        d.sleep = rnd.Next(max_sleep_time);
        d.times = rnd.Next(max_steps_in_thread);
        d.index = i;
        d.name = t.Name;
        d.thread = t;
        // LogManager.Instance.RegisterThread(t, d);
        ParameterizedThreadStart ps = new ParameterizedThreadStart(_worker);
        LogManager.Instance.RegisterThread( ps, d);

        //   t.Start(d);
      }

      // Thread.Sleep(13000);

      LogManager.Instance.StopAllThreads();
    }
    public static void _worker(object data)
    {
      dt d = (dt)data;

      Thread.Sleep(d.sleep);
      Logger.Instance.Log("index:\t{0}\t{1}\t{2}", d.index, d.sleep, d.times);
      for (int i = 0; i < d.times; i++)
      {
        Thread.Sleep(d.sleep + i);
        if(i == d.times - 2)
        {
          try
          {
            int x = d.times/( d.times - d.times);
          }
          catch(Exception ex)
          {
            Logger.Instance.Log(ex);
          }
        }
        Logger.Instance.Log("THREAD\t{0}\tstep\t{1}", d.name, i);
      }
    }
  }// end of class
}
