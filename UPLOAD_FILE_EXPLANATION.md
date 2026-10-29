# Giải Thích Chi Tiết: Cơ Chế Upload File

## Tổng Quan

Trong triển khai hiện tại, chúng ta sử dụng phương pháp **nhập đường dẫn file** thay vì upload file trực tiếp. Dưới đây tôi sẽ giải thích chi tiết cả **phương pháp hiện tại** và **cách triển khai upload file thực tế** nếu bạn muốn nâng cấp.

---

## Phương Pháp Hiện Tại: Nhập Đường Dẫn

### Cách Hoạt Động

Admin nhập **đường dẫn** của file vào trường `DuongDanFile`, ví dụ:
- `/uploads/documents/giaotrinh.pdf` (đường dẫn tương đối)
- `https://example.com/files/sach.pdf` (URL đầy đủ)
- `D:\Files\document.pdf` (đường dẫn tuyệt đối - không khuyến khích)

### Trong View (Create.cshtml / Edit.cshtml)

```html
<!-- Đường dẫn file -->
<div class="md:col-span-2">
    <label class="block text-sm font-semibold text-gray-700 mb-2">
        <i class="fas fa-link mr-1"></i> Đường dẫn file
    </label>
    @Html.TextBoxFor(model => model.DuongDanFile, 
        new { 
            @class = "w-full px-4 py-2 border border-gray-300 rounded-lg...",
            placeholder = "/uploads/documents/tailieu.pdf hoặc https://example.com/file.pdf" 
        })
    <p class="text-xs text-gray-500 mt-1">
        Đường dẫn tương đối (vd: /uploads/file.pdf) hoặc URL đầy đủ
    </p>
</div>
```

