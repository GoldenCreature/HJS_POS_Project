using HJS_POS_Project.Database;
using HJS_POS_Project.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HJS_POS_Project.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        // 상품 목록
        private ObservableCollection<Product> _productList;
        public ObservableCollection<Product> ProductList
        {
            get { return _productList; }
            set { _productList = value; OnPropertyChanged("ProductList"); }
        }

        // 선택된 상품
        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
                CommandManager.InvalidateRequerySuggested();
                // 선택된 상품 정보를 입력 폼에 자동 입력
                if (_selectedProduct != null)
                {
                    InputName = _selectedProduct.Name;
                    InputCategory = _selectedProduct.Category;
                    InputPrice = _selectedProduct.Price.ToString();
                    InputStock = _selectedProduct.Stock.ToString();
                    InputImagePath = _selectedProduct.ImagePath;
                    LoadPreviewImage(InputImagePath);
                }
            }
        }

        // 검색 키워드
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged("SearchKeyword"); }
        }

        // 입력 필드들
        private string _inputName;
        public string InputName
        {
            get { return _inputName; }
            set { _inputName = value; OnPropertyChanged("InputName"); }
        }

        private string _inputCategory;
        public string InputCategory
        {
            get { return _inputCategory; }
            set { _inputCategory = value; OnPropertyChanged("InputCategory"); }
        }

        private string _inputPrice;
        public string InputPrice
        {
            get { return _inputPrice; }
            set { _inputPrice = value; OnPropertyChanged("InputPrice"); }
        }

        private string _inputStock;
        public string InputStock
        {
            get { return _inputStock; }
            set { _inputStock = value; OnPropertyChanged("InputStock"); }
        }

        // 이미지 경로 (DB에 저장될 값)
        private string _inputImagePath;
        public string InputImagePath
        {
            get { return _inputImagePath; }
            set { _inputImagePath = value; OnPropertyChanged("InputImagePath"); }
        }

        // 화면에 보여줄 미리보기 이미지
        private BitmapImage _previewImage;
        public BitmapImage PreviewImage
        {
            get { return _previewImage; }
            set { _previewImage = value; OnPropertyChanged("PreviewImage"); }
        }

        // 커맨드
        public ICommand SearchCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }

        // 생성자
        public ProductViewModel()
        {
            SearchCommand = new RelayCommand(SearchProduct);
            //AddCommand = new RelayCommand(AddProduct);
            AddCommand = new RelayCommand(AddProduct, () => SelectedProduct == null);
            UpdateCommand = new RelayCommand(UpdateProduct);
            DeleteCommand = new RelayCommand(DeleteProduct);
            SelectImageCommand = new RelayCommand(SelectImage);

            // 초기 상품 목록 로드
            try
            {
                LoadProducts();
            }
            catch
            {
                // DB 연결 안될 때 무시
            }
        }

        // 이미지 파일 선택
        private void SelectImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp";
            dialog.Title = "상품 이미지 선택";

            if (dialog.ShowDialog() == true)
            {
                // 프로젝트 실행 폴더 안에 ProductImages 폴더 생성
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ProductImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 파일명 중복 방지를 위해 GUID 사용
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dialog.FileName);
                string destPath = Path.Combine(folderPath, fileName);

                // 선택한 이미지를 폴더에 복사
                File.Copy(dialog.FileName, destPath, true);

                // 상대 경로로 저장 (DB에는 상대경로만 저장)
                InputImagePath = Path.Combine("Resources", "ProductImages", fileName);

                LoadPreviewImage(InputImagePath);
            }
        }

        // 미리보기 이미지 로드
        private void LoadPreviewImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                PreviewImage = null;
                return;
            }

            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                if (File.Exists(fullPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    PreviewImage = bitmap;
                }
                else
                {
                    PreviewImage = null;
                }
            }
            catch
            {
                PreviewImage = null;
            }
        }

        // 상품 목록 불러오기
        private void LoadProducts()
        {
            string query = "SELECT * FROM Products ORDER BY ProductID DESC";
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
                    Stock = (int)row["Stock"],
                    ImagePath = row["ImagePath"].ToString()
                });
            }
        }

        // 상품 검색
        private void SearchProduct()
        {
            string query = "SELECT * FROM Products WHERE Name LIKE @Keyword ORDER BY ProductID DESC";
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
                    Stock = (int)row["Stock"],
                    ImagePath = row["ImagePath"].ToString()
                });
            }

            // 검색할 때 입력 필드 초기화 (추가 버튼 다시 활성화)
            ClearInputs();
        }

        // 상품 추가
        private void AddProduct()
        {
            if (string.IsNullOrEmpty(InputName) || string.IsNullOrEmpty(InputPrice))
            {
                MessageBox.Show("상품명과 단가는 필수입니다.", "알림");
                return;
            }

            // 숫자 변환 검증
            decimal price;
            int stock;

            if (!decimal.TryParse(InputPrice, out price))
            {
                MessageBox.Show("단가를 올바르게 입력해주세요.", "알림");
                return;
            }

            if (!int.TryParse(InputStock, out stock))
            {
                stock = 0;
            }

            string query = @"INSERT INTO Products (Name, Category, Price, Stock, ImagePath) 
                            VALUES (@Name, @Category, @Price, @Stock, @ImagePath)";
            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", InputName),
                new SqlParameter("@Category", InputCategory ?? ""),
                new SqlParameter("@Price", price),
                new SqlParameter("@Stock", stock),
                new SqlParameter("@ImagePath", (object)InputImagePath ?? DBNull.Value)
            };

            DBHelper.ExecuteQuery(query, parameters);
            MessageBox.Show("상품이 추가됐습니다.", "완료");
            ClearInputs();
            LoadProducts();
        }

        // 상품 수정
        private void UpdateProduct()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("수정할 상품을 선택해주세요.", "알림");
                return;
            }

            // 숫자 변환 검증
            decimal price;
            int stock;

            if (!decimal.TryParse(InputPrice, out price))
            {
                MessageBox.Show("단가를 올바르게 입력해주세요.", "알림");
                return;
            }

            if (!int.TryParse(InputStock, out stock))
            {
                MessageBox.Show("재고 수량을 올바르게 입력해주세요.", "알림");
                return;
            }

            string query = @"UPDATE Products 
                            SET Name=@Name, Category=@Category, Price=@Price, Stock=@Stock, ImagePath=@ImagePath
                            WHERE ProductID=@ProductID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", InputName),
                new SqlParameter("@Category", InputCategory ?? ""),
                new SqlParameter("@Price", price),
                new SqlParameter("@Stock", stock),
                new SqlParameter("@ImagePath", (object)InputImagePath ?? DBNull.Value),
                new SqlParameter("@ProductID", SelectedProduct.ProductID)
            };

            DBHelper.ExecuteQuery(query, parameters);
            MessageBox.Show("상품이 수정됐습니다.", "완료");
            ClearInputs();
            LoadProducts();
        }

        // 상품 삭제
        private void DeleteProduct()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("삭제할 상품을 선택해주세요.", "알림");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"{SelectedProduct.Name}을 삭제하시겠습니까?",
                "삭제 확인",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                string query = "DELETE FROM Products WHERE ProductID=@ProductID";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ProductID", SelectedProduct.ProductID)
                };

                DBHelper.ExecuteQuery(query, parameters);
                MessageBox.Show("상품이 삭제됐습니다.", "완료");
                ClearInputs();
                LoadProducts();
            }
        }

        // 입력 필드 초기화
        private void ClearInputs()
        {
            InputName = "";
            InputCategory = "";
            InputPrice = "";
            InputStock = "";
            InputImagePath = "";
            PreviewImage = null;
            SelectedProduct = null;
        }
    }
}