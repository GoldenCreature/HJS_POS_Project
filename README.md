# 🏪 HJS POS System

> C# WPF + SQL Server 기반 재고·판매 관리 POS 시스템  
> 1인 개발 포트폴리오 프로젝트

---

## 📌 프로젝트 소개

소규모 매장에서 사용할 수 있는 POS(Point of Sale) 시스템입니다.  
상품 등록부터 판매 처리, 매출 통계까지 매장 운영에 필요한 핵심 기능을 구현했습니다.

---

## 🛠 기술 스택

| 항목 | 내용 |
|---|---|
| 언어 | C# (.NET Framework 4.7.2) |
| UI 프레임워크 | WPF (Windows Presentation Foundation) |
| 아키텍처 패턴 | MVVM (Model-View-ViewModel) |
| 데이터베이스 | SQL Server 2022 Express |
| DB 연동 | ADO.NET |
| 차트 라이브러리 | OxyPlot.Wpf |
| 버전 관리 | Git / GitHub |
| 개발 환경 | Visual Studio 2022 |

---

## 📦 NuGet 패키지

| 패키지 | 버전 | 용도 |
|---|---|---|
| System.Data.SqlClient | 4.9.0 | SQL Server 연결 및 쿼리 실행 (ADO.NET) |
| OxyPlot.Wpf | 2.2.0 | 매출 통계 막대 차트 시각화 |

- **System.Data.SqlClient**: .NET Framework 4.7.2 환경에서 SQL Server와 안정적으로 연동하기 위해 사용
- **OxyPlot.Wpf**: WPF 기본 Chart 컨트롤이 없어 외부 라이브러리 도입. LiveCharts2와 비교 검토 후 .NET Framework 호환성이 검증된 OxyPlot 선택

---

## ✅ 주요 기능

### 🔐 로그인 및 권한 관리
- SQL Server 기반 사용자 인증
- 관리자(admin) / 직원(staff) 권한 분리
- 관리자만 상품 관리 및 매출 통계 접근 가능
- 로그아웃 기능

### 📦 상품 관리 (관리자 전용)
- 상품 등록 / 수정 / 삭제 (CRUD)
- 상품 이미지 업로드 및 미리보기
- 상품명 검색 기능
- 재고 부족 경고 색상 표시 (10개 미만: 노랑, 0개: 빨강)

### 🛒 판매 처리
- 상품 검색 및 장바구니 담기
- 선택 상품 이미지 미리보기
- 결제수단 선택 (현금 / 카드)
- 결제 처리 시 재고 자동 차감
- 결제 완료 후 영수증 미리보기 창 출력

### 📊 매출 통계 (관리자 전용)
- 기간별 매출 조회 (시작일 ~ 종료일)
- 일간 / 월간 전환 버튼
- OxyPlot 막대 차트 시각화
- 최고 매출일 (파란색) / 최저 매출일 (빨간색) 강조 표시
- 주문 목록 및 총 매출 표시

---

## 🗄 데이터베이스 설계

```sql
Users (사용자)
├── UserID (PK)
├── Username
├── Password
└── Role

Products (상품)
├── ProductID (PK)
├── Name
├── Category
├── Price
├── Stock
└── ImagePath

Orders (주문)
├── OrderID (PK)
├── OrderDate
├── TotalAmount
└── PaymentType

OrderDetails (주문상세)
├── DetailID (PK)
├── OrderID (FK → Orders)
├── ProductID (FK → Products)
├── Qty
└── UnitPrice
```

---

## 📁 프로젝트 구조
HJS_POS_Project
├── Database
│   ├── DBHelper.cs         # DB 연결 및 쿼리 실행
│   └── CurrentUser.cs      # 로그인 사용자 정보 공유
├── Models
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   └── User.cs
├── ViewModels
│   ├── ViewModelBase.cs    # INotifyPropertyChanged 구현
│   ├── RelayCommand.cs     # ICommand 구현
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   ├── ProductViewModel.cs
│   ├── SalesViewModel.cs
│   └── StatisticsViewModel.cs
├── Views
│   ├── LoginWindow.xaml
│   ├── MainWindow.xaml
│   ├── ProductView.xaml
│   ├── SalesView.xaml
│   ├── StatisticsView.xaml
│   └── ReceiptWindow.xaml
├── Converters
│   └── StockColorConverter.cs  # 재고 경고 색상 변환
└── Query
└── database.sql            # DB 생성 스크립트

---

## 👤 개발자

**한종수**  
C# WPF 개발 포트폴리오
