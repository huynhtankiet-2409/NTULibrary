/* ==========================================================================
   SCRIPT: THƯ VIỆN SỐ (DIGITAL LIBRARY) - FINAL VERSION 2.0
   TÍNH NĂNG: TÀI LIỆU SỐ + DIỄN ĐÀN + QUOTA + YÊU CẦU CHUNG (GENERIC REQUEST)
   ========================================================================== */

USE master;
GO

-- 1. LÀM SẠCH DB (XÓA CŨ TẠO MỚI)
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ThuVienDaiHocNhaTrang')
BEGIN
    ALTER DATABASE ThuVienDaiHocNhaTrang SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ThuVienDaiHocNhaTrang;
END
GO

-- 2. TẠO DATABASE
CREATE DATABASE ThuVienDaiHocNhaTrang;
GO

USE ThuVienDaiHocNhaTrang;
GO

SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

/* ==========================================================================
   PHẦN 1: TẠO CÁC BẢNG (TABLES)
   ========================================================================== */

-- BẢNG: NGƯỜI DÙNG
CREATE TABLE NguoiDung (
    MaNguoiDung INT PRIMARY KEY IDENTITY(1,1),
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MaSoSinhVien VARCHAR(20), -- Cho phép NULL để dùng cho Admin/Cán bộ
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(20) DEFAULT 'SinhVien' CHECK (VaiTro IN ('SinhVien', 'CanBo', 'QuanTri')),
    SoDienThoai VARCHAR(15),
    Khoa NVARCHAR(100), 
    TrangThai NVARCHAR(20) DEFAULT 'HoatDong' CHECK (TrangThai IN ('HoatDong', 'BiKhoa')),
    AnhDaiDien VARCHAR(255),
    NgayTao DATETIME2 DEFAULT GETDATE()
);
GO

-- INDEX ĐẢM BẢO MÃ SINH VIÊN KHÔNG TRÙNG (TRỪ NULL)
SET QUOTED_IDENTIFIER ON;
CREATE UNIQUE NONCLUSTERED INDEX UQ_NguoiDung_MaSoSinhVien 
ON NguoiDung(MaSoSinhVien) 
WHERE MaSoSinhVien IS NOT NULL;
GO

-- BẢNG: DANH MỤC CHA
CREATE TABLE DanhMucCha (
    MaDanhMucCha INT PRIMARY KEY IDENTITY(1,1),
    TenDanhMucCha NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);
GO

-- BẢNG: DANH MỤC CON
CREATE TABLE DanhMucCon (
    MaDanhMucCon INT PRIMARY KEY IDENTITY(1,1),
    TenDanhMucCon NVARCHAR(100) NOT NULL,
    MaDanhMucCha INT NOT NULL, 
    FOREIGN KEY (MaDanhMucCha) REFERENCES DanhMucCha(MaDanhMucCha)
);
GO

-- BẢNG: TÁC GIẢ
CREATE TABLE TacGia (
    MaTacGia INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL, 
    GioiThieu NVARCHAR(MAX)
);
GO

-- BẢNG: NHÀ XUẤT BẢN
CREATE TABLE NhaXuatBan (
    MaNXB INT PRIMARY KEY IDENTITY(1,1),
    TenNXB NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255)
);
GO

-- BẢNG: TÀI LIỆU SỐ
CREATE TABLE TaiLieu (
    MaTaiLieu INT PRIMARY KEY IDENTITY(1,1),
    TenTaiLieu NVARCHAR(255) NOT NULL, 
    ISBN VARCHAR(20),
    MaDanhMucCon INT, 
    MaNXB INT,
    NamXuatBan INT,
    MoTa NVARCHAR(MAX),
    AnhBia VARCHAR(255),
    DuongDanFile VARCHAR(255) NOT NULL,
    DinhDang VARCHAR(10) DEFAULT 'PDF', 
    DungLuongMB FLOAT DEFAULT 0,
    ChoPhepTai BIT DEFAULT 1, 
    LuotXem INT DEFAULT 0,
    LuotTai INT DEFAULT 0,
    NgayTao DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (MaDanhMucCon) REFERENCES DanhMucCon(MaDanhMucCon),
    FOREIGN KEY (MaNXB) REFERENCES NhaXuatBan(MaNXB)
);
GO

-- BẢNG: LIÊN KẾT TÀI LIỆU - TÁC GIẢ
CREATE TABLE TaiLieu_TacGia (
    MaTaiLieu INT,
    MaTacGia INT,
    PRIMARY KEY (MaTaiLieu, MaTacGia),
    FOREIGN KEY (MaTaiLieu) REFERENCES TaiLieu(MaTaiLieu),
    FOREIGN KEY (MaTacGia) REFERENCES TacGia(MaTacGia)
);
GO

-- BẢNG: LỊCH SỬ TRUY CẬP
CREATE TABLE LichSuTruyCap (
    MaLichSu INT PRIMARY KEY IDENTITY(1,1),
    MaNguoiDung INT,
    MaTaiLieu INT,
    ThoiGian DATETIME2 DEFAULT GETDATE(),
    HanhDong NVARCHAR(20) CHECK (HanhDong IN ('Xem', 'Tai')),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaTaiLieu) REFERENCES TaiLieu(MaTaiLieu)
);
GO

-- BẢNG: TỦ SÁCH CÁ NHÂN
CREATE TABLE TaiLieuYeuThich (
    MaNguoiDung INT,
    MaTaiLieu INT,
    NgayThem DATETIME2 DEFAULT GETDATE(),
    PRIMARY KEY (MaNguoiDung, MaTaiLieu),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaTaiLieu) REFERENCES TaiLieu(MaTaiLieu)
);
GO

