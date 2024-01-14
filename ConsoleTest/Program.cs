using KSUtil.CommonUtils.DB;
using KSUtil.CommonUtils.DB.DBAttribute;
using System.Data;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DbHelperMySQL.connectionString = "server=10.40.18.18;port=3306;user=hbl;password=hbl_password;database=mesdb_nca_test;Allow User Variables=True;SslMode=None";

            DBBaseModel<Object>.TransactionMethod(() =>
            {
                DataTable dataTable = DBBaseModel<Object>.Query("select * from sys_user");
                Console.WriteLine(dataTable);
            });
            Console.WriteLine("Hello, World!");
        }
    }
}
