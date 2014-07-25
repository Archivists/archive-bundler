using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Logging;

namespace Bundler
{
  public class Mover
  {
    const string Sides = "sides";
    const string Pattern = "*.wav*";
    private Options _options;
    private Job _job;
    static readonly LogWriter _log = HostLogger.Get<Mover>();


    public Mover(Options options, Job job)
    {
      _options = options;
      _job = job;
    }

    protected string ArchiveId
    {
      get
      {
        return _job.Id.Substring(0, 14);
      }
    }

    public bool PrepareBundles()
    {

      _log.Info(String.Format("Preparing bundles for Archive ID {0}", ArchiveId));

      var result = true;
      result = CopyWorkflow();
      if (result)
        result = PrepareForArchive();
      if(result)
        result = PrepareSIP();

      if (result)
        _log.Info(String.Format("Successfully processed Dobbin job {0} for Archive ID {1}", _job.Id, ArchiveId));
      else
        _log.Error(String.Format("There was a problem processing Dobbin job {0}", _job.Id));
      return result;
    }

    private bool CopyWorkflow()
    {
      try
      {
        var source = Path.Combine(_options.Workflow, ArchiveId);
        var destination = Path.Combine(_options.Staging, ArchiveId);
        DirectoryCopy(source, destination, true, true);
        return true;
      }
      catch (Exception e)
      {
        _log.Error(String.Format("An error occured in Mover.CopyWorkflow: {0}", e.Message));
        return false;
      }
    }

    private bool PrepareForArchive()
    {
      try
      {
        var destination = Path.Combine(_options.Staging, ArchiveId, Sides, _job.Id);
        var source = Path.Combine(_options.Dobbin, _job.Id);
        DirectoryCopy(source, destination, true, true);
        Directory.Delete(source, true);
        return true;
      }
      catch (Exception e)
      {
        _log.Error(String.Format("An error occured in Mover.PrepareForArchive: {0}", e.Message));
        return false;
      }
    }

    private bool PrepareSIP()
    {
      try
      {
        var source = Path.Combine(_options.Staging, ArchiveId);
        var destination = Path.Combine(_options.Sips, ArchiveId);
        
        DirectoryCopy(source, destination, true, true);

        var wavFiles = Directory.GetFiles(destination, _job.Id + Pattern, SearchOption.AllDirectories);
        if (wavFiles.Any())
        {
          foreach(var file in wavFiles)
          {
            File.Delete(file);
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _log.Error(String.Format("An error occured in Mover.PrepareSIP: {0}", e.Message));
        return false;
      }
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
    {
      // Get the subdirectories for the specified directory.
      DirectoryInfo dir = new DirectoryInfo(sourceDirName);
      DirectoryInfo[] dirs = dir.GetDirectories();

      if (!dir.Exists)
      {
        throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + sourceDirName);
      }

      // If the destination directory doesn't exist, create it. 
      if (!Directory.Exists(destDirName))
      {
        Directory.CreateDirectory(destDirName);
      }

      // Get the files in the directory and copy them to the new location.
      FileInfo[] files = dir.GetFiles();
      foreach (FileInfo file in files)
      {
        string temppath = Path.Combine(destDirName, file.Name);
        file.CopyTo(temppath, overwrite);
      }

      // If copying subdirectories, copy them and their contents to new location. 
      if (copySubDirs)
      {
        foreach (DirectoryInfo subdir in dirs)
        {
          string temppath = Path.Combine(destDirName, subdir.Name);
          DirectoryCopy(subdir.FullName, temppath, copySubDirs, overwrite);
        }
      }
    }
  }
}
