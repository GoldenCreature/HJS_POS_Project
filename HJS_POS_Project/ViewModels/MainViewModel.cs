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

        // 생성자
        public MainViewModel()
        {
            ProductCommand = new RelayCommand(OpenProduct);
            SalesCommand = new RelayCommand(OpenSales);
            StatisticsCommand = new RelayCommand(OpenStatistics);
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
    }
}