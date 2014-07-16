using System;
using System.IO;
using System.Timers;
using Topshelf;
using Topshelf.Logging;

namespace Bundler
{
  public class Runner
  {
    readonly Options _options;
    Timer timer;
    static readonly LogWriter _log = HostLogger.Get<Runner>();

    public Runner(Options options)
    {
      _options = options;
    }

    public void Start()
    {
      timer = new Timer(15000);
      timer.Elapsed += OnTimedEvent;
      timer.Enabled = true;
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
      string[] subdirectoryEntries = Directory.GetDirectories(_options.Dobbin);
      foreach (string subdirectory in subdirectoryEntries)
      {
        var j = Job.Scan(_options.Dobbin, subdirectory);
        if (j.IsReadyForProcessing)
        {
          var mover = new Mover(_options, j);
          mover.PrepareBundles();
        }
      }
    }

    public void Stop()
    {
      timer.Stop();
      timer.Dispose();
    }
  }
}
