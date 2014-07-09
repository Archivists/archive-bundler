using System;
using System.IO;

namespace Bundler.Core
{
  public class Runner
  {
    readonly Options _options;
    Boolean _run;

    public Runner(Options options)
    {
      _options = options;
      _run = true;
    }

    public void Start()
    {
      Console.WriteLine("Bundler started. Polling for jobs...");
      while (_run)
      {
        System.Threading.Thread.Sleep(5000);
        //Console.WriteLine("Cycled.");
        string [] subdirectoryEntries = Directory.GetDirectories(_options.Dobbin);
        foreach (string subdirectory in subdirectoryEntries)
        {
          Console.WriteLine(subdirectory);
          var j = Job.Scan(_options.Dobbin, subdirectory);
          if (j.IsReadyForProcessing)
          {
            var mover = new Mover(_options, j);
            mover.PrepareBundles();
          }
        }
      }
    }

    private void MoveFiles(Job j)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
      _run = false;
    }
  }
}