-- BẢNG: ĐÁNH GIÁ (REVIEW SÁCH)
CREATE TABLE DanhGia (
    MaDanhGia INT PRIMARY KEY IDENTITY(1,1),
    MaNguoiDung INT,
    MaTaiLieu INT,
    DiemSao INT CHECK (DiemSao >= 1 AND DiemSao <= 5),
    BinhLuan NVARCHAR(MAX),
    DuocHienThi BIT DEFAULT 1,
    NgayDang DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaTaiLieu) REFERENCES TaiLieu(MaTaiLieu)
);
GO

-- [CẬP NHẬT] BẢNG: YÊU CẦU / HỖ TRỢ (Request Management)
-- Thay đổi: Gộp chung thành Tiêu đề và Nội dung tùy ý
CREATE TABLE YeuCau (
    MaYeuCau INT PRIMARY KEY IDENTITY(1,1),
    MaNguoiDung INT,
    TieuDe NVARCHAR(255) NOT NULL, -- Ví dụ: "Xin sách Clean Code", "Lỗi tải file"
    NoiDung NVARCHAR(MAX) NOT NULL, -- Nội dung chi tiết
    PhanHoiTuAdmin NVARCHAR(MAX), -- Admin trả lời (nếu có)
    TrangThai NVARCHAR(20) DEFAULT 'ChoXuLy' CHECK (TrangThai IN ('ChoXuLy', 'DangXuLy', 'DaXong', 'TuChoi')),
    NgayGui DATETIME2 DEFAULT GETDATE(),
    NgayXuLy DATETIME2,
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);
GO

/* --------------------------------------------------------------------------
   CÁC BẢNG PHỤC VỤ TÍNH NĂNG DIỄN ĐÀN (FORUM)
   -------------------------------------------------------------------------- */

CREATE TABLE DienDan_ChuyenMuc (
    MaChuyenMuc INT PRIMARY KEY IDENTITY(1,1),
    TenChuyenMuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255),
    ThuTuHienThi INT DEFAULT 0
);
GO

CREATE TABLE DienDan_BaiViet (
    MaBaiViet INT PRIMARY KEY IDENTITY(1,1),
    TieuDe NVARCHAR(255) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    MaNguoiDung INT NOT NULL, 
    MaChuyenMuc INT NOT NULL,
    NgayDang DATETIME2 DEFAULT GETDATE(),
    LuotXem INT DEFAULT 0,
    TrangThai NVARCHAR(20) DEFAULT 'ChoDuyet' CHECK (TrangThai IN ('ChoDuyet', 'HienThi', 'BiKhoa', 'BiAn')),
    LaGhim BIT DEFAULT 0, 
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaChuyenMuc) REFERENCES DienDan_ChuyenMuc(MaChuyenMuc)
);
GO

CREATE TABLE DienDan_BinhLuan (
    MaBinhLuan INT PRIMARY KEY IDENTITY(1,1),
    MaBaiViet INT NOT NULL,
    MaNguoiDung INT NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    NgayBinhLuan DATETIME2 DEFAULT GETDATE(),
    DuocHienThi BIT DEFAULT 1,
    FOREIGN KEY (MaBaiViet) REFERENCES DienDan_BaiViet(MaBaiViet),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);
GO

CREATE TABLE DienDan_BaoCao (
    MaBaoCao INT PRIMARY KEY IDENTITY(1,1),
    MaBaiViet INT NOT NULL,
    MaNguoiDungBaoCao INT NOT NULL,
    LyDo NVARCHAR(255),
    NgayBaoCao DATETIME2 DEFAULT GETDATE(),
    TrangThaiXuLy NVARCHAR(20) DEFAULT 'ChoXuLy' CHECK (TrangThaiXuLy IN ('ChoXuLy', 'DaXuLy', 'BoQua')),
    FOREIGN KEY (MaBaiViet) REFERENCES DienDan_BaiViet(MaBaiViet),
    FOREIGN KEY (MaNguoiDungBaoCao) REFERENCES NguoiDung(MaNguoiDung)
);
GO

/* ==========================================================================
   PHẦN 2: DỮ LIỆU MẪU (FULL DATASET)
   ========================================================================== */

-- 1. Danh Mục Cha (15 khoa/viện)
INSERT INTO DanhMucCha (TenDanhMucCha, MoTa) VALUES 
(N'Công nghệ thông tin', N'Các tài liệu thuộc khoa Công nghệ thông tin'),
(N'Kinh tế', N'Các tài liệu thuộc khoa Kinh tế'),
(N'Du lịch', N'Các tài liệu thuộc khoa Du lịch'),
(N'Thủy sản', N'Các tài liệu thuộc khoa Thủy sản'),
(N'Kỹ thuật tàu thủy', N'Các tài liệu thuộc khoa Kỹ thuật tàu thủy'),
(N'Cơ khí', N'Các tài liệu thuộc khoa Cơ khí'),
(N'Điện - Điện tử', N'Các tài liệu thuộc khoa Điện - Điện tử'),
(N'Công nghệ thực phẩm', N'Các tài liệu thuộc khoa Công nghệ thực phẩm'),
(N'Kỹ thuật xây dựng', N'Các tài liệu thuộc khoa Kỹ thuật xây dựng'),
(N'Ngoại ngữ', N'Các tài liệu thuộc khoa Ngoại ngữ'),
(N'Khoa học cơ bản', N'Các tài liệu thuộc khoa Khoa học cơ bản'),
(N'Kế toán - Tài chính', N'Các tài liệu thuộc khoa Kế toán - Tài chính'),
(N'Quản trị kinh doanh', N'Các tài liệu thuộc khoa Quản trị kinh doanh'),
(N'Kỹ thuật giao thông', N'Các tài liệu thuộc khoa Kỹ thuật giao thông'),
(N'Lý luận chính trị', N'Các tài liệu thuộc khoa Lý luận chính trị');

