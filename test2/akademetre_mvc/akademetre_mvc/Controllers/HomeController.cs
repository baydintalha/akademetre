using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DocumentFormat.OpenXml.Packaging;

namespace akademetre_mvc.Controllers
{
    public class HomeController : Controller
    {
        public DataTable kayitlar { get; set; }
        public ActionResult Index()
        {
            kayitlar = new DataTable();
            if (Session["kullanici"] == null)
            {
                kayitlar.Columns.Add("isim", typeof(String));
                kayitlar.Columns.Add("soyisim", typeof(String));
                kayitlar.Columns.Add("adres", typeof(String));
                kayitlar.Columns.Add("mail", typeof(String));
                Session["kullanici"] = kayitlar;
            }
            else
            {
                kayitlar = (DataTable)Session["kullanici"];
            }

            var ds = new DataSet();
            ds.Tables.Add(kayitlar.Copy());
            if (ds.Tables[0].Rows.Count<1)
            {
                return View();
            }
            else
            {
                return View(ds);
            }
            
        }

        public ActionResult HandleButtonClick(string buton)
        {
            DataTable dt = new DataTable();
            dt = (DataTable)Session["kullanici"];
            if (buton=="excel")
            {
                var ds = new DataSet();
                ds.Tables.Add(dt.Copy());

                /*
                 * Aynı anda excel çıktısı almak isteyen kullanıcılar olabileceğinden, şahsen sık sık kullandığım ve bize tamamiyle eşsiz değerler döndüren bir kod bloğu kullanıyorum. Random sınıfı da
                 * kullanılabilirdi fakat daha uniq bir sistem olması açısından ben bunu tercih ediyorum. Sonrasında aldığımız bu benzersiz değer adında geçici olarak bir excel dosyası açıyorum.
                */
                long ticks = DateTime.Now.Ticks;
                byte[] bytes = BitConverter.GetBytes(ticks);
                string dosyadi = Convert.ToBase64String(bytes).Replace('+', '_').Replace('/', '-').TrimEnd('=');

                string exceldosyam = HttpRuntime.AppDomainAppPath + dosyadi + @".xlsx";
                excele_aktar(ds, exceldosyam); // Excel olarak export işlemi için verileri ve dosya yolunu ilgili işleme yönlendiriyorum.

                Response.ContentType = "application/excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Veriler.xlsx");
                Response.WriteFile(exceldosyam);
                Response.Flush();
                System.IO.File.Delete(exceldosyam);
                Response.End();
            }
            if (buton == "ekle")
            {
                dt.Rows.Add(new Object[]{
                    Request["isim"],
                    Request["soyisim"],
                    Request["adres"],
                    Request["mail"]
                });
            }
            if (buton == "temizle")
            {
                Session.Abandon();
            }
            return RedirectToAction("Index", new { e = 1});
        }

        public static void excele_aktar(DataSet ds, string dosyayolu)
        {
            // Önceki projelerimde ClosedXML ile çalıştığım için fazla OpenXml tecrübem yoktu. Bu yüzden aşağıdaki stackoverflow adresinden yardım aldım.
            // Kodları olduğu gibi kopyalamaktan ziyade anlayıp, açıklama yaparak sizlere iletiyorum.
            // stackoverflow.com/questions/11811143/export-datatable-to-excel-with-open-xml-sdk-in-c-sharp

            using (var dosya = SpreadsheetDocument.Create(dosyayolu, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook)) // OpenXml kütüphanesi kullanılarak verilen dosya yolu üzerinde excel açılıyor.
            {
                var sayfa = dosya.AddWorkbookPart(); // Excel dosyası üzerinde bir çalışma alanı açıyoruz.
                dosya.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                dosya.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = 1;
                // Dosyamızı, sayfamızı ve kaçıncı sayfada işlem yapacağımızı kütüphaneye belirterek veriler üzerinde bir döngü başlatıyoruz.
                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = dosya.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = dosya.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = dosya.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();

                    //Header alanımız için verilerimiz üzerinde ayı bir döngü başlatarak kolon içeriklerini excel'e ekliyoruz.
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    //Satır satır tüm verileri eklemek için yine ayrı bir döngü başlatıyor ve DataTable'den verileri alarak Excel'e aktarıyoruz.
                    foreach (DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }
                dosya.Save();
                dosya.Close();
            }
        }
    }
}