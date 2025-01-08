using System;
using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Date
{
    public class TestDateFunction
    {
        [Test]
        public void TestValidDateStringNoFormat()
        {
            var exp = "Date('2023-10-01')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is DateTime);
            Assert.That((DateTime)res, Is.EqualTo(new DateTime(2023, 10, 1)));
        }

        [Test]
        public void TestValidDateStringWithFormat()
        {
            var exp = "Date('01-10-2023', 'dd-MM-yyyy')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is DateTime);
            Assert.That((DateTime)res, Is.EqualTo(new DateTime(2023, 10, 1)));
        }

        [Test]
        public void TestInvalidDateString()
        {
            var exp = "Date('invalid-date')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        

        [Test]
        public void TestExceedsParameterCount()
        {
            var exp = "Date('2023-10-01', 'dd-MM-yyyy', 'extra-parameter')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestNullDateString()
        {
            var exp = "Date(null)";
            var res = Helpers.Evaluate(exp);
            Assert.IsNull(res);
        }

        [Test]
        public void TestInvalidTypeDateString()
        {
            var exp = "Date(12345)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
