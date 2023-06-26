using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleStoryTools
{
    class Files
    {
        public void Write(string fileName, List<string> message)
        {
            string filePath = Path.Combine(Application.StartupPath, fileName);

            try
            {
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (string msg in message)
                    {
                        writer.WriteLine(msg);
                    }
                }

                Console.WriteLine("資料已成功寫入檔案。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
        }

        public List<string> Read(string fileName)
        {
            List<string> fileContent = new List<string>();
            string filePath = Path.Combine(Application.StartupPath, fileName);

            try
            {
                // 檢查檔案是否存在
                if (File.Exists(filePath))
                {
                    // 讀取檔案內容
                    fileContent = File.ReadAllLines(filePath).ToList();
                }
                else
                {
                    Console.WriteLine("檔案不存在");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
            return fileContent;
        }
    }
}
