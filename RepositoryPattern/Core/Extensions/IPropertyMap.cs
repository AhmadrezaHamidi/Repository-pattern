using System.Reflection;

namespace Core.Extensions
{
    public interface IPropertyMap
    {
        void SetColumnName(string columnName);

        PropertyInfo Type();

        string GetColumnName();
    }
}
