using System;
using System.IO;

namespace Bundler.Core.Expectations
{
  interface IExpectation
  {
    bool Verify();
    string GetMessage();
  }

  class Fail : IExpectation
  {
    readonly string _message;
    readonly object[] _args;

    public Fail(string message, params object[] args)
    {
      _message = message;
      _args = args;
    }

    public string GetMessage()
    {
      return String.Format(_message, _args);
    }

    public bool Verify()
    {
      return false;
    }
  }

  class FileExists : IExpectation
  {
    readonly string _path;

    public FileExists(string path)
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

  class DirectoryExists : IExpectation
  {
    readonly string _path;

    public DirectoryExists(string path)
    {
      _path = path;
    }

    public string GetMessage()
    {
      return String.Format("Expected that the directory \"{0}\" exists.", _path);
    }

    public bool Verify()
    {
      return Directory.Exists(_path);
    }
  }

  class Success : IExpectation
  {
      public string GetMessage()
      {
          return null;
      }

      public bool Verify()
      {
          return true;
      }
  }
}
