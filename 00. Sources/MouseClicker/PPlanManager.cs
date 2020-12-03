using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MouseClicker
{
    public class PPlanManager
    {
        public static PPlanManager Instance { get; set; } = new PPlanManager();

        private const string SAVE_FILE_PATH = "./dat.psav";

        public List<PPlan> Plans { get; set; } = new List<PPlan>();

        public bool SavePlans()
        {
            try
            {
                using (FileStream fs = new FileStream(SAVE_FILE_PATH, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Plans);
                    fs.Close();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool LoadPlans()
        {
            FileInfo info = new FileInfo(SAVE_FILE_PATH);
            if (!info.Exists)
                return false;

            try
            {
                List<PPlan> desObj = null;
                using (FileStream fs = new FileStream(SAVE_FILE_PATH, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    desObj = bf.Deserialize(fs) as List<PPlan>;
                    fs.Close();
                };

                if (desObj != null)
                    Plans = desObj;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}
