using System;
using System.Collections.Generic;
using System.IO;

using Bundler.Core;

using Microsoft.Reactive.Testing;

using NUnit.Framework;

namespace Specs
{
  public class ListenerSpecs : Spec
  {
    [Test]
    public void Should_capture_changes_every_second()
    {
      var scheduler = new TestScheduler();
      scheduler.Start();
      Console.WriteLine(scheduler.IsEnabled);

      var temp = Path.GetTempPath();
      var file = Path.Combine(temp, Guid.NewGuid() + "__" + Guid.NewGuid());

      var subject = Listener.Register(temp);
      var capture = new List<string>();
      var subscription = subject.Subscribe(capture.Add);

      File.WriteAllText(file, "foo");
      File.Delete(file);

      CollectionAssert.IsEmpty(capture);

      scheduler.AdvanceBy(TimeSpan.FromMilliseconds(500).Ticks);
      CollectionAssert.IsEmpty(capture);

      scheduler.AdvanceBy(TimeSpan.FromMilliseconds(600).Ticks);
      CollectionAssert.IsNotEmpty(capture);
      CollectionAssert.Contains(capture, file);

      var currentCount = capture.Count;
      subscription.Dispose();

      File.WriteAllText(file, "foo");
      File.Delete(file);

      scheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);
      Assert.AreEqual(currentCount, capture.Count);

      foreach (var c in capture)
      {
        Console.WriteLine(c);
      }
    }
  }
}
