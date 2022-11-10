using System;
using System.Collections.Generic;

namespace StormWarfare.Core
{

    public class JsonModel
    {

        private static Dictionary<Type, Type> _TypeMapping = new Dictionary<Type, Type>();
        public static void MapType(Type src, Type dest) => _TypeMapping[src] = dest;

        public virtual void Deserialize(object json) { }
        public virtual object Serialize()
        {
            throw new Exception("Unimplemented");
        }
        public static T Deserialize<T>(object json)
        {
            if (json == null) return default(T);
            if (json is System.Collections.IList jsonList && typeof(System.Collections.IList).IsAssignableFrom(typeof(T)))
            {
                var genericList = (T)Activator.CreateInstance(typeof(T));
                var list = genericList as System.Collections.IList;
                var subType = typeof(T).GetGenericArguments()[0];
                foreach (var jsonItem in jsonList)
                {
                    if (typeof(JsonModel).IsAssignableFrom(subType))
                    {
                        var model = Activator.CreateInstance(subType);
                        ((JsonModel)model).Deserialize(jsonItem);
                        list.Add(model);
                    }
                    else
                    if (typeof(System.Collections.IList).IsAssignableFrom(subType))
                    {
                        list.Add(Deserialize<System.Collections.IList>(jsonItem));
                    }
                    else
                        list.Add(Deserialize<object>(jsonItem));
                }
                return genericList;
            }

            if (typeof(JsonModel).IsAssignableFrom(typeof(T)))
            {
                Type targetType;
                if (!_TypeMapping.TryGetValue(typeof(T), out targetType)) targetType = typeof(T);
                JsonModel model = (JsonModel)Activator.CreateInstance(targetType);
                model.Deserialize(json);
                return (T)(object)model;
            }

            if (typeof(T).IsEnum)
            {
                return (T)Enum.ToObject(typeof(T), json);
            }
            return default(T);
        }

        /// <summary>
        /// Icerisine verilen modeli serialize edip json objesi halinde doner.
        /// Su anda sadece tek model destekliyor List<object> destegi eklenicek.
        /// </summary>
        /// <param name="model">Serialize edilicek model</param>
        /// <returns></returns>
        public static object Serialize(object model)
        {
            if (model is null) return default(object);

            if (model is JsonModel)
            {
                return ((JsonModel)model).Serialize();
            }

            return default(object);
        }
    }

    public class JsonModelParseException : Exception
    {
        public JsonModelParseException()
	    : base()
        {
        }

        public JsonModelParseException(String message)
          : base(message)
        {
        }

        public JsonModelParseException(String message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}