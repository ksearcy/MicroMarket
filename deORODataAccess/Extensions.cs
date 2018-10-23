using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Web;

namespace deORODataAccess
{
    public static class Extensions
    {
        private readonly static object _lock = new object();

        public static T cloneObject<T>(T original, List<string> propertyExcludeList)
        {
            try
            {
                Monitor.Enter(_lock);
                T copy = Activator.CreateInstance<T>();
                PropertyInfo[] piList = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo pi in piList)
                {
                    if (!propertyExcludeList.Contains(pi.Name))
                    {
                        if (pi.GetValue(copy, null) != pi.GetValue(original, null))
                        {
                            pi.SetValue(copy, pi.GetValue(original, null), null);
                        }
                    }
                }
                return copy;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }


        static IQueryable<T> ETForStartsWith<T>(IQueryable<T> query, string propertyValue, PropertyInfo propertyInfo)
        {
            ParameterExpression e = Expression.Parameter(typeof(T), "e");
            MemberExpression m = Expression.MakeMemberAccess(e, propertyInfo);
            ConstantExpression c = Expression.Constant(propertyValue, typeof(string));
            MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            Expression call = Expression.Call(m, mi, c);

            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(call, e);
            return query.Where(lambda);
        }


        public static Expression<Func<T, bool>> PropertyEquals<T, TValue>(PropertyInfo property, TValue value)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.Equal(Expression.Property(param, property), Expression.Constant(value));

            return Expression.Lambda<Func<T, bool>>(body, param);
        }


        public static T ConvertToEntity<T>(this DataRow tableRow) where T : new()
        {
            // Create a new type of the entity I want
            Type t = typeof(T);
            T returnObject = new T();


            foreach (DataColumn col in tableRow.Table.Columns)
            {
                string colName = col.ColumnName;

                // Look for the object's property with the columns name, ignore case
                PropertyInfo pInfo = t.GetProperty(colName.ToLower(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // did we find the property ?
                if (pInfo != null)
                {
                    object val = tableRow[colName];

                    // is this a Nullable<> type
                    bool IsNullable = (Nullable.GetUnderlyingType(pInfo.PropertyType) != null);
                    if (IsNullable)
                    {
                        if (val is System.DBNull)
                        {
                            val = null;
                        }
                        else
                        {
                            // Convert the db type into the T we have in our Nullable<T> type
                            val = Convert.ChangeType(val, Nullable.GetUnderlyingType(pInfo.PropertyType));
                        }
                    }
                    else
                    {
                        // Convert the db type into the type of the property in our entity
                        val = Convert.ChangeType(val, pInfo.PropertyType);
                    }
                    // Set the value of the property with the value from the db
                    pInfo.SetValue(returnObject, val, null);
                }
            }

            // return the entity object with values
            return returnObject;
        }

        public static void CopyPropertyValues(object source, object destination, string[] ignoreKeys)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                if (!ignoreKeys.Contains(sourceProperty.Name))
                    foreach (var destProperty in destProperties)
                    {
                        if (destProperty.Name == sourceProperty.Name && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                        {
                            destProperty.SetValue(destination, sourceProperty.GetValue(
                                source, new object[] { }), new object[] { });

                            break;
                        }
                    }
            }
        }

        public static DataTable ToDataTable<T>(this IList<T> data, string tableName = "")
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(tableName);
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static object ExecuteScalar(this Database database, string sql, params object[] parameters)
        {
            using (System.Data.IDbCommand command = database.Connection.CreateCommand())
            {
                try
                {
                    database.Connection.Open();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    return command.ExecuteScalar();
                }
                finally
                {
                    database.Connection.Close();
                    command.Parameters.Clear();
                }
            }

        }


        public static System.Collections.IEnumerable DynamicSqlQuery(this Database database, string sql, params object[] parameters)
        {
            TypeBuilder builder = createTypeBuilder(
                    "MyDynamicAssembly", "MyDynamicModule", "MyDynamicType");

            using (System.Data.IDbCommand command = database.Connection.CreateCommand())
            {
                try
                {
                    database.Connection.Open();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    using (System.Data.IDataReader reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();

                        foreach (System.Data.DataRow row in schema.Rows)
                        {
                            string name = (string)row["ColumnName"];
                            //var a=row.ItemArray.Select(d=>d.)
                            Type type = (Type)row["DataType"];
                            if (type != typeof(string) && (bool)row.ItemArray[schema.Columns.IndexOf("AllowDbNull")])
                            {
                                type = typeof(Nullable<>).MakeGenericType(type);
                            }
                            createAutoImplementedProperty(builder, name, type);
                        }
                    }
                }
                finally
                {
                    database.Connection.Close();
                    command.Parameters.Clear();
                }
            }

            Type resultType = builder.CreateType();

            return database.SqlQuery(resultType, sql, parameters);
        }

        private static TypeBuilder createTypeBuilder(
            string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName),
                                       AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private static void createAutoImplementedProperty(
            TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                              propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
    }
}