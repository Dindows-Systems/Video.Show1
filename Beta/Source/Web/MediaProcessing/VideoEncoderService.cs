using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using VideoShow.Streaming.Services.Config;

namespace VideoShow.Streaming.Services
{
    public class VideoEncoderService
    {
        private string _MediaEncoderPath;

        private string GetJobFileTemplate()
        {
            return Utils.GetConfigFile("JobFile.xml");
        }

        #region IVideoEncoderService Members

        public string MediaEncoderPath
        {
            get
            {
                return _MediaEncoderPath;
            }
            set
            {
                _MediaEncoderPath = value;
            }
        }
        public string GetTmpDir(Guid vid)
        {
            string tmpDir = Path.Combine(Path.GetTempPath(), "VideoShowEncoder");
            tmpDir = Path.Combine(tmpDir, vid.ToString() + ".Encoded");
            return tmpDir;
        }

        public string Encode(string sourceFile, Guid videoId, string workingDirectory)
        {
            string exeEncoder = "Encoder.exe";
            string tmpjobFile = Path.GetTempFileName();
            string tmpDir = GetTmpDir(videoId);

            // -- Create local dir to encode             
            if (!Directory.Exists(tmpDir))
                Directory.CreateDirectory(tmpDir);

            // -- Loading My Template job file
            XmlSerializer xs = new XmlSerializer(typeof(JobFile));
            JobFile jobfile = null;
            using (Stream configStream = File.OpenRead(System.IO.Path.Combine(workingDirectory,"JobFile.xml")))
            {
                jobfile = (JobFile)xs.Deserialize(configStream);
                jobfile.Job[0].OutputDirectory = tmpDir;
                jobfile.Job[0].MediaFiles[0].Source = sourceFile;             
            }
            using (Stream configStream = File.OpenWrite(tmpjobFile))
            {
                xs.Serialize (configStream, jobfile);
            }

            // -- Create process start info
            ProcessStartInfo psi = new ProcessStartInfo(exeEncoder); 
            psi.Arguments = "/JobFile \"" + tmpjobFile + "\"" + " /Log On";
            psi.WorkingDirectory = MediaEncoderPath;

            //Hide MediaEncoder process window
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = Process.Start(psi);

            Console.WriteLine("Encoding >" + psi.FileName + " " + psi.Arguments);            

            p.WaitForExit();

            return tmpDir;
        }

        #endregion
    }
}
