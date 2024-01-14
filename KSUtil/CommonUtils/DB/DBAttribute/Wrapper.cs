using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    public class Wrapper<T> //where T : DBBaseModel<T>
    {
        private String sql = "";
        private String ConcatStr = "AND";
        private String last = "";
        private String orderbyStr = "";
        private List<String> selectFields = null;
        public Wrapper()
        {

        }
        #region select 指定字段
        public virtual Wrapper<T> select(params Expression<Func<T, object>>[] expressions)
        {
            if (expressions.Length != 0)
            {
                if (selectFields == null) selectFields = new List<String>();
                foreach (var expression in expressions)
                {
                    var name = DBBaseModel<T>.getSqlFeildName(expression);
                    selectFields.Add($"`{name}`");
                }
            }
            return this;
        }
        public virtual Wrapper<T> select(params String[] fields)
        {
            if (fields.Length != 0)
            {
                if (selectFields == null) selectFields = new List<String>();
                foreach (var field in fields)
                {
                    selectFields.Add($"`{field}`");
                }
            }
            return this;
        }
        public virtual Wrapper<T> selectN(params String[] fields)
        {
            if (fields.Length != 0)
            {
                if (selectFields == null) selectFields = new List<String>();
                foreach (var field in fields)
                {
                    selectFields.Add($"{field}");
                }
            }
            return this;
        }
        #endregion

        #region 表达式树生成
        public virtual Wrapper<T> calculate(Expression<Func<T, object>> expression, object val, String operatorStr, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} {operatorStr} '{val}'");
            }
            return this;
        }

        public virtual Wrapper<T> Lt(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return calculate(expression, val, "<", condition);
        }
        public virtual Wrapper<T> Gt(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return calculate(expression, val, ">", condition);
        }
        public virtual Wrapper<T> Le(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return calculate(expression, val, "<=", condition);
        }
        public virtual Wrapper<T> Ge(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return calculate(expression, val, ">=", condition);
        }
        public virtual Wrapper<T> Eq(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return calculate(expression, val, "=", condition);
        }
        public virtual Wrapper<T> Ne(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            return calculate(expression, val, "!=", condition);
        }
        public virtual Wrapper<T> BetWeen(Expression<Func<T, object>> expression, object val1, object val2, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} BETWEEN '{val1}' AND '{val2}'");
            }
            return this;
        }
        public virtual Wrapper<T> INSql(Expression<Func<T, object>> expression, String sql, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} IN ({sql})");
            }
            return this;
        }
        public virtual Wrapper<T> NotINSql(Expression<Func<T, object>> expression, String sql, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} NOT IN ({sql})");
            }
            return this;
        }
        public virtual Wrapper<T> IN(Expression<Func<T, object>> expression, IEnumerable<object> val)
        {
            var name = DBBaseModel<T>.getSqlFeildName(expression);
            concatSql($"{name} IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> NotIN(Expression<Func<T, object>> expression, IEnumerable<object> val)
        {
            var name = DBBaseModel<T>.getSqlFeildName(expression);
            concatSql($"{name} NOT IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> IN(Expression<Func<T, object>> expression, params object[] val)
        {
            var name = DBBaseModel<T>.getSqlFeildName(expression);
            concatSql($"{name} IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> NotIN(Expression<Func<T, object>> expression, params object[] val)
        {
            var name = DBBaseModel<T>.getSqlFeildName(expression);
            concatSql($"{name} NOT IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> LikeLeft(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} LIKE '%{val}'");
            }
            return this;
        }
        public virtual Wrapper<T> LikeRight(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} LIKE '{val}%'");
            }
            return this;
        }
        public virtual Wrapper<T> Like(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} LIKE '%{val}%'");
            }
            return this;
        }
        public virtual Wrapper<T> NotLike(Expression<Func<T, object>> expression, object val, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} NOT LIKE '%{val}%'");
            }
            return this;
        }
        public virtual Wrapper<T> IsNULL(Expression<Func<T, object>> expression, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} IS NULL");
            }
            return this;
        }
        public virtual Wrapper<T> IsNOTNULL(Expression<Func<T, object>> expression, bool condition = true)
        {
            if (condition)
            {
                var name = DBBaseModel<T>.getSqlFeildName(expression);
                concatSql($"{name} IS NOT NULL");
            }
            return this;
        }
        private Wrapper<T> OrderBy(string flag, params Expression<Func<T, object>>[] expressions)
        {
            var sqlFileds = "";
            foreach (var expression in expressions)
            {
                sqlFileds += (sqlFileds != "" ? "," : "") + DBBaseModel<T>.getSqlFeildName(expression);
            }
            this.orderbyStr = $"Order By {sqlFileds} {flag}";
            //this.Last($"Order By {sqlFileds} {flag}");
            return this;
        }
        public Wrapper<T> OrderByDesc(params Expression<Func<T, object>>[] expressions)
        {
            OrderBy("desc", expressions);
            return this;
        }
        public Wrapper<T> OrderByDesc(bool condition, params Expression<Func<T, object>>[] expressions)
        {
            if (condition)
            {
                OrderBy("desc", expressions);
            }
            return this;
        }
        public Wrapper<T> OrderByAsc(params Expression<Func<T, object>>[] expressions)
        {
            OrderBy("asc", expressions);
            return this;
        }
        public Wrapper<T> OrderByAsc(bool condition, params Expression<Func<T, object>>[] expressions)
        {
            if (condition)
            {
                OrderBy("asc", expressions);
            }
            return this;
        }
        #endregion

        #region 字符串生成
        public virtual Wrapper<T> calculate(string sqlField, object val, String operatorStr, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} {operatorStr} '{val}'");
            }
            return this;
        }
        public virtual Wrapper<T> Lt(string sqlField, object val, bool condition = true)
        {
            return calculate(sqlField, val, "<", condition);
        }
        public virtual Wrapper<T> Gt(string sqlField, object val, bool condition = true)
        {
            return calculate(sqlField, val, ">", condition);
        }
        public virtual Wrapper<T> Le(string sqlField, object val, bool condition = true)
        {
            return calculate(sqlField, val, "<=", condition);
        }
        public virtual Wrapper<T> Ge(string sqlField, object val, bool condition = true)
        {
            return calculate(sqlField, val, ">=", condition);
        }
        public virtual Wrapper<T> Eq(string sqlField, object val, bool condition = true)
        {
            return calculate(sqlField, val, "=", condition);
        }
        public virtual Wrapper<T> Ne(string sqlField, object val, bool condition = true)
        {
            return calculate(sqlField, val, "!=", condition);
        }
        public virtual Wrapper<T> BetWeen(string sqlField, object val1, object val2, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} BETWEEN '{val1}' AND '{val2}'");
            }
            return this;
        }
        public virtual Wrapper<T> INSql(string sqlField, String sql, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} IN ({sql})");
            }
            return this;
        }
        public virtual Wrapper<T> NotINSql(string sqlField, String sql, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} NOT IN ({sql})");
            }
            return this;
        }
        public virtual Wrapper<T> IN(string sqlField, params object[] val)
        {
            concatSql($"{sqlField} IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> NotIN(string sqlField, params object[] val)
        {
            concatSql($"{sqlField} NOT IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> IN(string sqlField, IEnumerable<object> val)
        {
            concatSql($"{sqlField} IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> NotIN(string sqlField, IEnumerable<object> val)
        {
            concatSql($"{sqlField} NOT IN ({val.joinList()})");
            return this;
        }
        public virtual Wrapper<T> LikeLeft(string sqlField, object val, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} LIKE '%{val}'");
            }
            return this;
        }
        public virtual Wrapper<T> LikeRight(string sqlField, object val, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} LIKE '{val}%'");
            }
            return this;
        }
        public virtual Wrapper<T> Like(string sqlField, object val, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} LIKE '%{val}%'");
            }
            return this;
        }
        public virtual Wrapper<T> NotLike(string sqlField, object val, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} NOT LIKE '%{val}%'");
            }
            return this;
        }
        public virtual Wrapper<T> IsNULL(string sqlField, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} IS NULL");
            }
            return this;
        }
        public virtual Wrapper<T> IsNOTNULL(string sqlField, bool condition = true)
        {
            if (condition)
            {
                concatSql($"{sqlField} IS NOT NULL");
            }
            return this;
        }
        private void OrderBy(string flag, params string[] fileds)
        {
            var sqlFileds = "";
            foreach (var filed in fileds)
            {
                sqlFileds += (sqlFileds != "" ? "," : "") + filed;
            }
            this.orderbyStr = $"Order By {sqlFileds} {flag}";
            // this.Last();
        }
        public void OrderByDesc(params string[] fileds)
        {
            OrderBy("desc", fileds);
        }
        public void OrderByDesc(bool condition, params string[] fileds)
        {
            if (condition)
            {
                OrderBy("desc", fileds);
            }
        }
        public void OrderByAsc(params string[] fileds)
        {
            OrderBy("asc", fileds);
        }
        public void OrderByAsc(bool condition, params string[] fileds)
        {
            if (condition)
            {
                OrderBy("asc", fileds);
            }
        }
        #endregion


        public virtual Wrapper<T> OR(Action<Wrapper<T>> action)
        {
            var query = new Wrapper<T>();
            action.Invoke(query);
            sql += (sql == "" ? $"{query.genIn()}" : $" OR {query.genIn()}");
            return this;
        }
        public virtual Wrapper<T> AND(Action<Wrapper<T>> action)
        {
            var query = new Wrapper<T>();
            action.Invoke(query);
            var insql = query.genIn();
            if (!string.IsNullOrEmpty(insql))
            {
                sql += (sql == "" ? $"{insql}" : $" AND {insql}");
            }
            return this;
        }
        public virtual Wrapper<T> Apply(string fun)
        {
            concatSql($" {fun} ");
            return this;
        }

        public virtual Wrapper<T> EXISTS(string sql, bool condition = true)
        {
            if (condition)
            {
                concatSql($"EXISTS ( {sql} )");// 1 = 
            }
            return this;
        }
        public virtual Wrapper<T> NotEXISTS(string sql, bool condition = true)
        {
            if (condition)
            {
                concatSql($"NOT EXISTS ( {sql} )");// 1 = 
            }
            return this;
        }
        public virtual Wrapper<T> OR()
        {
            ConcatStr = "OR";
            return this;
        }
        public virtual Wrapper<T> Last(String sql)
        {
            last = sql;
            return this;
        }
        public Wrapper<T> Limte(int start = 1, int end = 0)
        {
            string endStr = end == 0 ? "" : "," + end;
            this.Last($"limit {start} {endStr}");
            return this;
        }
        private void concatSql(string str)
        {
            sql += (sql == "" ? "" : $" {ConcatStr} ") + str;
            ConcatStr = "AND";
        }
        //生成内部拼接用的sql
        private string genIn()
        {
            return (sql == "" ? $"{last}" : $"({sql})");
        }
        //只生成查询条件
        public String gen()
        {
            return (sql == "" ? $"{last}" : $"({sql}) {orderbyStr} {last}");
        }
        //清理查询条件
        public void clean()
        {
            this.sql = "";
            this.ConcatStr = "";
            this.last = "";
            this.orderbyStr = "";
        }//生成完整sql
        public String genSql()
        {
            string filed = "*";
            if (selectFields != null)
            {
                filed = selectFields.join(",", false);
            }
            return $"select {filed} from `{DBBaseModel<T>.tableName}` {genWhere()}";

        }
        //生成加上 where 的sql
        public String genWhere()
        {
            return (sql == "" ? $"{last}" : $"where {gen()}");
        }

        public List<T> Execute()
        {
            return DBBaseModel<T>.getList(gen());
        }

        public DataTable ExecuteDt()
        {
            return DBBaseModel<T>.Query(genSql());
        }
    }
}
