using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

using NUnit.Framework;

namespace Specs
{
  public class When_package_is_checked_for_completeness : Spec
  {
    [Test]
    [TestCase("inputs/complete/single result/PBC-ISB-001365-A", true)]
    [TestCase("inputs/missing/dobbin.result.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/missing/meta.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/missing/mp3 from dobbin.result.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/missing/mp3 from meta.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/files for another job/some job id", false)]
    [TestCase("inputs/invalid/dobbin.result.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/invalid/meta.xml/PBC-ISB-001365-A", false)]
    public void Should_check_files(string path, bool expected)
    {
      if (!Directory.Exists(path))
      {
        throw new DirectoryNotFoundException(String.Format("{0} to test was not found", path));
      }

      var complete = Package.Complete(path);
      Assert.AreEqual(expected, complete);
    }

    [Test]
    public void Should_check_if_path_exists()
    {
      var complete = Package.Complete("path does not exists");
      Assert.IsFalse(complete);
    }
  }

  public static class Package
  {
    public static bool Complete(string path)
    {
      if (!Directory.Exists(path))
      {
        return false;
      }

      var jobId = Path.GetFileName(path);

      var dobbinResults = Directory.GetFiles(path, jobId + "*.dobbin.result.xml", SearchOption.TopDirectoryOnly);
      if (!dobbinResults.Any())
      {
        return false;
      }

      if (!Exist(() => DobbinAudioFiles(dobbinResults)))
      {
        return false;
      }

      var metas = Directory.GetFiles(path, jobId + "*.meta.xml", SearchOption.TopDirectoryOnly);
      if (!metas.Any())
      {
        return false;
      }

      if (!Exist(() => MetaAudioFiles(metas)))
      {
        return false;
      }

      return true;
    }

    static bool Exist(Func<IEnumerable<string>> audioFileReader)
    {
      try
      {
        var audioFiles = audioFileReader();
        return audioFiles.All(File.Exists);
      }
      catch (XmlException)
      {
        return false;
      }
    }

    static IEnumerable<string> DobbinAudioFiles(IEnumerable<string> dobbinResults)
    {
      return Extract(dobbinResults, "/Dobbin/Path/text()");
    }

    static IEnumerable<string> MetaAudioFiles(IEnumerable<string> metas)
    {
      return Extract(metas, "/MetaData/Files/Set[@Type='audio']/File/FileName/text()");
    }

    static IEnumerable<string> Extract(IEnumerable<string> files, string xpath)
    {
      foreach (var result in files)
      {
        var file = new XPathDocument(result)
          .CreateNavigator()
          .SelectSingleNode(xpath)
          .Value;

        if (!Path.IsPathRooted(file))
        {
          file = Path.Combine(Path.GetDirectoryName(result), file);
        }

        yield return file;
      }
    }
  }
}
