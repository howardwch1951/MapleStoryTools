using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Reflection;
using log4net;

namespace Update
{
    public partial class frmMain : Form
    {
        string message = "程式更新中，請稍後";
        private Point mouseDownLocation;
        public readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            //log4net 載入設定檔
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Path.Combine(Application.StartupPath, "log4net.config.xml")));
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // 取得主螢幕的尺寸和位置
            Screen mainScreen = Screen.PrimaryScreen;
            Rectangle screenBounds = mainScreen.Bounds;

            // 計算 Form 的中心位置
            int centerX = screenBounds.Left + (screenBounds.Width - Width) / 2;
            int centerY = screenBounds.Top + (screenBounds.Height - Height) / 2;

            // 設定 Form 的位置為中心位置
            Location = new Point(centerX, centerY);

            Task.Run(DoUpdateWork);
        }

        private void timerUpdateLabel_Tick(object sender, EventArgs e)
        {
            if (message.Contains("網路連線失敗，即將重新啟動程式"))
                labUpdateText.ForeColor = Color.Red;

            switch (DateTime.Now.Second % 6)
            {
                case 0:
                    labUpdateText.Text = message;
                    break;
                case 1:
                    labUpdateText.Text = $"{message}.";
                    break;
                case 2:
                    labUpdateText.Text = $"{message}..";
                    break;
                case 3:
                    labUpdateText.Text = $"{message}....";
                    break;
                case 4:
                    labUpdateText.Text = $"{message}.....";
                    break;
                case 5:
                    labUpdateText.Text = $"{message}......";
                    break;
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 記錄滑鼠點擊的起始位置
                mouseDownLocation = e.Location;
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 計算滑鼠移動的距離
                int deltaX = e.X - mouseDownLocation.X;
                int deltaY = e.Y - mouseDownLocation.Y;

                // 調整 Form 的位置
                Location = new Point(Left + deltaX, Top + deltaY);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 重置滑鼠點擊的起始位置
                mouseDownLocation = Point.Empty;
            }
        }

        #region 檢查更新
        private async Task DoUpdateWork()
        {
            JObject obj;
            JArray data;
            bool canUpdate = false;
            string body = "";
            string serverVersion = "";
            string fileName = "";
            string fileUrl = "";
            string zipFilePath = "";
            string extractPath = "";
            string note = "";
            try
            {
                log.Debug("準備更新");
                #region 關閉MapleStoryTools程式
                Process[] processes = Process.GetProcessesByName("MapleStoryTools");

                foreach (Process process in processes)
                {
                    process.Kill();
                }
                log.Debug("已關閉主程式");
                #endregion

                if (CheckConnect())
                {
                    log.Debug("設定Notion API");
                    #region 設定Notion API
                    Uri apiUrl;
                    string api = "secret_d9q9iNSM6RsFK6SAgpfdTAgrWn8nul9HmypATEgTktU";
                    if (CheckSimulationn())
                        apiUrl = new Uri("https://api.notion.com/v1/databases/5ffc33fd3e9c455586c827d509c04852/query");
                    else
                        apiUrl = new Uri("https://api.notion.com/v1/databases/fce1f59ac3674047a2590da3f3e2bc06/query");

                    var httpClient = new HttpClient();
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = apiUrl,
                        Headers =
                    {
                        { "Authorization", $"Bearer {api}" },
                        { "accept", "application/json" },
                        { "Notion-Version", "2022-06-28" },
                    },
                        Content = new StringContent("{\"page_size\":100}")
                        {
                            Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                        }
                    };
                    #endregion

                    log.Debug("取得最新版本號、讀取更新檔");
                    #region Notion Table 取得最新版本號、更新檔讀取
                    using (var response = await httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        body = await response.Content.ReadAsStringAsync();
                    }

                    obj = JObject.Parse(body);
                    data = obj.GetValue("results") as JArray;
                    canUpdate = Convert.ToBoolean(data[0]["properties"]["Push"]["checkbox"]);
                    serverVersion = data[0]["properties"]["Version"]["title"][0]["text"]["content"].ToString();
                    fileName = data[0]["properties"]["Files"]["files"][0]["name"].ToString();
                    fileUrl = data[0]["properties"]["Files"]["files"][0]["file"]["url"].ToString();
                    note = data[0]["properties"]["Note"]["rich_text"][0]["text"]["content"].ToString();
                    #endregion

                    log.Debug("檢查是否需更新");
                    //檢查目前版本是否須更新
                    if (canUpdate && CheckLocalVersion() != serverVersion)
                    {
                        //檢查更新檔URL和檔名是否成功取得
                        if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(fileName))
                        {

                            log.Debug("下載更新檔");
                            #region 下載更新檔
                            using (var webClient = new WebClient())
                            {
                                webClient.DownloadFile(fileUrl, Path.Combine(Application.StartupPath, fileName));
                            }
                            #endregion

                            log.Debug("解壓縮更新檔");
                            #region 解壓縮更新檔
                            zipFilePath = Path.Combine(Application.StartupPath, fileName);
                            extractPath = Application.StartupPath;
                            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    string destinationPath = Path.Combine(extractPath, entry.FullName);

                                    // 如果檔案已存在，則先刪除
                                    if (File.Exists(destinationPath))
                                    {
                                        File.Delete(destinationPath);
                                    }

                                    entry.ExtractToFile(destinationPath);
                                }
                            }
                            #endregion

                            log.Debug("新增Library引用檔");
                            #region 新增Library引用檔
                            if (File.Exists(Path.Combine(Application.StartupPath, "Library.zip")))
                            {
                                //解壓縮Library資料夾
                                ZipFile.ExtractToDirectory(Path.Combine(Application.StartupPath, "Library.zip"), Application.StartupPath);
                            }
                            #endregion

                            log.Debug("刪除更新檔壓縮檔");
                            #region 刪除更新檔壓縮檔
                            File.Delete(zipFilePath);
                            #endregion

                            #region 更新版本號
                            /*
                            List<Dictionary<string, string>> ver = new List<Dictionary<string, string>>();
                            ver.Add(new Dictionary<string, string>() { { "Version", serverVersion } });
                            string updatedJson = JsonConvert.SerializeObject(ver, Formatting.Indented);
                            if (!File.Exists(Path.Combine(Application.StartupPath, "Version.json")))
                            {
                                using (StreamWriter writer = File.CreateText(Path.Combine(Application.StartupPath, "Version.json")))
                                {

                                }
                            }
                            File.WriteAllText(Path.Combine(Application.StartupPath, "Version.json"), updatedJson);
                            */
                            #endregion

                            log.Debug("更新版本日誌");
                            #region 更新版本日誌
                            WriteUpdateNote(data);
                            #endregion
                        }
                    }
                }
                else
                    message = "網路連線失敗，即將重新啟動程式";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                log.Debug("啟動主程式");
                #region 開啟MapleStoryTools執行檔
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(Application.StartupPath, "MapleStoryTools.exe");
                startInfo.Verb = "runas"; // 以系統管理員身分執行
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                #endregion

                log.Debug("關閉更新程式");
                #region 關閉更新程式
                Environment.Exit(0);
                #endregion
            }
        }
        #endregion

        #region 更新版本日誌
        private void WriteUpdateNote(JArray data)
        {
            try
            {
                int rowNum = data.Count < 10 ? data.Count : 10;
                string path = Path.Combine(Application.StartupPath, "UpdateNote.json");

                if (!File.Exists(path))
                    using (StreamWriter writer = File.CreateText(path)) { }

                // 讀取原始 JSON 檔案的內容
                string json = File.ReadAllText(path);

                List<UpdateNote> notes = new List<UpdateNote>();

                for (int i = 0; i < rowNum; i++)
                {
                    notes.Add(new UpdateNote
                    {
                        Version = data[i]["properties"]["Version"]["title"][0]["text"]["content"].ToString(),
                        Note = data[i]["properties"]["Note"]["rich_text"][0]["text"]["content"].ToString()
                    });
                }

                // 將資料結構轉換回 JSON 格式的字串
                string updatedJson = JsonConvert.SerializeObject(notes, Formatting.Indented);

                // 寫入更新後的 JSON 字串到原始檔案
                File.WriteAllText(path, updatedJson);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 檢查程式目前版本
        public string CheckLocalVersion()
        {
            #region 註解
            /*
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            string path = Path.Combine(Application.StartupPath, "Version.json");

            try
            {
                if (!File.Exists(path))
                {
                    List<Dictionary<string, string>> ver = new List<Dictionary<string, string>>();
                    ver.Add(new Dictionary<string, string>() { { "Version", "0.0.0" } });
                    string updatedJson = JsonConvert.SerializeObject(ver, Formatting.Indented);
                    File.WriteAllText(path, updatedJson);
                }

                string fileData = File.ReadAllText(Path.Combine(Application.StartupPath, "Version.json"));

                // 將 JSON 字串轉換為物件結構
                data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(fileData);

                // 獲取 "Version" 的值
                string version = data[0]["Version"];

                JsonConvert.PopulateObject(fileData, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("版本有誤請確認!\n" + ex.Message);
            }
            return data[0]["Version"];
            */
            #endregion

            string version = "";

            // 現在可以使用 config 物件來讀取其他專案的 app.config 設定
            try
            {
                string configFilePath = Path.Combine(Application.StartupPath, "MapleStoryTools.exe.config"); // 指定其他專案的 app.config 檔案路徑

                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = configFilePath;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                version = config.AppSettings.Settings["Version"].Value;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return version;

        }
        #endregion

        #region 檢查網路連線
        private bool CheckConnect()
        {
            string ipAddress = "8.8.8.8"; // 使用 Google 的 DNS 伺服器作為範例
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ipAddress, 1000);

                if (reply.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch
            {
                return  false;
            }
        }
        #endregion

        #region 檢查程式是否為模擬模式 
        public bool CheckSimulationn()
        {
            bool isSimulation = false;

            // 現在可以使用 config 物件來讀取其他專案的 app.config 設定
            try
            {
                string configFilePath = Path.Combine(Application.StartupPath, "MapleStoryTools.exe.config"); // 指定其他專案的 app.config 檔案路徑

                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = configFilePath;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                isSimulation = Convert.ToBoolean(config.AppSettings.Settings["isSimulation"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSimulation;

        }
        #endregion
    }

    public class UpdateNote
    {
        public string Version { get; set; }
        public string Note { get; set; }
    }
}
