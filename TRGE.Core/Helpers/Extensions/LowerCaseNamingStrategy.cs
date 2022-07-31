using Newtonsoft.Json.Serialization;

namespace TRGE.Core
{
    internal class LowerCaseNamingStrategy : DefaultNamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return base.ResolvePropertyName(name).ToLower();
        }
    }
}