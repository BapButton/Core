using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Helpers
{

    public static class EnumHelpers<T>
    {

        public static bool DoesEnumHaveDuplicates()
        {
            var enums = (T[])Enum.GetValues(typeof(T));
            List<int> enumInts = new List<int>();
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                enumInts.Add(i);
            }
            return !(enums.Count() == enumInts.Distinct().Count());
        }
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static Dictionary<int, string> ToDictionary()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type must be an enum");
            }
            Dictionary<int, string> values = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(T)).Cast<T>().ToList())
            {
                if (item != null)
                {
                    values.Add((int)(object)item, GetDisplayValue(item));
                }
            }
            return values;
        }

        public static List<T> ToList()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }



        public static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    var propValue = staticProperty.GetValue(null, null);
                    if (propValue != null)
                    {
                        System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)propValue;
                        string? name = resourceManager.GetString(resourceKey);
                        if (name != null)
                        {
                            return name;
                        }
                    }

                }
            }

            return resourceKey; // Fallback with the key name
        }



        public static string GetDisplayValue(T value)
        {
            try
            {
                if (value != null)
                {
                    var fieldInfo = value.GetType().GetField(value?.ToString() ?? "");
                    if (fieldInfo != null)
                    {
                        DisplayAttribute[]? descriptionAttributes = fieldInfo.GetCustomAttributes(
                        typeof(DisplayAttribute), false) as DisplayAttribute[];
                        if (descriptionAttributes != null)
                        {
                            if (descriptionAttributes.Length > 0 && descriptionAttributes[0].ResourceType != null)
                                return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

                            if (descriptionAttributes != null && descriptionAttributes.Length > 0)
                            {
                                if (descriptionAttributes[0].Name != null)
                                {
                                    return descriptionAttributes[0].Name;
                                }
                                else
                                {
                                    return value?.ToString() ?? "";
                                }
                            }

                        }


                    }

                }
            }

            catch (Exception)
            {
                return value?.ToString() ?? "";
            }
            return value?.ToString() ?? "";
        }
    }
}

