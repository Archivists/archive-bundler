using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Bundler;

using Topshelf;
using Topshelf.Logging;

namespace Bundler.Expectations
{
  static class DobbinExpectation
  {
    const string Pattern = "*.dobbin.result.xml";
    const string AudioFile = "string(/Dobbin/Path/text())";
    const string Status = "string(/Dobbin/Result/text())";
    static readonly LogWriter _log = HostLogger.Get<Job>();

    public static IEnumerable<IExpectation> Expectations(Job job)
    {

      // Irrispective of whether the event was triggered by a *.dobbin.result.xml in the Job level
      // directory, or the tracks directory - we'll check to see that the job level set is complete.
      var jobLevelFiles = new string[] {};
      try
      {
        jobLevelFiles = Directory.GetFiles(job.JobDirectory, job.Id + Pattern, SearchOption.TopDirectoryOnly);
      }
      catch(DirectoryNotFoundException e)
      {
        _log.Error("DobbinExpectation: Directory not found", e);
      }
      if (!jobLevelFiles.Any())
      {
        yield return new FailExpectation("Job directory does not yet contain dobbin results files.");
        yield break;
      }

      if (jobLevelFiles.Count() < 4)
      {
          yield return new FailExpectation("Job directory does not yet contain all four dobbin result files.");
          yield break;
      }

      foreach (var file in jobLevelFiles)
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
        return new FileExistsExpectation(audio);
      }
      catch (XmlException)
      {
        return new FailExpectation(String.Format("Expected that {0} is valid XML and contains element at {1}", file, AudioFile));
      }
    }

    static IExpectation StatusSuccess(string file)
    {
        try
        {
            var status = file.Extract(Status).ToString();
            if (status != "success")
                return new FailExpectation("Dobbin task error.");
            
            return new SuccessExpectation();
        }
        catch (XmlException)
        {
            return new FailExpectation(String.Format("Expected that {0} is valid XML and contains element at {1}", file, AudioFile));
        }
    }
  }
}