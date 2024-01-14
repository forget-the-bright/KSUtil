using System;
using System.Linq.Expressions;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    public class DeleteWrapper<T> : Wrapper<T> //where T : DBBaseModel<T>
    {
       
        public DeleteWrapper()
        {

        }
        private DeleteWrapper<T> Limte(int start = 1, int end = 0)
        {
            return this;
        }
        public String genSql()
        {
            return $"delete from `{DBBaseModel<T>.tableName}`  {(base.gen()==""?"":$"where {base.gen()}")}";
        }
        public int Execute()
        {
            return DBBaseModel<T>.ExecuteSql(genSql());
        }


        public new DeleteWrapper<T> Lt(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Lt(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Gt(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Gt(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Le(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Le(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Ge(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Ge(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Eq(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Eq(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Ne(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Ne(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> BetWeen(Expression<Func<T, object>> expression, object val1, object val2, bool condition = true)
        {
            return base.BetWeen(expression, val1, val2, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> INSql(Expression<Func<T, object>> expression, string sql, bool condition = true)
        {
            return base.INSql(expression, sql, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> NotINSql(Expression<Func<T, object>> expression, string sql, bool condition = true)
        {
            return base.NotINSql(expression, sql, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> IN(Expression<Func<T, object>> expression, params object[] val)
        {
            return base.IN(expression, val) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> NotIN(Expression<Func<T, object>> expression, params object[] val)
        {
            return base.NotIN(expression, val) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> LikeLeft(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.LikeLeft(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> LikeRight(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.LikeRight(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Like(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.Like(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> NotLike(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return base.NotLike(expression, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> IsNULL(Expression<Func<T, object>> expression, bool condition = true)
        {
            return base.IsNULL(expression, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> IsNOTNULL(Expression<Func<T, object>> expression, bool condition = true)
        {
            return base.IsNOTNULL(expression, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Lt(string sqlField, object val, bool condition = true)
        {
            return base.Lt(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Gt(string sqlField, object val, bool condition = true)
        {
            return base.Gt(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Le(string sqlField, object val, bool condition = true)
        {
            return base.Le(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Ge(string sqlField, object val, bool condition = true)
        {
            return base.Ge(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Eq(string sqlField, object val, bool condition = true)
        {
            return base.Eq(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Ne(string sqlField, object val, bool condition = true)
        {
            return base.Ne(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> BetWeen(string sqlField, object val1, object val2, bool condition = true)
        {
            return base.BetWeen(sqlField, val1, val2, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> INSql(string sqlField, string sql, bool condition = true)
        {
            return base.INSql(sqlField, sql, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> NotINSql(string sqlField, string sql, bool condition = true)
        {
            return base.NotINSql(sqlField, sql, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> IN(string sqlField, params object[] val)
        {
            return base.IN(sqlField, val) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> NotIN(string sqlField, params object[] val)
        {
            return base.NotIN(sqlField, val) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> LikeLeft(string sqlField, object val, bool condition = true)
        {
            return base.LikeLeft(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> LikeRight(string sqlField, object val, bool condition = true)
        {
            return base.LikeRight(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Like(string sqlField, object val, bool condition = true)
        {
            return base.Like(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> NotLike(string sqlField, object val, bool condition = true)
        {
            return base.NotLike(sqlField, val, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> IsNULL(string sqlField, bool condition = true)
        {
            return base.IsNULL(sqlField, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> IsNOTNULL(string sqlField, bool condition = true)
        {
            return base.IsNOTNULL(sqlField, condition) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> OR(Action<Wrapper<T>> action)
        {
            return base.OR(action) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> AND(Action<Wrapper<T>> action)
        {
            return base.AND(action) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Apply(string fun)
        {
            return base.Apply(fun) as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> OR()
        {
            return base.OR() as DeleteWrapper<T>;
        }

        public new DeleteWrapper<T> Last(string sql)
        {
            return base.Last(sql) as DeleteWrapper<T>;
        }
    }
}
