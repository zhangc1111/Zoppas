namespace Zoppas.Model
{
    using System.IO;
    using System;
    using System.Collections.Generic;

    public class Saver
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Saver()
        {

        }

        public void Save(List<string> results, string frame, bool isCalibrated)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string date = DateTime.Now.ToString("yyyyMMdd");
            CreateDirectory("Calibration");
            CreateDirectory(date);
            FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + frame + ".txt", FileMode.OpenOrCreate);
            Directory.SetCurrentDirectory(currentDirectory);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Min", "Max", "Result", "Name");
            for (int i = 0; i < results.Count; i++)
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}", Global.ListItemParameterCalibration[i].UserMin, Global.ListItemParameterCalibration[i].UserMax, results[i], Global.ListItemNameCalibration[i]);
            }
            string result = isCalibrated ? "Pass" : "Fail";
            sw.WriteLine(result);
            sw.Close();
            fs.Close();
        }

        private void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\" + directory);
        }
    }
}
