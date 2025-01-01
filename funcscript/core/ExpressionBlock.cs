﻿using funcscript.error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using funcscript.model;

namespace funcscript.core
{
    /// <summary>
    /// Represents is FuncScript expression block
    /// </summary>
    public abstract class ExpressionBlock
    {
        public int CodePos;
        public int CodeLength;
        public abstract void SetReferenceProvider(KeyValueCollection provider);
        public abstract object Evaluate();
        public abstract IList<ExpressionBlock> GetChilds();
        public abstract string AsExpString();
        public abstract ExpressionBlock CloneExpression();
    }
}
