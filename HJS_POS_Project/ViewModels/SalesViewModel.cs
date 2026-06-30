using HJS_POS_Project.Database;
using HJS_POS_Project.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace HJS_POS_Project.ViewModels
{
    public class SalesViewModel : ViewModelBase
    {
        // 상품 목록
        private ObservableCollection<Product> _productList;
        public ObservableCollection<Product> ProductList
        {
            get { return _productList; }
            set { _productList = value; OnPropertyChanged("ProductList"); }
        }

        // 장바구니 목록
        private ObservableCollection<Product> _cartList;
        public ObservableCollection<Product> CartList
        {
            get { return _cartList; }
            set { _cartList = value; OnPropertyChanged("CartList"); }
        }

        // 선택된 상품
        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged("SelectedProduct"); }
        }

        // 선택된 장바구니 아이템
        private Product _selectedCartItem;
        public Product SelectedCartItem
        {
            get { return _selectedCartItem; }
            set { _selectedCartItem = value; OnPropertyChanged("SelectedCartItem"); }
        }

        // 검색 키워드
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged("SearchKeyword"); }
        }

        // 수량
        private string _quantity = "1";
        public string Quantity
        {
            get { return _quantity; }
            set { _quantity = value; OnPropertyChanged("Quantity"); }
        }

        // 합계 금액
        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; OnPropertyChanged("TotalAmount"); }
        }

        // 커맨드
        public ICommand SearchCommand { get; set; }
        public ICommand AddToCartCommand { get; set; }
        public ICommand PayCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        // 생성자
        public SalesViewModel()
        {
            CartList = new ObservableCollection<Product>();
            SearchCommand = new RelayCommand(SearchProduct);
            AddToCartCommand = new RelayCommand(AddToCart);
            PayCommand = new RelayCommand(Pay);
            CancelCommand = new RelayCommand(Cancel);

            // 초기 상품 목록 로드
            LoadProducts();
        }

        // 상품 목록 불러오기
        private void LoadProducts()
        {
            string query = "SELECT * FROM Products ORDER BY Name";
            DataTable dt = DBHelper.GetData(query);

            ProductList = new ObservableCollection<Product>();
            foreach (DataRow row in dt.Rows)
            {
                ProductList.Add(new Product
                {
                    ProductID = (int)row["ProductID"],
                    Name = row["Name"].ToString(),
                    Category = row["Category"].ToString(),
                    Price = (decimal)row["Price"],
                    Stock = (int)row["Stock"]
                });
            }
        }

        // 상품 검색
        private void SearchProduct()
        {
            string query = "SELECT * FROM Products WHERE Name LIKE @Keyword ORDER BY Name";
            SqlParameter[] parameters =
            {
                new SqlParameter("@Keyword", "%" + SearchKeyword + "%")
            };
            DataTable dt = DBHelper.GetData(query, parameters);

            ProductList = new ObservableCollection<Product>();
            foreach (DataRow row in dt.Rows)
            {
                ProductList.Add(new Product
                {
                    ProductID = (int)row["ProductID"],
                    Name = row["Name"].ToString(),
                    Category = row["Category"].ToString(),
                    Price = (decimal)row["Price"],
                    Stock = (int)row["Stock"]
                });
            }
        }

        // 장바구니에 추가
        private void AddToCart()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("상품을 선택해주세요.", "알림");
                return;
            }

            int qty;
            if (!int.TryParse(Quantity, out qty) || qty <= 0)
            {
                MessageBox.Show("수량을 올바르게 입력해주세요.", "알림");
                return;
            }

            if (qty > SelectedProduct.Stock)
            {
                MessageBox.Show("재고가 부족합니다.", "알림");
                return;
            }

            // 이미 장바구니에 있는 상품이면 수량 추가
            foreach (var item in CartList)
            {
                if (item.ProductID == SelectedProduct.ProductID)
                {
                    item.Stock += qty;
                    item.Price = SelectedProduct.Price * item.Stock;
                    UpdateTotal();
                    return;
                }
            }

            // 새 상품 장바구니에 추가
            CartList.Add(new Product
            {
                ProductID = SelectedProduct.ProductID,
                Name = SelectedProduct.Name,
                Stock = qty,
                Price = SelectedProduct.Price * qty
            });

            UpdateTotal();
        }

        // 합계 계산
        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (var item in CartList)
            {
                total += item.Price;
            }
            TotalAmount = total;
        }

        // 결제 처리
        private void Pay()
        {
            if (CartList.Count == 0)
            {
                MessageBox.Show("장바구니가 비어있습니다.", "알림");
                return;
            }

            // Orders 테이블에 주문 INSERT
            string orderQuery = @"INSERT INTO Orders (TotalAmount, PaymentType) 
                                 VALUES (@TotalAmount, @PaymentType);
                                 SELECT SCOPE_IDENTITY();";
            SqlParameter[] orderParams =
            {
                new SqlParameter("@TotalAmount", TotalAmount),
                new SqlParameter("@PaymentType", "현금")
            };

            // 주문 ID 가져오기
            DataTable orderDt = DBHelper.GetData(orderQuery, orderParams);
            int orderId = int.Parse(orderDt.Rows[0][0].ToString());

            // OrderDetails 및 재고 차감
            foreach (var item in CartList)
            {
                // OrderDetails INSERT
                string detailQuery = @"INSERT INTO OrderDetails (OrderID, ProductID, Qty, UnitPrice)
                                      VALUES (@OrderID, @ProductID, @Qty, @UnitPrice)";
                SqlParameter[] detailParams =
                {
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@ProductID", item.ProductID),
                    new SqlParameter("@Qty", item.Stock),
                    new SqlParameter("@UnitPrice", item.Price / item.Stock)
                };
                DBHelper.ExecuteQuery(detailQuery, detailParams);

                // 재고 차감
                string stockQuery = @"UPDATE Products 
                                     SET Stock = Stock - @Qty 
                                     WHERE ProductID = @ProductID";
                SqlParameter[] stockParams =
                {
                    new SqlParameter("@Qty", item.Stock),
                    new SqlParameter("@ProductID", item.ProductID)
                };
                DBHelper.ExecuteQuery(stockQuery, stockParams);
            }

            MessageBox.Show($"결제 완료!\n총 금액: {TotalAmount:N0}원", "결제 완료");
            Cancel();
            LoadProducts();  // 상품 목록 새로고침
        }

        // 장바구니 초기화
        private void Cancel()
        {
            CartList.Clear();
            TotalAmount = 0;
            Quantity = "1";
        }
    }
}