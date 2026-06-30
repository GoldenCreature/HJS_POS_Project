using HJS_POS_Project.Database;
using HJS_POS_Project.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
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

        // OxyPlot 차트 모델
        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        // 차트가 비어있는지 여부 (안내 문구 표시용)
        private Visibility _isChartEmpty = Visibility.Visible;
        public Visibility IsChartEmpty
        {
            get { return _isChartEmpty; }
            set { _isChartEmpty = value; OnPropertyChanged("IsChartEmpty"); }
        }

        // 통계 모드 (true: 일간, false: 월간)
        private bool _isDailyMode = true;

        // 일간 모드 커맨드
        public ICommand DailyModeCommand { get; set; }

        // 월간 모드 커맨드
        public ICommand MonthlyModeCommand { get; set; }

        // 커맨드
        public ICommand SearchCommand { get; set; }

        // 생성자
        public StatisticsViewModel()
        {
            SalesList = new ObservableCollection<Order>();
            SearchCommand = new RelayCommand(SearchSales);

            // 일간/월간 모드 전환 커맨드 초기화
            DailyModeCommand = new RelayCommand(() =>
            {
                _isDailyMode = true;
                StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);  // 이번 달 1일
                EndDate = DateTime.Today;
                SearchSales();
            });

            MonthlyModeCommand = new RelayCommand(() =>
            {
                _isDailyMode = false;
                StartDate = new DateTime(DateTime.Today.Year, 1, 1);  // 이번 년도 1월 1일
                EndDate = DateTime.Today;
                SearchSales();
            });

            PlotModel = new PlotModel { Title = "" };
        }

        // 매출 조회
        private void SearchSales()
        {
            string query = @"SELECT * FROM Orders 
                            WHERE OrderDate >= @StartDate 
                            AND OrderDate <= @EndDate
                            ORDER BY OrderDate ASC";
            SqlParameter[] parameters =
            {
                new SqlParameter("@StartDate", StartDate.Date),
                new SqlParameter("@EndDate", EndDate.Date.AddDays(1))
            };

            DataTable dt = DBHelper.GetData(query, parameters);

            SalesList = new ObservableCollection<Order>();
            decimal total = 0;

            // 날짜별 매출 합계를 담을 딕셔너리
            Dictionary<string, decimal> dailySales = new Dictionary<string, decimal>();

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

                // 날짜별로 매출 합산 (MM/dd 형식)
                string dateKey = _isDailyMode
                    ? order.OrderDate.ToString("MM/dd")
                    : order.OrderDate.ToString("yyyy/MM");
                if (dailySales.ContainsKey(dateKey))
                    dailySales[dateKey] += order.TotalAmount;
                else
                    dailySales[dateKey] = order.TotalAmount;
            }

            TotalSales = total;

            // 차트 그리기
            if (dailySales.Count > 0)
            {
                // 차트 모델 생성 + 범례(legend) 위치와 스타일 설정
                // 범례는 차트 우측 상단 안쪽에, 세로로, 테두리 없이 표시
                var newModel = new PlotModel
                {
                    Title = _isDailyMode ? "일별 매출" : "월별 매출",
                    Legends = { new Legend
                    {
                        LegendPosition = LegendPosition.RightTop,
                        LegendPlacement = LegendPlacement.Outside,
                        LegendOrientation = LegendOrientation.Vertical,
                        LegendBorderThickness = 0
                    }}
                };

                // X축 역할 (카테고리 - 날짜) - Bottom에 위치, Key 지정
                // Key를 지정해야 BarSeries에서 XAxisKey/YAxisKey로 명시적으로 연결 가능
                var categoryAxis = new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    Key = "DateAxis"
                };
                foreach (var key in dailySales.Keys)
                {
                    categoryAxis.Labels.Add(key);
                }

                // Y축 역할 (매출 금액) - Left에 위치, Key 지정
                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Key = "ValueAxis",
                    Minimum = 0
                };

                newModel.Axes.Add(categoryAxis);
                newModel.Axes.Add(valueAxis);

                // 최고/최저 매출 찾기 (막대 색상 구분에 사용)
                decimal maxValue = 0;
                decimal minValue = decimal.MaxValue;
                foreach (var value in dailySales.Values)
                {
                    if (value > maxValue) maxValue = value;
                    if (value < minValue) minValue = value;
                }

                // 색상 정의: 최고 매출(밝은 파란색), 최저 매출(빨간색), 일반(기존 남색)
                var maxColor = OxyColor.Parse("#4D8FE8");
                var minColor = OxyColor.Parse("#E84D4D");
                var normalColor = OxyColor.Parse("#1E2761");

                // 세로 막대 그래프 (BarSeries를 transpose해서 세로로 표시)
                // XAxisKey/YAxisKey를 서로 바꿔 지정하면 가로 막대 대신 세로 막대가 그려짐
                var barSeries = new BarSeries
                {
                    XAxisKey = "ValueAxis",
                    YAxisKey = "DateAxis",
                    BarWidth = 20
                };

                // 각 날짜별 매출 금액에 따라 막대 색상을 다르게 지정
                foreach (var value in dailySales.Values)
                {
                    OxyColor barColor;

                    if (value == maxValue)
                        barColor = maxColor;
                    else if (value == minValue)
                        barColor = minColor;
                    else
                        barColor = normalColor;

                    barSeries.Items.Add(new BarItem { Value = (double)value, Color = barColor });
                }

                newModel.Series.Add(barSeries);

                // 범례 표시용 더미 시리즈
                // BarSeries는 항목별 색상을 범례에 자동으로 표시해주지 않기 때문에
                // 실제 데이터 없는 빈 LineSeries를 만들어 Title과 Color만 지정해서
                // 범례에 "최고 매출", "최저 매출" 항목이 보이도록 트릭을 사용
                var maxLegend = new LineSeries
                {
                    Title = "최고 매출",
                    Color = maxColor,
                    StrokeThickness = 6
                };
                var minLegend = new LineSeries
                {
                    Title = "최저 매출",
                    Color = minColor,
                    StrokeThickness = 6
                };

                newModel.Series.Add(maxLegend);
                newModel.Series.Add(minLegend);

                PlotModel = newModel;
                IsChartEmpty = Visibility.Collapsed;
            }
            else
            {
                PlotModel = new PlotModel { Title = _isDailyMode ? "일별 매출" : "월별 매출" };
                IsChartEmpty = Visibility.Visible;
            }
        }
    }
}