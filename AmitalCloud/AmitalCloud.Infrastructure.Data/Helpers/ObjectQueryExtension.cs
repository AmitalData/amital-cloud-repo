using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class ObjectQueryExtension
    {
        public static IQueryable<TSource> Include<TSource>(this IQueryable<TSource> source, string path) where TSource : class
        {
            return EntityFrameworkQueryableExtensions.Include(source, path);
        }

        public static IQueryable<T> FullTextSearch<T>(this IQueryable<T> queryable, string searchKey)
        {
            return FullTextSearch<T>(queryable, searchKey, false);
        }
        public static IQueryable<T> FullTextSearch<T>(this IQueryable<T> queryable, string searchKey, bool exactMatch)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "c");

            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            MethodInfo toStringMethod = typeof(object).GetMethod("ToString", new Type[] { });

            var publicProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(p => p.PropertyType == typeof(string));
            Expression orExpressions = null;

            string[] searchKeyParts;
            if (exactMatch)
            {
                searchKeyParts = new[] { searchKey };
            }
            else
            {
                searchKeyParts = searchKey.Split(' ');
            }

            foreach (var property in publicProperties)
            {
                Expression nameProperty = Expression.Property(parameter, property);
                foreach (var searchKeyPart in searchKeyParts)
                {
                    Expression searchKeyExpression = Expression.Constant(searchKeyPart);
                    Expression callContainsMethod = Expression.Call(nameProperty, containsMethod, searchKeyExpression);
                    if (orExpressions == null)
                    {
                        orExpressions = callContainsMethod;
                    }
                    else
                    {
                        orExpressions = Expression.Or(orExpressions, callContainsMethod);
                    }
                }
            }

            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { queryable.ElementType },
                queryable.Expression,
                Expression.Lambda<Func<T, bool>>(orExpressions, new ParameterExpression[] { parameter }));

            return queryable.Provider.CreateQuery<T>(whereCallExpression);
        }


        #region Entity Framework Utilities
        //we cache table names in a a dictionary because
        //they take a lot of work to figure out
        static readonly object _LockProxy = new object();
        static readonly Dictionary<Type, string> _TableNameCache = new Dictionary<Type, string>();

        /// <summary>
        /// Generate an Entity set for TEntity
        /// so that we can inspect the metadata
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static IEntityType GetMetaDataEntitySet<TEntity>(DbContext dbCtx)
        {
            Type entityType = typeof(TEntity);
            string entityName = entityType.Name;

            // Get the entity type from the model in DbContext
            var entityTypeModel = dbCtx.Model.FindEntityType(entityType);

            if (entityTypeModel == null)
            {
                throw new InvalidOperationException($"Entity type {entityType.Name} not found in model.");
            }

            // Find the corresponding table in the model
            var entitySets = dbCtx.Model.GetEntityTypes()
                                        .Where(t => t.GetTableName() != null) // Ensure it's a table (not a view or other type)
                                        .ToList();

            // Look for the entity set by name (entity name corresponds to table name in EF Core)
            var returnValue = entitySets.FirstOrDefault(t => t.Name == entityName);

            // If not found, check if the entity is a subclass and get the base type
            while (returnValue == null && entityType.BaseType != null)
            {
                entityType = entityType.BaseType;
                entityName = entityType.Name;
                returnValue = entitySets.FirstOrDefault(t => t.Name == entityName);
            }

            return returnValue;
        }

        /// <summary>
        /// Extract a named property value 
        /// from the metadata properties
        /// </summary>
        /// <param name="entitySet"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        static string GetStringPropertyFromEntityMetaData(IEntityType entityType, string propertyName)
        {
            var property = entityType.FindProperty(propertyName);
            return property?.Name;
        }

        /// <summary>
        /// Get the provisioned table name for a Class (TEntity)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        static string GetProvisionedTableName<TEntity>(DbContext dbCtx, bool includeSchemaPrefix = false) where TEntity : class
        {
            Type tableAttribute = typeof(TableAttribute);
            TableAttribute tableNameAttribute = typeof(TEntity).GetCustomAttributes(tableAttribute, false).FirstOrDefault() as TableAttribute;
            if (null != tableNameAttribute)
            {
                //if its specified use that
                return tableNameAttribute.Name;
            }
            else
            {
                Type entityType = typeof(TEntity);
                string returnValue;
                lock (_LockProxy)
                {
                    //Its expensive to create sort out the object name so
                    //we will cache the name, it needs to be verified 
                    //because for some reason the table names are not 
                    //always aligned with the EntitySet name
                    if (!_TableNameCache.TryGetValue(entityType, out returnValue))
                    {
                        //Ok its not in the cache so try to figure it out
                        var entitySet = GetMetaDataEntitySet<TEntity>(dbCtx);
                        if (entitySet == null)
                        {
                            //This should be typed better, but for our example its a simple Exception
                            throw new ApplicationException("Unable to find entity set '{0}' in edm metadata" + entityType.Name);
                        }
                        returnValue = GetStringPropertyFromEntityMetaData(entitySet, "Schema") + "." + GetStringPropertyFromEntityMetaData(entitySet, "Table");
                        if (string.IsNullOrEmpty(returnValue)) throw new Exception("Could not find Entity Table name for " + entityType.Name);
                        _TableNameCache.Add(entityType, returnValue);
                    }
                }
                if (!includeSchemaPrefix)
                {
                    string[] nameParts = returnValue.Split('.');
                    returnValue = nameParts.Last();
                }
                return returnValue;
            }
        }


        /// <summary>
        /// Gets a column name suitable for naming constraints
        /// and indices, not actually validated with the Model
        /// since there dont seem to be any conflicts other
        /// than ComplexTypes which are simple to calculate
        /// </summary>
        /// <param name="columnProp"></param>
        /// <returns></returns>
        static string GetSimpleColumnName(System.Reflection.PropertyInfo columnProp)
        {
            Type columnAttribute = typeof(ColumnAttribute);
            ColumnAttribute columnNameAttribute = columnProp.GetCustomAttributes(columnAttribute, false).FirstOrDefault() as ColumnAttribute;
            if (null != columnNameAttribute && !string.IsNullOrEmpty(columnNameAttribute.Name))
            {
                return columnNameAttribute.Name;
            }
            else
            {
                return columnProp.Name;
            }
        }

        /// <summary>
        /// Gets the names of columns with the Key attribute 
        /// and return their names and types
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        static string[] GetKeyColumnNamesAndTypes(Type tableType, out Type[] columnTypes)
        {
            List<string> returnValue = new List<string>();
            List<Type> keyTypes = new List<Type>();
            var keyColumns = tableType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute))).ToArray();
            foreach (PropertyInfo columnInfo in keyColumns)
            {
                returnValue.Add(GetSimpleColumnName(columnInfo));
                keyTypes.Add(columnInfo.PropertyType);
            }
            columnTypes = keyTypes.ToArray();
            return returnValue.ToArray();
        }

        /// <summary>
        /// convert generic object collection to a typed collection
        /// this only works with primitives or things with explicit
        /// conversion operators
        /// </summary>
        /// <typeparam name="T">type of objects in the untypedList</typeparam>
        /// <param name="untypedList">collection of untyped objects</param>
        /// <returns>Typed collection</returns>
        static HashSet<T> ConvertUntypedCollectionToTypedHashSet<T>(ArrayList untypedList)
        {
            HashSet<T> returnValue = new HashSet<T>();
            foreach (object untypedObject in untypedList)
            {
                returnValue.Add((T)untypedObject);
            }
            return returnValue;
        }
        #endregion


        #region FreeText operations
        /// <summary>
        /// Build and execute a FreeText or Contains 
        /// query  
        /// </summary>
        /// <param name="dbCtx">active DbContext</param>
        /// <param name="tableName">Table name</param>
        /// <param name="primaryKeyColumnName">Table primary key column name</param>
        /// <param name="searchText">FREETEXT or CONTAINS formated query text</param>
        /// <param name="columnNames">array of full text indexed column names, null or 0 length means use "*"</param>
        /// <param name="useContains">this is a Contains query</param>
        /// <returns></returns>
        static ArrayList ExecuteFreeTextSearch(DbContext dbCtx, string tableName, string primaryKeyColumnName, string searchText, string[] columnNames, bool useContains)
        {
            if (string.IsNullOrWhiteSpace(searchText)) throw new ArgumentNullException("searchText");
            string columnNameText = "*";
            if (null != columnNames && columnNames.Length > 0 && columnNames[0].Trim() != "*")
            {
                columnNameText = "(" + string.Join(",", columnNames) + ")";
            }
            ArrayList returnValue = new ArrayList();
            if (searchText.StartsWith("'")) searchText = searchText.Trim(new char[] { '\'' });
            string commandType = useContains ? "CONTAINS" : "FREETEXT";
            string sqlQuery = string.Format("SELECT {0} FROM {1} WHERE {2}({3},'{4}')", primaryKeyColumnName, tableName, commandType, columnNameText, searchText);

            dbCtx.Database.GetDbConnection().Open();
            try
            {
                //exeute the query 
                using (DbCommand dbCmd = dbCtx.Database.GetDbConnection().CreateCommand())
                {
                    dbCmd.CommandText = sqlQuery;
                    dbCmd.CommandType = CommandType.Text;
                    using (DbDataReader dbRead = dbCmd.ExecuteReader())
                    {
                        while (dbRead.Read())
                        {
                            returnValue.Add(dbRead.GetValue(0));
                        }
                    }
                }
            }
            finally
            {
                //Dont dispose it, it will be needed again
                dbCtx.Database.GetDbConnection().Close();

            }
            return returnValue;
        }

        /// <summary>
        /// Internal processing of the free text query
        /// </summary>
        /// <typeparam name="TEntity">entity for our table</typeparam>
        /// <param name="thisQuery"></param>
        /// <param name="dbCtx">active DbContext</param>
        /// <param name="searchText">FREETEXT or CONTAINS formated query text</param>
        /// <param name="columnNames">array of full text indexed column names, null or 0 length means use "*"</param>
        /// <returns></returns>
        static IQueryable<TEntity> FreeTextExInternal<TEntity>(IQueryable<TEntity> thisQuery, DbContext dbCtx, string searchText, string[] columnNames, bool useContains) where TEntity : class
        {
            ///Assuming that the table has been preped for Full Text indexing we will execute
            ///a query for the primary key of matchign results, then we will convert our results 
            ///to a typed collection and build an expression that will use the "Contains" method 
            ///of the typed results to build this component of the expression tree. The 
            ///remainder of the expression remains fully composable so things like Include and other
            ///join related queries will still work.
            ///
            /// Essentially mimicking something like this (assuming a guid based Key named UserId):  
            ///     HashSet<Guid> userMatchs = "SELECT UserId from Users WHERE ....... "
            ///     (from secUser in secCtx.Users.Where(usr => userMatchs.Contains(usr.UserId)) select secUser);


            Type[] keyColumnTypes = null;
            //Query the data model for the actual table name to query against
            string tableName = GetProvisionedTableName<TEntity>(dbCtx);
            //Query the entity object for the column name
            string[] entityKeyNames = GetKeyColumnNamesAndTypes(typeof(TEntity), out keyColumnTypes);
            if (entityKeyNames.Length == 0 || entityKeyNames.Length > 1)
            {
                //We need a single Key property , both for our Query and because its
                //a requirement of the FullText Catalog 

                //This should be typed better, but for our example its a simple ApplicationException
                throw new ApplicationException(typeof(TEntity).Name + " as (dbo." + tableName + ") does not have a a valid index to support a full-text search key.");
            }
            //Ok get the matching TEntity parameter
            ParameterExpression parameterExpression = Expression.Parameter(thisQuery.ElementType, "p");
            //And now project the propery name on TEntity (we know its only one level deep so no need to evaluate dotted naming)
            MemberExpression memberPropertyAccess = MemberExpression.Property(parameterExpression, entityKeyNames[0]);

            //Now get the untyped collection of results from the query
            ArrayList untypedResults = ExecuteFreeTextSearch(dbCtx, tableName, entityKeyNames[0], searchText, columnNames, useContains);

            // Convert the results to a typed collection so that the comparisonCondition can be correctly constructed , if we dont
            // we will get errors like this (assuming a key of type guid and an untyped array)
            //  " generic type 'System.Guid' cannot be used for parameter of type 'System.Object' of method 'Boolean Contains(System.Object)'"
            Expression comparisonCondition = null;
            LambdaExpression lambdaExpression = null;
            MethodInfo containsMethod = null;
            switch (keyColumnTypes[0].Name)
            {
                case "Guid":
                    {
                        HashSet<Guid> containedIn = ConvertUntypedCollectionToTypedHashSet<Guid>(untypedResults);
                        containsMethod = typeof(HashSet<Guid>).GetMethod("Contains", new Type[] { typeof(Guid) });
                        comparisonCondition = Expression.Call(Expression.Constant(containedIn), containsMethod, memberPropertyAccess);
                        lambdaExpression = Expression.Lambda(comparisonCondition, parameterExpression);
                    }
                    break;
                case "String":
                    {
                        HashSet<string> containedIn = ConvertUntypedCollectionToTypedHashSet<string>(untypedResults);
                        containsMethod = typeof(HashSet<string>).GetMethod("Contains", new Type[] { typeof(string) });
                        comparisonCondition = Expression.Call(Expression.Constant(containedIn), containsMethod, memberPropertyAccess);
                        lambdaExpression = Expression.Lambda(comparisonCondition, parameterExpression);
                    }
                    break;
                case "Int32":
                    {
                        HashSet<int> containedIn = ConvertUntypedCollectionToTypedHashSet<int>(untypedResults);
                        containsMethod = typeof(HashSet<int>).GetMethod("Contains", new Type[] { typeof(int) });
                        comparisonCondition = Expression.Call(Expression.Constant(containedIn), containsMethod, memberPropertyAccess);
                        lambdaExpression = Expression.Lambda(comparisonCondition, parameterExpression);
                    }
                    break;
                case "Int64":
                    {
                        HashSet<Int64> containedIn = ConvertUntypedCollectionToTypedHashSet<Int64>(untypedResults);
                        containsMethod = typeof(HashSet<Int64>).GetMethod("Contains", new Type[] { typeof(Int64) });
                        comparisonCondition = Expression.Call(Expression.Constant(containedIn), containsMethod, memberPropertyAccess);
                        lambdaExpression = Expression.Lambda(comparisonCondition, parameterExpression);
                    }
                    break;
                case "Int16":
                    {
                        HashSet<Int16> containedIn = ConvertUntypedCollectionToTypedHashSet<Int16>(untypedResults);
                        containsMethod = typeof(HashSet<Int16>).GetMethod("Contains", new Type[] { typeof(Int16) });
                        comparisonCondition = Expression.Call(Expression.Constant(containedIn), containsMethod, memberPropertyAccess);
                        lambdaExpression = Expression.Lambda(comparisonCondition, parameterExpression);
                    }
                    break;
                default:
                    throw new Exception("unsupported primary key type " + keyColumnTypes[0].Name + " for " + tableName);
                    break;
            }

            //and now complete our composition by adding the Where "our free text result keys contains(...)" clause
            MethodCallExpression conditionResult = Expression.Call(typeof(Queryable), "Where", new[] { thisQuery.ElementType }, thisQuery.Expression, lambdaExpression);
            return thisQuery.Provider.CreateQuery<TEntity>(conditionResult);

        }
        #endregion

        #region public FreeTextSearch extension
        /// <summary>
        /// FreeTextSearch Executes a free text search (using FREETEXT) for Entities that were processed with the 
        /// SqlFullTextIndexAttribute (or are just in a free text catalog). It queries the FreeText catalog for the primary keys
        /// of the TEntity objects that are in scope for the query, then creates an expression
        /// tree to include those entities.The DbContext is used to execute these initial
        /// queries 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="thisQuery"></param>
        /// <param name="dbCtx">DbContext attached to TEntity</param>
        /// <param name="containsSearchText">FREETEXT formated search expresssion</param>
        /// <returns>IQueryable<TEntity></returns>
        public static IQueryable<TEntity> FreeTextSearch<TEntity>(this IQueryable<TEntity> thisQuery, DbContext dbCtx, string freetextSearchText) where TEntity : class
        {
            return FreeTextExInternal<TEntity>(thisQuery, dbCtx, freetextSearchText, null, false);
        }

        /// <summary>
        /// FreeTextSearch Executes a free text search (using FREETEXT) for Entities that were processed with the 
        /// SqlFullTextIndexAttribute (or are just in a free text catalog). It queries the FreeText catalog for the primary keys
        /// of the TEntity objects that are in scope for the query, then creates an expression
        /// tree to include those entities.The DbContext is used to execute these initial
        /// queries 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="thisQuery"></param>
        /// <param name="dbCtx">DbContext attached to TEntity</param>
        /// <param name="containsSearchText">FREETEXT formated search expresssion</param>
        /// <param name="columnNames">array of full text indexed column names, null or 0 length means use "*"</param>
        /// <returns>IQueryable<TEntity></returns>
        public static IQueryable<TEntity> FreeTextSearch<TEntity>(this IQueryable<TEntity> thisQuery, DbContext dbCtx, string freetextSearchText, string[] columnNames) where TEntity : class
        {
            return FreeTextExInternal<TEntity>(thisQuery, dbCtx, freetextSearchText, columnNames, false);
        }
        #endregion

        #region public FreeTextContains extension
        /// <summary>
        /// FreeTextContains Executes a free text search (using CONTAINS) for Entities that were processed with the 
        /// SqlFullTextIndexAttribute (or are just in a free text catalog). It queries the FreeText catalog for the primary keys
        /// of the TEntity objects that are in scope for the query, then creates an expression
        /// tree to include those entities. TThe DbContext is used to execute these initial
        /// queries 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="thisQuery"></param>
        /// <param name="dbCtx">DbContext attached to TEntity</param>
        /// <param name="containsSearchText">CONTAINS formated search expresssion</param>
        /// <returns>IQueryable<TEntity></returns>
        public static IQueryable<TEntity> FreeTextContains<TEntity>(this IQueryable<TEntity> thisQuery, DbContext dbCtx, string containsSearchText) where TEntity : class
        {
            return FreeTextExInternal<TEntity>(thisQuery, dbCtx, containsSearchText, null, true);
        }


        /// <summary>
        /// FreeTextContains Executes a free text search (using CONTAINS) for Entities that were processed with the 
        /// SqlFullTextIndexAttribute (or are just in a free text catalog). It queries the FreeText catalog for the primary keys
        /// of the TEntity objects that are in scope for the query, then creates an expression
        /// tree to include those entities. The DbContext is used to execute these initial
        /// queries 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="thisQuery"></param>
        /// <param name="dbCtx">DbContext attached to TEntity</param>
        /// <param name="containsSearchText">CONTAINS formated search expresssion</param>
        /// <param name="columnNames">array of full text indexed column names, null or 0 length means use "*"</param>
        /// <returns>IQueryable<TEntity></returns>
        public static IQueryable<TEntity> FreeTextContains<TEntity>(this IQueryable<TEntity> thisQuery, DbContext dbCtx, string containsSearchText, string[] columnNames) where TEntity : class
        {
            return FreeTextExInternal<TEntity>(thisQuery, dbCtx, containsSearchText, columnNames, true);
        }
        #endregion

    }
}
