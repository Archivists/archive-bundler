using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

using NUnit.Framework;

namespace Specs
{
  [TestFixture]
  public abstract class Spec
  {
    protected virtual void Establish()
    {
    }

    protected virtual void Because()
    {
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      Establish();
      Because();
    }
  }

  public class When_package_is_checked_for_completeness : Spec
  {
    [Test]
    [TestCase("inputs/complete/PBC-ISB-001365-A", true)]
    [TestCase("inputs/missing dobbin.result.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/missing meta.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/missing mp3/PBC-ISB-001365-A", false)]
    [TestCase("inputs/files for another job/some job id", false)]
    [TestCase("inputs/invalid dobbin.result.xml/PBC-ISB-001365-A", false)]
    [TestCase("inputs/invalid meta.xml/PBC-ISB-001365-A", false)]
    [TestCase("path does not exists", false)]
    public void Should_check_files(string path, bool expected)
    {
      var check = Package.Complete(path);
      Assert.AreEqual(expected, check);
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
      var audioFilesComplete = AudioFilesComplete(dobbinResults);

      var metas = Directory.GetFiles(path, jobId + "*.meta.xml", SearchOption.TopDirectoryOnly);

      return dobbinResults.Any() && metas.Any() && audioFilesComplete;
    }

    static bool AudioFilesComplete(IEnumerable<string> dobbinResults)
    {
      try
      {
        var audioFiles = AudioFiles(dobbinResults);
        return audioFiles.All(File.Exists);
      }
      catch (XmlException)
      {
        return false;
      }
    }

    static IEnumerable<string> AudioFiles(IEnumerable<string> dobbinResults)
    {
      foreach (var result in dobbinResults)
      {
        var file = new XPathDocument(result).CreateNavigator().SelectSingleNode("/Dobbin/Path/text()").Value;
        
        if (!Path.IsPathRooted(file))
        {
          file = Path.Combine(Path.GetDirectoryName(result), file);
        }

        yield return file;
      }
    }
  }
}
