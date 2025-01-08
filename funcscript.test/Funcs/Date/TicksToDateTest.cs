using System;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Date
{
    public class TestTicksToDateFunction
    {
        [Test]
        public void TestTicksToDate_ValidTicks()
        {
            var exp = "TicksToDate(637701984000000000)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(new DateTime(2021, 10, 19)));
        }

        [Test]
        public void TestTicksToDate_NullInput()
        {
            var exp = "TicksToDate(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestTicksToDate_InvalidTypeInput()
        {
            var exp = "TicksToDate('invalid')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestTicksToDate_TooManyParameters()
        {
            var exp = "TicksToDate(1, 2)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
