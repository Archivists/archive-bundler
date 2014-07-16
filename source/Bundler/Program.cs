using Topshelf;

namespace Bundler
{
  class Program
  {
    static void Main(string[] args)
    {
      HostFactory.Run(x =>
      {
        x.Service<Runner>(s =>
        {
          s.ConstructUsing(name => new Runner(Options.FromAppSettings()));
          s.WhenStarted(l => l.Start());
          s.WhenStopped(l => l.Stop());
        });

        x.RunAsLocalSystem();
        x.StartAutomaticallyDelayed();

        x.UseLog4Net("log4net.config");

        x.SetDescription("Bundler packages audio archives");
        x.SetDisplayName("Bundler");
        x.SetServiceName("Bundler");
      });
    }
  }
}
