using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Tests.Models
{
    using System.Reflection;
    using TechTalk.SpecFlow;

    public static class ModelEx
    {
        public static bool EqualsIgnoreCase(this string subj, string obj)
        {
            if (subj == null && obj == null)
                return true;
            if (subj != null && obj != null)
            {
                return subj.Equals(obj, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        public static List<T> ToModelList<T>(this Table table) where T : class, new()
        {
            var studentGrades = new List<T>();
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite).ToList();
            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            if (table.RowCount > 0)
            {
                var firstRow = table.Rows[0];
                int idx = 0;
                foreach (var colName in firstRow.Keys)
                {
                    columns.Add(colName, idx++);
                }
            }
            foreach (var row in table.Rows)
            {
                T instance = Activator.CreateInstance<T>();
                foreach (var prop in props)
                {
                    if (columns.ContainsKey(prop.Name))
                    {
                        object value = row[columns[prop.Name]];
                        if (prop.PropertyType != value.GetType())
                        {
                            value = Convert.ChangeType(value, prop.PropertyType);
                        }
                        prop.SetValue(instance, value, null);
                    }
                }
                studentGrades.Add(instance);
            }
            return studentGrades;
        }
    }
}
