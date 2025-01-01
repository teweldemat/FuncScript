using funcscript.core;
using funcscript.model;

namespace funcscript.block
{
    public class ReferenceBlock : ExpressionBlock
    {
        string _name, _nameLower;
        private bool _fromParent;
        private KeyValueCollection _context = null;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == null)
                {
                    _name = _nameLower = null;
                }
                else
                {
                    _name = value;
                    _nameLower = _name.ToLower();
                }
            }
        }
        public ReferenceBlock(string name)
        {
            Name = name;
            _fromParent = false;
        }
        public ReferenceBlock(string name, string nameLower)
        {
            Name = name;
            _nameLower = nameLower;
            _fromParent = false;
        }
        public ReferenceBlock(string name, string nameLower,bool fromParent)
        {
            Name = name;
            _nameLower = nameLower;
            _fromParent =fromParent;
        }
        public override object Evaluate()
        {
            if (_fromParent)
                return _context.ParentContext?.Get(_nameLower);
            return _context.Get(_nameLower);
        }

        public override IList<ExpressionBlock> GetChilds()
        {
            return new ExpressionBlock[0];
        }
        public override string ToString()
        {
            return Name;
        }

        public override string AsExpString()
        {
            return Name;
        }
        public override void SetReferenceProvider(KeyValueCollection provider)
        {
            _context = provider;
        }
        public override ExpressionBlock CloneExpression()
        {
            return new ReferenceBlock(_name,_nameLower)
            {
                _fromParent = this._fromParent,
                CodePos = this.CodePos,
                CodeLength = this.CodeLength,
            };
        }
    }

}
