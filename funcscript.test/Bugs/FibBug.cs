using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Bugs;

public class FibBug
{
    [Test]
    public void FibBugTest()
    {
        var exp = @"{
  nums: series(1, 5);
  cubedObject: (nums map (x) => {x: x, cube: x * x * x});
  return cubedObject;
}";
        var res=Helpers.Evaluate(exp);
        Assert.That(res,Is.AssignableTo<FsList>());
        var json = Helpers.FormatToJson(res);
    }
}