-- 2. Danh Mục Con (Chuyên ngành chi tiết)
-- CNTT (ID 1)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Kỹ thuật phần mềm', 1), (N'Hệ thống thông tin', 1), (N'Mạng máy tính', 1), (N'An toàn thông tin', 1), (N'Khoa học dữ liệu', 1);
-- Kinh tế (ID 2)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Kinh tế học', 2), (N'Kinh tế phát triển', 2), (N'Kinh tế quốc tế', 2);
-- Du lịch (ID 3)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Quản trị dịch vụ du lịch và lữ hành', 3), (N'Quản trị khách sạn', 3), (N'Quản trị nhà hàng', 3);
-- Thủy sản (ID 4)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Nuôi trồng thủy sản', 4), (N'Công nghệ chế biến thủy sản', 4), (N'Quản lý thủy sản', 4);
-- Kỹ thuật tàu thủy (ID 5)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Thiết kế thân tàu', 5), (N'Công nghệ đóng tàu', 5);
-- Cơ khí (ID 6)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Chế tạo máy', 6), (N'Công nghệ kỹ thuật cơ khí', 6), (N'Kỹ thuật ô tô', 6);
-- Điện - Điện tử (ID 7)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Công nghệ kỹ thuật điện - điện tử', 7), (N'Kỹ thuật điều khiển và tự động hóa', 7);
-- Công nghệ thực phẩm (ID 8)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Công nghệ thực phẩm', 8), (N'Đảm bảo chất lượng và ATTP', 8);
-- Kỹ thuật xây dựng (ID 9)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Xây dựng dân dụng và công nghiệp', 9), (N'Xây dựng cầu đường', 9);
-- Ngoại ngữ (ID 10)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Ngôn ngữ Anh', 10), (N'Ngôn ngữ Trung Quốc', 10);
-- Khoa học cơ bản (ID 11)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Toán học ứng dụng', 11), (N'Vật lý kỹ thuật', 11);
-- Kế toán (ID 12)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Kế toán doanh nghiệp', 12), (N'Tài chính ngân hàng', 12);
-- QTKD (ID 13)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Quản trị kinh doanh tổng hợp', 13), (N'Marketing', 13), (N'Quản trị nhân lực', 13);
-- Giao thông (ID 14)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Logistics và quản lý chuỗi cung ứng', 14), (N'Kỹ thuật công trình giao thông', 14);
-- Lý luận chính trị (ID 15)
INSERT INTO DanhMucCon (TenDanhMucCon, MaDanhMucCha) VALUES 
(N'Giáo dục chính trị', 15),
(N'Thống kê kinh tế', 2),
(N'Tướng-Việt Nam', 15),
(N'Khoa học thường thức', 11),
(N'Tư duy', 11),
(N'Trí tuệ nhân tạo', 1),
(N'Doanh nhân', 13),
(N'Tuổi thọ', 11),
(N'Thế giới', 11),
(N'Tập luyện', 11),
(N'Chăm sóc sức khỏe', 11),
(N'Tài chính', 12),
(N'Thông tin chính trị', 15),
(N'Giao tiếp', 13),
(N'Khả năng lãnh đạo', 13),
(N'Trí tuệ cảm xúc', 11),
(N'Tự hiện thực hóa', 11),
(N'Lối sống', 11),
(N'Tham nhũng', 15),
(N'Lãnh đạo', 13),
(N'Phong cách sống', 11),
(N'Toàn cầu hóa', 2),
(N'Kinh tế Việt Nam', 2),
(N'Lịch sử', 15),
(N'Cuộc sống', 11),
(N'Chủ nghĩa xã hội', 15),
(N'Trí tưởng tượng', 11);

-- 3. Nhà Xuất Bản
INSERT INTO NhaXuatBan (TenNXB, DiaChi) VALUES 
(N'NXB Giáo dục Việt Nam', N'Hà Nội'),
(N'NXB Đại học Quốc gia Hà Nội', N'Hà Nội'),
(N'NXB Khoa học và Kỹ thuật', N'Hà Nội'),   
(N'NXB Trẻ', N'TP.HCM'),                  
(N'NXB Đại học Nha Trang', N'Nha Trang'), 
(N'NXB Thế Giới', N'Hà Nội'),               
(N'NXB Giao thông vận tải', N'Hà Nội'),   
(N'NXB Nông nghiệp', N'Hà Nội'),             
(N'NXB Chính trị Quốc gia', N'Hà Nội'),       
(N'NXB ĐHQG TP.HCM', N'TP.HCM'),               
(N'NXB Hà Nội', N'Hà Nội'),              
(N'NXB Bách Khoa Hà Nội', N'Hà Nội'),
(N'NXB Thông tin và Truyền thông', N'Hà Nội'),
(N'NXB Khoa học xã hội', N'Hà Nội'),  
(N'NXB Kim Đồng', N'Hà Nội'),    
(N'NXB Thống kê', N'Hà Nội'),             
(N'NXB Chính trị Quốc gia Sự thật', N'Hà Nội'),
(N'NXB Hội Nhà Văn', N'Hà Nội'),             
(N'NXB Văn học', N'Hà Nội'),                
(N'NXB Dân Trí', N'Hà Nội'),                  
(N'NXB Lao Động', N'Hà Nội'),                
(N'NXB Thanh Niên', N'Hà Nội'),                
(N'NXB Công Thương', N'Hà Nội'),             
(N'NXB Tri Thức', N'Hà Nội'),                  
(N'NXB Thể thao và Du lịch', N'Hà Nội');      

