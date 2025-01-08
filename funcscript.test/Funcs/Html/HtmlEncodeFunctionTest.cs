using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Html
{
    public class TestHtmlEncodeFunction
    {
        [Test]
        public void TestHtmlEncodeValidString()
        {
            var exp = "HEncode('<div>Hello World!</div>')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("&lt;div&gt;Hello World!&lt;/div&gt;"));
        }

        [Test]
        public void TestHtmlEncodeEmptyString()
        {
            var exp = "HEncode('')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(""));
        }

        [Test]
        public void TestHtmlEncodeNullString()
        {
            var exp = "HEncode(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestHtmlEncodeParameterCountMismatch()
        {
            var exp = "HEncode('first', 'second')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
