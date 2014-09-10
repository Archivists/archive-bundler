using System;
using System.IO;
using System.Collections;
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
    ArrayList jobs = new ArrayList();

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
      _log.Debug("OnTimedEvent called");
      if (jobs.Count == 0)
      {
        jobs.AddRange((Directory.GetDirectories(_options.Dobbin)));
        _log.Debug(String.Format("Job queue length after directory scan is {0}", jobs.Count));
        foreach (string jobDirectory in new System.Collections.ArrayList(jobs))
        {
          var j = Job.Scan(_options.Dobbin, jobDirectory);
          if (j.IsReadyForProcessing)
          {
            var mover = new Mover(_options, j);
            mover.PrepareBundles();
            jobs.Remove(jobDirectory);
            _log.Debug(String.Format("Job queue length after job was processed is {0}", jobs.Count));
          } else
          {
            _log.Debug(j.ToString());
            _log.Debug(String.Format("Removing {0} from job queue as it is not ready for processing", jobDirectory));
            jobs.Remove(jobDirectory);
            
          }
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