-- 4. Tác Giả
INSERT INTO TacGia (HoTen, GioiThieu) VALUES 
(N'GS.TS. Võ Văn Hải', N'Chuyên gia đầu ngành'),
(N'PGS.TS. Nguyễn Khắc Thành', N'Nghiên cứu viên cao cấp'),
(N'TS. Lê Thị Thu Thủy', N'Giảng viên chính'),
(N'ThS. Trần Văn Nam', N'Tác giả trẻ'),
(N'GS. Bùi Phan Anh', N'Giáo sư danh dự'),
(N'Nguyễn Văn A', N'Tác giả tự do'),
(N'Lê Văn B', N'Nhà nghiên cứu'),
(N'Trần Thị C', N'Chuyên gia kinh tế'),
(N'Phạm Văn D', N'Kỹ sư trưởng'),
(N'Hoàng Văn E', N'Kiến trúc sư'),
(N'Vũ Thị F', N'Nhà giáo ưu tú'),
(N'Đỗ Văn G', N'Tiến sĩ khoa học'),
(N'Bùi Thị H', N'Thạc sĩ'),
(N'Ngô Văn I', N'Diễn giả'),
(N'Lý Văn K', N'Chuyên gia công nghệ'),
(N'Hà Văn Sơn', N''),
(N'Trần Trọng Trung', N''),
(N'Bill Bryson', N''),
(N'Daniel Smith', N''),
(N'Dịch Dương', N''),
(N'Donald J. Trump', N''),
(N'Michael F. Roizen', N''),
(N'Francis Fukuyama', N''),
(N'Jeff Gaudette', N''),
(N'Seiji Nishino', N''),
(N'David Clark', N''),
(N'Brooks Jackson', N''),
(N'Trương Tiểu Hằng', N''),
(N'Kacie Berghoef', N''),
(N'Patrick King', N''),
(N'Roxie Nafousi', N''),
(N'Alexandra Gilles', N''),
(N'Thomas Piketty', N''),
(N'Simon Sinek', N''),
(N'Shiomi Naoki', N''),
(N'Tren Griffin', N''),
(N'Robert Greene', N''),
(N'Jeffrey E. Garten', N''),
(N'Phạm Minh Chính', N''),
(N'John Gray', N''),
(N'Paul S. Adler', N''),
(N'Yuval Noah Harari', N''),
(N'Jerome Bruner', N'');

-- 5. Người Dùng
-- Admin (ID 1, 2)
INSERT INTO NguoiDung (TenDangNhap, MaSoSinhVien, HoTen, Email, MatKhau, VaiTro, Khoa, TrangThai) VALUES
('admin', NULL, N'Quản Trị Viên Hệ Thống', 'admin@ntu.edu.vn', '123456', 'QuanTri', N'Phòng Đào Tạo', 'HoatDong'),
('admin_backup', NULL, N'Quản Trị Viên Dự Phòng', 'admin2@ntu.edu.vn', '123456', 'QuanTri', N'Phòng CNTT', 'HoatDong');

-- Cán bộ (ID 3, 4, 5)
INSERT INTO NguoiDung (TenDangNhap, MaSoSinhVien, HoTen, Email, MatKhau, VaiTro, Khoa, TrangThai) VALUES
('cb_hung', NULL, N'Thầy Hùng', 'hung.le@ntu.edu.vn', '123456', 'CanBo', N'CNTT', 'HoatDong'),
('cb_lan', NULL, N'Cô Lan', 'lan.nguyen@ntu.edu.vn', '123456', 'CanBo', N'Kinh Tế', 'HoatDong'),
('cb_thao', NULL, N'Cô Thảo', 'thao.pham@ntu.edu.vn', '123456', 'CanBo', N'Ngoại Ngữ', 'HoatDong');

