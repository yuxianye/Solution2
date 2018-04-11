using OSharp.Core.Dependency;


namespace Solution.Core.Contracts
{
    public interface ITestContract : IScopeDependency
    {
        void Test();
    }
}
