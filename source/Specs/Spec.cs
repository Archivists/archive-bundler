using NUnit.Framework;

namespace Specs
{
  [TestFixture]
  public abstract class Spec
  {
    protected virtual void Establish()
    {
    }

    protected virtual void Because()
    {
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      Establish();
      Because();
    }
  }
}