using AutoHotkey.Interop;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notion.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Point = System.Drawing.Point;

namespace MapleStoryTools
{
    public partial class frmMain : Form
    {
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        //正常
        private const int WS_SHOWNORMAL = 1;

        public bool isMouseScriptRun = false;
        public bool isKeyboardScriptRun = false;
        bool showUpdateMsg = false;
        string pathConfig = Path.Combine(Application.StartupPath, "Config", "Location.json");
        string localVersion = ConfigurationManager.AppSettings["Version"];
        string UpdateFileUrl = "";
        bool isNewUpdate = Convert.ToBoolean(ConfigurationManager.AppSettings["isNewUpdate"]);
        bool isSimulation = Convert.ToBoolean(ConfigurationManager.AppSettings["isSimulation"]);
        List<Release> listRelease = new List<Release>();
        API api = new API();
        JsonFunction JS = new JsonFunction();
        public List<Location> listLocation = new List<Location>();
        public readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));

            //log4net 載入設定檔
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Path.Combine(Application.StartupPath, "Library", "log4net.config.xml")));

            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                //IsMdiContainer = true;

                #region 更新Update主程式
                if (File.Exists(Path.Combine(Application.StartupPath, "Update.zip")))
                {
                    //移除舊程式版本記錄檔
                    if (File.Exists(Path.Combine(Application.StartupPath, "Version.json")))
                        File.Delete(Path.Combine(Application.StartupPath, "Version.json"));

                    //更新Update主程式
                    if (File.Exists(Path.Combine(Application.StartupPath, "Update.exe")))
                    {
                        File.Delete(Path.Combine(Application.StartupPath, "Update.exe"));
                        File.Delete(Path.Combine(Application.StartupPath, "Update.exe.config"));

                        ZipFile.ExtractToDirectory(Path.Combine(Application.StartupPath, "Update.zip"), Application.StartupPath);

                        File.Delete(Path.Combine(Application.StartupPath, "Update.zip"));
                    }
                }
                #endregion

                #region Delete Old File
                if (false && localVersion == @"v1.1.9")
                {
                    if (File.Exists(Path.Combine(Application.StartupPath, "log4net.config")))
                        File.Delete(Path.Combine(Application.StartupPath, "log4net.config"));
                    if (File.Exists(Path.Combine(Application.StartupPath, "log4net.dll")))
                        File.Delete(Path.Combine(Application.StartupPath, "log4net.dll"));
                }
                if (File.Exists(Path.Combine(Application.StartupPath, "UpdateNote.json")))
                    File.Delete(Path.Combine(Application.StartupPath, "UpdateNote.json"));
                #endregion

                #region 更新Library

                if (File.Exists(Path.Combine(Application.StartupPath, "Library.zip")))
                {
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Library")))
                    {
                        ZipFile.ExtractToDirectory(Path.Combine(Application.StartupPath, "Library.zip"), Application.StartupPath);

                        string[] newLibraryFiles = Directory.GetFiles(Path.Combine(Application.StartupPath, "Library"));
                        string[] oriLibraryFiles = Directory.GetFiles(Application.StartupPath);

                        foreach (string newFile in newLibraryFiles)
                        {
                            string newFileName = Path.GetFileName(newFile);
                            string oriFileName = Array.Find(oriLibraryFiles, fileB => Path.GetFileName(fileB) == newFileName);

                            if (oriFileName != null)
                            {
                                File.Delete(oriFileName);
                                Console.WriteLine($"刪除檔案: {oriFileName}");
                            }
                        }

                        File.Delete(Path.Combine(Application.StartupPath, "Library.zip"));
                    }
                }

                #endregion

                #region Read API TOKEN
                // 從資源中讀取資源數據到 MemoryStream
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MapleStoryTools.API.json"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        api = JsonConvert.DeserializeObject<API>(json);
                    }
                }
                #endregion

                #region Check Login Info

                #endregion

                toolVersion.Text = $"目前版本：{localVersion}";

                SetFormLocation(this);

                DoUpdateWork();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        private void tmrCheckUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isMouseScriptRun && !isKeyboardScriptRun)
                {
                    DoUpdateWork();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        private void btnKeyBoard_Click(object sender, EventArgs e)
        {
            try
            {
                btnKeyBoard.Enabled = false;
                frmAutoScripts fAutoScripts = new frmAutoScripts();
                if (Application.OpenForms["frmUpdateNote"] != null)
                    Application.OpenForms["frmUpdateNote"].Close();
                //this.Hide();
                fAutoScripts.frm = this;    
                fAutoScripts.isFormOpen = true;
                fAutoScripts.log = this.log;
                fAutoScripts.TopLevel = false;
                SetFormLocation(fAutoScripts);
                //fAutoScripts.MdiParent = this;

                panelForm.Controls.Add(fAutoScripts);

                fAutoScripts.Dock = DockStyle.Fill;
                this.Size = new System.Drawing.Size(panelOptions.Width + fAutoScripts.Size.Width + 20, menuStrip1.Height + fAutoScripts.Size.Height + 20);
                fAutoScripts.Show();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        private void menuCheckUpdate_Click(object sender, EventArgs e)
        {
            showUpdateMsg = true;
            DoUpdateWork();
        }

        private void menuUpdateNote_Click(object sender, EventArgs e)
        {
            ShowUpdateNote(this);
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            try
            {
                if (Application.OpenForms["frmUpdateNote"] != null)
                {
                    Application.OpenForms["frmUpdateNote"].StartPosition = FormStartPosition.Manual;
                    Application.OpenForms["frmUpdateNote"].Location = new Point(
                        this.Location.X + this.Width,
                        this.Location.Y);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Console.WriteLine("frmMain_FormClosing");
                frmAutoScripts frm = new frmAutoScripts();
                SaveFormLocation(this);
                if (Application.OpenForms["frmAutoScripts"] != null)
                {
                    frm = (frmAutoScripts)Application.OpenForms["frmAutoScripts"];
                    Application.OpenForms["frmAutoScripts"].Close();                
                }

                if (frm.isFormOpen == false)
                    Environment.Exit(0);
                else
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 版本更新
        /// </summary>
        private async void DoUpdateWork()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(DoUpdateWork));
            }
            else
            {
                try
                {
                    this.Enabled = false;
                    ShowUpdateMessageBox(false, "檢查更新中，請稍後");

                    string serverVersion = await CheckVersion();

                    if (CheckConnect())
                    {
                        if (string.IsNullOrEmpty(serverVersion))
                        {
                            log.Debug("無法取得伺服器最新版本資訊");
                            ShowMessageBox("檢查更新", "伺服器更新檔取得失敗，無法正常更新!", MessageBoxIcon.Information, MessageBoxButtons.OK);
                        }
                        else if (localVersion != serverVersion || localVersion == null)
                        {
                            isNewUpdate = true;
                            ModifyConfig("isNewUpdate", "true");

                            log.Debug("發現更新");
                            DialogResult dia = ShowMessageBox("檢查更新", "目前有新版本可更新，是否要現在更新?", MessageBoxIcon.Information, MessageBoxButtons.YesNo);

                            if (dia == DialogResult.Yes)
                            {
                                log.Debug("執行更新作業");
                                Process.Start(Path.Combine(Application.StartupPath, "Update.exe"));
                                Environment.Exit(0);
                            }
                            else
                            {
                                log.Debug("使用者已延後更新");
                                ShowMessageBox("提示", "更新作業已延後30分鐘", MessageBoxIcon.Information, MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            if (isNewUpdate)
                            {
                                isNewUpdate = false;
                                ModifyConfig("isNewUpdate", "false");
                                ShowUpdateNote(this);
                            }

                            if (showUpdateMsg)
                            {
                                ShowMessageBox("檢查更新", "目前已經是最新版本!", MessageBoxIcon.Information, MessageBoxButtons.OK);
                                showUpdateMsg = false;
                            }
                        }
                    }
                    else
                    {
                        log.Debug("網路連線失敗無法更新");
                        ShowMessageBox("檢查更新", "網路連線失敗，無法檢查更新!", MessageBoxIcon.Information, MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    ShowMessageBox("錯誤", "檢查更新失敗!" + Environment.NewLine + ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ShowUpdateMessageBox(true, "檢查更新完成!");
                    this.Enabled = true;
                }
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
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }

            return Version;
        }

        /// <summary>
        /// 檢查網路連線
        /// </summary>
        /// <returns></returns>
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
                return false;
            }
        }

        private async void CheckAccount()
        {
            try
            {
                JObject obj;
                JArray data;
                bool canUpdate = false;
                string body = "";
                string serverVersion = "";
                string fileName = "";
                string fileUrl = "";
                string note = "";

                log.Debug("設定Notion API");
                #region 設定Notion API
                Uri apiUrl = new Uri("https://api.notion.com/v1/databases/dc71107bc514473dbc32189881e5bfc4/query");

                var httpClient = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = apiUrl,
                    Headers =
                    {
                        { "Authorization", $"Bearer {api.Notion}" },
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
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        #region MessageBox
        /// <summary>
        /// 顯示自定義MessageBox
        /// </summary>
        /// <param name="pform"></param>
        /// <param name="pTitle"></param>
        /// <param name="pMessage"></param>
        /// <param name="pIcon"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox(string pTitle, string pMessage, MessageBoxIcon pIcon, MessageBoxButtons pButton)
        {

            if (Application.OpenForms["frmMessageBox"] != null)
                Application.OpenForms["frmMessageBox"].Close();

            frmMessageBox msg = new frmMessageBox();
            if (this != null && Application.OpenForms["frmMessageBox"] == null)
            {

                msg.title = pTitle;
                msg.message = pMessage;
                msg.icon = pIcon;
                msg.buttons = pButton;
                msg.TopMost = true;

                msg.StartPosition = FormStartPosition.Manual;
                msg.Location = new Point(
                    this.Location.X + (this.Width - msg.Width) / 2,
                    this.Location.Y + (this.Height - msg.Height) / 2);

                msg.Refresh();

                msg.ShowDialog();
            }

            return msg.DialogResult;
        }

        /// <summary>
        /// 檢查更新MessageBox
        /// </summary>
        /// <param name="isClose"></param>
        /// <param name="pMessage"></param>
        public void ShowUpdateMessageBox(bool isClose, string pMessage)
        {
            if (isClose)
            {
                if (Application.OpenForms["frmUpdateMessageBox"] != null)
                {
                    Application.OpenForms["frmUpdateMessageBox"].Close();
                }
            }
            else
            {
                if (Application.OpenForms["frmUpdateMessageBox"] == null)
                {
                    frmUpdateMessageBox frmUpdateMsg = new frmUpdateMessageBox();
                    frmUpdateMsg.StartPosition = FormStartPosition.Manual;
                    frmUpdateMsg.Location = new Point(
                        this.Location.X + (this.Width - frmUpdateMsg.Width) / 2,
                        this.Location.Y + (this.Height - frmUpdateMsg.Height) / 2);
                    frmUpdateMsg.message = pMessage;
                    frmUpdateMsg.Refresh();
                    frmUpdateMsg.TopMost = true;
                    frmUpdateMsg.Show();
                }
            }
        }
        #endregion

        /// <summary>
        /// 防止畫面閃爍
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED 
                return cp;
            }
        }

        /// <summary>
        /// 顯示更新日誌
        /// </summary>
        public void ShowUpdateNote(Form pForm)
        {
            try
            {
                if (Application.OpenForms["frmUpdateNote"] == null)
                {
                    frmUpdateNote frm = new frmUpdateNote();
                    frm.note = string.Join(Environment.NewLine + "-" + Environment.NewLine, listRelease.Take(10).Select(t => t.Name + Environment.NewLine + t.Body));

                    frm.Refresh();

                    frm.StartPosition = FormStartPosition.Manual;
                    frm.Location = new Point(
                        pForm.Location.X + pForm.Width,
                        pForm.Location.Y);

                    frm.Show(this);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 修改App.config
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        private void ModifyConfig(string pKey, string pValue)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings[pKey].Value = pValue;

                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 設定Form的位置
        /// </summary>
        /// <param name="pForm"></param>
        private void SetFormLocation(Form pForm)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(Application.StartupPath, "Config")))
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Config"));

                if (!File.Exists(pathConfig))
                    using (FileStream fileStream = File.Create(pathConfig)) { }
                else
                {
                    string fileData = File.ReadAllText(pathConfig);

                    // 將 JSON 字串轉換為物件結構
                    listLocation = JsonConvert.DeserializeObject<List<Location>>(fileData);

                    Location location = listLocation.FirstOrDefault(t => t.Form == pForm.Name);

                    if (location != null)
                    {
                        pForm.StartPosition = FormStartPosition.Manual;
                        pForm.Location = new Point(location.X, location.Y);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 儲存Form的位置
        /// </summary>
        /// <param name="pForm"></param>
        public void SaveFormLocation(Form pForm)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(Application.StartupPath, "Config")))
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Config"));

                if (!File.Exists(pathConfig))
                    using (FileStream fileStream = File.Create(pathConfig)) { }

                string fileData = File.ReadAllText(pathConfig);
                List<Location> data = new List<Location>();

                if (fileData.Length > 0)
                {
                    // 將 JSON 字串轉換為物件結構
                    data = JsonConvert.DeserializeObject<List<Location>>(fileData);

                    data.RemoveAll(t => t.Form == pForm.Name);
                }

                data.Add(new Location
                {
                    Form = pForm.Name,
                    X = pForm.Location.X,
                    Y = pForm.Location.Y
                });

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                File.WriteAllText(pathConfig, json);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
        }

        #region AES
        /// <summary>
        /// AES加密(非對稱式)
        /// </summary>
        /// <param name="Source">加密前字串</param>
        /// <returns>加密後字串</returns>
        public string aesEncryptBase64(string SourceStr)
        {
            string encrypt = "";
            string Key = "This is Secret";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(Key));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(Key));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Encoding.UTF8.GetBytes(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            return encrypt;
        }

        /// <summary>
        /// AES解密(非對稱式)
        /// </summary>
        /// <param name="Source">解密前字串</param>
        /// <returns>解密後字串</returns>
        public string aesDecryptBase64(string SourceStr)
        {
            string decrypt = "";
            string CryptoKey = "This is Secret";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Convert.FromBase64String(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowMessageBox("錯誤", ex.Message, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            return decrypt;
        }
        #endregion

        private void button1_Click_1(object sender, EventArgs e)
        {
            frmColorSelecter frm = new frmColorSelecter();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmPoint f = new frmPoint();
            f.SetPoint("右上");
            f.Show();
        }

        private void menuLogin_Click(object sender, EventArgs e)
        {
            CheckAccount();
        }
    }

    public class Location
    {
        public string Form { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
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