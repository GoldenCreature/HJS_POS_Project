using HJS_POS_Project.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace HJS_POS_Project.Views
{
    /// <summary>
    /// SalesView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SalesView : Window
    {
        public SalesView()
        {
            InitializeComponent();
        }

        // 결제 수단 라디오버튼 선택 시 ViewModel에 값 전달
        private void PaymentType_Checked(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SalesViewModel;
            if (vm == null) return;

            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                vm.SelectedPaymentType = rb.Content.ToString();
            }
        }
    }
}