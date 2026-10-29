using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.Admin_65131559.Controllers
{
    public class TaiLieu_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/TaiLieu_65131559
        public ActionResult Index(int? page)
        {
            var taiLieux = db.TaiLieux
                .Include(t => t.DanhMucCon)
                .Include(t => t.NhaXuatBan)
                .Include(t => t.TacGias)
                .OrderByDescending(t => t.NgayTao)
                .ThenByDescending(t => t.MaTaiLieu);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<TaiLieu>.Create(taiLieux, pageNumber, pageSize));
        }

        // GET: Admin_65131559/TaiLieu_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            return View(taiLieu);
        }

        // GET: Admin_65131559/TaiLieu_65131559/Create
        public ActionResult Create()
        {
            ViewBag.MaDanhMucConList = new SelectList(db.DanhMucCons, "MaDanhMucCon", "TenDanhMucCon");
            ViewBag.MaNXBList = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
            return View();
        }

        // POST: Admin_65131559/TaiLieu_65131559/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaTaiLieu,TenTaiLieu,ISBN,MaDanhMucCon,MaNXB,NamXuatBan,MoTa,AnhBia,DuongDanFile,DinhDang,DungLuongMB,ChoPhepTai")] TaiLieu taiLieu, HttpPostedFileBase imageFile, HttpPostedFileBase docFile)
        {
            if (ModelState.IsValid)
            {
                // Handle Cover Image Upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    
                    if (System.IO.File.Exists(path))
                    {
                         fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
                         path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    }

                    imageFile.SaveAs(path);
                    taiLieu.AnhBia = "/Images/" + fileName;
                }

                // Handle Document File Upload
                if (docFile != null && docFile.ContentLength > 0)
                {
                    // Create directory if not exists
                    string docFolder = Server.MapPath("~/Uploads/Documents/");
                    if (!System.IO.Directory.Exists(docFolder))
                    {
                        System.IO.Directory.CreateDirectory(docFolder);
                    }

                    string docFileName = System.IO.Path.GetFileName(docFile.FileName);
                    string docPath = System.IO.Path.Combine(docFolder, docFileName);

                    if (System.IO.File.Exists(docPath))
                    {
                        docFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + docFileName;
                        docPath = System.IO.Path.Combine(docFolder, docFileName);
                    }

                    docFile.SaveAs(docPath);
                    taiLieu.DuongDanFile = "/Uploads/Documents/" + docFileName;
                    
                    // Auto-detect format if not set
                    if (string.IsNullOrEmpty(taiLieu.DinhDang))
                    {
                        string ext = System.IO.Path.GetExtension(docFileName).ToUpper().Replace(".", "");
                        taiLieu.DinhDang = ext.Length > 10 ? ext.Substring(0, 10) : ext;
                    }

                    // Auto-detect size
                    taiLieu.DungLuongMB = Math.Round(docFile.ContentLength / 1024f / 1024f, 2);
                }

                // Tự động set các giá trị mặc định
                taiLieu.NgayTao = DateTime.Now;
                taiLieu.LuotXem = 0;
                taiLieu.LuotTai = 0;
                
                // Mặc định cho phép tải nếu không được set
                if (!taiLieu.ChoPhepTai.HasValue)
                {
                    taiLieu.ChoPhepTai = true;
                }

                try
                {
                    db.TaiLieux.Add(taiLieu);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm tài liệu thành công!";
                    return RedirectToAction("Index");
                }
                 catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError("", $"Lỗi: {validationError.ErrorMessage} (Thuộc tính: {validationError.PropertyName})");
                        }
                    }
                }
                catch (Exception ex)
                {
                     ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.MaDanhMucConList = new SelectList(db.DanhMucCons, "MaDanhMucCon", "TenDanhMucCon", taiLieu.MaDanhMucCon);
            ViewBag.MaNXBList = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", taiLieu.MaNXB);
            return View(taiLieu);
        }

        // GET: Admin_65131559/TaiLieu_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = db.TaiLieux
                .Include(t => t.TacGias)
                .FirstOrDefault(t => t.MaTaiLieu == id);
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDanhMucConList = new SelectList(db.DanhMucCons, "MaDanhMucCon", "TenDanhMucCon", taiLieu.MaDanhMucCon);
            ViewBag.MaNXBList = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", taiLieu.MaNXB);
            return View(taiLieu);
        }

        // POST: Admin_65131559/TaiLieu_65131559/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTaiLieu,TenTaiLieu,ISBN,MaDanhMucCon,MaNXB,NamXuatBan,MoTa,AnhBia,DuongDanFile,DinhDang,DungLuongMB,ChoPhepTai,LuotXem,LuotTai,NgayTao")] TaiLieu taiLieu, HttpPostedFileBase imageFile, HttpPostedFileBase docFile)
        {
            string uploadedImagePath = null;
            string uploadedDocPath = null;

            if (ModelState.IsValid)
            {
                // Handle Image upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                         fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
                         path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    }

                    imageFile.SaveAs(path);
                    uploadedImagePath = path; // Tracking to delete if fail
                    taiLieu.AnhBia = "/Images/" + fileName;
                }

                // Handle Document Upload
                if (docFile != null && docFile.ContentLength > 0)
                {
                     // Create directory if not exists
                    string docFolder = Server.MapPath("~/Uploads/Documents/");
                    if (!System.IO.Directory.Exists(docFolder))
                    {
                        System.IO.Directory.CreateDirectory(docFolder);
                    }

                    string docFileName = System.IO.Path.GetFileName(docFile.FileName);
                    string docPath = System.IO.Path.Combine(docFolder, docFileName);

                    if (System.IO.File.Exists(docPath))
                    {
                        docFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + docFileName;
                        docPath = System.IO.Path.Combine(docFolder, docFileName);
                    }

                    docFile.SaveAs(docPath);
                    uploadedDocPath = docPath; // Tracking
                    taiLieu.DuongDanFile = "/Uploads/Documents/" + docFileName;

                     // Auto-update size if not set or just update it
                     taiLieu.DungLuongMB = Math.Round(docFile.ContentLength / 1024f / 1024f, 2);
                }

                try
                {
                    db.Entry(taiLieu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật tài liệu thành công!";
                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Cleanup uploaded files if DB save failed
                    if (uploadedImagePath != null && System.IO.File.Exists(uploadedImagePath)) System.IO.File.Delete(uploadedImagePath);
                    if (uploadedDocPath != null && System.IO.File.Exists(uploadedDocPath)) System.IO.File.Delete(uploadedDocPath);

                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError("", $"Lỗi: {validationError.ErrorMessage} (Thuộc tính: {validationError.PropertyName})");
                        }
                    }
                }
                catch (Exception ex)
                {
                     // Cleanup
                    if (uploadedImagePath != null && System.IO.File.Exists(uploadedImagePath)) System.IO.File.Delete(uploadedImagePath);
                    if (uploadedDocPath != null && System.IO.File.Exists(uploadedDocPath)) System.IO.File.Delete(uploadedDocPath);

                     ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
            ViewBag.MaDanhMucConList = new SelectList(db.DanhMucCons, "MaDanhMucCon", "TenDanhMucCon", taiLieu.MaDanhMucCon);
            ViewBag.MaNXBList = new SelectList(db.NhaXuatBans, "MaNXB",  "TenNXB", taiLieu.MaNXB);
            return View(taiLieu);
        }

        // GET: Admin_65131559/TaiLieu_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            return View(taiLieu);
        }

        // POST: Admin_65131559/TaiLieu_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            db.TaiLieux.Remove(taiLieu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin_65131559/TaiLieu_65131559/ManageAuthors/5
        public ActionResult ManageAuthors(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var taiLieu = db.TaiLieux
                .Include(t => t.TacGias)
                .Include(t => t.DanhMucCon)
                .FirstOrDefault(t => t.MaTaiLieu == id);
                
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            
            // Lấy danh sách tất cả tác giả
            ViewBag.AllAuthors = db.TacGias.OrderBy(t => t.HoTen).ToList();
            return View(taiLieu);
        }

        // POST: Admin_65131559/TaiLieu_65131559/ManageAuthors/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAuthors(int id, int[] selectedAuthors)
        {
            var taiLieu = db.TaiLieux
                .Include(t => t.TacGias)
                .FirstOrDefault(t => t.MaTaiLieu == id);
                
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            
            // Xóa tất cả tác giả hiện tại
            taiLieu.TacGias.Clear();
            
            // Thêm các tác giả được chọn
            if (selectedAuthors != null && selectedAuthors.Length > 0)
            {
                foreach (var authorId in selectedAuthors)
                {
                    var tacGia = db.TacGias.Find(authorId);
                    if (tacGia != null)
                    {
                        taiLieu.TacGias.Add(tacGia);
                    }
                }
            }
            
            db.SaveChanges();
            TempData["SuccessMessage"] = $"Đã cập nhật tác giả cho tài liệu: {taiLieu.TenTaiLieu}";
            return RedirectToAction("Edit", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
