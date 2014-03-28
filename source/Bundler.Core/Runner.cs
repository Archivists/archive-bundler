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
        .Register(_options.Dobbin)
        .Subscribe(s => Console.WriteLine(Job.Scan(s).ToString()));
    }

    public void Stop()
    {
      _subscription.Dispose();
    }
  }
}
