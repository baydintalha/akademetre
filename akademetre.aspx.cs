using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web;

public partial class akademetre : System.Web.UI.Page
{
    public DataTable ReferansData { get; set; }
    public List<string> departman_list { get; set; }
    public List<string> yillik_list { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void vericek_Click(object sender, EventArgs e)
    {
        ReferansData = new DataTable();
        ReferansData.Columns.Add("ad", typeof(String));

        string crdpat = HttpRuntime.AppDomainAppPath;
        var options = new ChromeOptions();
        //options.AddArgument("user-data-dir=" + crdpat + @"\akademetre\"); // kariyer.net auth talebine karşın cookie kullanmayı tercih ettim fakat gerek kalmadı.
        options.BinaryLocation = crdpat + @"\drivers\Google\Chrome\Application\chrome.exe"; //Chrome.exe ' nin bulunduğu alan belirtiliyor. Tüm sistemlerde çalışması için dosyalara ekliyorum fakat github kabul etmediği için bilgisayardaki chrome.exe yolu girilmeli.
        IWebDriver driver = new ChromeDriver(crdpat + @"\drivers", options);
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
             * Sonrasında bu bu urllerin yalnızca dosya adını almak için string değişkeninde gerekli manipülasyonları uyguladım.
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
            ReferansData.Rows.Add(referansadi);
        }
        // Şirket websitesinde veri çekme işlemi tamamlandıktan sonra kariyer.net sayfasına geçiş yaptım.
        driver.Navigate().GoToUrl("https://www.kariyer.net/firma-profil/akademetre-21590-12728");
        departman_list = new List<string>();
        yillik_list = new List<string>();
        IList<IWebElement> departmanlar = w.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//*[@aria-colindex='1']")));
        for (int i = 1; i < departmanlar.Count - 1; i++)
        {
            departman_list.Add(departmanlar[i].Text.ToString());
        }
        //Grafik üzerinde javascript olarak verileri işleyeceğim için alınan verileri bir string listesine alıp gerekli manipülasyonları yaptım.
        IList<IWebElement> yillik_veriler = w.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//*[@aria-colindex='4']")));
        for (int i = 1; i < departmanlar.Count - 1; i++)
        {
            yillik_list.Add(yillik_veriler[i].Text.ToString().Replace("%", ""));
        }
        driver.Dispose();
        driver.Quit();
        ReferansRepeater.DataSource = ReferansData;
        ReferansRepeater.DataBind();
    }

    //JavaScript koduna değişken olarak ileteceğim aşağıdaki verileri listeden stringe çevirdim.
    public string departman_java
    {
        get
        {
            try
            {
                return String.Join("\",\"", departman_list.ToArray());
            }
            catch
            {
                return "";
            }

        }
    }
    public string yillik_java
    {
        get
        {
            try
            {
                return String.Join(",", yillik_list.ToArray());
            }
            catch
            {
                return "";
            }

        }
    }
}