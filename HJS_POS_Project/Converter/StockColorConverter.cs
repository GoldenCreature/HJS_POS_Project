using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HJS_POS_Project.Converters
{
    // 재고 수량에 따라 글자 색상을 바꿔주는 Converter
    // 재고가 10개 미만이면 빨간색, 그 외에는 검정색
    public class StockColorConverter : IValueConverter
    {
        // 재고 부족 기준값
        private const int LowStockThreshold = 10;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock <= 0)
                    return Brushes.Red;           // 재고 0 - 빨강
                else if (stock < LowStockThreshold)
                    return Brushes.Gold;          // 재고 부족 - 노랑(금색 계열)
            }
            return Brushes.Black;                  // 정상 - 검정
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}