using System;
using System.IO;

namespace Bundler.Expectations
{

  class SuccessExpectation : IExpectation
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
