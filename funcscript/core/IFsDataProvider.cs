namespace funcscript.core
{
    public interface IFsDataProvider        
    {
        object Get(String name);
        public IFsDataProvider ParentContext { get; }
        bool IsDefined(string key);
    }
}
