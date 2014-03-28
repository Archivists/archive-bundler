using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Bundler.Core
{
  interface IExpectation
  {
    bool Verify();
    string GetMessage();
  }

  class LoadsAsXml : IExpectation
  {
    readonly string _path;

    public LoadsAsXml(string path)
    {
      _path = path;
    }

    public string GetMessage()
    {
      return String.Format("Expected that {0} is valid XML", _path);
    }

    public bool Verify()
    {
      try
      {
        Trace.Assert(new XPathDocument(_path) != null);
        return true;
      }
      catch (XmlException)
      {
        return false;
      }
    }
  }

  class HasElements : IExpectation
  {
    readonly IEnumerable _collection;
    readonly string _description;

    public HasElements(IEnumerable collection, string description)
    {
      _collection = collection;
      _description = description;
    }

    public string GetMessage()
    {
      return String.Format("Expected that {0}", _description);
    }

    public bool Verify()
    {
      return _collection.OfType<object>().Any();
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
      return String.Format("Expected that the file \"{0}\" exists", _path);
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
      return String.Format("Expected that the directory \"{0}\" exists", _path);
    }

    public bool Verify()
    {
      return Directory.Exists(_path);
    }
  }
}
