using HJS_POS_Project.Database;
using HJS_POS_Project.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace HJS_POS_Project.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        // 매출 목록
        private ObservableCollection<Order> _salesList;
        public ObservableCollection<Order> SalesList
        {
            get { return _salesList; }
            set { _salesList = value; OnPropertyChanged("SalesList"); }
        }

        // 시작일
        private DateTime _startDate = DateTime.Today.AddMonths(-1);
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; OnPropertyChanged("StartDate"); }
        }

        // 종료일
        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; OnPropertyChanged("EndDate"); }
        }

        // 총 매출
        private decimal _totalSales;
        public decimal TotalSales
        {
            get { return _totalSales; }
            set { _totalSales = value; OnPropertyChanged("TotalSales"); }
        }

        // 커맨드
        public ICommand SearchCommand { get; set; }

        // 생성자
        public StatisticsViewModel()
        {
            SalesList = new ObservableCollection<Order>();
            SearchCommand = new RelayCommand(SearchSales);
        }

        // 매출 조회
        private void SearchSales()
        {
            string query = @"SELECT * FROM Orders 
                            WHERE OrderDate >= @StartDate 
                            AND OrderDate <= @EndDate
                            ORDER BY OrderDate DESC";
            SqlParameter[] parameters =
            {
                new SqlParameter("@StartDate", StartDate.Date),
                new SqlParameter("@EndDate", EndDate.Date.AddDays(1))
            };

            DataTable dt = DBHelper.GetData(query, parameters);

            SalesList = new ObservableCollection<Order>();
            decimal total = 0;

            foreach (DataRow row in dt.Rows)
            {
                Order order = new Order
                {
                    OrderID = (int)row["OrderID"],
                    OrderDate = (DateTime)row["OrderDate"],
                    TotalAmount = (decimal)row["TotalAmount"],
                    PaymentType = row["PaymentType"].ToString()
                };
                SalesList.Add(order);
                total += order.TotalAmount;
            }

            TotalSales = total;
        }
    }
}