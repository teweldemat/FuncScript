using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncScript.Error
{
    public class UnsupportedUnderlyingType : Exception
    {
        public UnsupportedUnderlyingType(string message) : base(message)
        {
        }
    }
}
