using HJS_POS_Project.Database;
using HJS_POS_Project.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HJS_POS_Project.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // 아이디 속성
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }

        // 비밀번호 속성
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        // 로그인 커맨드
        public ICommand LoginCommand { get; set; }

        // 생성자
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        // 로그인 로직
        public void Login()
        {
            // 입력값 검증
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("아이디와 비밀번호를 입력해주세요.", "알림");
                return;
            }

            // DB에서 사용자 조회
            string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            System.Data.SqlClient.SqlParameter[] parameters =
            {
                new System.Data.SqlClient.SqlParameter("@Username", Username),
                new System.Data.SqlClient.SqlParameter("@Password", Password)
            };

            DataTable dt = DBHelper.GetData(query, parameters);

            if (dt.Rows.Count > 0)
            {
                // 로그인 성공 - 사용자 정보를 CurrentUser에 저장
                CurrentUser.Username = dt.Rows[0]["Username"].ToString();
                CurrentUser.Role = dt.Rows[0]["Role"].ToString();

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Application.Current.Windows[0].Close();
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 틀렸습니다.", "로그인 실패");
            }
        }
    }
}
