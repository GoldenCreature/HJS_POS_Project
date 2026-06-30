using HJS_POS_Project.Database;
using HJS_POS_Project.Views;
using System.Windows;
using System.Windows.Input;

namespace HJS_POS_Project.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // 상품 관리 커맨드
        public ICommand ProductCommand { get; set; }

        // 판매 처리 커맨드
        public ICommand SalesCommand { get; set; }

        // 매출 통계 커맨드
        public ICommand StatisticsCommand { get; set; }

        // 로그아웃 커맨드
        public ICommand LogoutCommand { get; set; }

        // 생성자
        public MainViewModel()
        {
            ProductCommand = new RelayCommand(OpenProduct);
            SalesCommand = new RelayCommand(OpenSales);
            StatisticsCommand = new RelayCommand(OpenStatistics);
            LogoutCommand = new RelayCommand(Logout);
        }

        // 상품 관리 화면 열기 (관리자만 접근 가능)
        private void OpenProduct()
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("관리자 권한이 필요합니다.", "접근 제한");
                return;
            }

            ProductView productView = new ProductView();
            productView.Show();
        }

        // 판매 처리 화면 열기
        private void OpenSales()
        {
            SalesView salesView = new SalesView();
            salesView.Show();
        }

        // 매출 통계 화면 열기 (관리자만 접근 가능)
        private void OpenStatistics()
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("관리자 권한이 필요합니다.", "접근 제한");
                return;
            }

            StatisticsView statisticsView = new StatisticsView();
            statisticsView.Show();
        }

        // 로그아웃 - 사용자 정보 초기화 후 로그인 창으로 이동
        private void Logout()
        {
            MessageBoxResult result = MessageBox.Show(
                "로그아웃 하시겠습니까?",
                "로그아웃 확인",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                CurrentUser.Logout();

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                // 현재 MainWindow 닫기
                Application.Current.Windows[0].Close();
            }
        }
    }
}