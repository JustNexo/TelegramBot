using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegaBot
{
    [Obsolete]
    class Program
    {


        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private static Regex regexExtractId = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
        private static string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

        private static TelegramBotClient bot;
        private static string hello = @"YouStats -  инструмент для проверки статистики💎

Что я умею?👋
· Проверка статистики видео по ссылке📈

· Перед использованием обязательно прочитай инструкцию!👨‍💻

Воспользуйся кнопками ниже⬇️"; //hello msg
        private static InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
        // first row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "👀Профиль", callbackData: "profile"),
            InlineKeyboardButton.WithCallbackData(text: "💸Пополнить баланс💸", callbackData: "balance"),
        },
        // second row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "💾Загрузить видео💾", callbackData: "uploadvideo"),
            InlineKeyboardButton.WithCallbackData(text: "⚠Инструкция⚠", callbackData: "instruction"),
        }
        }); //main menu inline keyboard
        private static InlineKeyboardMarkup homebutton = new InlineKeyboardMarkup(new[] {new []
        {
            InlineKeyboardButton.WithCallbackData(text: "⬅️ Вернуться в главное меню", callbackData: "menu"),
        } });
        private static string sqlcon = "server=localhost;port=3306;username=root;password=root;database=telegabot";
        private static MySqlConnection con = new MySqlConnection(sqlcon);
        private static string authority;

        private static List<string> list = new List<string>();
        private static List<string> myList = new List<string>();
        private static List<string> ids = new List<string>();
        private static List<Results> results = new List<Results>();

        public class Results
        {
            public string Url { get; set; }
            public int Views { get; set; }
            public int Likes { get; set; }
            public int Dislikes { get; set; }
        }

        private static void Main()
        {
            var token = "1210995999:AAFhVUVrqLQHjdkk_kAKp_c_T7elOr_ebt8";
            bot = new TelegramBotClient(token);
            bot.OnMessage += Bot_OnMessageAsync;
            bot.StartReceiving();
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Distrib();
            Console.ReadLine();
            bot.StopReceiving();
        }

        private static void Distrib() // рассылка
        {
            var token = "1185472193:AAEpsueEM1jNGHHOCoYBju9PM4MxBDZIGxE";
            TelegramBotClient bot = new TelegramBotClient(token);
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();

    }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            using (MySqlConnection con1 = new MySqlConnection(sqlcon))
            {
                con1.Open();
                MySqlCommand command1 = new MySqlCommand("SELECT `id` FROM `clients` WHERE 1", con1);
                using (MySqlDataReader reader = command1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                }
                if (e.Message.Photo != null)
                {
                    foreach (var item in list)
                    {
                        //bot.SendPhotoAsync();
                        try { await bot.GetFileAsync(e.Message.Photo[1].FileId); await bot.SendPhotoAsync(chatId: item, photo: e.Message.Photo[1].FileId); }
                        catch (Exception) { Console.WriteLine(item + " Has blocked the bot, or chat is not available "); }
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        //bot.SendPhotoAsync();
                        try { await bot.SendTextMessageAsync(item, e.Message.Text); }
                        catch (Exception) { Console.WriteLine(item + " Has blocked the bot, or chat is not available "); }
                    }        
                }
                list.Clear();
            }
        }

        private async static void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            int id = e.CallbackQuery.Message.MessageId;
            long chatid = e.CallbackQuery.From.Id;
            using (MySqlConnection con1 = new MySqlConnection(sqlcon))
            {
                con1.Open();
                switch (e.CallbackQuery.Data)
                {
                    case "profile":
                        MySqlCommand command = new MySqlCommand($"SELECT balance FROM `clients` WHERE id = {chatid}", con1);
                        await bot.EditMessageTextAsync(chatId: chatid, messageId: id, text:
    @$"👤{e.CallbackQuery.From.Username} | {chatid}
💰Баланс: {Convert.ToString(await command.ExecuteScalarAsync())}",
    replyMarkup: homebutton);

                        break;
                    case "menu":
                        await bot.EditMessageTextAsync(chatId: chatid, messageId: id, text: hello,  replyMarkup: inlineKeyboard);
                        break;
                    case "uploadvideo":
                        await bot.SendTextMessageAsync(chatid, "Следующим сообщением отправь мне .txt файл с ссылками на видео");
                        break;
                    case "distrib":
                        if (chatid == 673492271)
                        {
                            MySqlCommand command1 = new MySqlCommand("SELECT `id` FROM `clients` WHERE 1", con1);
                            using (MySqlDataReader reader = command1.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    list.Add(reader.GetString(0));
                                }
                            }
                            foreach (var item in list)
                            {
                                try { await bot.SendTextMessageAsync(item, "Тест рассылка"); }
                                catch (Exception) { Console.WriteLine(item + " Has blocked the bot, or chat is not available "); }
                            }
   
                        }
                        break;
                    case "instruction":
                        await bot.EditMessageTextAsync(chatId: chatid, messageId: id, text: @"
· Вставь в .txt файл ссылки на видео YouTube
❗️Каждая ссылка с новой строки ❗️
· Отправь этот файл сюда и жди ответа.", replyMarkup: homebutton);
                        break;
                        

                }
            }
        }

        static async void Bot_OnMessageAsync(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            int chatid = Convert.ToInt32(e.Message.Chat.Id);
            using (MySqlConnection con = new MySqlConnection(sqlcon))
            {
                con.Open();
                string sql = $"select 1 from clients where id = {chatid} limit 1";

                MySqlCommand command = new MySqlCommand(sql, con);

                string name = Convert.ToString(await command.ExecuteScalarAsync());
                if (e.Message.Text == "/start")
                {
                    if (name == "1")
                    {
                        await bot.SendStickerAsync(chatid, "CAACAgIAAxkBAAEDKdtheHoSKSn7VYFdHBHjWhDIa8hiKgAClwADO2AkFLPjVSHrbN7ZIQQ");
                        await bot.SendTextMessageAsync(chatid, hello, replyMarkup: inlineKeyboard);
                    }
                    else
                    {
                        MySqlCommand command1 = new MySqlCommand($"INSERT INTO `clients`(`id`, `balance`) VALUES ({chatid},0)", con);
                        await command1.ExecuteScalarAsync();
                        await bot.SendStickerAsync(chatid, "CAACAgIAAxkBAAEDKdtheHoSKSn7VYFdHBHjWhDIa8hiKgAClwADO2AkFLPjVSHrbN7ZIQQ",
                            replyMarkup: inlineKeyboard);
                        await bot.SendTextMessageAsync(chatid, hello, replyMarkup: inlineKeyboard);
                    }
                }
                else if (e.Message.Document != null)
                {
                    if (e.Message.Document.MimeType == "text/plain")
                    {
                        try
                        {
                            await bot.SendTextMessageAsync(chatid, "Загружаю файл... ⌛️");

                            using (var fileStream = System.IO.File.OpenWrite($"{e.Message.Document.FileId}.txt"))
                            {
                                Telegram.Bot.Types.File fileInfo = await bot.GetInfoAndDownloadFileAsync(
                                  fileId: e.Message.Document.FileId,
                                  destination: fileStream
                                );
                            }
                            myList = new List<string>(System.IO.File.ReadAllLines($"{e.Message.Document.FileId}.txt"));
                            if (myList.Count == 0)
                            {
                                await bot.SendTextMessageAsync(chatid, "Файл пустой");
                            }
                            else
                            {
                                foreach (string item in myList)
                                {
                                    try { authority = new UriBuilder(item).Uri.Authority.ToLower(); }
                                    catch { }

                                    if (validAuthorities.Contains(authority))
                                    {
                                        var regRes = regexExtractId.Match(item.ToString());
                                        if (regRes.Success)
                                        {
                                            ids.Add(regRes.Groups[1].Value);
                                        }
                                    }
                                }
                                foreach (string item in ids)
                                {

                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                                        $"https://www.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails%2Cstatistics&id={item}&key=AIzaSyAtgCuWBbnYA8-aWEJRI7yn40k9nV9BeD8");
                                    HttpWebResponse response = (HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse,
                                        request.EndGetResponse, null);
                                    Stream dataStream = response.GetResponseStream();
                                    StreamReader reader = new StreamReader(dataStream);
                                    string responseFromServer = reader.ReadToEnd();
                                    dynamic data = JObject.Parse(responseFromServer);
                                    if (data["pageInfo"]["totalResults"] != 0)
                                    {
                                        results.Add(new Results()
                                        {
                                            Url = "www.youtube.com/watch?v=" + item,
                                            Views = Convert.ToInt32(data["items"][0]["statistics"]["viewCount"]),
                                            Likes = Convert.ToInt32(data["items"][0]["statistics"]["likeCount"]),
                                            Dislikes = Convert.ToInt32(data["items"][0]["statistics"]["dislikeCount"])
                                        });
                                    }
                                }
                                StringBuilder stringBuilder = new StringBuilder();
                                foreach (var item1 in results)
                                {
                                    stringBuilder.Append(item1.Url + " - " + "Total 👀: " + item1.Views + " Total 👍: " + item1.Likes +
                                        " Total 👎: " + item1.Dislikes + "\n");
                                }
                                await bot.SendTextMessageAsync(chatid, stringBuilder.ToString(), replyMarkup: homebutton);
                                results.Clear();
                                myList.Clear();
                                ids.Clear();
                                System.IO.File.Delete($"{e.Message.Document.FileId}.txt");
                            }
                        }
                        catch (Exception ex) { Console.WriteLine(ex); }
                    }
                }
                else await bot.SendTextMessageAsync(chatid, "Неизвестная команда 😳");
            }
        }
    }
}