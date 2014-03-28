using System.IO;
using System.Linq;

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
      var metas = Directory.GetFiles(path, jobId + "*.meta.xml", SearchOption.TopDirectoryOnly);
      var mp3s = Directory.GetFiles(path, jobId + "*.mp3", SearchOption.TopDirectoryOnly);

      return dobbinResults.Any() && metas.Any() && mp3s.Any();
    }
  }
}
