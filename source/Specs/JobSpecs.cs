using System;
using System.IO;

using Bundler.Core;

using NUnit.Framework;

namespace Specs
{
  static class Ensure
  {
    public static void DirectoryExists(string path)
    {
      if (Directory.Exists(path))
      {
        return;
      }

      throw new DirectoryNotFoundException(String.Format("The directory with files to test \"{0}\" was not found",
                                                         path));
    }
  }

  public class When_a_job_directory_is_scanned : Spec
  {
    [Test]
    [TestCase("inputs/complete/single result/PBC-ISB-001365-A")]
    public void Should_create_a_job(string path)
    {
      var job = Job.Scan(path);
      Assert.IsInstanceOf<Job>(job);
    }

    [Test]
    [TestCase("inputs/complete/single result/PBC-ISB-001365-A")]
    public void Should_describe_to_job(string path)
    {
      var job = Job.Scan(path);
      Assert.AreEqual(
                      "Job ID PBC-ISB-001365-A based off of inputs/complete/single result/PBC-ISB-001365-A is ready for processing",
                      job.ToString());
    }
  }

  public class When_a_job_is_checked_whether_it_is_ready_for_processing : Spec
  {
    Job _job;

    protected override void Because()
    {
      _job = Job.Scan("path does not exists");
    }

    [Test]
    public void Should_not_be_ready_for_processing()
    {
      Assert.IsFalse(_job.IsReadForProcessing);
    }

    [Test]
    public void Should_report_the_error()
    {
      CollectionAssert.Contains(_job.UnmetExpectations, "Expected that the directory \"path does not exists\" exists.");
    }
  }

  class When_expectations_are_met
  {
    [Test]
    [TestCase("inputs/complete/single result/PBC-ISB-001365-A")]
    [TestCase("inputs/complete/single result with 1 track/PBC-ISB-001365-A")]
    public void Should_not_have_unmet_expectations(string path)
    {
      Ensure.DirectoryExists(path);

      var job = Job.Scan(path);
      CollectionAssert.IsEmpty(job.UnmetExpectations);
    }
  }

  class When_expectations_are_unmet
  {
    [TestCase("inputs/missing/dobbin.result.xml/PBC-ISB-001365-A")]
    [TestCase("inputs/missing/meta.xml/PBC-ISB-001365-A")]
    [TestCase("inputs/missing/mp3 from dobbin.result.xml/PBC-ISB-001365-A")]
    [TestCase("inputs/missing/mp3 from meta.xml/PBC-ISB-001365-A")]
    [TestCase("inputs/missing/mp3 from tracks/PBC-ISB-001365-A")]
    [TestCase("inputs/files for another job/some job id")]
    [TestCase("inputs/invalid/dobbin.result.xml/PBC-ISB-001365-A")]
    [TestCase("inputs/invalid/meta.xml/PBC-ISB-001365-A")]
    public void Should_report_failed_expectations(string path)
    {
      Ensure.DirectoryExists(path);

      var job = Job.Scan(path);
      CollectionAssert.IsNotEmpty(job.UnmetExpectations);
      Assert.IsNotEmpty(job.ToString());
    }
  }
}
