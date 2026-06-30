using HJS_POS_Project.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HJS_POS_Project.Views
{
    public partial class ReceiptWindow : Window
    {
        public ReceiptWindow()
        {
            InitializeComponent();
        }

        // 영수증에 표시할 데이터를 받아서 화면 채우는 메서드
        public void LoadReceipt(List<Product> cartItems, decimal totalAmount, string paymentType)
        {
            // 상품 목록을 영수증 표시용 형태로 변환
            var displayItems = new List<ReceiptItem>();
            foreach (var item in cartItems)
            {
                displayItems.Add(new ReceiptItem
                {
                    Name = item.Name,
                    QtyText = $"{item.Stock}개",
                    PriceText = $"{item.Price:N0}원"
                });
            }

            icItems.ItemsSource = displayItems;
            txtTotal.Text = $"{totalAmount:N0}원";
            txtPaymentType.Text = paymentType;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        // 영수증 한 줄을 표현하는 내부 클래스
        private class ReceiptItem
        {
            public string Name { get; set; }
            public string QtyText { get; set; }
            public string PriceText { get; set; }
        }
    }
}