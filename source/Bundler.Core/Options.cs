using System.Configuration;

namespace Bundler.Core
{
  public class Options
  {
    public string Dobbin { get; set; }
    public string Filter { get; set; }
    public string Workflow { get; set; }
    public string Sips { get; set; }
    public string Staging { get; set; }
    public string Archive { get; set; }

    public static Options FromAppSettings()
    {
      return new Options
             {
               Dobbin = ConfigurationManager.AppSettings["Dobbin"],
               Filter = ConfigurationManager.AppSettings["Filter"],
               Workflow = ConfigurationManager.AppSettings["Workflow"],
               Sips = ConfigurationManager.AppSettings["SIPS"],
               Staging = ConfigurationManager.AppSettings["Staging"],
               Archive = ConfigurationManager.AppSettings["Archive"]
             };
    }
  }
}