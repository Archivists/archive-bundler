using System;
using System.IO;

namespace Bundler.Core.Expectations
{
  interface IExpectation
  {
    bool Verify();
    
    string GetMessage();
  }
}
