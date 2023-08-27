using Core.Extensions;
using Dapper;
using Dapper.FastCrud;
using DataAccess.Dapper.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Dapper
{
    public class DataService<T> : IDataService<T> where T : class
    {
        private readonly IDbConnection _connection;
        private string _tableName;
        private string _rowTableName;
        private string _allColumns = "*";
        private string _schema = "dbo";

        public DataService(IDbConnection connection)
        {
            _connection = connection;

            //Get name of table from object type
            var tblName = GetTableName<T>();
            var temp = tblName.Split('.');

            if (temp.Length > 1)
            {
                _rowTableName = temp[temp.Length - 1];
                _tableName = $"[{_schema}].[{_rowTableName}]";
            }
            else
            {
                var shcema = GetSchema<T>();
                if (!string.IsNullOrEmpty(shcema))
                    _schema = shcema;

                _rowTableName = tblName;
                _tableName = $"[{_schema}].[{_rowTableName}]";
            }

            _allColumns = GetAllColumns<T>();
        }

        private string GetTableName<Tbl>()
        {
            if (typeof(Tbl).GetCustomAttributes(typeof(TableAttribute), true)
                .SingleOrDefault() is TableAttribute table)
            {
                return table.Name;
            }
            var typeName = typeof(Tbl).ToString();
            var i = typeName.IndexOf('`');
            if (i >= 0)
            {
                typeName = typeName.Substring(0, i);
            }
            return typeName;
        }

        private string GetSchema<Tbl>()
        {
            if (typeof(Tbl).GetCustomAttributes(typeof(TableAttribute), true)
                .SingleOrDefault() is TableAttribute table)
            {
                if (table.Schema != null)
                    return table.Schema;
                else
                    return string.Empty;
            }

            return string.Empty;
        }

        private string GetAllColumns<Tbl>()
        {
            var columns = new List<string>();
            var type = typeof(Tbl);
            var properties = type.GetProperties().ToList();

            foreach (var propertie in properties)
            {
                var columnAttribute = propertie.GetCustomAttribute<ColumnAttribute>();
                var notMappedAttribute = propertie.GetCustomAttribute<NotMappedAttribute>();

                if (notMappedAttribute is not null) continue;

                if (columnAttribute != null)
                {
                    if (columnAttribute.Name != null)
                        columns.Add($"{columnAttribute.Name} AS {propertie.Name}");

                    else
                        columns.Add(propertie.Name);
                }
                else
                    columns.Add(propertie.Name);
            }

            var result = string.Join(", ", columns.ToArray());
            return result;
        }

        public string Schema
        {
            get => _schema;
            set
            {
                _schema = $"{value.Replace("[", "").Replace("]", "")}";
                _tableName = $"[{_schema}].{_rowTableName}";
            }
        }

        public string TableName { get; } = "";

        public IEnumerable<T> Find(string predicate, object data, bool distinct)
        {
            return Find(predicate, data, _allColumns, distinct);
        }

        public async Task<IEnumerable<T>> FindAsync(string predicate, object data, bool distinct)
        {
            return await FindAsync(predicate, data, _allColumns, distinct);
        }

        /// <summary>
        /// Find with predicate
        /// /// </summary>
        /// <param name="predicate">predicate format 'Id=@Id and Name=@Name'</param>
        /// <param name="data">data for find with</param>
        /// <param name="filterColumns">result column filter  format 'Id , Name, ...' for all column can emty this</param>
        /// <returns></returns>
        public IEnumerable<T> Find(string predicate, object data, string filterColumns, bool distinct)
        {
            var d = distinct ? "distinct" : "";
            var sql = $"SELECT {d} {filterColumns} FROM {_tableName} WHERE {predicate}";
            var result = _connection.Query<T>(sql, data);
            return result;
        }

        public async Task<IEnumerable<T>> FindAsync(string predicate, object data, string filterColumns, bool distinct)
        {
            var d = distinct ? "distinct" : "";
            var sql = $"SELECT {d} {filterColumns} FROM {_tableName} WHERE {predicate}";
            var result = await _connection.QueryAsync<T>(sql, data);
            return result;
        }

        public IEnumerable<T> FindTop(int count, string predicate, object data, string filterColumns = "*", bool distinct = false)
        {
            IEnumerable<T> result;
            var d = distinct ? "distinct" : "";

            if (filterColumns == "*")
                result = _connection.Query<T>($"SELECT TOP({count}) {d} {_allColumns} FROM {_tableName} WHERE {predicate}", data);
            else
                result = _connection.Query<T>($"SELECT TOP({count}) {d} {filterColumns} FROM {_tableName} WHERE {predicate}", data);

            return result;
        }

        public T First()
        {
            return First(_allColumns);
        }

        public T First(string filterColumns)
        {
            T result = _connection.QueryFirst<T>($"SELECT {filterColumns} FROM {_tableName}");
            return result;
        }

        public T First(string predicate, object data)
        {
            return First(predicate, data, _allColumns);
        }

        public T First(string predicate, object data, string filterColumns)
        {
            var result = _connection.QueryFirst<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public T FirstOrDefault()
        {
            return FirstOrDefault(_allColumns);
        }

        public T FirstOrDefault(string filterColumns)
        {
            var result = _connection.QueryFirstOrDefault<T>($"SELECT {filterColumns} FROM {_tableName}");
            return result;
        }

        public T FirstOrDefault(string predicate, object data)
        {
            return FirstOrDefault(predicate, data, _allColumns);
        }

        public T FirstOrDefault(string predicate, object data, string filterColumns)
        {
            var sql = $"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}";
            var result = _connection.QueryFirstOrDefault<T>(sql, data);
            return result;
        }

        public IEnumerable<T> GetAll()
        {
            return GetAll(_allColumns);
        }

        public IEnumerable<T> GetAll(string filterColumns)
        {
            var result = _connection.Query<T>($"SELECT {filterColumns} FROM {_tableName} ");
            return result;
        }

        public T GetEntity(string predicate, object data)
        {
            return GetEntity(predicate, data, _allColumns);
        }

        public T GetEntity(string predicate, object data, string filterColumns)
        {
            return Single(predicate, data);
        }

        public PagedData<T> GetPagedData(int pageIndex)
        {
            return GetPagedData(pageIndex, _allColumns);
        }

        public PagedData<T> GetPagedData(int pageIndex, string filterColumns)
        {
            return GetPagedData(20, pageIndex);
        }

        public PagedData<T> GetPagedData(int pageSize, int pageIndex)
        {
            return GetPagedData(pageSize, pageIndex, _allColumns);
        }

        public PagedData<T> GetPagedData(int pageSize, int pageIndex, string filterColumns)
        {
            var orderBy = $"{_tableName}Id";
            orderBy = orderBy.Replace("[dbo].[", "");
            orderBy = orderBy.Replace("]", "");
            var itemsCount = _connection.QuerySingle<int>($"SELECT COUNT({orderBy}) FROM {_tableName} ");
            var result = _connection.Query<T>($"SELECT  {filterColumns} FROM {_tableName}  Order By {orderBy} OFFSET {pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY").ToList();
            var pagedData = new PagedData<T>(itemsCount, pageSize, pageIndex, result);
            return pagedData;
        }

        public PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, object data)
        {
            return GetPagedData(pageSize, pageIndex, predicate, _allColumns, data);
        }

        public PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, string filterColumns, object data)
        {
            var orderBy = $"{_tableName}Id";
            return GetPagedData(pageSize, pageIndex, predicate, data, orderBy);
        }

        public PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, object data, string orderBy)
        {
            return GetPagedData(pageSize, pageIndex, predicate, data, orderBy, _allColumns);
        }

        public PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, object data, string orderBy, string filterColumns)
        {
            return GetPagedData(pageIndex, pageSize, predicate, data, true, orderBy);
        }

        public PagedData<T> GetPagedData(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy)
        {
            return GetPagedData(pageIndex, pageSize, predicate, data, isAscendingOrder, orderBy, _allColumns);
        }

        public PagedData<T> GetPagedData(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy, string filterColumns)
        {
            var sortType = "Desc";
            if (isAscendingOrder)
                sortType = "Asc";

            var itemsCount = _connection.QuerySingle<int>($"SELECT COUNT({orderBy}) FROM {_tableName} WHERE {predicate}", data);
            var result = _connection.Query<T>($"SELECT  {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {sortType} OFFSET {pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY", data).AsList();

            var pagedData = new PagedData<T>(itemsCount, pageSize, pageIndex, result);
            return pagedData;
        }

        public IEnumerable<T> GetTop(int count)
        {
            return GetTop(count, _allColumns);
        }

        public IEnumerable<T> GetTop(int count, string filterColumns)
        {
            var result = _connection.Query<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} ");
            return result;
        }

        public IEnumerable<T> GetTop(int count, string predicate, object data)
        {
            return GetTop(count, predicate, data, _allColumns);
        }

        public IEnumerable<T> GetTop(int count, string predicate, object data, string filterColumns)
        {
            var result = _connection.Query<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} WHERE {predicate} ", data);
            return result;
        }

        public bool HasEntity(string predicate, object data)
        {
            return HasEntity(predicate, data, _allColumns);
        }

        public bool HasEntity(string predicate, object data, string filterColumns)
        {
            var result = _connection.QueryFirstOrDefault<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate} ", data) != null;
            return result;
        }

        public T Last()
        {
            return Last(_allColumns);
        }

        public T Last(string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = _connection.QueryFirst<T>($"SELECT {filterColumns} FROM {_tableName} Order By {orderBy} {SortType}");
            return result;
        }

        public T Last(string predicate, object data)
        {
            return Last(predicate, data, _allColumns);
        }

        public T Last(string predicate, object data, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = _connection.QueryFirst<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {SortType}", data);
            return result;
        }

        public T LastOrDefault()
        {
            return LastOrDefault(_allColumns);
        }

        public T LastOrDefault(string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = _connection.QueryFirstOrDefault<T>($"SELECT {filterColumns} FROM {_tableName} Order By {orderBy} {SortType}");
            return result;
        }

        public T LastOrDefault(string predicate, object data)
        {
            return LastOrDefault(predicate, data, _allColumns);
        }

        public T LastOrDefault(string predicate, object data, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"Id";
            var sql = $"SELECT {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {SortType}";
            var result = _connection.QueryFirstOrDefault<T>(sql, data);
            return result;
        }

        public IEnumerable<T> GetLast(int count)
        {
            return GetLast(count, _allColumns);
        }

        public IEnumerable<T> GetLast(int count, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = _connection.Query<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} Order By {orderBy} {SortType}");
            return result;
        }

        public IEnumerable<T> GetLast(int count, string predicate, object data)
        {
            return GetLast(count, predicate, data, _allColumns);
        }

        public IEnumerable<T> GetLast(int count, string predicate, object data, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = _connection.Query<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {SortType}", data);
            return result;
        }

        public T GetLastEntity(string predicate, object data)
        {
            return GetLastEntity(predicate, data, _allColumns);
        }

        public T GetLastEntity(string predicate, object data, string filterColumns)
        {
            return Last(predicate, data);
        }

        public T Single(string predicate, object data)
        {
            return Single(predicate, data, _allColumns);
        }

        public T Single(string predicate, object data, string filterColumns)
        {
            var result = _connection.QuerySingle<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public T SingleOrDefault()
        {
            return SingleOrDefault(_allColumns);
        }

        public T SingleOrDefault(string filterColumns)
        {
            var result = _connection.QuerySingleOrDefault<T>($"SELECT {filterColumns} FROM {_tableName}");
            return result;
        }

        public T SingleOrDefault(string predicate, object data)
        {
            return SingleOrDefault(predicate, data, _allColumns);
        }

        public T SingleOrDefault(string predicate, object data, string filterColumns)
        {
            var result = _connection.QuerySingleOrDefault<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public TEntity ExecuteStoredProcedureSingle<TEntity>(string name, object data)
        {
            //var input = data.ProtectFarsiYeKeCorrection();
            return _connection.QuerySingle<TEntity>(name, data, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name, object data)
        {
            return _connection.Query<TEntity>(name, data, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name)
        {
            return _connection.Query<TEntity>(name, commandType: CommandType.StoredProcedure);
        }

        public TEntity ExecuteStoredProcedureSingle<TEntity>(string name)
        {
            //var input = data.ProtectFarsiYeKeCorrection();
            return _connection.QuerySingle<TEntity>(name, commandType: CommandType.StoredProcedure);
        }

        public TEntity ExecuteFunctionSingle<TEntity>(string name, string predicate, object data)
        {
            return _connection.QuerySingle<TEntity>($"SELECT {name}({predicate})", data);
        }

        public IEnumerable<TEntity> ExecuteFunction<TEntity>(string name, string predicate, object data)
        {
            return _connection.Query<TEntity>($"SELECT {name}({predicate})", data);
        }

        #region CRUD

        public int Insert(T entity, string columns, string columnsData)
        {
            var query = $"Insert Into {_tableName}({columns}) Values ({columnsData})";
            _connection.Execute(query, entity);
            var result = string.Format("SELECT top 1 IDENT_CURRENT('" + _tableName + "') FROM " + _tableName);
            var q = _connection.Query<int>(result).Single();
            return q;

            //_connection.ins
        }

        public bool Insert(string columns, string columnsData, T entity)
        {
            try
            {
                var query = $"Insert Into {_tableName}({columns}) Values ({columnsData})";
                _connection.Execute(query, entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Insert(T entity)
        {
            var tableName = _tableName;

            var dataType = typeof(T);

            var columnsName = GetPropertiesNames(dataType, tableName);
            var columnsDataName = GetPropertiesDataNames(dataType, tableName);

            var columns = string.Empty;
            var columnsData = string.Empty;

            foreach (var item in columnsName)
            {
                columns += $"{item}, ";
            }

            foreach (var item in columnsDataName)
            {
                columnsData += $"@{item}, ";
            }

            columns = columns.Trim().TrimEnd(',');
            columnsData = columnsData.Trim().TrimEnd(',');

            var query = $"Insert Into {tableName}({columns}) Values ({columnsData})";
            _connection.Execute(query, entity);

            return true;
        }

        public int Insert(T entity, bool auto)
        {
            var tableName = _tableName;

            var dataType = typeof(T);

            var properties = GetPropertiesNames(dataType, tableName);

            var columns = string.Empty;
            var columnsData = string.Empty;

            foreach (var item in properties)
            {
                columns += $"{item}, ";
                columnsData += $"@{item}, ";
            }

            columns = columns.Trim().TrimEnd(',');
            columnsData = columnsData.Trim().TrimEnd(',');

            var query = $"Insert Into {tableName}({columns}) Values ({columnsData})";
            _connection.Execute(query, entity);

            var result = string.Format("SELECT top 1 IDENT_CURRENT('" + _tableName + "') FROM " + _tableName);
            var q = _connection.Query<int>(result).Single();
            return q;
        }

        public void Add(T entity)
        {
            _connection.Insert(entity);
        }

        /// <summary>
        /// updating data to database.
        /// </summary>
        /// <param name="entity">data object for update</param>
        /// <param name="columns">columns pattern to update example: Name=@Name,Family=@Family,WebSite=@WebSite,Email=@Email</param>
        /// <param name="whereColumns">where columns pattern example: ID=@ID AND Name=@Name</param>
        /// <returns>true if update data, otherwise return false.</returns>
        public bool Update(T entity, string columns, string whereColumns)
        {
            try
            {
                var query = $"Update {_tableName} Set {columns} where {whereColumns}";
                _connection.Execute(query, entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int Update(object entity, Expression<Func<T, bool>> predicate)
        {
            var tableName = _tableName;
            var dataType = entity.GetType();

            var properties = GetPropertiesNames(dataType, _rowTableName);
            var columns = string.Empty;
            var whereColumns = predicate.ToSQL();

            foreach (var item in properties)
            {
                columns += $"{item} = @{item},";
            }

            columns = columns.TrimEnd(',');

            var query = $"Update {tableName} Set {columns} where {whereColumns}";

            var columnAffected = _connection.Execute(query, entity);

            return columnAffected;
        }

        public bool Delete(T entity, string whereColumns)
        {
            try
            {
                var query = $"Delete From {_tableName} Where {whereColumns}";
                _connection.Execute(query, entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int Delete(T entity)
        {
            var dataType = typeof(T);
            var keys = GetKeys(dataType, _tableName);

            var whereColumns = string.Empty;

            foreach (var item in keys)
            {
                whereColumns += $"{item} = @{item} & ";
            }

            whereColumns = whereColumns.TrimEnd(' ');
            whereColumns = whereColumns.TrimEnd('&');
            whereColumns = whereColumns.Replace("&", "AND");

            var query = $"Delete From {_tableName} Where {whereColumns}";

            var result = _connection.Execute(query, entity);

            return result;
        }

        public int DeleteAll()
        {
            var result = _connection.Count<T>();
            _connection.Execute($"TRUNCATE TABLE {_tableName}");
            return result;
        }

        private static IEnumerable<string> GetKeys(Type entityType, string tableName)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(x => entityType.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                     .ToList();

            var finalprops = new List<string>();

            foreach (var item in properties)
            {
                var prop = TypeDescriptor.GetProperties(entityType).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == item.Name);
                var isKey = prop?.Attributes.OfType<KeyAttribute>().Any();

                if (isKey == null) isKey = false;

                if (item.Name.ToLower() == "id" || string.Equals(item.Name, $"{tableName}id", StringComparison.CurrentCultureIgnoreCase) || isKey.Value)
                {
                    finalprops.Add(item.Name);
                }
            }

            return finalprops;
        }

        private static IEnumerable<string> GetPropertiesNames(Type entityType, string tableName)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(x => entityType.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                                 .ToList();

            var finalprops = new List<string>();
            foreach (var item in properties)
            {
                if (item.Name.ToLower() == "id" || string.Equals(item.Name, $"{tableName}id", StringComparison.CurrentCultureIgnoreCase)) continue;

                var prop = TypeDescriptor.GetProperties(entityType).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == item.Name);
                var notMapped = prop?.Attributes.OfType<NotMappedAttribute>().Any();
                var databaseGenerated = prop?.Attributes.OfType<DatabaseGeneratedAttribute>().Any();
                var columnAttribute = prop?.Attributes.OfType<ColumnAttribute>().Any();

                if (!(notMapped == null || notMapped.Value || databaseGenerated == null || databaseGenerated.Value))
                {
                    if (columnAttribute != null && columnAttribute.Value)
                    {
                        var columnName = prop?.Attributes.OfType<ColumnAttribute>().FirstOrDefault()?.Name;
                        if (!string.IsNullOrEmpty(columnName))
                        {
                            finalprops.Add(columnName);
                        }
                        else
                        {
                            finalprops.Add(item.Name);
                        }
                    }
                    else
                    {
                        finalprops.Add(item.Name);
                    }
                }
            }

            return finalprops;
        }

        private static IEnumerable<string> GetPropertiesDataNames(Type entityType, string tableName)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(x => entityType.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                                 .ToList();

            var finalprops = new List<string>();
            foreach (var item in properties)
            {
                if (item.Name.ToLower() == "id" || string.Equals(item.Name, $"{tableName}id", StringComparison.CurrentCultureIgnoreCase)) continue;

                var prop = TypeDescriptor.GetProperties(entityType).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == item.Name);
                var notMapped = prop?.Attributes.OfType<NotMappedAttribute>().Any();
                var databaseGenerated = prop?.Attributes.OfType<DatabaseGeneratedAttribute>().Any();
                var columnAttribute = prop?.Attributes.OfType<ColumnAttribute>().Any();

                if (!(notMapped == null || notMapped.Value || databaseGenerated == null || databaseGenerated.Value))
                {
                    finalprops.Add(item.Name);
                }
            }

            return finalprops;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetAllAsync(_allColumns);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string filterColumns)
        {
            var result = await _connection.QueryAsync<T>($"SELECT {filterColumns} FROM {_tableName} ");
            return result;
        }

        public async Task<T> GetEntityAsync(string predicate, object data)
        {
            return await GetEntityAsync(predicate, data, _allColumns);
        }

        public async Task<T> GetEntityAsync(string predicate, object data, string filterColumns)
        {
            return await SingleAsync(predicate, data);
        }

        public async Task<bool> HasEntityAsync(string predicate, object data)
        {
            return await HasEntityAsync(predicate, data, _allColumns);
        }

        public async Task<bool> HasEntityAsync(string predicate, object data, string filterColumns)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate} ", data) != null;
            return result;
        }

        public async Task<IEnumerable<T>> FindTopAsync(int count, string predicate, object data, string filterColumns = "*", bool distinct = false)
        {
            IEnumerable<T> result;
            var d = distinct ? "distinct" : "";

            if (filterColumns == "*")
                result = await _connection.QueryAsync<T>($"SELECT TOP({count}) {d} {_allColumns} FROM {_tableName} WHERE {predicate}", data);
            else
                result = await _connection.QueryAsync<T>($"SELECT TOP({count}) {d} {filterColumns} FROM {_tableName} WHERE {predicate}", data);

            return result;
        }

        public async Task<T> FirstAsync()
        {
            return await FirstAsync(_allColumns);
        }

        public async Task<T> FirstAsync(string filterColumns)
        {
            T result = await _connection.QueryFirstAsync<T>($"SELECT {filterColumns} FROM {_tableName}");
            return result;
        }

        public async Task<T> FirstAsync(string predicate, object data)
        {
            return await FirstAsync(predicate, data, _allColumns);
        }

        public async Task<T> FirstAsync(string predicate, object data, string filterColumns)
        {
            var result = await _connection.QueryFirstAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            return await FirstOrDefaultAsync(_allColumns);
        }

        public async Task<T> FirstOrDefaultAsync(string filterColumns)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName}");
            return result;
        }

        public async Task<T> FirstOrDefaultAsync(string predicate, object data)
        {
            return await FirstOrDefaultAsync(predicate, data, _allColumns);
        }

        public async Task<T> FirstOrDefaultAsync(string predicate, object data, string filterColumns)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public async Task<IEnumerable<T>> GetLastAsync(int count)
        {
            return await GetLastAsync(count, _allColumns);
        }

        public async Task<IEnumerable<T>> GetLastAsync(int count, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = await _connection.QueryAsync<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} Order By {orderBy} {SortType}");
            return result;
        }

        public async Task<IEnumerable<T>> GetLastAsync(int count, string predicate, object data)
        {
            return await GetLastAsync(count, predicate, data, _allColumns);
        }

        public async Task<IEnumerable<T>> GetLastAsync(int count, string predicate, object data, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = await _connection.QueryAsync<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {SortType}", data);
            return result;
        }

        public async Task<T> GetLastEntityAsync(string predicate, object data)
        {
            return await GetLastEntityAsync(predicate, data, _allColumns);
        }

        public async Task<T> GetLastEntityAsync(string predicate, object data, string filterColumns)
        {
            return await LastAsync(predicate, data);
        }

        public async Task<IEnumerable<T>> GetTopAsync(int count)
        {
            return await GetTopAsync(count, _allColumns);
        }

        public async Task<IEnumerable<T>> GetTopAsync(int count, string filterColumns)
        {
            var result = await _connection.QueryAsync<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} ");
            return result;
        }

        public async Task<IEnumerable<T>> GetTopAsync(int count, string predicate, object data)
        {
            return await GetTopAsync(count, predicate, data, _allColumns);
        }

        public async Task<IEnumerable<T>> GetTopAsync(int count, string predicate, object data, string filterColumns)
        {
            var result = await _connection.QueryAsync<T>($"SELECT TOP {count} {filterColumns} FROM {_tableName} WHERE {predicate} ", data);
            return result;
        }

        public async Task<T> LastAsync()
        {
            return await LastAsync(_allColumns);
        }

        public async Task<T> LastAsync(string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = await _connection.QueryFirstAsync<T>($"SELECT {filterColumns} FROM {_tableName} Order By {orderBy} {SortType}");
            return result;
        }

        public async Task<T> LastAsync(string predicate, object data)
        {
            return await LastAsync(predicate, data, _allColumns);
        }

        public async Task<T> LastAsync(string predicate, object data, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = await _connection.QueryFirstAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {SortType}", data);
            return result;
        }

        public async Task<T> LastOrDefaultAsync()
        {
            return await LastOrDefaultAsync(_allColumns);
        }

        public async Task<T> LastOrDefaultAsync(string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = await _connection.QueryFirstOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName} Order By {orderBy} {SortType}");
            return result;
        }

        public async Task<T> LastOrDefaultAsync(string predicate, object data)
        {
            return await LastOrDefaultAsync(predicate, data, _allColumns);
        }

        public async Task<T> LastOrDefaultAsync(string predicate, object data, string filterColumns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";

            var result = await _connection.QueryFirstOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {SortType}", data);
            return result;
        }

        public async Task<T> SingleAsync(string predicate, object data)
        {
            return await SingleAsync(predicate, data, _allColumns);
        }

        public async Task<T> SingleAsync(string predicate, object data, string filterColumns)
        {
            var result = await _connection.QuerySingleAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public async Task<T> SingleOrDefaultAsync()
        {
            return await SingleOrDefaultAsync(_allColumns);
        }

        public async Task<T> SingleOrDefaultAsync(string filterColumns)
        {
            var result = await _connection.QuerySingleOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName}");
            return result;
        }

        public async Task<T> SingleOrDefaultAsync(string predicate, object data)
        {
            return await SingleOrDefaultAsync(predicate, data, _allColumns);
        }

        public async Task<T> SingleOrDefaultAsync(string predicate, object data, string filterColumns)
        {
            var result = await _connection.QuerySingleOrDefaultAsync<T>($"SELECT {filterColumns} FROM {_tableName} WHERE {predicate}", data);
            return result;
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageIndex)
        {
            return await GetPagedDataAsync(pageIndex, _allColumns);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageIndex, string filterColumns)
        {
            return await GetPagedDataAsync(20, pageIndex);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex)
        {
            return await GetPagedDataAsync(pageSize, pageIndex, _allColumns);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string filterColumns)
        {
            var orderBy = $"{_tableName}Id";
            orderBy = orderBy.Replace("[dbo].[", "");
            orderBy = orderBy.Replace("]", "");
            var itemsCount = await _connection.QuerySingleAsync<int>($"SELECT COUNT({orderBy}) FROM {_tableName} ");
            var result = await _connection.QueryAsync<T>($"SELECT  {filterColumns} FROM {_tableName}  Order By {orderBy} OFFSET {pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY");
            var res = result.ToList();
            var pagedData = new PagedData<T>(itemsCount, pageSize, pageIndex, res);
            return pagedData;
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, object data)
        {
            return await GetPagedDataAsync(pageSize, pageIndex, predicate, _allColumns, data);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, object data, string orderBy)
        {
            return await GetPagedDataAsync(pageSize, pageIndex, predicate, data, orderBy, _allColumns);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, object data, string orderBy, string filterColumns)
        {
            return await GetPagedDataAsync(pageIndex, pageSize, predicate, data, true, orderBy);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy)
        {
            return await GetPagedDataAsync(pageIndex, pageSize, predicate, data, isAscendingOrder, orderBy, _allColumns);
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy, string filterColumns)
        {
            var sortType = "Desc";
            if (isAscendingOrder)
                sortType = "Asc";

            var itemsCount = await _connection.QuerySingleAsync<int>($"SELECT COUNT({orderBy}) FROM {_tableName} WHERE {predicate}", data);
            var result = await _connection.QueryAsync<T>($"SELECT  {filterColumns} FROM {_tableName} WHERE {predicate} Order By {orderBy} {sortType} OFFSET {pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY", data);
            var res = result.AsList();
            var pagedData = new PagedData<T>(itemsCount, pageSize, pageIndex, res);
            return pagedData;
        }

        public async Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name, object data)
        {
            return await _connection.QuerySingleAsync<TEntity>(name, data, commandType: CommandType.StoredProcedure);
        }

        public async Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name)
        {
            return await _connection.QuerySingleAsync<TEntity>(name, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name, object data)
        {
            return await _connection.QueryAsync<TEntity>(name, data, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name)
        {
            return await _connection.QueryAsync<TEntity>(name, commandType: CommandType.StoredProcedure);
        }

        public async Task<TEntity> ExecuteFunctionSingleAsync<TEntity>(string name, string predicate, object data)
        {
            return await _connection.QuerySingleAsync<TEntity>($"SELECT {name}({predicate})", data);
        }

        public async Task<IEnumerable<TEntity>> ExecuteFunctionAsync<TEntity>(string name, string predicate, object data)
        {
            return await _connection.QueryAsync<TEntity>($"SELECT {name}({predicate})", data);
        }

        public async Task<int> InsertAsync(T entity, string columns, string columnsData)
        {
            var query = $"Insert Into {_tableName}({columns}) Values ({columnsData})";
            await _connection.ExecuteAsync(query, entity);
            var result = string.Format("SELECT top 1 IDENT_CURRENT('" + _tableName + "') FROM " + _tableName);
            var q = await _connection.QueryAsync<int>(result);
            var r = q.Single();
            return r;
        }

        public async Task<bool> InsertAsync(string columns, string columnsData, T entity)
        {
            try
            {
                var query = $"Insert Into {_tableName}({columns}) Values ({columnsData})";
                await _connection.ExecuteAsync(query, entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertAsync(T entity)
        {
            var tableName = _tableName;

            var dataType = typeof(T);

            var properties = GetPropertiesNames(dataType, tableName);

            var columns = string.Empty;
            var columnsData = string.Empty;

            foreach (var item in properties)
            {
                columns += $"{item}, ";
                columnsData += $"@{item}, ";
            }

            columns = columns.Trim().TrimEnd(',');
            columnsData = columnsData.Trim().TrimEnd(',');

            var query = $"Insert Into {tableName}({columns}) Values ({columnsData})";
            await _connection.ExecuteAsync(query, entity);

            return true;
        }

        public async Task<int> InsertAsync(T entity, bool auto)
        {
            var tableName = _tableName;

            var dataType = typeof(T);

            var properties = GetPropertiesNames(dataType, tableName);

            var columns = string.Empty;
            var columnsData = string.Empty;

            foreach (var item in properties)
            {
                columns += $"{item}, ";
                columnsData += $"@{item}, ";
            }

            columns = columns.Trim().TrimEnd(',');
            columnsData = columnsData.Trim().TrimEnd(',');

            var query = $"Insert Into {tableName}({columns}) Values ({columnsData})";
            await _connection.ExecuteAsync(query, entity);

            var result = string.Format("SELECT top 1 IDENT_CURRENT('" + _tableName + "') FROM " + _tableName);
            var q = await _connection.QueryAsync<int>(result);
            var r = q.Single();
            return r;
        }

        public async Task<bool> UpdateAsync(T entity, string columns, string whereColumns)
        {
            try
            {
                var query = $"Update {_tableName} Set {columns} where {whereColumns}";
                await _connection.ExecuteAsync(query, entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> UpdateAsync(object entity, Expression<Func<T, bool>> predicate)
        {
            var tableName = _tableName;
            var dataType = entity.GetType();

            var properties = GetPropertiesNames(dataType, _rowTableName);
            var columns = string.Empty;
            var whereColumns = predicate.ToSQL();

            foreach (var item in properties)
            {
                columns += $"{item} = @{item},";
            }

            columns = columns.TrimEnd(',');

            var query = $"Update {tableName} Set {columns} where {whereColumns}";

            var columnAffected = await _connection.ExecuteAsync(query, entity);

            return columnAffected;
        }

        public async Task<bool> DeleteAsync(T entity, string whereColumns)
        {
            try
            {
                var query = $"Delete From {_tableName} Where {whereColumns}";
                await _connection.ExecuteAsync(query, entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> DeleteAsync(T entity)
        {
            var dataType = typeof(T);
            var keys = GetKeys(dataType, _tableName);

            var whereColumns = string.Empty;

            foreach (var item in keys)
            {
                whereColumns += $"{item} = @{item} & ";
            }

            whereColumns = whereColumns.TrimEnd(' ');
            whereColumns = whereColumns.TrimEnd('&');
            whereColumns = whereColumns.Replace("&", "AND");

            var query = $"Delete From {_tableName} Where {whereColumns}";

            var result = await _connection.ExecuteAsync(query, entity);

            return result;
        }

        public async Task<int> DeleteAllAsync()
        {
            var result = _connection.Count<T>();
            await _connection.ExecuteAsync($"TRUNCATE TABLE {_tableName}");
            return result;
        }

        public async Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, string filterColumns, object data)
        {
            var orderBy = $"{_tableName}Id";
            return await GetPagedDataAsync(pageSize, pageIndex, predicate, data, orderBy);
        }

        #endregion CRUD
    }

}
