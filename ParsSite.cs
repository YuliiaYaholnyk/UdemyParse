using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParsSite
{
    class VideoInfo
    {
        public string Name { get; set; }
        public DateTime time { get; set; }

        public VideoInfo()
        {

        }
        public VideoInfo(string Name, DateTime time)
        {
            this.Name = Name;
            this.time = time;
        }
        public override string ToString()
        {
            return Name + "  " + time.Hour + ":" + time.Minute;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //get HTML code
            string urlAddress = "https://www.udemy.com/course/learn_flutter/";
            string HTML = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.UserAgent = "Yuliia";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                HTML = readStream.ReadToEnd();


                response.Close();
                readStream.Close();
            }

            HtmlDocument hap = new HtmlDocument();
            hap.LoadHtml(HTML);

            //Search classes "lectures"
            HtmlNodeCollection divContainer = hap.DocumentNode.SelectNodes("//div[contains(@class, 'lectures-container')]");
            hap = new HtmlDocument();
            HTML = string.Empty;
            foreach (var item in divContainer)
            {
                HTML += item.InnerHtml;
            }
            hap.LoadHtml(HTML);

            //Get Free lectures
            HtmlNodeCollection htmlNodes = null;
            foreach (var item in divContainer)
            {
                htmlNodes = item.SelectNodes("//div[contains(@class, 'lecture-container lecture-container--preview')]");
                
            }
            VideoInfo[] videos = new VideoInfo[htmlNodes.Count];
           
            for (int i = 0; i < videos.Length; i++)
            {
                videos[i] = new VideoInfo();
                videos[i].Name = htmlNodes[i].InnerText;
                videos[i].Name = videos[i].Name.Replace("Предварительный просмотр", " ");
                videos[i].Name = Regex.Replace(videos[i].Name, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
                videos[i].Name = videos[i].Name.Replace(videos[i].Name.Substring(0, videos[i].Name.IndexOf('\n')), " ");
               
                videos[i].time = DateTime.Parse(Regex.Match(videos[i].Name, @"[0-9][0-9]:[0-9][0-9]").ToString());
                videos[i].Name = Regex.Replace(videos[i].Name, @"[0-9][0-9]:[0-9][0-9]", "", RegexOptions.Multiline);
                videos[i].Name = Regex.Replace(videos[i].Name, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
               
            }

            //final Console out
            for (int i = 0; i < videos.Length; i++)
            {
                Console.WriteLine("Видео: " + videos[i].ToString());
            }

        }
    }
}
