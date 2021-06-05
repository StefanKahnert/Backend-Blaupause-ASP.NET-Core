using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Blaupause_Unit_Test.Helper
{
    public static class PrivateValueSetter
    {
        public static void SetPropertyValue(object target, string memberName, object newValue)
        {
            PropertyInfo prop = GetPropertyReference(target.GetType(), memberName);
            prop.SetValue(target, newValue, null);
        }

        private static PropertyInfo GetPropertyReference(Type targetType, string memberName)
        {
            PropertyInfo propInfo = targetType.GetProperty(memberName,
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Instance);

            if (propInfo == null && targetType.BaseType != null)
            {

                return GetPropertyReference(targetType.BaseType, memberName);

            }
            return propInfo;
        }

        public static void SetFieldValue(object target, string fieldName, object newValue)
        {
            FieldInfo field = GetFieldReference(target.GetType(), fieldName);
            field.SetValue(target, newValue);
        }

        private static FieldInfo GetFieldReference(Type targetType, string fieldName)
        {
            FieldInfo field = targetType.GetField(fieldName,
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Instance);

            if (field == null && targetType.BaseType != null)
            {
                return GetFieldReference(targetType.BaseType, fieldName);

            }
            return field;
        }
    }
}
