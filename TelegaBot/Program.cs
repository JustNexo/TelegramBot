using MySql.Data.MySqlClient;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegaBot
{
    class Program
    {
        private static TelegramBotClient bot;
        private static string hello = @"YouHub - мощный инструмент для арбитража💎

Что я умею?👋
· Загрузка видео на любые YouTube каналы🎞

· Продвижение YouTube видео: накрутка лайков, просмотров, подписок📈

· При загрузке все видео уникализируется, используя лучшие методы, незамеченные YouTube🥇

· Загружая видео массово, ты можешь указать много вариаций оформления👨‍💻

· У нас самый лучший бесплатный чекер каналов👁

Воспользуйся кнопками ниже⬇️";
        private static InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
        // first row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "👀Профиль", callbackData: "profile"),
            InlineKeyboardButton.WithCallbackData(text: "💸Пополнить баланс💸", callbackData: "12"),
        },
        // second row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Реферальная система", callbackData: "21"),
            InlineKeyboardButton.WithCallbackData(text: "Покупка", callbackData: "22"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Загрузить видео", callbackData: "21"),
            InlineKeyboardButton.WithCallbackData(text: "Продвинуть видео", callbackData: "22"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Информация", callbackData: "21"),
            InlineKeyboardButton.WithCallbackData(text: "Чекер каналов", callbackData: "22"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Узнать пароль от любого канала", callbackData: "21"),
        }
        });

        private static void Main(string[] args)
        {
            var token = "1210995999:AAFhVUVrqLQHjdkk_kAKp_c_T7elOr_ebt8";
            bot = new TelegramBotClient(token);
            bot.OnMessage += Bot_OnMessageAsync;
            bot.StartReceiving();
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Console.ReadLine();
            bot.StopReceiving();
        }

        private async static void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            int id = e.CallbackQuery.Message.MessageId;
            if (e.CallbackQuery.Data == "profile")
            {
                try
                {
                    await bot.EditMessageTextAsync(chatId: e.CallbackQuery.From.Id, messageId: id, text: "123");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                
            }
        }

        static async void Bot_OnMessageAsync(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            int chatid = Convert.ToInt32(e.Message.Chat.Id);

            MySqlConnection con = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=telegabot");
            con.Open();

            string sql = $"select 1 from clients where id = {chatid} limit 1";

            MySqlCommand command = new MySqlCommand(sql, con);

            string name = Convert.ToString(await command.ExecuteScalarAsync());
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
                await bot.SendTextMessageAsync(chatid, hello);
            }

            con.Close();

        }
    }
}
