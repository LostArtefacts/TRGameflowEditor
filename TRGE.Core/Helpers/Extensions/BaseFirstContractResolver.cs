using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TRGE.Core
{
    public class BaseFirstContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                ?.OrderBy(p => p.DeclaringType.BaseTypesAndSelf().Count()).ToList();
        }
    }

    public static class TypeExtensions
    {
        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}