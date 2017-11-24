using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bundler.Expectations;

namespace Bundler
{
  public class Job
  {
    Job(string dobbin, string path)
    {
    }

    public IEnumerable<string> UnmetExpectations { get; private set; }

    public bool IsReadyForProcessing
    {
      get
      {
        return !UnmetExpectations.Any();
      }
    }
   
    public string Id { get; private set; }

    public string JobDirectory { get; private set; }

    IEnumerable<IExpectation> BuildExpectations()
    {
      return DobbinExpectation.Expectations(this)
        .Concat(TracksExpectation.Expectations(this));
    }

    public override string ToString()
    {
      var failed = "- " + String.Join(Environment.NewLine + "- ", UnmetExpectations);
      return String.Format("Job ID {0} located at {1} is {2} ready for processing {3}",
                           Id,
                           JobDirectory,
                           IsReadyForProcessing ? "" : "not ",
                           UnmetExpectations.Any() ? " because of:" + Environment.NewLine + failed : "");
    }

    public static Job Scan(string dobbin, string path)
    {
      return new Job(dobbin, path);
    }
  }
}
