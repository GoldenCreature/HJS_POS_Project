using System;
using System.Collections.Generic;
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
using HJS_POS_Project.Database;

namespace HJS_POS_Project.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // DB 연결 테스트
            /*
            if (DBHelper.TestConnection())
                MessageBox.Show("DB 연결 성공!");
            else
                MessageBox.Show("DB 연결 실패!");
            */
        }
    }
}