### Trong Controller

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create([Bind(Include = "...DuongDanFile...")] TaiLieu taiLieu)
{
    if (ModelState.IsValid)
    {
        // DuongDanFile được lưu trực tiếp vào database
        taiLieu.NgayTao = DateTime.Now;
        db.TaiLieux.Add(taiLieu);
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    // ...
}
```

### Ưu Điểm

✅ **Đơn giản**: Không cần xử lý upload, lưu trữ file  
✅ **Linh hoạt**: Có thể link đến file bên ngoài (CDN, cloud storage)  
✅ **Không tốn dung lượng server**: File có thể ở bất kỳ đâu  
✅ **Phù hợp nếu**: File đã được upload bởi công cụ khác (FTP, cloud dashboard)

### Nhược Điểm

❌ **Không thân thiện**: Admin phải biết đường dẫn chính xác  
❌ **Dễ sai**: Nhập sai đường dẫn → link bị broken  
❌ **Thiếu kiểm soát**: Không kiểm tra file có tồn tại hay không  
❌ **Không tự động**: Không auto-detect định dạng, dung lượng

---

## Nâng Cấp: Upload File Thực Tế

### Bước 1: Thêm Input File vào View

**Create.cshtml / Edit.cshtml:**

```html
@using (Html.BeginForm("Create", "TaiLieu_65131559", FormMethod.Post, 
    new { enctype = "multipart/form-data" }))  <!-- 👈 Quan trọng! -->
{
    <div class="md:col-span-2">
        <label class="block text-sm font-semibold text-gray-700 mb-2">
            <i class="fas fa-upload mr-1"></i> Chọn file tải lên <span class="text-red-500">*</span>
        </label>
        
        <!-- File Input -->
        <input type="file" 
               name="uploadedFile" 
               id="uploadedFile"
               accept=".pdf,.doc,.docx,.xlsx,.pptx,.mp3,.mp4,.zip"
               class="w-full px-4 py-2 border border-gray-300 rounded-lg cursor-pointer
                      file:mr-4 file:py-2 file:px-4
                      file:rounded-full file:border-0
                      file:text-sm file:font-semibold
                      file:bg-blue-50 file:text-blue-700
                      hover:file:bg-blue-100"
               onchange="updateFileInfo(this)" />
        
        <p class="text-xs text-gray-500 mt-1">
            <i class="fas fa-info-circle mr-1"></i>
            Chọn file từ máy tính của bạn. Định dạng hỗ trợ: PDF, DOCX, XLSX, MP3, MP4, ZIP
        </p>
    </div>
    
    <!-- Hiển thị thông tin file đã chọn -->
    <div id="fileInfo" class="md:col-span-2 hidden">
        <div class="bg-blue-50 border-l-4 border-blue-500 p-4 rounded">
            <p class="text-sm text-blue-700">
                <i class="fas fa-file mr-2"></i>
                <strong>File được chọn:</strong> <span id="fileName"></span>
            </p>
            <p class="text-xs text-blue-600 mt-1">
                Kích thước: <span id="fileSize"></span> | 
                Định dạng: <span id="fileFormat"></span>
            </p>
        </div>
    </div>
}

<script>
    function updateFileInfo(input) {
        const file = input.files[0];
        if (file) {
            // Hiển thị thông tin file
            document.getElementById('fileName').textContent = file.name;
            document.getElementById('fileSize').textContent = (file.size / (1024 * 1024)).toFixed(2) + ' MB';
            
            // Tự động điền định dạng
            const extension = file.name.split('.').pop().toUpperCase();
            document.getElementById('fileFormat').textContent = extension;
            document.querySelector('select[name="DinhDang"]').value = extension;
            
            // Tự động điền dung lượng
            document.querySelector('input[name="DungLuongMB"]').value = (file.size / (1024 * 1024)).toFixed(2);
            
            // Hiển thị card thông tin
            document.getElementById('fileInfo').classList.remove('hidden');
        }
    }
</script>
```

### Bước 2: Xử Lý Upload trong Controller

**TaiLieu_65131559Controller.cs:**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create(
    [Bind(Include = "TenTaiLieu,ISBN,MaDanhMuc,MaNXB,NamXuatBan,MoTa,AnhBia,DinhDang,ChoPhepTai")] TaiLieu taiLieu,
    HttpPostedFileBase uploadedFile)  // 👈 Tham số cho file upload
{
    if (ModelState.IsValid)
    {
        // XỬ LÝ UPLOAD FILE
        if (uploadedFile != null && uploadedFile.ContentLength > 0)
        {
            try
            {
                // 1. Tạo thư mục lưu trữ nếu chưa tồn tại
                string uploadFolder = Server.MapPath("~/Uploads/Documents");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                
                // 2. Tạo tên file duy nhất (tránh trùng lặp)
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string originalFileName = Path.GetFileName(uploadedFile.FileName);
                string extension = Path.GetExtension(originalFileName);
                string safeFileName = $"{timestamp}_{Path.GetFileNameWithoutExtension(originalFileName)}{extension}";
                
                // 3. Đường dẫn đầy đủ
                string filePath = Path.Combine(uploadFolder, safeFileName);
                
                // 4. Lưu file vào server
                uploadedFile.SaveAs(filePath);
                
                // 5. Lưu đường dẫn tương đối vào database
                taiLieu.DuongDanFile = $"/Uploads/Documents/{safeFileName}";
                
                // 6. Tự động phát hiện định dạng và dung lượng
                taiLieu.DinhDang = extension.TrimStart('.').ToUpper();
                taiLieu.DungLuongMB = (double)uploadedFile.ContentLength / (1024 * 1024); // Convert to MB
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi upload file: {ex.Message}");
                ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
                ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
                return View(taiLieu);
            }
        }
        else
        {
            // Nếu không có file upload, yêu cầu nhập đường dẫn thủ công
            if (string.IsNullOrEmpty(taiLieu.DuongDanFile))
            {
                ModelState.AddModelError("uploadedFile", "Vui lòng chọn file hoặc nhập đường dẫn file");
                ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
                ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
                return View(taiLieu);
            }
        }
        
        // Set các giá trị mặc định
        taiLieu.NgayTao = DateTime.Now;
        taiLieu.LuotXem = 0;
        taiLieu.LuotTai = 0;
        
        if (!taiLieu.ChoPhepTai.HasValue)
        {
            taiLieu.ChoPhepTai = true;
        }
        
        db.TaiLieux.Add(taiLieu);
        db.SaveChanges();
        TempData["SuccessMessage"] = "Thêm tài liệu thành công!";
        return RedirectToAction("Index");
    }
    
    ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
    ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
    return View(taiLieu);
}
```

### Bước 3: Tạo Thư Mục Uploads

Trong project, tạo cấu trúc thư mục:

```
ProjectCK_65131559/
├── Uploads/
│   └── Documents/      👈 File tài liệu sẽ được lưu ở đây
│       ├── 20241231_143022_giaotrinh.pdf
│       ├── 20241231_143045_baigiang.docx
│       └── ...
```

**Cấu hình Web.config** (nếu cần tăng giới hạn upload):

```xml
<configuration>
  <system.web>
    <!-- Tăng giới hạn upload lên 100MB -->
    <httpRuntime maxRequestLength="102400" executionTimeout="360" />
  </system.web>
  
  <system.webServer>
    <security>
      <requestFiltering>
        <!-- IIS: Tăng giới hạn lên 100MB -->
        <requestLimits maxAllowedContentLength="104857600" />
      </requestFiltering>
    </security>
  </system.webServer>
</configuration>
```

---

## So Sánh Hai Phương Pháp

| Tiêu chí | Nhập Đường Dẫn | Upload File |
|----------|----------------|-------------|
| **Độ phức tạp** | Đơn giản | Trung bình |
| **Trải nghiệm UX** | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Tự động hóa** | Không | Có (format, size) |
| **Kiểm soát** | Thấp | Cao |
| **Lưu trữ** | Bên ngoài | Trên server |
| **Quản lý file** | Thủ công | Tự động |
| **Link ngoài** | ✅ Hỗ trợ | ❌ Không |
| **Phù hợp cho** | File sẵn có hoặc từ CDN | Upload mới |

---

## Kết Hợp Hai Phương Pháp (Khuyến Nghị)

Để linh hoạt nhất, bạn có thể cho phép **CẢ HAI** phương pháp:

```html
<div class="mb-6">
    <div class="flex items-center mb-4">
        <input type="radio" name="uploadMethod" value="file" id="methodFile" checked 
               onchange="toggleUploadMethod()" class="mr-2">
        <label for="methodFile" class="font-medium">Tải file lên từ máy tính</label>
        
        <input type="radio" name="uploadMethod" value="url" id="methodUrl" 
               onchange="toggleUploadMethod()" class="ml-6 mr-2">
        <label for="methodUrl" class="font-medium">Nhập đường dẫn/URL</label>
    </div>
    
    <!-- File upload -->
    <div id="fileUploadSection">
        <input type="file" name="uploadedFile" .../>
    </div>
    
    <!-- URL input -->
    <div id="urlInputSection" class="hidden">
        @Html.TextBoxFor(model => model.DuongDanFile, ...)
    </div>
</div>

<script>
    function toggleUploadMethod() {
        const method = document.querySelector('input[name="uploadMethod"]:checked').value;
        document.getElementById('fileUploadSection').classList.toggle('hidden', method !== 'file');
        document.getElementById('urlInputSection').classList.toggle('hidden', method !== 'url');
    }
</script>
```

---

## Xử Lý Download (User Side)

Khi user muốn tải file:

**Controller:**
```csharp
public ActionResult Download(int id)
{
    var taiLieu = db.TaiLieux.Find(id);
    
    if (taiLieu == null)
        return HttpNotFound();
    
    // Kiểm tra quyền tải
    if (taiLieu.ChoPhepTai != true)
    {
        TempData["ErrorMessage"] = "Tài liệu này chỉ được xem, không cho phép tải xuống";
        return RedirectToAction("Details", new { id });
    }
    
    // Tăng lượt tải
    taiLieu.LuotTai = (taiLieu.LuotTai ?? 0) + 1;
    db.SaveChanges();
    
    // Nếu file trên server
    if (taiLieu.DuongDanFile.StartsWith("/"))
    {
        string filePath = Server.MapPath(taiLieu.DuongDanFile);
        if (System.IO.File.Exists(filePath))
        {
            string fileName = Path.GetFileName(filePath);
            return File(filePath, "application/octet-stream", fileName);
        }
    }
    
    // Nếu là URL ngoài -> redirect
    return Redirect(taiLieu.DuongDanFile);
}
```

---

## Tóm Tắt

### Hiện Tại (Đã triển khai)
- ✅ Admin nhập đường dẫn file thủ công
- ✅ Đơn giản, nhanh chóng
- ✅ Phù hợp khi file đã có sẵn trên server hoặc CDN

### Nâng Cấp (Nếu muốn)
- 📤 Thêm `<input type="file">` vào form
- 📤 Thêm `enctype="multipart/form-data"` vào form
- 📤 Xử lý `HttpPostedFileBase` trong controller
- 📤 Lưu file vào `/Uploads/Documents/`
- 📤 Tự động phát hiện định dạng & dung lượng
- 📤 Tạo tên file unique với timestamp

Nếu bạn muốn triển khai upload file thực tế, tôi có thể cập nhật code ngay!
