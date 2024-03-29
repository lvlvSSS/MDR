using System.Collections;
using System.Data;
using System.Reflection;

namespace MDR.Infrastructure.Extensions;

public static class DataTableExtension
{
    /// <summary>
    /// 转化一个DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static DataTable ToDataTable<T>(this IEnumerable<T> list)
    {
        //创建属性的集合
        var pList = new List<PropertyInfo>();
        //获得反射的入口

        var type = typeof(T);
        var dt = new DataTable();
        //把所有的public属性加入到集合 并添加DataTable的列
        Array.ForEach<PropertyInfo>(type.GetProperties(), p =>
        {
            pList.Add(p);
            var colType = p.PropertyType;
            if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                colType = colType.GetGenericArguments()[0];
            }

            dt.Columns.Add(p.Name, colType);
        });
        foreach (var item in list)
        {
            //创建一个DataRow实例
            var row = dt.NewRow();
            //给row 赋值
            pList.ForEach(p => row[p.Name] = p.GetValue(item, null) ?? DBNull.Value);
            //加入到DataTable
            dt.Rows.Add(row);
        }

        return dt;
    }


    /// <summary>
    /// DataTable 转换为List 集合
    /// </summary>
    /// <typeparam name="TResult">类型</typeparam>
    /// <param name="dt">DataTable</param>
    /// <returns></returns>
    public static List<T> ToList<T>(this DataTable dt) where T : class, new()
    {
        //创建一个属性的列表
        var prlist = new List<PropertyInfo>();
        //获取TResult的类型实例  反射的入口

        var t = typeof(T);

        //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表
        Array.ForEach<PropertyInfo>(t.GetProperties(), p =>
        {
            if (dt.Columns.IndexOf(p.Name) != -1)
            {
                prlist.Add(p);
            }
        });

        //创建返回的集合

        var oblist = new List<T>();

        foreach (DataRow row in dt.Rows)
        {
            //创建TResult的实例
            var ob = new T();
            //找到对应的数据  并赋值
            prlist.ForEach(p =>
            {
                if (row[p.Name] != DBNull.Value)
                {
                    p.SetValue(ob, row[p.Name], null);
                }
            });
            //放入到返回的集合中.
            oblist.Add(ob);
        }

        return oblist;
    }


    /// <summary>
    /// 将集合类转换成DataTable
    /// </summary>
    /// <param name="list">集合</param>
    /// <returns></returns>
    public static DataTable ToDataTableTow(IList list)
    {
        var result = new DataTable();
        if (list.Count > 0)
        {
            var propertys = list[0]!.GetType().GetProperties();

            foreach (var pi in propertys)
            {
                result.Columns.Add(pi.Name, pi.PropertyType);
            }

            for (var i = 0; i < list.Count; i++)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    var obj = pi.GetValue(list[i], null);
                    tempList.Add(obj);
                }

                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
        }

        return result;
    }


    /**/

    /// <summary>
    /// 将泛型集合类转换成DataTable
    /// </summary>
    /// <typeparam name="T">集合项类型</typeparam>
    /// <param name="list">集合</param>
    /// <returns>数据集(表)</returns>
    public static DataTable ToDataTable<T>(IList<T> list)
    {
        return ToDataTable<T>(list, null);
    }


    /**/

    /// <summary>
    /// 将泛型集合类转换成DataTable
    /// </summary>
    /// <typeparam name="T">集合项类型</typeparam>
    /// <param name="list">集合</param>
    /// <param name="propertyName">需要返回的列的列名</param>
    /// <returns>数据集(表)</returns>
    public static DataTable ToDataTable<T>(IList<T> list, params string[]? propertyName)
    {
        var propertyNameList = new List<string>();
        if (propertyName != null)
        {
            propertyNameList.AddRange(propertyName);
        }

        var result = new DataTable();
        if (list.Count > 0)
        {
            var propertys = list[0]!.GetType().GetProperties();
            foreach (var pi in propertys)
            {
                if (propertyNameList.Count == 0)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                else
                {
                    if (propertyNameList.Contains(pi.Name))
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
            }

            for (var i = 0; i < list.Count; i++)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        var obj = pi.GetValue(i, null);
                        tempList.Add(obj);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                        {
                            var obj = pi.GetValue(i, null);
                            tempList.Add(obj);
                        }
                    }
                }

                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
        }

        return result;
    }
}
