using System;
using System.IO;

namespace Bundler.Expectations
{
  class FileExistsExpectation : IExpectation
  {
    readonly string _path;

    public FileExistsExpectation(string path)
    {
      _path = path;
    }

    public string GetMessage()
    {
      return String.Format("Expected that the file \"{0}\" exists.", _path);
    }

    public bool Verify()
    {
      return File.Exists(_path);
    }
  }
}
