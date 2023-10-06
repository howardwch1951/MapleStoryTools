using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleStoryTools
{
    public class JsonFunction
    {
        public Object Read<Object>(string pPath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(pPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(pPath));

                if (!File.Exists(pPath))
                    using (FileStream fileStream = File.Create(pPath)) { }

                string fileData = File.ReadAllText(pPath);
                Object output = JsonConvert.DeserializeObject<Object>(fileData);

                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Write(string pPath, Object pObj)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(pPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(pPath));

                if (!File.Exists(pPath))
                    using (FileStream fileStream = File.Create(pPath)) { }

                string json = JsonConvert.SerializeObject(pObj, Formatting.Indented);
                File.WriteAllText(pPath, json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
