using HtmlAgilityPack;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Round.WPF.MusicPlayer.Module.AppControls;
using Round_Log_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer
{
    internal class Server
    {
        public class VideoMessage
        {
            public string Title { get; set; }
            public string ImageURL { get; set; }
            public string VideoURL { get; set; }
        }

        public static string GetSearchVideo(string keyword)
        {
            string url = "https://search.bilibili.com/video?keyword=" + keyword.Replace(" ", "+");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "RWMP/1.0");

                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    List<string> titlestrs = new List<string>();
                    List<string> imgurls = new List<string>();
                    List<string> playerurls = new List<string>();

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(responseBody);
                    var titles = htmlDoc.DocumentNode.SelectNodes("//img[@alt][@src]");
                    var urls = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

                    int titjs = 0;
                    if (urls != null)
                    {
                        for (int i = 0; i <= urls.Count - 1; i++)
                        {
                            var node = urls[i];
                            if (!node.Attributes["href"].Value.Contains("space.bilibili.com"))
                            {
                                if (!node.InnerText.Contains("课时"))
                                {
                                    if (playerurls.Count != 0)
                                    {
                                        for (int j = 0; j <= playerurls.Count - 1; j++)
                                        {
                                            if (node.Attributes["href"].Value == playerurls[j])
                                            {
                                                break;
                                            }
                                            else if (j == playerurls.Count - 1)
                                            {
                                                playerurls.Add(node.Attributes["href"].Value);
                                                titjs++;

                                                if (titles[titjs].Attributes["alt"].Value != "")
                                                {
                                                    titlestrs.Add(titles[titjs].Attributes["alt"].Value);
                                                    imgurls.Add(titles[titjs].Attributes["src"].Value.Substring(0, titles[titjs].Attributes["src"].Value.IndexOf("@")));
                                                    //imgurls.Add(titles[titjs].Attributes["src"].Value);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        playerurls.Add(node.Attributes["href"].Value);
                                        titjs++;
                                    }
                                }
                                else
                                {
                                    titjs++;
                                }
                            }
                        }
                    }

                    List<VideoMessage> Video = new List<VideoMessage>();
                    for (int i = 0; i <= titlestrs.Count; i++)
                    {
                        try
                        {
                            if (!playerurls[i].Contains("cheese"))
                            {
                                RLog.Message.WriteLine($"Title :{titlestrs[i]} ImageURL :https:{imgurls[i]} PlayerURL :https:{playerurls[i]}");

                                Video.Add(new VideoMessage
                                {
                                    Title = titlestrs[i],
                                    ImageURL = imgurls[i],
                                    VideoURL = playerurls[i]
                                });
                            }
                        }
                        catch { }
                    }

                    string json = JsonConvert.SerializeObject(Video);

                    return json;
                }
                catch
                {
                    RLog.Error.WriteLine("\nException Caught!");
                    GL.Main.MainWindow.Dispatcher.Invoke(() =>
                    {
                        GL.Main.MainWindow.ShowMessageAsync("连接错误!", "无法连接至服务器!");
                    });
                    return null;
                }
            }
        }

        public static void Search(string keyword)
        {
            GL.Main.MainShowWrapPanel.Children.Clear();
            GL.Main.MainShowWrapPanel.Children.Add(new LoadingBox());

            GL.Main.TabControl.SelectedIndex = 1;
            Task.Run(() => {
                try
                {
                    List<VideoMessage> items = JsonConvert.DeserializeObject<List<VideoMessage>>(Server.GetSearchVideo(keyword));

                    GL.Main.MainShowWrapPanel.Dispatcher.Invoke(() =>
                    {
                        GL.Main.MainShowWrapPanel.Children.Clear();
                    });
                    // 遍历并打印每个对象的属性
                    foreach (var item in items)
                    {
                        GL.Main.MainShowWrapPanel.Dispatcher.Invoke(new Action(() => {
                            VideoShowButton videoShowButton = new VideoShowButton(new Module.Cs.ControlConfigClass.VideoShowButtonConfig
                            {
                                Title = item.Title,
                                ImageURL = item.ImageURL,
                                VideoURL = item.VideoURL
                            });
                            GL.Main.MainShowWrapPanel.Children.Add(videoShowButton);
                        }));
                        Thread.Sleep(100);
                    }
                }
                catch { }
            });
        }
    }
}
