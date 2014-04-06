using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Bundler.Core.Expectations
{
  class TracksExpectation
  {
    const string ResultPattern = "*.dobbin.result.xml";
    const string MetaPattern = "*.mp3.meta.xml";
    const string NumberOfTracks = "count(/MetaData/AdlTrackList/AdlTrack)";

    public static IEnumerable<IExpectation> Expectations(Job job)
    {

      var files = Directory.GetFiles(job.JobDirectory, job.Id + MetaPattern, SearchOption.TopDirectoryOnly);
      if (!files.Any())
      {
        yield return new FailExpectation(String.Format("MP3 meta.xml files does not exist yet for Job ID {0}", job.Id));
        yield break;
      }

      foreach (var file in files)
      {

        foreach (var track in EnumerateTracks(job, file))
        {
          yield return track;
        }
      }
    }

    static IEnumerable<IExpectation> EnumerateTracks(Job job, string file)
    {
      var tracks = 0;
      var fail = false;
      try
      {
        tracks = Convert.ToInt32(file.Extract(NumberOfTracks));
      }
      catch (XmlException)
      {
        fail = true;
      }

      if (fail)
      {
        yield return new FailExpectation(String.Format("Expected that {0} is valid XML and contains element at {1}",
                                            file,
                                            NumberOfTracks));
        yield break;
      }

      if (tracks <= 0)
      {
        yield break;
      }

      var path = Path.Combine(job.JobDirectory, "tracks");
      if (!Directory.Exists(path))
      {
        yield return new FailExpectation("Path \"{0}\" does not exist.", path);
        yield break;
      }

      var resultFiles = Directory.GetFiles(path, job.Id + ResultPattern, SearchOption.TopDirectoryOnly);
      
      if (resultFiles.Count() != tracks * 2) // * 2 since we will get result files for both wav and mp3 files.
      {
        yield return new FailExpectation(String.Format("Job ID \"{0}\" expects {1} tracks.", job.Id, tracks));
      }
    }
  }
}
