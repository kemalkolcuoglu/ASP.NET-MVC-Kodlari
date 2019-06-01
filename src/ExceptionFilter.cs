// System.Web.Mvc ad uzayında yer olan filter sınıflarının bir türevi olan ExceptionFilter; attribute olarak tanımlanan action ve controllerlarda hata olduğu zaman o hatayı yakalar ve durum yönetimi sağlar.

// Filter sınıfının oluşması için mutlaka FilterAttribute sınıfından kalıt alması gerekmektedir.
// Bu sınıfın bir Exception Filter olabilmesi için mutlaka IExceptionFilter arayüzünü uygulaması gerekmektedir.
public class ExceptionFilter : System.Web.Mvc.FilterAttribute, System.Web.Mvc.IExceptionFilter
{
	// Bu metot arayüz ile gelmektedir. İşlemler burada gerçekleşir.
	// Almış olduğu ExceptionContext nesnesi içerisinde gönderilen Request, HttpContext, Exception nesnelerinin bilgilerini içerir.
	// Daha detaylı bilgi için Bknz: https://docs.microsoft.com/en-us/dotnet/api/system.web.mvc.exceptioncontext?view=aspnet-mvc-5.2
	public void OnException(System.Web.Mvc.ExceptionContext filterContext)
	{
		string logText = GetLogText(filterContext); // filterContext nesnesi içerisinde yer alan değerlere göre kendimize özel bir log kayıt metni oluşturuyoruz.

		Logging(logText); // Log kaydı bu metot ile dosyaya yazılır.

		// Log kaydı bu metot ile mail olarak yetkiliye gönderilir.
		// Bu metot bu repo içerisinde yer alan "MailHelper" sınıfı kullanılarak yazılmıştır. O sınıfa göz atmanız tavsiye edilir.
		MailHelper.SendMail(logText, "abc@xyz.com", "Exception", false); 

		filterContext.ExceptionHandled = true; // Oluşan hatanın handle edilmesi işlemi. Eğer edilmezse kullanıcı sistemin hata sayfasını görür.
		filterContext.Result = new System.Web.Mvc.RedirectResult("/Home/Error"); // Son olarak kullanıcıyı kendi oluşturduğumuz hata sayfasına yönlendiririz.
	}

	public string GetLogText(System.Web.Mvc.ExceptionContext filterContext)
	{
		// String manipülasyonunda + operatörünü kullanmaktansa StringBuilder kullanılması tercih edilir. Bu yüzden StringBuilder kullanılmıştır.
		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		// Bu değer yalnızca görünümü güzelleştirmek için kullanılmıştır.
		// Başka herhangi bir anlamı yoktur.
		string separator = "\n----------------------------------------\n";

		// Hatayı Yaşayan Kullanıcının IP Adresi
		sb.Append("IP : ");
		sb.Append(filterContext.HttpContext.Request.UserHostName);
		sb.Append(separator);

		// Hatanın Gerçekleşme Tarihi ve Saati
		sb.Append("Exception Time: ");
		sb.Append(System.DateTime.Now);
		sb.Append(separator);

		// Hatanın Hangi Sayfada Gerçekleştiği
		sb.Append("URL: ");
		sb.Append(filterContext.HttpContext.Request.Url);
		sb.Append(separator);

		// Kullanıcının Kullandığı Browser
		sb.Append("Browser: ");
		sb.Append(filterContext.HttpContext.Request.Headers["User-Agent"].ToString());
		sb.Append(separator);

		// O An İşlem Gören HTTP Metodu (GET, POST, PUT, DELETE, vb.)
		sb.Append("Http Method: ");
		sb.Append(filterContext.HttpContext.Request.HttpMethod);
		sb.Append(separator);

		// Kullanıcının İşletim Sistemi
		sb.Append("Platform: ");
		sb.Append(filterContext.HttpContext.Request.Browser.Platform);
		sb.Append(separator);

		// Kullanının Kullandığı Browserın Kısa Adı
		sb.Append("Browser Type: ");
		sb.Append(filterContext.HttpContext.Request.Browser.Type);
		sb.Append(separator);

		// Kısa Hata Açıklaması
		sb.Append("Message: ");
		sb.Append(filterContext.Exception.Message);
		sb.Append(separator);

		// Detaylı Hata Çıktısı
		sb.Append("Exception: \n");
		sb.Append(filterContext.Exception);
		sb.Append(separator);

		// İç Hata Çıktısı
		sb.Append("Inner Exception: \n");
		sb.Append(filterContext.Exception.InnerException);
		sb.Append(separator);

		// Base Hata Çıktısı
		sb.Append("Base Exception: \n");
		sb.Append(filterContext.Exception.GetBaseException());
		sb.Append(separator);

		/*
		 * Son üç çıktının detayları için
		 * Bknz: https://docs.microsoft.com/en-us/dotnet/api/system.exception?view=netframework-4.8
		 */

		return sb.ToString();
	}

	public void Logging(string logText)
	{
		string path = System.IO.Path.Combine("D:\\Logs", "log.txt"); // Belirtilen dizindeki belirtilen dosya path olarak belirtilir.

		try // Bu tür IO işlemlerini try-catch bloklarının içerisinde yazılması tavsiye edilir.
		{
			if (!System.IO.File.Exists(path)) // Belirtilen dizinde belirtilen dosya yoksa bu kod sayesinde oluşturulur.
				System.IO.File.Create(path).Dispose();

			// Belirtilen dizindeki dosyaya veri yazılacaktır.
			// Dosyada daha önceden var olan verilerin kaybolmaması için append True olarak belirtilmiştir.
			// Encoding Türkçe karakterleri destekleyen formattadır.
			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path: path, append: true, encoding: System.Text.Encoding.GetEncoding("windows-1254")))
			{
				sw.Write(logText);
			}
		}
		catch (System.Exception exp)
		{
			System.Console.WriteLine(exp.Message);
			// Kendi özel istisna durumlarınızı yazabilirsiniz.
		}
	}
}