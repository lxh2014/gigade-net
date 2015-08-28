using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using MySql.Data.MySqlClient;
namespace BLL.gigade.Common
{
    public class MySqlHelper
    {
        private string ConnString;

        public MySqlHelper(string connectionString)
        {
            ConnString = connectionString;
        }
        public IList<T> RunMySqlSelect4ReturnList<T>(string strCommand) where T : new()
        {

            MySqlCommand mySqlCommand = new MySqlCommand();
            mySqlCommand.CommandText = strCommand.ToString();
            IList<PropertyInfo> ilPropertyInfo = typeof(T).GetProperties().ToList();  //p[0].Name =travel_id;
            IList<T> ilResult = new List<T>();

            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(this.ConnString))
            {
                conn.Open();
                using (MySqlCommand cmd = mySqlCommand)
                {
                    cmd.Connection = conn;
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        dt.Load(rdr);
                    }
                }
            }

            foreach (DataRow dr in dt.Rows)
            {
                T tItem = new T();
                foreach (var v in ilPropertyInfo)
                {
                    if (dt.Columns[v.Name] == null) continue;
                    ConvertionExtensions.SetValue(tItem, v.Name, dr[v.Name]);
                }
                ilResult.Add(tItem);
            }
            return ilResult;
        }
        public T getSinggleObj<T>(string strCommand) where T : new()
        {

            MySqlCommand mySqlCommand = new MySqlCommand();
            mySqlCommand.CommandText = strCommand.ToString();
            IList<PropertyInfo> ilPropertyInfo = typeof(T).GetProperties().ToList();  //p[0].Name =travel_id;
            T ilResult = new T();

            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(this.ConnString))
            {
                conn.Open();
                using (MySqlCommand cmd = mySqlCommand)
                {
                    cmd.Connection = conn;
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        dt.Load(rdr);
                    }
                }
            }
            if (dt.Rows.Count <= 0)
            {
                return default(T);
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T tItem = new T();
                    foreach (var v in ilPropertyInfo)
                    {
                        if (dt.Columns[v.Name] == null) continue;
                        ConvertionExtensions.SetValue(tItem, v.Name, dr[v.Name]);
                    }
                    ilResult = tItem;
                }
            }
            return ilResult;
        }
        public IList<T> RunMySqlSelect4ReturnList<T>(DataTable dts) where T : new()
        {


            IList<PropertyInfo> ilPropertyInfo = typeof(T).GetProperties().ToList();  //p[0].Name =travel_id;
            IList<T> ilResult = new List<T>();

            DataTable dt = dts;

            foreach (DataRow dr in dt.Rows)
            {
                T tItem = new T();
                foreach (var v in ilPropertyInfo)
                {
                    if (dt.Columns[v.Name] == null) continue;
                    ConvertionExtensions.SetValue(tItem, v.Name, dr[v.Name]);
                }
                ilResult.Add(tItem);
            }
            return ilResult;
        }
    }
    public static class ConvertionExtensions
    {
        public static T ConvertTo<T>(this IConvertible convertibleValue)
        {
            var t = typeof(T);
            if (null == convertibleValue)
            {
                return default(T);
            }
            if (!typeof(T).IsGenericType)
            {
                return (T)Convert.ChangeType(convertibleValue, typeof(T));
            }
            else
            {
                Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeof(T)));
                }
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, typeof(T).FullName));
        }

        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

    }
}
//缺少编译器要求的成员“System.Runtime.CompilerServices.ExtensionAttribute..ctor”
namespace System.Runtime.CompilerServices { public class ExtensionAttribute : Attribute { } }
