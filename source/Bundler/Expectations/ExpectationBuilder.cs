using System.IO;
using System.Xml.XPath;

namespace Bundler.Expectations
{
  static class ExpectationBuilder
  {
    internal static object Extract(this string file, string xpath)
    {
      return new XPathDocument(file)
        .CreateNavigator()
        .Evaluate(xpath);
    }

    internal static string AbsolutePath(this object file, string relativeTo)
    {
      var f = file.ToString();
      if (Path.IsPathRooted(f))
      {
        return f;
      }

      return Path.Combine(Path.GetDirectoryName(relativeTo), f);
    }
  }
}
