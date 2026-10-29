using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;
using System.Data.Entity;
using System.IO;

namespace ProjectCK_65131559.Areas.User_65131559.Controllers
{
    public class TraCuu_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: User_65131559/TraCuu_65131559
        public ActionResult Index(SearchViewModel_65131559 searchModel)
        {
            if (searchModel == null) searchModel = new SearchViewModel_65131559();
            
            // Populate Filter Dropdowns
            searchModel.DanhMucCons = db.DanhMucCons.OrderBy(x => x.TenDanhMucCon).ToList(); 
            searchModel.NhaXuatBans = db.NhaXuatBans.OrderBy(x => x.TenNXB).ToList();
            
            // Unique Years from TaiLieu
            searchModel.NamXuatBans = db.TaiLieux
                .Where(t => t.NamXuatBan != null)
                .Select(t => t.NamXuatBan.Value)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // Query Documents
            var query = db.TaiLieux
                .Include(t => t.DanhMucCon)
                .Include(t => t.NhaXuatBan)
                .Include(t => t.TacGias)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.Keyword))
            {
                string kw = searchModel.Keyword.ToLower();
                query = query.Where(t => t.TenTaiLieu.ToLower().Contains(kw) || 
                                         t.TacGias.Any(tg => tg.HoTen.ToLower().Contains(kw)));
            }

            if (searchModel.SelectedMaDanhMuc.HasValue)
            {
                // Note: explicitly compare nullable values
                int catId = searchModel.SelectedMaDanhMuc.Value;
                query = query.Where(t => t.MaDanhMucCon == catId);
            }

            if (searchModel.SelectedMaNXB.HasValue)
            {
                int nxbId = searchModel.SelectedMaNXB.Value;
                query = query.Where(t => t.MaNXB == nxbId);
            }

            if (searchModel.SelectedNamXB.HasValue)
            {
                int year = searchModel.SelectedNamXB.Value;
                query = query.Where(t => t.NamXuatBan == year);
            }

            // Order by Created Date descending
            // Pagination Logic
            int pageSize = 12;
            int pageNumber = searchModel.Page ?? 1;
            int totalRecords = query.Count();
            
            searchModel.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            searchModel.PageNumber = pageNumber;

            // Order by Created Date descending and Apply Pagination
            searchModel.Results = query.OrderByDescending(t => t.NgayTao)
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToList();
            
            return View(searchModel);
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            
            var document = db.TaiLieux
                .Include(t => t.DanhMucCon)
                .Include(t => t.NhaXuatBan)
                .Include(t => t.TacGias)
                .Include(t => t.DanhGias)
                .Include("DanhGias.NguoiDung")
                .FirstOrDefault(t => t.MaTaiLieu == id);

            if (document == null) return HttpNotFound();

            // Retrieve related documents (same category)
            ViewBag.RelatedDocs = db.TaiLieux
                .Where(t => t.MaDanhMucCon == document.MaDanhMucCon && t.MaTaiLieu != document.MaTaiLieu)
                .Take(4)
                .ToList();

            return View(document);
        }

        // POST: Read (View Online)
        [Authorize] 
        public ActionResult Read(int id)
        {
            var document = db.TaiLieux.Find(id);
            if (document == null) return HttpNotFound();

            LogInteraction(id, "Xem");

            // Increment View Count
            document.LuotXem = (document.LuotXem ?? 0) + 1;
            db.SaveChanges();

            if (!string.IsNullOrEmpty(document.DuongDanFile))
            {
                // Check if it's an external URL (http:// or https://)
                if (document.DuongDanFile.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    document.DuongDanFile.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // External URL: redirect to it
                    return Redirect(document.DuongDanFile);
                }
                else
                {
                    // Local file: use Server.MapPath
                    string filePath = Server.MapPath(document.DuongDanFile);
                    if (System.IO.File.Exists(filePath))
                    {
                         string contentType = MimeMapping.GetMimeMapping(filePath);
                         return File(filePath, contentType);
                    }
                }
            }

            return RedirectToAction("Details", new { id = id });
        }

        // POST: Download
        [Authorize]
        public ActionResult Download(int id)
        {
            var document = db.TaiLieux.Find(id);
            if (document == null) return HttpNotFound();

            if (document.ChoPhepTai != true)
            {
                TempData["ErrorMessage"] = "Tài liệu này không cho phép tải xuống.";
                return RedirectToAction("Details", new { id = id });
            }

            // Quota Check
            var username = User.Identity.Name;
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);

            if (user != null && user.VaiTro != "Admin")
            {
                int limit = user.VaiTro == "CanBo" ? 30 : 4;
                int days = user.VaiTro == "CanBo" ? 30 : 7;
                DateTime since = DateTime.Now.AddDays(-days);

                int downloadCount = db.LichSuTruyCaps.Count(l => l.MaNguoiDung == user.MaNguoiDung && 
                                                                 l.HanhDong == "Tai" && 
                                                                 l.ThoiGian >= since);

                if (downloadCount >= limit)
                {
                    TempData["QuotaError"] = string.Format("Bạn đã hết hạn ngạch tải ({0} bản/{1} ngày). Vui lòng xem online hoặc đợi kỳ tiếp theo.", limit, days);
                    return RedirectToAction("Details", new { id = id });
                }
            }

            LogInteraction(id, "Tai");

            // Increment Download Count
            document.LuotTai = (document.LuotTai ?? 0) + 1;
            db.SaveChanges();

            if (!string.IsNullOrEmpty(document.DuongDanFile))
            {
                // Check if it's an external URL (http:// or https://)
                if (document.DuongDanFile.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    document.DuongDanFile.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // External URL: redirect to it
                    return Redirect(document.DuongDanFile);
                }
                else
                {
                    // Local file: use Server.MapPath
                    string filePath = Server.MapPath(document.DuongDanFile);
                    if (System.IO.File.Exists(filePath))
                    {
                        string contentType = "application/octet-stream";
                        string fileName = Path.GetFileName(filePath);
                        return File(filePath, contentType, fileName);
                    }
                }
            }

            return RedirectToAction("Details", new { id = id });
        }

        private void LogInteraction(int docId, string action)
        {
            var username = User.Identity.Name;
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);
            if (user != null)
            {
                var history = new LichSuTruyCap
                {
                    MaNguoiDung = user.MaNguoiDung,
                    MaTaiLieu = docId,
                    ThoiGian = DateTime.Now,
                    HanhDong = action
                };
                db.LichSuTruyCaps.Add(history);
                db.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

namespace ProjectCK_65131559.Models
{
    public class SearchViewModel_65131559
    {
        public string Keyword { get; set; }
        public int? SelectedMaDanhMuc { get; set; }
        public int? SelectedMaNXB { get; set; }
        public int? SelectedNamXB { get; set; }
        
        public int? Page { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public List<TaiLieu> Results { get; set; }
        public List<DanhMucCon> DanhMucCons { get; set; }
        public List<NhaXuatBan> NhaXuatBans { get; set; }
        public List<int> NamXuatBans { get; set; }
    }
}
