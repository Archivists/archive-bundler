using System;
using System.IO;

namespace Bundler.Expectations
{
  interface IExpectation
  {
    bool Verify();
    
    string GetMessage();
  }
}