-- Sinh viên (ID 6 - 15)
INSERT INTO NguoiDung (TenDangNhap, MaSoSinhVien, HoTen, Email, MatKhau, VaiTro, Khoa, TrangThai) VALUES
('sv_an', '65130001', N'Nguyễn Văn An', 'an.nv@st.ntu.edu.vn', '123456', 'SinhVien', N'CNTT', 'HoatDong'),
('sv_binh', '65130002', N'Trần Thị Bình', 'binh.tt@st.ntu.edu.vn', '123456', 'SinhVien', N'Kinh Tế', 'HoatDong'),
('sv_cuong', '65130003', N'Lê Văn Cường', 'cuong.lv@st.ntu.edu.vn', '123456', 'SinhVien', N'Du Lịch', 'HoatDong'),
('sv_dung', '65130004', N'Phạm Tiến Dũng', 'dung.pt@st.ntu.edu.vn', '123456', 'SinhVien', N'Cơ Khí', 'HoatDong'),
('sv_em', '65130005', N'Hoàng Thị Em', 'em.ht@st.ntu.edu.vn', '123456', 'SinhVien', N'Thủy Sản', 'HoatDong'),
('sv_giang', '65130006', N'Vũ Trường Giang', 'giang.vt@st.ntu.edu.vn', '123456', 'SinhVien', N'Xây Dựng', 'HoatDong'),
('sv_hoa', '65130007', N'Ngô Thị Hoa', 'hoa.nt@st.ntu.edu.vn', '123456', 'SinhVien', N'Ngoại Ngữ', 'HoatDong'),
('sv_ien', '65130008', N'Lý Văn Iên', 'ien.lv@st.ntu.edu.vn', '123456', 'SinhVien', N'Điện - Điện Tử', 'HoatDong'),
('sv_khoi', '65130009', N'Đỗ Minh Khôi', 'khoi.dm@st.ntu.edu.vn', '123456', 'SinhVien', N'CNTT', 'HoatDong'),
('sv_lam', '65130010', N'Bùi Tùng Lâm', 'lam.bt@st.ntu.edu.vn', '123456', 'SinhVien', N'Kế Toán', 'HoatDong');

-- 6. Tài Liệu
INSERT INTO TaiLieu (TenTaiLieu, MaDanhMucCon, MaNXB, NamXuatBan, DuongDanFile, DinhDang, ChoPhepTai, LuotXem, LuotTai, MoTa, AnhBia) VALUES
(N'Nexus :Lược sử của những mạng lưới thông tin', 60, 18, 2024, 'https://up.nhuttruong.com/file/nhuttruongcom/tbooks/tbooks/2024/12/781361600-Nexus.pdf', 'BOOK', 0, 0, 0, N'Yuval Noah Harari', '/Images/s1.jpg'),
(N'Giáo trình lý thuyết thống kê :Ứng dụng trong quản trị và kinh tế', 38, 16, 2004, '', 'BOOK', 0, 0, 0, N'Hà Văn Sơn (Chủ biên)', '/Images/s2.jpg'),
(N'Tổng Tư lệnh Võ Nguyên Giáp :Trong những năm đế quốc Mỹ leo thang chiến tranh (1965-1969)', 39, 17, 2021, '', 'BOOK', 0, 0, 0, N'Trần Trọng Trung', '/Images/s3.jpg'),
(N'Cơ thể người :Chuyến du hành đến mọi bộ phận của chúng ta', 40, 18, 2024, '', 'BOOK', 0, 0, 0, N'Bill Bryson', '/Images/s4.jpg'),
(N'Tư duy như Sherlock Holmes', 41, 19, 2020, '', 'BOOK', 0, 0, 0, N'Daniel Smith', '/Images/s5.jpg'),
(N'ChatGPT thực chiến :Tối ưu nội dung bằng AI', 42, 20, 2024, '', 'BOOK', 0, 0, 0, N'Dịch Dương; Phan Trạch Bân; Lý Thế Minh', '/Images/s6.jpg'),
(N'Nghệ thuật đàm phán', 43, 1, 2016, '', 'BOOK', 0, 0, 0, N'Donald J. Trump', '/Images/s7.jpg'),
(N'Bạn mãi trẻ :Vì sao bạn già đi ? làm thế nào để bạn trẻ mãi ?', 44, 4, 2010, '', 'BOOK', 0, 0, 0, N'Michael F. Roizen', '/Images/s8.jpg'),
(N'Trật tự chính trị & suy tàn chính trị :Từ cách mạng công nghiệp tới toàn cầu hóa', 45, 24, 2020, '', 'BOOK', 0, 0, 0, N'Francis Fukuyama', '/Images/s9.jpg'),
(N'Tập chạy không khó :Kế hoạch tập luyện toàn diện cho tốc độ, sức mạnh và sức bền', 46, 25, 2020, '', 'BOOK', 0, 0, 0, N'Jeff Gaudette', '/Images/s10.jpg'),
(N'Charlie Munger :Nhà đầu tư thông minh trên thị trường chứng khoán', 48, 21, 2020, '', 'BOOK', 0, 0, 0, N'David Clark', '/Images/s11.jpg'),
(N'Thế giới lọc lừa :tìm kiếm sự thật trong một thế giới hỗn loạn', 49, 21, 2008, '', 'BOOK', 0, 0, 0, N'Brooks Jackson', '/Images/s12.jpg'),
(N'Nói chuyện là bản năng, giữ miệng là tu dưỡng, im lặng là trí tuệ', 50, 22, 2021, '', 'BOOK', 0, 0, 0, N'Trương Tiểu Hằng', '/Images/s13.jpg'),
(N'Enneagram :Khám phá bản thân - giải mã tính cách - thấu hiểu tâm lý', 51, 22, 2022, '', 'BOOK', 0, 0, 0, N'Kacie Berghoef', '/Images/s14.jpg'),
(N'Tâm lý học ứng dụng :Thuật đọc tâm trí để nuôi dưỡng mối quan hệ và rèn luyện trí tuệ cảm xúc', 52, 22, 2022, '', 'BOOK', 0, 0, 0, N'Patrick King', '/Images/s15.jpg'),
(N'Manifest :7 bước để thay đổi cuộc đời bạn mãi mãi', 53, 22, 2023, '', 'BOOK', 0, 0, 0, N'Roxie Nafousi', '/Images/s16.jpg'),
(N'Nghĩ như một tỷ phú :Mọi thứ bạn cần biết về thành công, bất động sản và cuộc sống', 54, 4, 2006, '', 'BOOK', 0, 0, 0, N'Donald J. Trump', '/Images/s17.jpg'),
(N'Tham nhũng dầu mỏ - Thế lực nhiễu loạn thế giới', 55, 22, 2021, '', 'BOOK', 0, 0, 0, N'Alexandra Gilles', '/Images/s18.jpg'),
(N'Tư bản thế kỷ 21', 6, 24, 2021, '', 'BOOK', 0, 0, 0, N'Thomas Piketty', '/Images/s19.jpg'),
(N'Trò chơi vô cực :Chiến lược dẫn đầu trong thời đại khách hàng thông minh', 56, 23, 2020, '', 'BOOK', 0, 0, 0, N'Simon Sinek', '/Images/s20.jpg'),
(N'Sống bán nông bán X :Triết lý mới về cuộc sống điền viên', 57, 23, 2021, '', 'BOOK', 0, 0, 0, N'Shiomi Naoki', '/Images/s21.jpg'),
(N'Charlie Munger :Phương pháp đầu tư giá trị : bí quyết đánh bại thị trường của thiên tài đầu tư hàng đầu thế giới, cánh tay phải đã cùng Warren Buffett tạo nên đế chế Berkshire Hathaway vĩ đại'
,48, 23, 2022, '', 'BOOK', 0, 0, 0, N'Tren Griffin', '/Images/s22.jpg'),
(N'Làm chủ', 41, 4, 2020, '', 'BOOK', 0, 0, 0, N'Robert Greene', '/Images/s23.jpg'),
(N'Từ tơ lụa đến silicon :Câu chuyện về toàn cầu hóa thông qua mười cuộc đời lạ thường', 58, 4, 2017, '', 'BOOK', 0, 0, 0, N'Jeffrey E. Garten', '/Images/s24.jpg'),
(N'Kinh tế Việt Nam thăng trầm và đột phá', 59, 9, 2009, '', 'BOOK', 0, 0, 0, N'Phạm Minh Chính', '/Images/s25.jpg'),
(N'Nguồn gốc trật tự chính trị', 60, 24, 2021, '', 'BOOK', 0, 0, 0, N'Francis Fukuyama', '/Images/s26.jpg'),
(N'Mèo và triết lý nhân sinh :Mèo đã dạy chúng ta điều gì về cuộc sống', 61, 22, 2022, '', 'BOOK', 0, 0, 0, N'John Gray', '/Images/s27.jpg'),
(N'Nền kinh tế 99% :Cách thức Chủ nghĩa Xã hội vượt qua các cuộc khủng hoảng của Chủ nghĩa Tư bản', 62, 17, 2020, '', 'BOOK', 0, 0, 0, N'Paul S. Adler', '/Images/s28.jpg'),
(N'Những thế giới trong tâm trí', 63, 24, 2022, '', 'BOOK', 0, 0, 0, N'Jerome Bruner', '/Images/s29.jpg'),
(N'Ngủ ngon theo phương pháp Stanford :Cuộc cách mạng về giấc ngủ', 47, 25, 2021, '', 'BOOK', 0, 0, 0, N'Seiji Nishino', '/Images/s30.jpg')
;

