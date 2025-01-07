using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test;

public class KvcMerge
{
    [Test]
    public void KvcMergeHeriarchy()
    {
        var g = new DefaultFsDataProvider();
        var res = FuncScript.Evaluate(g, @"{a:12,b:{c:10,z:10}}+{d:13,b:{c:12,x:5}}");
        var expected = new ObjectKvc(new { a = 12, b = new { c = 12, z = 10, x = 5 }, d = 13 });

        Assert.AreEqual(FuncScript.FormatToJson(expected), FuncScript.FormatToJson(res));
    }

    [Test]
    public void TestKvcMergeDifferentParents()
    {
        var exp =
            @"{
  a:{
      aa:2;
      ab:3;
    };
  b:{
    c:{
      ca:6;
      cb:7;
    }
  };

  return a+b.c;
}";
        Assert.Throws<EvaluationException>(() =>
        {
            var res = FuncScript.Evaluate(exp);
        });
    }
}
