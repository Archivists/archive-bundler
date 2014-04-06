using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Bundler.Core.Expectations
{
  class Meta
  {
    const string Pattern = "*.meta.xml";
    const string AudioFile = "string(/MetaData/Files/Set[@Type='audio']/File/FileName/text())";
    const string NumberOfTracks = "count(/MetaData/AdlTrackList/AdlTrack)";

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

      foreach (var file in files)
      {
        yield return ReferenceToAudioFile(file);

        foreach (var track in EnumerateTracks(job, file))
        {
          yield return track;
        }
      }
    }

    static IEnumerable<IExpectation> EnumerateTracks(Job job, string topLevelMeta)
    {
      var tracks = 0;
      var fail = false;
      try
      {
        tracks = Convert.ToInt32(topLevelMeta.Extract(NumberOfTracks));
      }
      catch (XmlException)
      {
        fail = true;
      }

      if (fail)
      {
        yield return new Fail(String.Format("Expected that {0} is valid XML and contains element at {1}",
                                            topLevelMeta,
                                            NumberOfTracks));
        yield break;
      }

      if (tracks <= 0)
      {
        yield break;
      }

      var path = Path.Combine(job.Directory, "tracks");
      if (!Directory.Exists(path))
      {
        yield return new Fail("Path \"{0}\" does not exist.", path);
        yield break;
      }

      for (var track = 1; track <= tracks; track++)
      {
        var trackMetaFilePattern = String.Format("{0}-{1:D2}.{2}", job.Id, track, Pattern);

        var files = Directory.GetFiles(path,
                                       trackMetaFilePattern,
                                       SearchOption.TopDirectoryOnly);
        if (!files.Any())
        {
          yield return new Fail("No files in path \"{0}\" match the pattern {1}.", path, trackMetaFilePattern);
        }

        //Check for wav and mp3 file for the track
        if (files.Count() < 2)
        {
            yield return new Fail("Expected two XML files for track (MP3 and WAV) but found one.");
            yield break;
        }

        foreach (var file in files)
        {
          yield return ReferenceToAudioFile(file);
        }
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
  }
}
