using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Bundler.Core.Expectations
{
  static class Dobbin
  {
    const string Pattern = "*.dobbin.result.xml";
    const string AudioFile = "string(/Dobbin/Path/text())";
    const string Status = "string(/Dobbin/Result/text())";

    public static IEnumerable<IExpectation> Expectations(Job job)
    {
      if (!Directory.Exists(job.Directory))
      {
        yield break;
      }

      var files = Directory.GetFiles(job.Directory, job.Id + Pattern, SearchOption.TopDirectoryOnly);
      if (!files.Any())
      {
        yield return new Fail("No files in path \"{0}\" match the pattern {1}.", job.Directory, Pattern);
        yield break;
      }

      if (files.Count() < 4)
      {
          yield return new Fail("Top level file set is not complete.");
          yield break;
      }

      foreach (var file in files)
      {
        yield return ReferenceToAudioFile(file);
        yield return StatusSuccess(file);
      }
    }

    static IExpectation ReferenceToAudioFile(string file)
    {
      try
      {
        var audio = file.Extract(AudioFile).AbsolutePath(file);
        return new FileExists(audio);
      }
      catch (XmlException)
      {
        return new Fail(String.Format("Expected that {0} is valid XML and contains element at {1}", file, AudioFile));
      }
    }

    static IExpectation StatusSuccess(string file)
    {
        try
        {
            var status = file.Extract(Status).ToString();
            if (status != "success")
                return new Fail("Dobbin task error.");
            
            return new Success();
        }
        catch (XmlException)
        {
            return new Fail(String.Format("Expected that {0} is valid XML and contains element at {1}", file, AudioFile));
        }
    }
  }
}