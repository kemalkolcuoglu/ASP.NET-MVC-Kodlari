// Bu C# dosyası içerisinde SMTP (Simple Mail Transfer Protocol) ile mail atılma işleminin gerçekleştirilmesi kodlanmıştır.

/*
 * ************************************************
 *           Örnek Web.Config bilgileri           *
 * ************************************************
 *   <add key="MailHost" value="smtp.gmail.com"/> *
 *   <add key="MailPort" value="587"/>            *
 *   <add key="MailUser" value="xxxx@gmail.com"/> *
 *   <add key="MailPass" value="1111"/>           *
 * ************************************************
 */

public class ConfigHelper
{
	// Bu sınıf "Web.config" dosyası içerisinde yer alan Key değerlerinin Value değerlerini döndürür.
	// Örn: <add key="MailHost" value="smtp.gmail.com"/>
	// Bu örnekte Key değerimiz "MailHost" olarak belirlenmiştir
	public static T Get<T>(string key) // Key Mutlaka string olmalı
	{
		return (T)System.Convert.ChangeType(System.Configuration.ConfigurationManager.AppSettings[key], typeof(T));
	}
}

public class MailHelper
{
	/// <summary>
	///     Mail gönderim işleminin gerçekleştiği metottur.
	/// </summary>
	/// <param name="body">Mail gövdesi. HTML kodları içerebilir.</param>
	/// <param name="to">Maili alacak adreslerin listesi. String tipindedir.</param>
	/// <param name="subject">Mailin Konusudur. Kullanıcılara bu başlık ile maili görüntülenir.</param>
	/// <param name="isHtml">Eğer ki gövdede(body) HTML kodları yer alıyorsa bu değeri True yapmak gerekmektedir.</param>
	/// <returns>Komutlarda kullanılmak üzere XXXParameter dizisi tipinde değer döndürür</returns>
	public static bool SendMail(string body, string to, string subject, bool isHtml = true)
	{
		return SendMail(body, new System.Collections.Generic.List<string> { to }, subject, isHtml);
	}

	/// <summary>
	///     Mail gönderim işleminin gerçekleştiği metottur.
	/// </summary>
	/// <param name="body">Mail gövdesi. HTML kodları içerebilir.</param>
	/// <param name="to">Maili alacak adreslerin listesi. List tipindedir</param>
	/// <param name="subject">Mailin Konusudur. Kullanıcılara bu başlık ile maili görüntülenir.</param>
	/// <param name="isHtml">Eğer ki gövdede(body) HTML kodları yer alıyorsa bu değeri True yapmak gerekmektedir.</param>
	/// <returns>Komutlarda kullanılmak üzere XXXParameter dizisi tipinde değer döndürür</returns>
	public static bool SendMail(string body, System.Collections.Generic.List<string> to, string subject, bool isHtml = true)
	{
		bool result = false; // Geri dönüş değerimiz.

		try // Oluşabilecek herhangi bir istisna durumları için try-catch blokları kullanılması tavsiye edilir.
		{
			// MailMessage nesnesi kodlar üzerinde mail göndermemiz için kullanılır. System.Net.Mail ad uzayında yer almaktadır.
			var message = new System.Net.Mail.MailMessage
			{
				// MailAdress nesnesi gönderen için oluşturulur.
				// Maili gönderecek adres bilgisi web.config'den alınıyor.
				From = new System.Net.Mail.MailAddress(ConfigHelper.Get<string>("MailUser"))
			};

			// Mail gönderilecek adreslerin herbiri MailAdress nesnesi olarak oluşturuluyor.
			to.ForEach(x =>
			{
				message.To.Add(new System.Net.Mail.MailAddress(x));
				});

			// Gelen Parametreler nesne içerisine yazılır.
			message.Subject = subject; 
			message.Body = body; 
			message.IsBodyHtml = isHtml; 

			// Bu metot SMTP(Simple Mail Transfer Protocol) ile mail gönderimini içermektedir. O yüzden SMTP Client nesnesi ile uzaktaki smtp host ve port değerleri alınır.
			using (var smtp = new System.Net.Mail.SmtpClient(
				ConfigHelper.Get<string>("MailHost"), // Web.config dosyasından host ve port değerleri alınır.
				ConfigHelper.Get<int>("MailPort")))
			{
				smtp.EnableSsl = true; // SSL etkinse true değeri verilmelidir.

				// Aşağıda yorum satırı olarak eklenmiş olan iki satır Yandex.Mail kullanan geliştiriciler içindir.
				// Yandex üzerinden mail işlemlerinde bu iki satıra ihtiyacınız olacaktır ve eklemezseniz mailler gönderilmez.
				//smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                //smtp.UseDefaultCredentials = false;

				smtp.Credentials =
				new System.Net.NetworkCredential(
					ConfigHelper.Get<string>("MailUser"), // Maili gönderecek hesabın bilgileri alınır.
					ConfigHelper.Get<string>("MailPass")); // MailUser: E-Posta Adresi, MailPass: E-Posta adresinin şifresi

				smtp.Send(message); // Mailin gönderilmesi bu metotla gerçekleşir. Geri dönüş tipi yoktur ve iletilmezse hata fırlatır.
				result = true; // Hata oluşmaz ise mail gönderilmiştir ve bu metodun geri dönüşünün başarılı olduğu bildirilir.
			}
		}
		catch (System.Exception exp)
		{
			// Kendi özel istisna durumlarınızı yazabilirsiniz.
			// Örn: Hata oluşunca Mail gönderilmesi vb.
			System.Console.Write(exp.Message); 
		}

		return result;
	}
}