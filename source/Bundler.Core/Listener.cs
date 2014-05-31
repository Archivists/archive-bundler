using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Bundler.Core
{
  public class Listener
  {
    public static IObservable<string> Register(string path, string filter = null)
    {
      return Observable.Create<string>(subject =>
      {
        var disp = new CompositeDisposable();

        var fsw = CreateFileSystemWatcher(path, filter);
        fsw.IncludeSubdirectories = true;
        fsw.InternalBufferSize = 65536;

        disp.Add(fsw);

        var sources =
          new[]
          {
            Observable.FromEventPattern
              <FileSystemEventHandler, FileSystemEventArgs>(x => fsw.Changed += x,
                                                            x => fsw.Changed -= x),
            Observable.FromEventPattern
              <FileSystemEventHandler, FileSystemEventArgs>(x => fsw.Created += x,
                                                            x => fsw.Created -= x),
            Observable.FromEventPattern<ErrorEventArgs>(fsw, "Error")
                      .SelectMany(e => Observable.Throw<EventPattern<FileSystemEventArgs>>(e.EventArgs.GetException()))
          };

        var subscription = sources
          .Merge()
          .Select(x => x.EventArgs.FullPath)
          .Sample(TimeSpan.FromSeconds(1))
          .Synchronize(subject)
          .Subscribe(subject);


        disp.Add(subscription);

        fsw.EnableRaisingEvents = true;
        return disp;
      }).Publish().RefCount();
    }

    static FileSystemWatcher CreateFileSystemWatcher(string path, string filter)
    {
      if (filter != null)
      {
        return new FileSystemWatcher(path, filter);
      }
      return new FileSystemWatcher(path);
    }
  }
}
