using System;
using System.Linq;

namespace ULALA.Domain.Contracts.Utility
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeKindAttribute : Attribute
    {
        private readonly DateTimeKind m_kind;

        public DateTimeKindAttribute(DateTimeKind kind)
        {
            m_kind = kind;
        }

        public DateTimeKind Kind
        {
            get { return m_kind; }
        }

        public static void Apply(object entity)
        {
            if (entity == null)
                return;

            var properties = entity.GetType().GetProperties().Where(x => x.PropertyType == typeof(DateTime)
                                || x.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                var attr = (DateTimeKindAttribute)(property.GetCustomAttributes(typeof(DateTimeKindAttribute), false).FirstOrDefault());
                if (attr == null)
                    continue;

                var dt = property.PropertyType == typeof(DateTime?) ? (DateTime?)property.GetValue(entity, null) : (DateTime)property.GetValue(entity, null);
                if (dt == null)
                    continue;

                property.SetValue(entity, DateTime.SpecifyKind(dt.Value, attr.Kind), null);
            }
        }
    }
}
