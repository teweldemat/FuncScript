﻿using funcscript.core;
using funcscript.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace funcscript.block
{
    internal class SelectorExpression: ExpressionBlock
    {
        class SelectorProvider: IFsDataProvider
        {
            public IFsDataProvider Provider;
            public SelectorExpression Parent;
            public IFsDataProvider ParentProvider => Provider;

            public object SourceVal
            {
                set
                {
                    _sourceVal = value as KeyValueCollection;
                }
            }
            KeyValueCollection _sourceVal;
            public object Get(string name)
            {
                if (_sourceVal != null)
                {
                    if (_sourceVal.IsDefined(name))
                        return _sourceVal.Get(name);
                }
                return Provider.Get(name);
            }
            public bool IsDefined(string key)
            {
                if (_sourceVal != null)
                {
                    if (_sourceVal.IsDefined(key))
                        return true;
                }

                return Provider.IsDefined(key);
            }

        }
        public ExpressionBlock Source;
        public KvcExpression Selector;
        public override object Evaluate()
        {
            var sourceVal = Source.Evaluate();
            if (sourceVal is FsList)
            {
                var lst = (FsList)sourceVal;
                var ret = new object[lst.Length];
                int i = 0;
                
                foreach (var l in lst)
                {
                    var sel=new SelectorProvider
                    {
                        Parent = this,
                        SourceVal = l
                    };
                    sel.Provider = sel;
                    ret[i] = Selector.Evaluate();
                    i++;
                }
                return (new ArrayFsList(ret),this.CodeLocation);
            
            }
            else
            {
                Selector.Provider = new SelectorProvider
                {
                    Parent = this,
                    SourceVal = sourceVal
                };
                return Selector.Evaluate();
            }
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
    }
}
