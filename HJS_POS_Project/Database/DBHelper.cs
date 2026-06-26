using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HJS_POS_Project.Database
{
    public class DBHelper
    {
        // App.config에서 연결 문자열 읽어오기
        private static string connStr =
            ConfigurationManager.ConnectionStrings["POS_DB"].ConnectionString;

        // SELECT 쿼리 실행 → DataTable로 반환
        public static DataTable GetData(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (parameters != null)
                    da.SelectCommand.Parameters.AddRange(parameters);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // INSERT, UPDATE, DELETE 쿼리 실행
        public static void ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        // 연결 테스트
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}