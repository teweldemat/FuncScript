using NUnit.Framework;

namespace FuncScript.Test.Curated;

public class DotNetExperiment
{
    [Test]
    public void AreTwoNullsEqual()
    {
        Assert.That(null == null);
    }
}
