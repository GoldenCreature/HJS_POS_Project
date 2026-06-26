using HJS_POS_Project.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HJS_POS_Project.Views
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // 입력값 검증
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("아이디와 비밀번호를 입력해주세요.", "알림");
                return;
            }

            // DB에서 사용자 조회
            string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            System.Data.SqlClient.SqlParameter[] parameters =
            {
                new System.Data.SqlClient.SqlParameter("@Username", username),
                new System.Data.SqlClient.SqlParameter("@Password", password)
            };

            DataTable dt = DBHelper.GetData(query, parameters);

            if (dt.Rows.Count > 0)
            {
                // 로그인 성공
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 틀렸습니다.", "로그인 실패");
            }
        }
    }
}
