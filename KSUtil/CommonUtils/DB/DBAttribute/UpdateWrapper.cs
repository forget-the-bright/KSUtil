using System;
using System.Linq.Expressions;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    public class UpdateWrapper<T> : Wrapper<T> //where T : DBBaseModel<T>
    {

        private String SetSql = "";
        public UpdateWrapper()
        {

        }
        private UpdateWrapper<T> Limte(int start = 1, int end = 0)
        {
            return this;
        }
        public UpdateWrapper<T> Set(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                if (val is bool)
                {
                    concatSetSql($"{name} = {val}");
                }
                else
                {
                    concatSetSql($"{name} = '{val}'");
                }
            }
            return this;
        }

        private void concatSetSql(string str)
        {
            SetSql += (SetSql == "" ? " SET " : $" , ") + str;
        }
        public new void clean()
        {
            this.SetSql = "";
            base.clean();
        }

        public new String genSql()
        {
            return $"update `{DBBaseModel<T>.tableName}` {SetSql} {(base.gen() == "" ? "" : $"where {base.gen()}")}";
        }
        public new int Execute()
        {
            return DBBaseModel<T>.ExecuteSql(genSql());
        }

        public new UpdateWrapper<T> Lt(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Lt(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Gt(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Gt(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Le(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Le(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Ge(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Ge(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Eq(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Eq(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Ne(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Ne(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> BetWeen(Expression<Func<T, object>> expression, object val1, object val2, bool condition = true)
        {
            return base.BetWeen(expression, val1, val2, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> INSql(Expression<Func<T, object>> expression, string sql, bool condition = true)
        {
            return base.INSql(expression, sql, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> NotINSql(Expression<Func<T, object>> expression, string sql, bool condition = true)
        {
            return base.NotINSql(expression, sql, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> IN(Expression<Func<T, object>> expression, params object[] val)
        {
            return base.IN(expression, val) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> NotIN(Expression<Func<T, object>> expression, params object[] val)
        {
            return base.NotIN(expression, val) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> LikeLeft(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.LikeLeft(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> LikeRight(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.LikeRight(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Like(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Like(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> NotLike(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.NotLike(expression, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> IsNULL(Expression<Func<T, object>> expression, bool condition = true)
        {
            return base.IsNULL(expression, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> IsNOTNULL(Expression<Func<T, object>> expression, bool condition = true)
        {
            return base.IsNOTNULL(expression, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Lt(string sqlField, object val, bool condition = true)
        {
            return base.Lt(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Gt(string sqlField, object val, bool condition = true)
        {
            return base.Gt(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Le(string sqlField, object val, bool condition = true)
        {
            return base.Le(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Ge(string sqlField, object val, bool condition = true)
        {
            return base.Ge(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Eq(string sqlField, object val, bool condition = true)
        {
            return base.Eq(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Ne(string sqlField, object val, bool condition = true)
        {
            return base.Ne(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> BetWeen(string sqlField, object val1, object val2, bool condition = true)
        {
            return base.BetWeen(sqlField, val1, val2, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> INSql(string sqlField, string sql, bool condition = true)
        {
            return base.INSql(sqlField, sql, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> NotINSql(string sqlField, string sql, bool condition = true)
        {
            return base.NotINSql(sqlField, sql, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> IN(string sqlField, params object[] val)
        {
            return base.IN(sqlField, val) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> NotIN(string sqlField, params object[] val)
        {
            return base.NotIN(sqlField, val) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> LikeLeft(string sqlField, object val, bool condition = true)
        {
            return base.LikeLeft(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> LikeRight(string sqlField, object val, bool condition = true)
        {
            return base.LikeRight(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Like(string sqlField, object val, bool condition = true)
        {
            return base.Like(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> NotLike(string sqlField, object val, bool condition = true)
        {
            return base.NotLike(sqlField, val, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> IsNULL(string sqlField, bool condition = true)
        {
            return base.IsNULL(sqlField, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> IsNOTNULL(string sqlField, bool condition = true)
        {
            return base.IsNOTNULL(sqlField, condition) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> OR(Action<Wrapper<T>> action)
        {
            return base.OR(action) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> AND(Action<Wrapper<T>> action)
        {
            return base.AND(action) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Apply(string fun)
        {
            return base.Apply(fun) as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> OR()
        {
            return base.OR() as UpdateWrapper<T>;
        }

        public new UpdateWrapper<T> Last(string sql)
        {
            return base.Last(sql) as UpdateWrapper<T>;
        }
    }
}
