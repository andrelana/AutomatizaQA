using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TesteSite
{
    public static class Utilitarios
    {
        /// <summary>
        /// Muda o objeto driver de forma a execuar ações sobre o Chrome.
        /// </summary>
        /// <param name="driver">Driver isntanciado inicialmente.</param>
        /// <param name="chromeDriverDirectory">Pasta onde está o ChromeDriver.</param>
        /// <param name="options">Opções do ChromeDriver.</param>
        /// <param name="commandTimeoutInSeconds">Timeout que será respeitado pela ChromeDriver em cada comando.</param>
        /// <returns></returns>
        public static EventFiringWebDriver ToChromeDriver(this EventFiringWebDriver driver, string chromeDriverDirectory = "Deploy", ChromeOptions options = null, int commandTimeoutInSeconds = 30)
        {
            if (options is null)
            {
                options = new ChromeOptions();
            }

            Hooks.ChromeDriver = new ChromeDriver(chromeDriverDirectory, options);
            Hooks.ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(commandTimeoutInSeconds);
            Hooks.ChromeDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(commandTimeoutInSeconds);
            Hooks.ChromeDriver.Manage().Window.Maximize();

            Hooks.Driver = new EventFiringWebDriver(Hooks.ChromeDriver);
            Hooks.Driver.ElementClicking += Hooks.Driver_ElementClicking;
            Hooks.Driver.ElementValueChanging += Hooks.Driver_ElementValueChanging;
            Hooks.Driver.FindElementCompleted += Hooks.Driver_FindElementCompleted;
            Hooks.Driver.Navigated += Hooks.Driver_Navigated;

            return Hooks.Driver;
        }

        /// <summary>
        /// Muda o objeto driver de forma a execuar ações sobre o Android.
        /// </summary>
        /// <param name="driver">Driver isntanciado inicialmente.</param>
        /// <param name="androidConfig">Pasta para o arquivo Json com as configurações do Android.</param>
        /// <param name="remoteAddress">Endereçõ do onde o Android está instalado e aberto.</param>
        /// <param name="options">Opções do AndroidDriver.</param>
        /// <param name="commandTimeoutInSeconds">Timeout que será respeitado pela AndroidDriver em cada comando.</param>
        /// <returns></returns>
        public static EventFiringWebDriver ToAndroidDriver(this EventFiringWebDriver driver, string androidConfig = "Deploy//AndroidConfig.json", string remoteAddress = "http://127.0.0.1:4723/wd/hub", AppiumOptions options = null, int commandTimeoutInSeconds = 180)
        {
            if (!File.Exists(androidConfig))
            {
                Hooks.Fail($"Não foi possível encontrar o arquivo: {androidConfig}");
            }

            if (options is null)
            {
                options = new AppiumOptions();
            }

            options.PlatformName = "Android";
            options.AddAdditionalCapability("autoGrantPermissions", true);
            options.AddAdditionalCapability("autoAcceptAlerts", true);
            options.AddAdditionalCapability("ignoreUnimportantViews", true);
            try { options.AddAdditionalCapability("appPackage", androidConfig.GetJsonToken("appPackage")); } catch { }
            try { options.AddAdditionalCapability("appActivity", androidConfig.GetJsonToken("appActivity")); } catch { }
            try { options.AddAdditionalCapability("deviceName", androidConfig.GetJsonToken("deviceName")); } catch { }

            Hooks.AndroidDriver = new AndroidDriver<AndroidElement>(new Uri(remoteAddress), options, TimeSpan.FromSeconds(commandTimeoutInSeconds));
            Hooks.AndroidDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            Hooks.Driver = new EventFiringWebDriver(Hooks.AndroidDriver);
            Hooks.Driver.FindElementCompleted += Hooks.Driver_FindElementCompleted;
            Hooks.Driver.ElementValueChanging += Hooks.Driver_ElementValueChanging;
            Hooks.Driver.ElementClicking += Hooks.Driver_ElementClicking;

            return Hooks.Driver;
        }

        /// <summary>
        /// Digita uma string em um campo com intervalo de 1 segundo entre os caracteres.
        /// </summary>
        /// <param name="element">Elemento que receberá o texto.</param>
        /// <param name="message">Texto a ser digitado.</param>
        /// <param name="timerInSeconds">Tempo de espera entre a digitação dos caracteres.</param>
        public static void SendkeysCharByChar(this IWebElement element, string message, int timerInSeconds = 1)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (element != null && element.Enabled)
                {
                    foreach (var c in message.ToCharArray())
                    {
                        element.SendKeys(c.ToString());

                        Thread.Sleep(timerInSeconds * 1000);
                    }
                }
            }
        }

        /// <summary>
        /// Navega diretamento para uma determinada URL (FAVOR, UTILIZAR ESSE MÉTODO SOMENTE EM ÚLTIMO CASO). 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="relativeUrl"></param>
        /// <param name="webConfig"></param>
        public static void GotToRelativeUrl(this EventFiringWebDriver driver, string relativeUrl, string webConfig = "Deploy//WebConfig.json")
        {
            if (!string.IsNullOrEmpty(relativeUrl))
            {
                if (!File.Exists(webConfig))
                {
                    Hooks.Fail($"Não foi possível encontrar o arquivo: {webConfig}");
                }

                if (relativeUrl.StartsWith('/'))
                {
                    relativeUrl = relativeUrl.Substring(1);
                }

                try
                {
                    var baseUrl = new Uri(driver.Url).Authority;

                    var url = Path.Combine(JObject.Parse(File.ReadAllText(webConfig)).SelectToken("url").ToString(), relativeUrl);

                    driver.Navigate().GoToUrl(url);
                }
                catch (Exception e)
                {
                    Hooks.Fail($"Não foi possível navegar para {relativeUrl}. {e.Message} {e.InnerException} {e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Obtem o token contido em um arquivo de configuração JSON.
        /// </summary>
        /// <param name="fileName">Caminho do arquivo JSON.</param>
        /// <param name="token">O token que se deseja obter.</param>
        /// <returns></returns>
        public static string GetJsonToken(this string fileName, string token)
        {
            try
            {
                Hooks.DecryptFile(fileName);

                return JObject.Parse(Hooks.Decrypted).Value<string>(token);
            }
            catch { }

            try
            {
                return JObject.Parse(File.ReadAllText(fileName)).Value<string>(token);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Obtém o caminho completo para um arquivo.
        /// </summary>
        /// <param name="path">Caminho do arquivo.</param>
        /// <param name="fileName">Nome do arquivo.</param>
        /// <returns></returns>
        public static string GetFile(this string path, string fileName)
        {
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Grava no report do teste, um texto. 
        /// </summary>
        /// <param name="report">Caminho completo para o arquivo.</param>
        /// <param name="message">Texto da mensagem.</param>
        public static void LogMessage(this string report, string message)
        {
            using (StreamWriter sw = new StreamWriter(report, true))
            {
                sw.WriteLine($"<h4>{message}</h4>");
            }
        }

        /// <summary>
        /// Grava no report do teste, um texto estilizado (em azul e itálico). 
        /// </summary>
        /// <param name="report">Caminho completo para o arquivo.</param>
        /// <param name="message">Texto da mensagem.</param>
        public static void LogTextBlueItalic(this string report, string message)
        {
            Console.WriteLine(message);

            using (StreamWriter sw = new StreamWriter(report, true))
            {
                sw.WriteLine($"<hr/><h4 style='color:blue;'><i>{message}</i></h4>");
            }
        }

        /// <summary>
        /// Grava no report do teste, um texto estilizado (em verde). 
        /// </summary>
        /// <param name="report">Caminho completo para o arquivo.</param>
        /// <param name="message">Texto da mensagem.</param>
        public static void LogTextGreen(this string report, string message)
        {
            Console.WriteLine(message);

            using (StreamWriter sw = new StreamWriter(report, true))
            {
                sw.WriteLine($"<hr/><h4 style='color:green;'>{message}</h4>");
            }
        }

        /// <summary>
        /// Grava no report do teste, um texto estilizado (em vermelho). 
        /// </summary>
        /// <param name="report">Caminho completo para o arquivo.</param>
        /// <param name="message">Texto da mensagem.</param>
        public static void LogTextRed(this string report, string message)
        {
            Console.WriteLine(message);

            using (StreamWriter sw = new StreamWriter(report, true))
            {
                sw.WriteLine($"<hr/><h4 style='color:red;'>{message}</h4>");
            }
        }

        /// <summary>
        /// Grava no report do teste, um texto estilizado. 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="message"></param>
        public static void LogText(this string report, string message)
        {
            Console.WriteLine(message);

            using (StreamWriter sw = new StreamWriter(report, true))
            {
                sw.WriteLine($"<hr/><h4>{message}</h4>");
            }
        }

        /// <summary>
        /// Realiza o checkpoint de acordo com a resposta da requisição, no report.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void ApiCheckpoint(this string report, bool condition, string message = "")
        {
            report.LogTextBlueItalic($"Endpoint: {Hooks.Client.BaseUrl}{Hooks.Request.Resource}");
            report.LogText($"Method: {Hooks.Request.Method}");

            if (Hooks.Response.Request.Body != null)
            {
                report.LogText($"Request Body: {Hooks.Response.Request.Body.Value}");
            }

            report.LogText($"Http Status Code: {Hooks.Response.StatusCode}");
            report.LogText($"Response: {Hooks.Response.Content}");

            if (condition)
            {
                report.LogTextGreen(message);
            }
            else
            {
                report.LogTextRed(message);

                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Inicializa o RESTClient conforme as configurações encontradas no ApiConfig.json.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiConfig"></param>
        public static void InitClient(this RestClient client, string apiConfig = "Deploy\\ApiConfig.json")
        {
            string client_id, client_secret, baseUrl;
            string json = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig);

            if (!File.Exists(json))
            {
                Assert.Fail($"Não foi possível encontrar o arquivo {json}");
            }

            client_id = JObject.Parse(File.ReadAllText(json)).Value<string>("client_id");
            client_secret = JObject.Parse(File.ReadAllText(json)).Value<string>("client_secret");
            baseUrl = JObject.Parse(File.ReadAllText(json)).Value<string>("baseUrl");

            client = new RestClient(baseUrl);
            client.Authenticator = new HttpBasicAuthenticator(client_id, client_secret);
            client.AddDefaultHeader("Content-Type", ContentType.Json);
            client.AddDefaultHeader("client_id", client_id);

            Hooks.Client = client;
        }

        public static string GenerateRandomEmailAddress()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 10; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString() + "@teste.com." + builder.ToString().Substring(0, 2).ToLower();
        }

        /// <summary>
        /// Gera uma string randomica com entre 3 e 500 caracteres.
        /// </summary>
        /// <param name="prefixo">Prefixo a ser concatenado com o texto randômico gerado.</param>
        /// <param name="size">Tamanho em caracteres do texto a ser gerado.</param>
        /// <returns></returns>
        public static string GenerateRandomText(string prefixo = null, int size = 10)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            if (size > 500)
                size = 500;

            if (size < 3)
                size = 3;

            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            if (prefixo == null)
                return builder.ToString() + "Random";
            else
                return prefixo + builder.ToString();
        }
    }
}
