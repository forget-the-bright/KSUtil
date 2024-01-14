using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    public class BaseModel
    {

        public static void update<T>(UpdateWrapper<T> wrapper) where T : DBBaseModel<T>
        {
            DBBaseModel<T>.ExecuteSql(wrapper.genSql());
        }
        public static List<T> list<T>(Wrapper<T> wrapper) where T : DBBaseModel<T>
        {
            DataTable dataTable = DBBaseModel<T>.Query(wrapper.genSql());
            return DBAttributeUtil.fillData<T>(dataTable);
        }
        public static T getOne<T>(Wrapper<T> wrapper) where T : DBBaseModel<T>
        {
            DataTable dataTable = DBBaseModel<T>.Query(wrapper.genSql());
            var list = DBAttributeUtil.fillData<T>(dataTable);
            return list.FirstOrDefault();
        }

        public static T getOne<T>(Action<Wrapper<T>> wrapper) where T : DBBaseModel<T>
        {
            var query = new Wrapper<T>();
            wrapper(query);
            DataTable dataTable = DBBaseModel<T>.Query(query.genSql());
            var list = DBAttributeUtil.fillData<T>(dataTable);
            return list.FirstOrDefault();
        }
    }
}
