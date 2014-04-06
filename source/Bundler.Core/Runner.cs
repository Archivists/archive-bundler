using System;

namespace Bundler.Core
{
  public class Runner
  {
    readonly Options _options;
    IDisposable _subscription;

    public Runner(Options options)
    {
      _options = options;
    }

    public void Start()
    {
        _subscription = Listener
          .Register(_options.Dobbin, _options.Filter)
          //.Subscribe(s => Console.WriteLine(Job.Scan(_options.Dobbin, s).ToString()));
          .Subscribe(s => {
            var j= Job.Scan(_options.Dobbin, s);
            Console.WriteLine(j);
            if (j.IsReadyForProcessing)
            {
              var mover = new Mover(_options, j);
              mover.PrepareBundles();
            }
        });
    }

    private void MoveFiles(Job j)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
      _subscription.Dispose();
    }
  }
}
