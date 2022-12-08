using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using DocumentFormat.OpenXml.Packaging;

namespace ConsoleAkademetre
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable ReferansData = new DataTable();
            ReferansData.Columns.Add("Referans Adı", typeof(String));
            string excelyolu = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\ConsoleAkademetre\bin\Debug\", "\\");
            var options = new ChromeOptions();
            //options.AddArgument("user-data-dir=" + crdpat + @"\akademetre\"); // kariyer.net auth talebine karşın cookie kullanmayı tercih ettim fakat gerek kalmadı.
            //GitHub endi driver ve chrome dosyamı upload etmeme izin vermediği için yapılandırmayı default olarak bırakıyorum. 
            //options.BinaryLocation = crdpat + @"\drivers\Google\Chrome\Application\chrome.exe"; //Chrome.exe ' nin bulunduğu alan belirtiliyor. Tüm sistemlerde çalışması için dosyalara ekliyorum fakat github kabul etmediği için bilgisayardaki chrome.exe yolu girilmeli.
            //IWebDriver driver = new ChromeDriver(crdpat + @"\drivers", options);

            IWebDriver driver = new ChromeDriver(options);
            WebDriverWait w = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            driver.Navigate().GoToUrl("https://www.akademetre.com/Referanslar.aspx"); // Şirket web sitesi üzerinde botu başlattım.
            Thread.Sleep(500);
            IList<IWebElement> referanslar = new List<IWebElement>();
            referanslar.Clear();
            referanslar = w.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("img-responsive"))); // Referanslar sayfasında CSS sınıfı img-responsive olan ögeleri bir listeye topladım.
            foreach (var item in referanslar)
            {
                /*
                 * WebList üzerinde döngü kurdum ve bulunan tüm image kodlarının src attributelerini çektim.
                 * Sonrasında bu bu url'lerin yalnızca dosya adını almak için string değişkeninde gerekli manipülasyonları uyguladım.
                 */
                string referansadi = item.GetAttribute("src").ToString();
                referansadi = referansadi.Substring(referansadi.LastIndexOf("/") + 1);
                referansadi = referansadi.Substring(0, referansadi.LastIndexOf("."));
                try
                {
                    referansadi = referansadi.Substring(0, referansadi.LastIndexOf("-logo"));
                }
                catch
                {
                }
                Console.WriteLine(referansadi);
                ReferansData.Rows.Add(referansadi);
            }
            // Şirket websitesinde veri çekme işlemi tamamlandıktan sonra kariyer.net sayfasına geçiş yaptım.
            driver.Navigate().GoToUrl("https://www.kariyer.net/firma-profil/akademetre-21590-12728");
            List<string> departman_list = new List<string>();
            List<string> yillik_list = new List<string>();
            IList<IWebElement> departmanlar = w.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//*[@aria-colindex='1']")));
            for (int i = 1; i < departmanlar.Count - 1; i++)
            {
                departman_list.Add(departmanlar[i].Text.ToString());
            }
            //Grafik üzerinden işleyeceğim verileri bir string listesine alıp gerekli manipülasyonları yaptım.
            IList<IWebElement> yillik_veriler = w.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//*[@aria-colindex='4']")));
            for (int i = 1; i < departmanlar.Count - 1; i++)
            {
                yillik_list.Add(yillik_veriler[i].Text.ToString().Replace("%", ""));
            }
            driver.Dispose();
            driver.Quit();

            DataTable kariyerverileri = new DataTable();
            kariyerverileri.Columns.Add("Departmanlar", typeof(String));
            kariyerverileri.Columns.Add("Alımlar", typeof(int));

            for (int i = 0; i < yillik_list.Count; i++)
            {
                kariyerverileri.Rows.Add(new Object[]{
                    departman_list[i],
                    yillik_list[i]
                });
            }

            var ds_kariyer = new DataSet();
            ds_kariyer.Tables.Add(kariyerverileri.Copy());

            var ds_referans = new DataSet();
            ds_referans.Tables.Add(ReferansData.Copy());

            /*
             * Aynı anda excel çıktısı almak isteyen kullanıcılar olabileceğinden, şahsen sık sık kullandığım ve bize tamamiyle eşsiz değerler döndüren bir kod bloğu kullanıyorum. Random sınıfı da
             * kullanılabilirdi fakat daha uniq bir sistem olması açısından ben bunu tercih ediyorum. Sonrasında aldığımız bu benzersiz değer adında geçici olarak bir excel dosyası açıyorum.
            */

            excele_aktar(ds_referans, excelyolu + @"Referanslar.xlsx"); // Excel olarak export işlemi için verileri ve dosya yolunu ilgili işleme yönlendiriyorum.
            excele_aktar(ds_kariyer, excelyolu + @"Kariyer.xlsx"); // Excel olarak export işlemi için verileri ve dosya yolunu ilgili işleme yönlendiriyorum.

            Console.WriteLine("****************************************");
            Console.WriteLine("Selenium işlemleri tamamlandı, proje klasöründe Referanslar ve Kariyer olmak üzere iki excel dosyasına veriler aktarıldı.");
            Console.WriteLine("Dosya Yolu : " + excelyolu);

            var input = Console.ReadLine();

            Console.WriteLine(input);
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
