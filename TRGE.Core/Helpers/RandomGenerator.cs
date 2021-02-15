using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public class RandomGenerator
    {
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public enum Type
        {
            UnixTime = 0,
            Date = 1,
            Custom = 2
        }

        public Type RNGType { get; internal set; }

        internal int CustomValue { get; private set; }

        public RandomGenerator(int value)
        {
            RNGType = Type.Custom;
            CustomValue = value;
        }

        public RandomGenerator(Type type)
        {
            RNGType = type;
            CustomValue = -1;
        }

        internal RandomGenerator(Dictionary<string, object> json)
        {
            RNGType = (Type)Enum.ToObject(typeof(Organisation), json["Type"]);
            CustomValue = int.Parse(json["Custom"].ToString());
        }

        public int Value
        {
            get
            {
                switch (RNGType)
                {
                    case Type.UnixTime:
                    default:
                        return Convert.ToInt32((DateTime.Now.ToUniversalTime() - UNIX_EPOCH).TotalSeconds);
                    case Type.Date:
                        return int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    case Type.Custom:
                        return CustomValue;
                }
            }
            set
            {
                CustomValue = value;
            }
        }

        internal Random Create()
        {
            return new Random(Value);
        }

        internal object ToJson()
        {
            return new Dictionary<string, object>
            {
                { "Type", RNGType },
                { "Current", Value },
                { "Custom", Value }
            };
        }
    }
}