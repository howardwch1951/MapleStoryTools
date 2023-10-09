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
        bool isNewUpdate = Convert.ToBoolean(ConfigurationManager.AppSettings["isNewUpdate"]);
        bool isSimulation = Convert.ToBoolean(ConfigurationManager.AppSettings["isSimulation"]);
        JArray serverData;
        DateTime checkTime = new DateTime();
        API api = new API();
        public List<Location> listLocation = new List<Location>();
        public readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            try
            {
                this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));

                //log4net 載入設定檔
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Path.Combine(Application.StartupPath, "log4net.config.xml")));

                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
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

                        ZipFile.ExtractToDirectory(Path.Combine(Application.StartupPath, "Update.zip"), Application.StartupPath);

                        File.Delete(Path.Combine(Application.StartupPath, "Update.zip"));
                    }
                }
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

                toolVersion.Text = $"目前版本：{localVersion}";

                SetFormLocation(this);

                this.Enabled = false;
                ShowUpdateMessageBox(false, "檢查更新中，請稍後");
                //Task.Run(DoUpdateWork);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void timerCheckUpdate_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            if (!isMouseScriptRun && !isKeyboardScriptRun && now >= checkTime)
            {
                checkTime = DateTime.Now.AddMinutes(30);
                DoUpdateWork();
            }
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            frmMouse fMouse = new frmMouse();
            if (Application.OpenForms["frmUpdateNote"] != null)
                Application.OpenForms["frmUpdateNote"].Close();
            this.Hide();
            fMouse.frm = this;
            fMouse.log = this.log;
            SetFormLocation(fMouse);
            fMouse.ShowDialog();
            this.Show();
        }

        private void btnKeyBoard_Click(object sender, EventArgs e)
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
            //this.Show();
        }

        private void menuCheckUpdate_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            showUpdateMsg = true;
            DoUpdateWork();
        }

        private void menuUpdateNote_Click(object sender, EventArgs e)
        {
            ShowUpdateNote(this);
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmUpdateNote"] != null)
            {
                Application.OpenForms["frmUpdateNote"].StartPosition = FormStartPosition.Manual;
                Application.OpenForms["frmUpdateNote"].Location = new Point(
                    this.Location.X + this.Width,
                    this.Location.Y);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            frmLockWindows frm = new frmLockWindows();
            this.Hide();
            frm.Show();
        }

        #region 檢查更新
        private async void DoUpdateWork()
        {
            JObject obj;
            JArray data;
            bool canUpdate = false;
            string body = "";
            string serverVersion = "";
            string fileName = "";
            string fileUrl = "";
            string note = "";
            try
            {
                if (CheckConnect())
                {
                    #region 設定Notion API
                    Uri apiUrl;
                    if (isSimulation)
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

                    #region Notion Table 取得最新版本號、更新檔讀取
                    using (var response = await httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        body = await response.Content.ReadAsStringAsync();
                    }

                    obj = JObject.Parse(body);
                    data = obj.GetValue("results") as JArray;
                    var jsList = data
                    .Where(item =>
                    {
                        var checkboxValue = item["properties"]?["Push"]?["checkbox"];
                        return checkboxValue != null && (bool)checkboxValue == true;
                    })
                    .ToList();

                    serverData = data = new JArray(jsList);

                    canUpdate = Convert.ToBoolean(data[0]["properties"]["Push"]["checkbox"]);
                    serverVersion = data[0]["properties"]["Version"]["title"][0]["text"]["content"].ToString();
                    fileName = data[0]["properties"]["Files"]["files"][0]["name"].ToString();
                    fileUrl = data[0]["properties"]["Files"]["files"][0]["file"]["url"].ToString();
                    note = data[0]["properties"]["Note"]["rich_text"][0]["text"]["content"].ToString();
                    #endregion

                    //檢查目前版本是否須更新
                    if (canUpdate && (localVersion != serverVersion || localVersion == null))
                    {
                        isNewUpdate = true;
                        ModifyConfig("isNewUpdate", "true");
                        //檢查更新檔URL和檔名是否成功取得
                        if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(fileName))
                        {
                            DialogResult dia = ShowMessageBox("檢查更新", "目前有新版本可更新，是否要現在更新?", MessageBoxIcon.Information, MessageBoxButtons.YesNo);

                            if (dia == DialogResult.Yes)
                            {
                                Process.Start(Path.Combine(Application.StartupPath, "Update.exe"));

                                Environment.Exit(0);
                            }
                            else
                            {
                                ShowMessageBox("提示", "更新作業已延後30分鐘", MessageBoxIcon.Information, MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            ShowMessageBox("檢查更新", "伺服器更新檔取得失敗，無法正常更新!", MessageBoxIcon.Information, MessageBoxButtons.OK);
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
                    ShowMessageBox("檢查更新", "網路連線失敗，無法檢查更新!", MessageBoxIcon.Information, MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox("檢查更新", "檢查更新失敗!", MessageBoxIcon.Information, MessageBoxButtons.OK);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                this.Enabled = true;
                ShowUpdateMessageBox(true, "檢查更新完成!");
                //ShowForm();
            }
        }
        #endregion

        #region 檢查程式目前版本
        public string CheckVersion()
        {
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

                string fileData = File.ReadAllText(path);

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
                return false;
            }
        }
        #endregion

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
            if (Application.OpenForms["frmUpdateMessageBox"] != null)
                Application.OpenForms["frmUpdateMessageBox"].Close();

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

                if (isClose)
                {
                    frmUpdateMsg.Close();
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
            string note = "";
            string path = Path.Combine(Application.StartupPath, "UpdateNote.json");
            List<UpdateNote> updateNote = new List<UpdateNote>();

            if (serverData.Count > 0)
            {
                int rowNum = serverData.Count < 10 ? serverData.Count : 10;

                if (!File.Exists(path))
                    using (StreamWriter writer = File.CreateText(path)) { }

                string fileData = File.ReadAllText(path);

                for (int i = 0; i < rowNum; i++)
                {
                    updateNote.Add(new UpdateNote
                    {
                        Version = serverData[i]["properties"]["Version"]["title"][0]["text"]["content"].ToString(),
                        Note = serverData[i]["properties"]["Note"]["rich_text"][0]["text"]["content"].ToString()
                    });
                }

                // 將資料結構轉換回 JSON 格式的字串
                string updatedJson = JsonConvert.SerializeObject(updateNote, Formatting.Indented);

                // 寫入更新後的 JSON 字串到原始檔案
                File.WriteAllText(path, updatedJson);
            }

            if (updateNote.Count > 0)
            {
                int readNum = updateNote.Count <= 10 ? updateNote.Count : 10;

                for (int i = 0; i < readNum; i++)
                {
                    note += updateNote[i].Version + Environment.NewLine;
                    note += updateNote[i].Note + Environment.NewLine;
                    note += Environment.NewLine;
                }
            }

            if (Application.OpenForms["frmUpdateNote"] == null)
            {
                frmUpdateNote frm = new frmUpdateNote();
                frm.note = note;

                frm.Refresh();

                frm.StartPosition = FormStartPosition.Manual;
                frm.Location = new Point(
                    pForm.Location.X + pForm.Width,
                    pForm.Location.Y);

                frm.Show(this);
            }
        }

        /// <summary>
        /// 修改App.config
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        private void ModifyConfig(string pKey, string pValue)

        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings[pKey].Value = pValue;

            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");

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
            }
        }

        public void ShowForm()
        {
            Process processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)[0];
            ShowWindowAsync(processes.MainWindowHandle, WS_SHOWNORMAL);
        }

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

        private async void button3_Click(object sender, EventArgs e)
        {
            string token = "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";
            string owner = "howardwch1951";
            string repo = "MapleStoryTools";
            string releaseName = "v1.0.0";
            string releaseTag = "v1.0.0";

            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"token {token}");
            client.DefaultRequestHeaders.Add("User-Agent", "MapleStoryTools");

            var releaseData = new
            {
                tag_name = releaseTag,
                name = releaseName,
                body = "Release description"
            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(releaseData), Encoding.UTF8, "application/vnd.github");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("GitHub Release created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create GitHub Release.");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }

    public class UpdateNote
    {
        public string Version { get; set; }
        public string Note { get; set; }
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
}