-- 7. TaiLieu_TacGia (Liên kết)
INSERT INTO TaiLieu_TacGia (MaTaiLieu, MaTacGia) VALUES 
-- Real Book Links start at ID 1 (shifted from 16)
(1, 16), (2, 17), (3, 18), (4, 19), (5, 20),
(6, 21), (7, 22), (8, 23), (9, 24), (10, 25),
(11, 26), (12, 27), (13, 28), (14, 29), (15, 30),
(16, 31), (17, 21), (18, 32), (19, 33), (20, 34),
(21, 35), (22, 36), (23, 37), (24, 38), (25, 39),
(26, 23), (27, 40), (28, 41), (29, 42), (30, 43);

-- 8. Diễn Đàn - Chuyên Mục
INSERT INTO DienDan_ChuyenMuc (TenChuyenMuc, MoTa, ThuTuHienThi) VALUES
(N'Thông báo từ Thư viện', N'Các thông báo chính thức.', 1),
(N'Trao đổi học tập', N'Hỏi đáp bài tập, kiến thức.', 2),
(N'Góc Sinh Viên', N'Chia sẻ đời sống sinh viên.', 3),
(N'Tìm kiếm tài liệu', N'Nhờ tìm sách, báo.', 4),
(N'Tuyển dụng - Việc làm', N'Thông tin thực tập và việc làm.', 5),
(N'Câu lạc bộ Sách', N'Review và thảo luận sách.', 6),
(N'Hỏi đáp CNTT', N'Thắc mắc về máy tính, phần mềm.', 7),
(N'Giao lưu Văn nghệ', N'Hoạt động ngoại khóa.', 8),
(N'Góp ý cho Thư viện', N'Hòm thư góp ý.', 9),
(N'Mua bán giáo trình cũ', N'Trao đổi sách cũ.', 10),
(N'Học bổng - Du học', N'Cơ hội phát triển.', 11),
(N'Kỹ năng mềm', N'Chia sẻ kỹ năng sống.', 12),
(N'Pháp luật đại cương', N'Tư vấn pháp luật.', 13),
(N'Sức khỏe - Thể thao', N'Hoạt động thể chất.', 14),
(N'Góc xả stress', N'Chuyện vui vặt.', 15);

