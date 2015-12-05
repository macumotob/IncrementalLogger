using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;

namespace coba
{
  public class Logger
  {
    public static Logger Instance = new Logger("test");
    public Logger(string prefix, long max_size_in_kb = 128)
    {
      Debug.Assert(!string.IsNullOrEmpty(prefix));
      Debug.Assert(max_size_in_kb > 64);
      _prefix = prefix;
      _file_name_base = GetDateFileName(_prefix);
      _max_file_size = 1024 * max_size_in_kb;
    }
    Exception _exception;
    volatile object _locker = new object();
    private string _file_name_base = null;
    private string _prefix;
    ~Logger()
    {
    }
    public static string GetDateFileName(string prefix)
    {
      DateTime d = DateTime.Now;
      string s = string.Format("{3}_{0}_{1}_{2}", d.Day, d.Month, d.Year, prefix);
      return s;
    }
    private long _max_file_size = 1024 * 64;

    private string _make_file_name(int index)
    {
      string s = string.Format("{0}({1}).txt", _file_name_base, index);
      return s;
    }
    private int _find_max_index(string file)
    {
      string folder = Path.GetDirectoryName(file);
      string ext = Path.GetExtension(file);
      string mask = _file_name_base + "(*)" + ext;
      string[] files = Directory.GetFiles(folder, mask);

      int index = 0;
      int max = 0;
      foreach (string s in files)
      {
        string f = Path.GetFileNameWithoutExtension(s);
        f = f.Replace(_file_name_base, "").Replace("(", "").Replace(")", "");
        int.TryParse(f, out index);
        max = index > max ? index : max;
      }
      return max;
    }
    private string _create_file_name()
    {
      string folder = AppDomain.CurrentDomain.BaseDirectory;
      folder = folder.Replace("\\bin\\Debug", "");
      string file = folder + GetDateFileName(_prefix) + ".txt";
      int index = _find_max_index(file);
      string ff = folder + _make_file_name(index);
      if (File.Exists(ff))
      {
        file = ff;
      }
      FileInfo fi = new FileInfo(file);
      if (fi.Exists)
      {
        if (fi.Length > _max_file_size)
        {
          string backup = file;
          index = _find_max_index(file);

          index++;
          file = folder + _make_file_name(index);
        }
      }
      return file;
    }
    private void _log(string text)
    {
      try
      {
        lock (_locker)
        {
          string file = _create_file_name();

          DateTime dt = DateTime.Now;

          string s = string.Format("{0,2:D2}.{1,2:D2}.{2,4:D4} {3,2:D2}:{4,2:D2}:{5,2:D2}.{6,3:D3}/{7}\t",
            dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second, dt.Millisecond,
            Thread.CurrentThread.ManagedThreadId);

          s += text + Environment.NewLine;
          File.AppendAllText(file, s);
        }
      }
      catch (Exception ex)
      {
        _exception = ex;
      }
    }
    public void Log(string format, params object[] args)
    {
      try
      {
        string text = string.Format(format, args);
        _log(text);
      }
      catch (ThreadAbortException ex)
      {

      }
      catch (Exception ex)
      {
      }
      Thread.Sleep(10);
    }
  }//end of class
}
