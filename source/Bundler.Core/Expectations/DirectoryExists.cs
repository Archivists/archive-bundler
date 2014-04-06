using System;
using System.IO;

namespace Bundler.Core.Expectations
{
  
  class DirectoryExistsExpectation : IExpectation
  {
    readonly string _path;

    public DirectoryExistsExpectation(string path)
    {
      _path = path;
    }

    public string GetMessage()
    {
      return String.Format("Expected that directory \"{0}\" exists.", _path);
    }

    public bool Verify()
    {
      return Directory.Exists(_path);
    }
  }
}
