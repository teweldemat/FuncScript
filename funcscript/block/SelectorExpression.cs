﻿using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Block
{
    internal class SelectorExpression : ExpressionBlock
    {
        public ExpressionBlock Source;
        public KvcExpression Selector;

        public override object Evaluate()
        {
            var sourceVal = Source.Evaluate();
            if (sourceVal is FsList lst)
            {
                var ret = new object[lst.Length];
                int i = 0;

                foreach (var l in lst)
                {
                    if (l is KeyValueCollection e)
                    {
                        var clone = Selector.CloneExpression();
                        clone.SetReferenceProvider(e);
                        ret[i] = clone.Evaluate();
                    }
                    else
                        ret[i] = null;
                    i++;
                }
                return new ArrayFsList(ret);
            }

            if (sourceVal is KeyValueCollection kvcSource)
            {
                Selector.SetReferenceProvider(kvcSource);
                return Selector.Evaluate();
            }

            return null;
        }

        public override IList<ExpressionBlock> GetChilds()
        {
            return new ExpressionBlock[] { Source, Selector };
        }
        public override string ToString()
        {
            return "selector";
        }
        public override string AsExpString()
        {
            return $"{Source.AsExpString()} {Selector.AsExpString()}";
        }

        public override void SetReferenceProvider(KeyValueCollection provider)
        {
            Source.SetReferenceProvider(provider);
        }
        public override ExpressionBlock CloneExpression()
        {
            return new SelectorExpression
            {
                CodePos = this.CodePos,
                CodeLength = this.CodeLength,
                Source = this.Source.CloneExpression(),
                Selector = this.Selector.CloneExpression() as KvcExpression,
            };
        }
    }
}
