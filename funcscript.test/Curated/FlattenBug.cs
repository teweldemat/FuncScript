using System.Linq;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Curated;

public class FlattenBug
{
    [Test]
    public void TestFlattingBug()
    {
        var data = System.IO.File.ReadAllText("data/list.txt");
            
        var g = new DefaultFsDataProvider();
        var res = FuncScript.Evaluate(g,$@"
{{
    data:{data},
    flatten:(d)=>{{
  f:(l)=>reduce(l, (x,m)=>m+if(type(x)=""List"",f(x),x),[]);
  return f(d)
}},
    return Flatten(data);    
}}
");
        Assert.That(res is FsList);
        var list = (FsList)res;
        Assert.That(list.Length>10);
    }
}