-- 9. Diễn Đàn - Bài Viết
INSERT INTO DienDan_BaiViet (TieuDe, NoiDung, MaNguoiDung, MaChuyenMuc, TrangThai, LaGhim, LuotXem) VALUES
(N'Thông báo lịch nghỉ Tết Nguyên Đán 2025', N'Thư viện trân trọng thông báo lịch nghỉ tết...', 1, 1, 'HienThi', 1, 500),
(N'Cần tìm nhóm học chung môn CSDL', N'Mình đang cần tìm bạn học nhóm môn CSDL thầy Hưng.', 6, 2, 'HienThi', 0, 120),
(N'Review sách "Nhà Giả Kim"', N'Cuốn sách này thực sự thay đổi tư duy của mình...', 7, 6, 'HienThi', 0, 300),
(N'Tuyển thành viên CLB Guitar', N'CLB Guitar đang tuyển thành viên mới khóa K65.', 8, 8, 'HienThi', 0, 150),
(N'Lỗi đăng nhập wifi thư viện', N'Có ai bị lỗi không vào được wifi thư viện tầng 4 không?', 9, 7, 'HienThi', 0, 80);

-- 10. Yêu Cầu Chung
INSERT INTO YeuCau (MaNguoiDung, TieuDe, NoiDung, TrangThai) VALUES
(6, N'Yêu cầu bổ sung giáo trình AI', N'Mong thư viện bổ sung thêm sách về Trí tuệ nhân tạo.', 'ChoXuLy'),
(7, N'Lỗi tải file không được', N'Sách Marketing căn bản báo lỗi 403 khi tải.', 'DangXuLy'),
(8, N'Xin gia hạn sách mượn', N'Em muốn gia hạn thêm 1 tuần được không ạ?', 'DaXong');

-- 11. Đánh Giá
INSERT INTO DanhGia (MaNguoiDung, MaTaiLieu, DiemSao, BinhLuan) VALUES
(6, 1, 5, N'Sách viết rất dễ hiểu!'),
(7, 4, 4, N'Nội dung hay nhưng hơi cũ.'),
(8, 2, 5, N'Tuyệt vời.');

-- 12. Lịch Sử Truy Cập
INSERT INTO LichSuTruyCap (MaNguoiDung, MaTaiLieu, HanhDong, ThoiGian) VALUES
(6, 1, 'Xem', DATEADD(day, -1, GETDATE())),
(6, 1, 'Tai', GETDATE()),
(7, 2, 'Xem', GETDATE());
GO

/* ==========================================================================
   PHẦN 3: TRIGGERS
   ========================================================================== */

CREATE TRIGGER trg_CapNhatLuotTuongTac
ON LichSuTruyCap
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE t SET t.LuotXem = t.LuotXem + 1
    FROM TaiLieu t JOIN inserted i ON t.MaTaiLieu = i.MaTaiLieu WHERE i.HanhDong = 'Xem';
    UPDATE t SET t.LuotTai = t.LuotTai + 1
    FROM TaiLieu t JOIN inserted i ON t.MaTaiLieu = i.MaTaiLieu WHERE i.HanhDong = 'Tai';
END;
GO

/* ==========================================================================
   PHẦN 4: STORED PROCEDURES (QUẢN LÝ & NGHIỆP VỤ)
   ========================================================================== */

-- SP 1: GHI NHẬN TRUY CẬP & CHECK QUOTA
CREATE PROCEDURE sp_GhiNhanTruyCap
    @MaNguoiDung INT,
    @MaTaiLieu INT,
    @HanhDong NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF @HanhDong = 'Xem'
    BEGIN
        INSERT INTO LichSuTruyCap (MaNguoiDung, MaTaiLieu, HanhDong) VALUES (@MaNguoiDung, @MaTaiLieu, 'Xem');
        RETURN;
    END

    IF @HanhDong = 'Tai'
    BEGIN
        DECLARE @ChoPhepTai BIT;
        SELECT @ChoPhepTai = ChoPhepTai FROM TaiLieu WHERE MaTaiLieu = @MaTaiLieu;
        IF @ChoPhepTai = 0 THROW 51000, N'Tài liệu này không được phép tải.', 1;

        DECLARE @VaiTro NVARCHAR(20);
        SELECT @VaiTro = VaiTro FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung;

        DECLARE @SoNgay INT = 7, @MaxLuot INT = 4;
        IF @VaiTro = 'CanBo' BEGIN SET @SoNgay = 30; SET @MaxLuot = 30; END
        ELSE IF @VaiTro = 'QuanTri' BEGIN SET @SoNgay = 1; SET @MaxLuot = 999999; END

        DECLARE @DaTai INT;
        SELECT @DaTai = COUNT(*) FROM LichSuTruyCap 
        WHERE MaNguoiDung = @MaNguoiDung AND HanhDong = 'Tai' AND ThoiGian >= DATEADD(DAY, -@SoNgay, GETDATE());

        IF @DaTai >= @MaxLuot AND @VaiTro <> 'QuanTri'
            THROW 51001, N'Bạn đã hết hạn ngạch tải tài liệu.', 1;

        INSERT INTO LichSuTruyCap (MaNguoiDung, MaTaiLieu, HanhDong) VALUES (@MaNguoiDung, @MaTaiLieu, 'Tai');
    END
END;
GO

