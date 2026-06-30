USE POS_DB;

-- 관리자 계정
INSERT INTO Users (Username, Password, Role)
VALUES ('admin', '1234', 'admin');

-- 일반 직원 계정
INSERT INTO Users (Username, Password, Role)
VALUES ('staff', '1234', 'staff');

-- 판매 기록 (일간/월간 차트 테스트용)
INSERT INTO Orders (OrderDate, TotalAmount, PaymentType)
VALUES 
('2026-06-25', 5000, '현금'),
('2026-06-26', 8500, '현금'),
('2026-06-27', 3200, '카드'),
('2026-06-28', 12000, '현금'),
('2026-06-29', 6700, '카드'),
('2026-03-15', 15000, '현금'),
('2026-03-22', 23000, '카드'),
('2026-04-10', 8500, '현금'),
('2026-04-18', 31000, '카드'),
('2026-05-05', 19500, '현금'),
('2026-05-20', 27000, '카드');