using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestSortListFunction
    {
        [Test]
        public void TestSortValidList()
        {
            var exp = "Sort([3,1,2],(x,y)=> if(x<y,-1,if(x>y,1,0)))";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is ArrayFsList);
            var list = (ArrayFsList)res;
            Assert.That(list, Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void TestSortWithInvalidFunctionReturnType()
        {
            var exp = "Sort([3,1,2],(x,y)=> 'invalid')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSortWithInvalidParameterCount()
        {
            var exp = "Sort([3,1,2])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSortWithInvalidFirstParameter()
        {
            var exp = "Sort('not_a_list',(x,y)=> if(x<y,-1,if(x>y,1,0)))";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSortWithInvalidSecondParameter()
        {
            var exp = "Sort([3,1,2],'not_a_function')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
