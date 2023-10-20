using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyBoradSimulator
{
    internal class JSON
    {
        public Object ReadJson(string fileName)
        {
            Object data = new Object();
            string filePath = Path.Combine(Application.StartupPath,fileName);

            if (!File.Exists(filePath)) 
                File.WriteAllText(filePath, JsonConvert.SerializeObject(data));

            var fileData = File.ReadAllText(fileName);

            try
            {
                JsonConvert.PopulateObject(fileData, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("設定檔內容有誤，請確認!\n" + e.Message, "設定檔內容有誤");
            }
            return data;
        }

        public void WriteJson(string fileName, Object data)
        {
            string filePath = Path.Combine(Application.StartupPath, fileName);
            string output = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, output);
        }
    }
}
