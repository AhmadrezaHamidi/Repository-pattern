namespace Core.Extensions
{
    public interface IEntityMap
    {
        void SetTableName(string tableName);

        Type Type();

        string GetTableName();

        string Name();
    }
}
