using System;
using System.IO;

namespace Bundler.Core.Expectations
{

  class FailExpectation : IExpectation
  {
    readonly string _message;
    readonly object[] _args;

    public FailExpectation(string message, params object[] args)
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
}
