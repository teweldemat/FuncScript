using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;
using System.Text;

namespace FuncScript.Test.Funcs.Text
{
    public class TestTemplateMergeFunction
    {
        [Test]
        public void TestMergeSimpleStrings()
        {
            var exp = "_templatemerge(['Hello', ' ', 'World'])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestMergeWithLists()
        {
            var exp = "_templatemerge(['Start', ['Middle1', 'Middle2'], ' End'])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("StartMiddle1Middle2 End"));
        }

        [Test]
        public void TestMergeWithNulls()
        {
            var exp = "_templatemerge(['A', null, 'B', null, 'C'])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("ABC"));
        }

        [Test]
        public void TestMergeWithEmptyList()
        {
            var exp = "_templatemerge([])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(""));
        }

        [Test]
        public void TestMergeNullParameter()
        {
            var exp = "_templatemerge(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(""));
        }
    }
}
