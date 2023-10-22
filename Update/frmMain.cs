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
        Point mouseDownLocation;
        string UpdateFileUrl = "";
        List<Release> listRelease = new List<Release>();
        string[] UpdateFileName = new string[] { "MapleStoryTools.exe", "MapleStoryTools.exe.Config", "Update.zip" };
        API api = new API();
        public readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            //log4net 載入設定檔
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Path.Combine(Application.StartupPath, "Library", "log4net.config.xml")));
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

            #region Read API TOKEN
            // 從資源中讀取資源數據到 MemoryStream
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Update.API.json"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    api = JsonConvert.DeserializeObject<API>(json);
                }
            }
            #endregion

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

        /// <summary>
        /// 檢查最新版本
        /// </summary>
        /// <returns></returns>
        private async Task<string> CheckVersion()
        {
            string Version = "";
            try
            {
                string owner = "howardwch1951";
                string repo = "MapleStoryTools";

                string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"token {api.GitHub}");
                client.DefaultRequestHeaders.Add("User-Agent", "MapleStoryTools");

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    listRelease = JsonConvert.DeserializeObject<List<Release>>(body);
                    UpdateFileUrl = JsonConvert.DeserializeObject<Assets>(listRelease[0].Assets[0].ToString()).browser_download_url;
                    Version = listRelease[0].Name;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return Version;
        }

        /// <summary>
        /// 下載更新檔
        /// </summary>
        /// <returns></returns>
        private async Task DownloadUpdateFile(string pVersion, string pFileName)
        {
            try
            {
                string owner = "howardwch1951";
                string repo = "MapleStoryTools";

                string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/tags/{pVersion}";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"token {api.GitHub}");
                client.DefaultRequestHeaders.Add("User-Agent", "MapleStoryTools");

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    dynamic releaseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    foreach (var asset in releaseInfo.assets)
                    {
                        if (asset.name == pFileName)
                        {
                            string downloadUrl = asset.browser_download_url;

                            using (var fileStream = System.IO.File.Create(pFileName))
                            {
                                HttpResponseMessage downloadResponse = await client.GetAsync(downloadUrl);
                                if (downloadResponse.IsSuccessStatusCode)
                                {
                                    Stream contentStream = await downloadResponse.Content.ReadAsStreamAsync();
                                    await contentStream.CopyToAsync(fileStream);
                                    Console.WriteLine($"文件已下载到 {pFileName}");
                                }
                                else
                                {
                                    Console.WriteLine("下载失败，HTTP响应状态码: " + downloadResponse.StatusCode);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("获取 Release 信息失败，HTTP响应状态码: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 更新作業
        /// </summary>
        /// <returns></returns>
        private async Task DoUpdateWork()
        {
            string serverVersion = "";
            string fileName = "";
            string zipFilePath = "";
            string extractPath = "";
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
                    log.Debug("取得最新版本號、讀取更新檔");
                    serverVersion = await CheckVersion();

                    log.Debug("檢查是否需更新");
                    //檢查目前版本是否須更新
                    if (CheckLocalVersion() != serverVersion)
                    {
                        //檢查更新檔URL和檔名是否成功取得
                        fileName = Path.GetFileName(UpdateFileUrl);
                        if (!string.IsNullOrEmpty(UpdateFileUrl) && !string.IsNullOrEmpty(fileName))
                        {
                            #region 下載更新檔
                            log.Debug("下載更新檔");
                            await DownloadUpdateFile(serverVersion, fileName);
                            //await DownloadUpdateFile(UpdateFileUrl, Path.Combine(Application.StartupPath, fileName));
                            using (WebClient wc = new WebClient())
                            {
                                wc.DownloadFile(new System.Uri(UpdateFileUrl), Path.Combine(Application.StartupPath, fileName));
                            }
                            using (var webClient = new WebClient())
                            {
                                webClient.DownloadFile(UpdateFileUrl, Path.Combine(Application.StartupPath, fileName));
                            }
                            #endregion

                            #region 解壓縮更新檔
                            log.Debug("解壓縮更新檔");
                            zipFilePath = Path.Combine(Application.StartupPath, fileName);
                            extractPath = Application.StartupPath;
                            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    string destinationPath = Path.Combine(extractPath, entry.FullName);

                                    if (UpdateFileName.Contains(entry.FullName))
                                    {
                                        // 如果檔案已存在，則先刪除
                                        //if (File.Exists(destinationPath))
                                        //{
                                        //    File.Delete(destinationPath);
                                        //}
                                        entry.ExtractToFile(destinationPath, true);
                                    }
                                }
                            }
                            #endregion

                            #region 新增Library引用檔
                            log.Debug("新增Library引用檔");
                            if (File.Exists(Path.Combine(Application.StartupPath, "Library.zip")))
                            {
                                //解壓縮Library資料夾
                                ZipFile.ExtractToDirectory(Path.Combine(Application.StartupPath, "Library.zip"), Application.StartupPath);
                            }
                            #endregion

                            #region 刪除更新檔壓縮檔
                            log.Debug("刪除更新檔壓縮檔");
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
                #region 開啟MapleStoryTools執行檔
                log.Debug("啟動主程式");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(Application.StartupPath, "MapleStoryTools.exe");
                startInfo.Verb = "runas"; // 以系統管理員身分執行
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                #endregion

                #region 關閉更新程式
                log.Debug("關閉更新程式");
                Environment.Exit(0);
                #endregion
            }
        }

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

    public class API
    {
        public string Notion { get; set; }
        public string GitHub { get; set; }
    }

    public class Release
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public List<Object> Assets { get; set; }
    }

    public class Assets
    {
        public string browser_download_url { get; set; }
    }
}
