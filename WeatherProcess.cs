using System.Text;
using System;
using System.Security.AccessControl;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper;

namespace DallasWeatherService
{
    public class WeatherProcess
    {
        private readonly Timer _timer;
        private string xmlFileName = Path.GetFullPath("Meteorologic.xml");
        private string cvsFileName = Path.GetFullPath("Meteorologic.csv");
        
        public WeatherProcess()
        {
            _timer = new Timer(300000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;  
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            WeatherCommunication();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void WeatherCommunication()
        {
            string apiKey = "849d7421dd025cb328fff64f72d2524b";
            string apiBase = "https://api.openweathermap.org/data/2.5/weather?q=Dallas&mode=xml&units=metric&appid=";

            WebRequest request = WebRequest.Create(apiBase + apiKey);
            using (WebResponse response = request.GetResponse())
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Save(xmlFileName);
                    // Deserialización para obtener los datos del xml
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Current));
                    FileStream myFileStream = new FileStream(xmlFileName, FileMode.Open);
                    Current weatherObject = (Current)mySerializer.Deserialize(myFileStream);
                    AddRecord(weatherObject.Temperature.Value, weatherObject.Temperature.Unit, weatherObject.Precipitation.Mode, cvsFileName);
		            myFileStream.Close();
                }
            }
        }
       
        private void AddRecord(string value, string unit, string precipitation, string filepath)
        {
            if(!File.Exists(filepath))
            {
                File.Create(filepath).Dispose();
	            
                StringBuilder cvsContent = new StringBuilder();
                cvsContent.AppendLine(value + "," + unit + "," + precipitation);
                File.AppendAllText(filepath, cvsContent.ToString()); 

            }    
            else if (File.Exists(filepath))
            {
                StringBuilder cvsContent = new StringBuilder();
                cvsContent.AppendLine(value + "," + unit + "," + precipitation);
                File.AppendAllText(filepath, cvsContent.ToString()); 
            }
        }
    }
}