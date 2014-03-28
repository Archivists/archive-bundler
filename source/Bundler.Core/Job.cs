using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bundler.Core.Expectations;

namespace Bundler.Core
{
  public class Job
  {
    Job(string basePath)
    {
      BasePath = basePath;

      var expectations = BuildExpectations();
      UnmetExpectations = expectations.Where(x => !x.Verify()).Select(x => x.GetMessage());
    }

    public IEnumerable<string> UnmetExpectations { get; private set; }

    public bool IsReadForProcessing
    {
      get
      {
        return !UnmetExpectations.Any();
      }
    }

    public string BasePath { get; private set; }

    public string Id
    {
      get
      {
        return Path.GetFileName(BasePath);
      }
    }

    IEnumerable<IExpectation> BuildExpectations()
    {
      return new IExpectation[]
             {
               new DirectoryExists(BasePath)
             }
        .Concat(Dobbin.Expectations(this))
        .Concat(Meta.BuildExpectations(this));
    }

    public override string ToString()
    {
      var failed = "- " + String.Join(Environment.NewLine + "- ", UnmetExpectations);
      return String.Format("Job ID {0} based off of {1} is {2}ready for processing{3}",
                           Id,
                           BasePath,
                           IsReadForProcessing ? "" : "not ",
                           UnmetExpectations.Any() ? " because of:" + Environment.NewLine + failed : "");
    }

    public static Job Scan(string path)
    {
      return new Job(path);
    }
  }
}
