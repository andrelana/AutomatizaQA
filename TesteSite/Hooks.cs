using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TesteSite
{
    public class Hooks
    {
        #region Globals

        // Para uso geral.
        public static string Report = "";
        public static string Encrypted = "";
        public static string Decrypted = "";
        public static string ReproSteps = "";

        // Exclusivo para as aplicacoes WEB e Mobile.
        public static ChromeDriver ChromeDriver;
        public static AndroidDriver<AndroidElement> AndroidDriver;
        public static EventFiringWebDriver Driver;

        // Exclusivo para as APIs.
        public static string ClientId = "";
        public static string ClientSecret = "";
        public static string Access_Token = "";
        public static string Bearer = "";
        public static string RefreshToken = "";
        public static RestClient Client = new RestClient();
        public static RestRequest Request = new RestRequest();
        public static IRestResponse Response;

        #endregion

        #region Attributes
        [TestInitialize]
        /// <summary>
        /// Inicializa o teste, incluindo um report que será anexado a saída. 
        /// É executado a cada teste iniciado.
        /// </summary>        
        public void MyTestInitialize()
        {
            var WebConfig = "Deploy//WebConfig.json";

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--ignore-ssl-errors=yes");
            options.AddArgument("--ignore-certificate-errors");

            Console.WriteLine($"Test {TestContext.TestName} started at {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            var testResultsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");

            if (!Directory.Exists(testResultsDir))
            {
                Directory.CreateDirectory(testResultsDir);
            }

            Report = Path.Combine(testResultsDir, $"{TestContext.TestName}_{DateTime.Now:ddMMyyyyThhmmss}.html");

            using (StreamWriter sw = new StreamWriter(Report, true))
            {
                sw.WriteLine($"<hr/><h1 style='color:@OUTCOME'>{TestContext.TestName}</h1>");
            }

            Driver = Utilitarios.ToChromeDriver(Driver, "Deploy", options);
            Driver.Url = WebConfig.GetJsonToken("url");
        }

        [TestCleanup]
        /// <summary>
        /// Finaliza o report do teste, adiciona o log do teste à saída e fecha o Driver utilizado.
        /// Caso a propriedade 'bug' do TestContext esteja true um bug pode ser gerado automaticamente caso o teste falhe.
        /// </summary>        
        public void MyTestCleanup()
        {
            if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed)
            {
                File.WriteAllText(Report, File.ReadAllText(Report).Replace("@OUTCOME", "green"));
            }
            else
            {
                File.WriteAllText(Report, File.ReadAllText(Report).Replace("@OUTCOME", "red"));
            }

            try { using (StreamWriter sw = new StreamWriter(Report, true)) { sw.WriteLine($"<center><hr/><h4>Last Screenshoot</h4><img style='width:50%;height:auto;' src='data:image/png; base64, {Driver.GetScreenshot().AsBase64EncodedString}'/></center><hr/>"); } } catch { }

            try { Driver.Quit(); } catch { }

            try { ChromeDriver.Quit(); } catch { }

            //try { AndroidDriver.Quit(); } catch { }

            TestContext.AddResultFile(Report);

            Console.WriteLine(Report);
        }

        #endregion

        #region Properties

        private TestContext testContext;
        public TestContext TestContext { get { return testContext; } set { testContext = value; } }

        #endregion

        #region Methods

        /// <summary>
        /// Executa um comando SQL no Oracle. Caso seja uma consulta, retorna uma lista de pares chave-valor.
        /// </summary>
        /// <param name="queryString">Comando a ser executado.</param>
        /// <param name="connectionString">String de conexção a o banco Oracle.</param>
        /// <returns>Em caso de consulta, retorna uma lista de pares chave-valor.</returns>
        //public static List<List<KeyValuePair<string, string>>> MyOracleDataReader(string queryString, string connectionString)
        //{
        //    List<List<KeyValuePair<string, string>>> dataSet = new List<List<KeyValuePair<string, string>>>();

        //    using (OracleConnection connection = new OracleConnection(connectionString))
        //    {
        //        OracleCommand command = new OracleCommand(queryString, connection);
        //        connection.Open();
        //        OracleDataReader reader = command.ExecuteReader();
        //        try
        //        {
        //            while (reader.Read())
        //            {
        //                List<KeyValuePair<string, string>> row = new List<KeyValuePair<string, string>>();

        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    try
        //                    {
        //                        row.Add(new KeyValuePair<string, string>(reader.GetName(i), reader.GetString(i)));
        //                    }
        //                    catch
        //                    {
        //                        row.Add(new KeyValuePair<string, string>(reader.GetName(i), ""));
        //                    }
        //                }

        //                dataSet.Add(row);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine($"{e.Message} {e.InnerException} {e.StackTrace}");
        //        }
        //        finally
        //        {
        //            reader.Close();
        //        }

        //        return dataSet;
        //    }
        //}

        /// <summary>
        /// Executa uma consulta nom banco MongoDB. 
        /// </summary>
        /// <param name="connectionString">String de conexão para o MongoDB.</param>
        /// <param name="database">Nome da database.</param>
        /// <param name="collection">Nome da collection.</param>
        /// <param name="filter">Filtro da pesquisa.</param>
        /// <param name="project">Campos a serem retornados.</param>
        /// <param name="limit">Limite de registros retornados.</param>
        /// <returns>Lista de documentos Bson que atendem aos critérios.</returns>
        //public static List<BsonDocument> MyMongoConnect(string connectionString, string database, string collection, BsonDocument filter = null, string project = null, int limit = 1)
        //{
        //    try
        //    {
        //        var client = new MongoClient(connectionString);
        //        var db = client.GetDatabase(database);
        //        var col = db.GetCollection<BsonDocument>(collection);
        //        if (project is null)
        //        {
        //            if (filter != null)
        //            {
        //                return col.Find(filter).Limit(limit).ToList();
        //            }
        //            else
        //            {
        //                return col.Find(new BsonDocument()).Limit(limit).ToList();
        //            }
        //        }
        //        else
        //        {
        //            if (filter != null)
        //            {
        //                return col.Find(filter).Project(project).Limit(limit).ToList();
        //            }
        //            else
        //            {
        //                return col.Find(new BsonDocument()).Project(project).Limit(limit).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"{e.Message} {e.InnerException} {e.StackTrace}");
        //        Assert.Fail($"{e.Message} {e.InnerException} {e.StackTrace}");
        //    }

        //    return new List<BsonDocument>();
        //}

        /// <summary>
        /// Cria um workitem no Azure Devops do tipo BUG.
        /// </summary>
        /// <param name="azureConfig">Caminho para o arquivo de configuração do AzureDevops.</param>
        /// <returns></returns>
        //public WorkItem CreateBug(string azureConfig = "Deploy//AzureConfig.json")
        //{
        //    if (!File.Exists(azureConfig))
        //    {
        //        Fail($"Não foi possivel encontrar o arquivo: {azureConfig}");
        //    }

        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    string url = azureConfig.GetJsonToken("url");
        //    string project = azureConfig.GetJsonToken("project");
        //    string token = azureConfig.GetJsonToken("token");
        //    string login = azureConfig.GetJsonToken("login");

        //    RestClient client = new RestClient($"{url}/{project}/_apis/wit/attachments?fileName={TestContext.TestName}.html&api-version=6.0");
        //    client.Authenticator = new HttpBasicAuthenticator(login, token);
        //    RestRequest request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/octet-stream");
        //    byte[] dataToUpload = File.ReadAllBytes(Report);
        //    request.AddFile("tmpFile", dataToUpload, "tmp.html");
        //    var response = client.Execute(request);

        //    string attachmentUrl = JObject.Parse(response.Content).Value<string>("url");

        //    Uri uri = new Uri(azureConfig.GetJsonToken("url"));

        //    VssBasicCredential credentials = new VssBasicCredential("", token);
        //    JsonPatchDocument patchDocument = new JsonPatchDocument();

        //    patchDocument.Add(
        //        new JsonPatchOperation()
        //        {
        //            Operation = Operation.Add,
        //            Path = "/fields/System.Title",
        //            Value = TestContext.TestName
        //        }
        //    );

        //    patchDocument.Add(
        //        new JsonPatchOperation()
        //        {
        //            Operation = Operation.Add,
        //            Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
        //            Value = ReproSteps
        //        }
        //    );

        //    patchDocument.Add(
        //        new JsonPatchOperation()
        //        {
        //            Operation = Operation.Add,
        //            Path = "/fields/Microsoft.VSTS.Common.Priority",
        //            Value = "1"
        //        }
        //    );

        //    patchDocument.Add(
        //        new JsonPatchOperation()
        //        {
        //            Operation = Operation.Add,
        //            Path = "/fields/Microsoft.VSTS.Common.Severity",
        //            Value = "2 - High"
        //        }
        //    );

        //    if (!string.IsNullOrEmpty(attachmentUrl))
        //    {
        //        patchDocument.Add(
        //            new JsonPatchOperation()
        //            {
        //                Operation = Operation.Add,
        //                Path = "/relations/-",
        //                Value = new
        //                {
        //                    rel = "AttachedFile",
        //                    url = attachmentUrl,
        //                    attributes = new { comment = $"Bug criado automaticamente" }
        //                }
        //            }
        //            );
        //    }

        //    VssConnection connection = new VssConnection(uri, credentials);
        //    WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

        //    try
        //    {
        //        WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, project, "Bug").Result;

        //        Console.WriteLine("Bug Successfully Created: Bug #{0}", result.Id);

        //        return result;
        //    }
        //    catch (AggregateException ex)
        //    {
        //        Console.WriteLine("Error creating bug: {0}", ex.InnerException.Message);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Encripta um arquivo de configuração.
        /// </summary>
        /// <param name="EntradaDeDados">Caminho do arquivo a ser encriptado.</param>
        public static void Criptografar(string EntradaDeDados)
        {
            string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArquivoCriptografado.txt");

            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(AppDomain.CurrentDomain.FriendlyName);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(EntradaDeDados, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();

                Encrypted = File.ReadAllText(outputFile);
            }
            catch
            {
                Console.WriteLine("Criptografia falhou!", "Erro");
            }

            File.Copy(outputFile, EntradaDeDados, true);
        }

        /// <summary>
        /// Decripta um arquivo.
        /// </summary>
        /// <param name="EntradaDeDados">Caminho do arquivo a ser decriptado.</param>
        public static void DecryptFile(string EntradaDeDados)
        {
            string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArquivoDescriptografado.txt");

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = UE.GetBytes(AppDomain.CurrentDomain.FriendlyName);

            FileStream fsCrypt = new FileStream(EntradaDeDados, FileMode.Open);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateDecryptor(key, key),
                CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            int data;
            bool failed = false;
            Exception exception = new Exception();
            try
            {
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);
            }
            catch (Exception e)
            {
                failed = true;
                exception = e;
            }

            fsOut.Close();
            cs.Close();
            fsCrypt.Close();

            if (failed)
            {
                throw new Exception($"{exception.Message} {exception.InnerException} {exception.StackTrace}");
            }

            Decrypted = File.ReadAllText(outputFile);

            File.Copy(outputFile, EntradaDeDados, true);
        }

        /// <summary>
        /// Falha um teste.
        /// </summary>
        /// <param name="message">Mensagem a ser exibida junto com teste falhado.</param>
        public static void Fail(string message)
        {
            Assert.Fail(message);
        }

        /// <summary>
        /// Reporta um passo da navegação.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Driver_Navigated(object sender, WebDriverNavigationEventArgs e)
        {
            ReproSteps += $"<div>Ir para {e.Url}\n</div>";
        }

        /// <summary>
        /// Reporta a identificação de um elemento, juntamente com uma imagem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Driver_FindElementCompleted(object sender, FindElementEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(Report, true))
            {
                sw.WriteLine($"<center><hr/><h4>Elemento encontrado pelo identifcador: {e.FindMethod}</h4>");
                sw.WriteLine($"<img style='width:50%;height:auto;' src='data:image/png; base64, {Driver.GetScreenshot().AsBase64EncodedString}'/></center>");
            }
        }

        /// <summary>
        /// Reporta uma troca em um valor de um elemento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Driver_ElementValueChanging(object sender, WebElementValueEventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Report, true))
                {
                    sw.WriteLine($"<center><h4>SendKeys: {e.Value} | Element: {e.Element.TagName} {e.Element.Text} </center></h4>");
                    ReproSteps += $"<div>Digitar: {e.Value} no elemento {e.Element.TagName} {e.Element.Text}\n</div>";
                }
            }
            catch
            {
                using (StreamWriter sw = new StreamWriter(Report, true))
                {
                    sw.WriteLine($"<center><h4>SendKeys: {e.Value} | Element: {e.Element} {e.Element.Text} </center></h4>");
                    ReproSteps += $"<div>Digitar: {e.Value} no elemento {e.Element} {e.Element.Text}\n</div>";
                }
            }
        }

        /// <summary>
        /// Reporta o click em um elemento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Driver_ElementClicking(object sender, WebElementEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(Report, true))
            {
                try
                {
                    sw.WriteLine($"<center><h4>Click | Element: {e.Element.TagName} {e.Element.Text}</h4></center>");

                    ReproSteps += $"<div>Clicar no elemento {e.Element.TagName} {e.Element.Text}\n</div>";
                }
                catch
                {
                    sw.WriteLine($"<center><h4>Click | Element: {e.Element} {e.Element.Text}</h4></center>");

                    ReproSteps += $"<div>Clicar no elemento {e.Element} {e.Element.Text}\n</div>";
                }
            }
        }

        #endregion
    }
}
