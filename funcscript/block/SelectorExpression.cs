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
        public override (object,CodeLocation) Evaluate(IFsDataProvider provider,List<Action> connectionActions)
        {
            var (sourceVal,_) = Source.Evaluate(provider,connectionActions);
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
                        Provider = provider,
                        SourceVal = l
                    };
                    ret[i] = Selector.Evaluate(sel,connectionActions,true).Item1;
                    i++;
                }
                return (new ArrayFsList(ret),this.CodeLocation);
            
            }
            else
            {
                return Selector.Evaluate(new SelectorProvider
                {
                    Parent = this,
                    Provider = provider,
                    SourceVal=sourceVal
                },connectionActions,false);
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
        public override string AsExpString(IFsDataProvider provider)
        {
            return $"{Source.AsExpString(provider)} {Selector.AsExpString(provider)}";
        }
    }
}