-- SP 2: Tìm kiếm tài liệu
CREATE PROCEDURE sp_TimKiemTaiLieu
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SELECT t.MaTaiLieu, t.TenTaiLieu, t.AnhBia, t.DinhDang, t.ChoPhepTai, t.LuotXem, t.LuotTai,
           dmc.TenDanhMucCon, nxb.TenNXB, STRING_AGG(tg.HoTen, ', ') AS DanhSachTacGia
    FROM TaiLieu t
    LEFT JOIN DanhMucCon dmc ON t.MaDanhMucCon = dmc.MaDanhMucCon
    LEFT JOIN NhaXuatBan nxb ON t.MaNXB = nxb.MaNXB
    LEFT JOIN TaiLieu_TacGia tt ON t.MaTaiLieu = tt.MaTaiLieu
    LEFT JOIN TacGia tg ON tt.MaTacGia = tg.MaTacGia
    WHERE t.TenTaiLieu LIKE N'%' + @TuKhoa + N'%' OR tg.HoTen LIKE N'%' + @TuKhoa + N'%'
    GROUP BY t.MaTaiLieu, t.TenTaiLieu, t.AnhBia, t.DinhDang, t.ChoPhepTai, t.LuotXem, t.LuotTai, dmc.TenDanhMucCon, nxb.TenNXB
    ORDER BY t.LuotXem DESC;
END;
GO

-- SP 3: Dashboard
CREATE PROCEDURE sp_LayThongKeDashboard
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM NguoiDung) AS TongNguoiDung,
        (SELECT COUNT(*) FROM TaiLieu) AS TongTaiLieu,
        (SELECT COUNT(*) FROM YeuCau WHERE TrangThai = 'ChoXuLy') AS YeuCauMoi;
END;
GO

/* ==========================================================================
   PHẦN 5: [CẬP NHẬT] STORED PROCEDURES CHO YÊU CẦU / HỖ TRỢ
   ========================================================================== */

-- 5.1. User gửi yêu cầu chung (Nhan đề + Nội dung)
CREATE PROCEDURE sp_GuiYeuCau
    @MaNguoiDung INT,
    @TieuDe NVARCHAR(255),
    @NoiDung NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO YeuCau (MaNguoiDung, TieuDe, NoiDung)
    VALUES (@MaNguoiDung, @TieuDe, @NoiDung);
    PRINT N'Đã gửi yêu cầu thành công.';
END;
GO

-- 5.2. Admin xem danh sách yêu cầu
CREATE PROCEDURE sp_Admin_LayDanhSachYeuCau
    @TrangThai NVARCHAR(20) = NULL -- NULL thì lấy tất cả
AS
BEGIN
    SELECT yc.MaYeuCau, yc.TieuDe, yc.NoiDung, yc.TrangThai, yc.NgayGui, nd.HoTen, nd.MaSoSinhVien
    FROM YeuCau yc
    JOIN NguoiDung nd ON yc.MaNguoiDung = nd.MaNguoiDung
    WHERE (@TrangThai IS NULL OR yc.TrangThai = @TrangThai)
    ORDER BY yc.NgayGui DESC;
END;
GO

-- 5.3. Admin xử lý yêu cầu (Cập nhật trạng thái + Phản hồi)
CREATE PROCEDURE sp_Admin_XuLyYeuCau
    @MaYeuCau INT,
    @TrangThaiMoi NVARCHAR(20), -- 'DaXong', 'TuChoi', 'DangXuLy'
    @PhanHoi NVARCHAR(MAX) = NULL
AS
BEGIN
    UPDATE YeuCau
    SET TrangThai = @TrangThaiMoi,
        PhanHoiTuAdmin = @PhanHoi,
        NgayXuLy = GETDATE()
    WHERE MaYeuCau = @MaYeuCau;
END;
GO

/* ==========================================================================
   PHẦN 6: CÁC SP KHÁC (USER & FORUM)
   ========================================================================== */
-- (Giữ nguyên các SP User, Forum như phiên bản trước)

CREATE PROCEDURE sp_Home_LayTaiLieuMoi AS BEGIN
    SELECT TOP 8 t.MaTaiLieu, t.TenTaiLieu, t.AnhBia, t.NgayTao, nxb.TenNXB, dmc.TenDanhMucCon
    FROM TaiLieu t
    LEFT JOIN NhaXuatBan nxb ON t.MaNXB = nxb.MaNXB
    LEFT JOIN DanhMucCon dmc ON t.MaDanhMucCon = dmc.MaDanhMucCon
    ORDER BY t.NgayTao DESC;
END;
GO

CREATE PROCEDURE sp_HeThong_DangNhap @TenDangNhap VARCHAR(50), @MatKhau VARCHAR(255) AS BEGIN
    SELECT MaNguoiDung, HoTen, VaiTro, AnhDaiDien, Khoa
    FROM NguoiDung 
    WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau AND TrangThai = 'HoatDong';
END;
GO

-- DIỄN ĐÀN SP (Rút gọn cho đủ file)
CREATE PROCEDURE sp_DienDan_LayDanhSachBaiViet @MaChuyenMuc INT = NULL AS BEGIN
    SELECT bv.*, nd.HoTen AS NguoiDang, cm.TenChuyenMuc 
    FROM DienDan_BaiViet bv 
    JOIN NguoiDung nd ON bv.MaNguoiDung = nd.MaNguoiDung
    JOIN DienDan_ChuyenMuc cm ON bv.MaChuyenMuc = cm.MaChuyenMuc
    WHERE bv.TrangThai = 'HienThi' AND (@MaChuyenMuc IS NULL OR bv.MaChuyenMuc = @MaChuyenMuc)
    ORDER BY bv.LaGhim DESC, bv.NgayDang DESC;
END;
GO
