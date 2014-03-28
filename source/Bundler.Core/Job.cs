using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Bundler.Core
{
  public class Job
  {
    Job(string basePath)
    {
      BasePath = basePath;

      var dobbinResults = FindFiles("*.dobbin.result.xml");
      var metas = FindFiles("*.meta.xml");

      var expectations = BuildExpectations(dobbinResults, metas);
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

    IEnumerable<string> FindFiles(string pattern)
    {
      if (!Directory.Exists(BasePath))
      {
        return new string[0];
      }

      return Directory.GetFiles(BasePath, Id + pattern, SearchOption.AllDirectories);
    }

    IEnumerable<IExpectation> BuildExpectations(IEnumerable<string> dobbinResults, IEnumerable<string> metas)
    {
      dobbinResults = dobbinResults.ToArray();
      metas = metas.ToArray();

      var dobbinAudioFiles = DobbinAudioFiles(dobbinResults).ToArray();
      var metaAudioFiles = MetaAudioFiles(metas).ToArray();

      return new IExpectation[]
             {
               new DirectoryExists(BasePath),
               new HasElements(dobbinResults, "Dobbin result.xml files were found"),
               new HasElements(metas, "meta.xml files were found")
             }
        .Concat(dobbinResults.Select(f => new LoadsAsXml(f)))
        .Concat(metas.Select(f => new LoadsAsXml(f)))
        .Concat(new[]
                {
                  new HasElements(dobbinAudioFiles, "Dobbin result.xml contains audio files"),
                  new HasElements(metaAudioFiles, "meta.xml contains audio files")
                })
        .Concat(dobbinAudioFiles.Select(f => new FileExists(f)))
        .Concat(MetaAudioFiles(metas).Select(f => new FileExists(f)));
    }

    public override string ToString()
    {
      var failed = String.Join(Environment.NewLine, UnmetExpectations);
      return String.Format("Job ID {0} based off of {1} is {2}ready for processing{3}",
                           Id,
                           BasePath,
                           IsReadForProcessing ? "" : "not ",
                           UnmetExpectations.Any() ? "because of:" + Environment.NewLine + failed : "");
    }

    public static Job Scan(string path)
    {
      return new Job(path);
    }

    static IEnumerable<string> EagerlyToCatchXmlErrors(IEnumerable<string> list)
    {
      try
      {
        return list.ToList();
      }
      catch (XmlException)
      {
        return new string[0];
      }
    }

    static IEnumerable<string> DobbinAudioFiles(IEnumerable<string> dobbinResults)
    {
      return EagerlyToCatchXmlErrors(dobbinResults.Select(f => Extract(f, "/Dobbin/Path/text()")));
    }

    static IEnumerable<string> MetaAudioFiles(IEnumerable<string> metas)
    {
      return
        EagerlyToCatchXmlErrors(metas.Select(f => Extract(f, "/MetaData/Files/Set[@Type='audio']/File/FileName/text()")));
    }

    static string Extract(string file, string xpath)
    {
      var value = new XPathDocument(file)
        .CreateNavigator()
        .SelectSingleNode(xpath)
        .Value;

      if (!Path.IsPathRooted(value))
      {
        value = Path.Combine(Path.GetDirectoryName(file), value);
      }

      return value;
    }
  }
}
