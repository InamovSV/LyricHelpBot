using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Genius;
using Genius.Models;
using QuickType;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace LyricHelp_bot_v2._0
{
    class Program
    {
        static ITelegramBotClient botClient;

        static void Main()
        {
            try
            {
                botClient = new TelegramBotClient("649403381:AAHzJGlQuzg4B8TUIM-vgDbpvlux2EmtJWU");
                var me = botClient.GetMeAsync().Result;
                Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
                botClient.OnMessage += Bot_OnMessage;
                botClient.StartReceiving();
                Thread.Sleep(int.MaxValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e) //reaction on Message
        {
            if (e.Message.Text != null)
            {
                string helpText = "/start - начать общение с ботом\n/help - посмотреть список команд\n/search - найти текст трека\n/cancel - отменить последнее действие";
                Console.WriteLine($"Received '{e.Message.Text}' in chat with '{e.Message.From.Username}'.");
                if (e.Message.Text == "/start") //answer on /start
                    await botClient.SendTextMessageAsync(
                       chatId: e.Message.Chat,
                       text: $"Привет, {e.Message.From.FirstName}, тут ты сможешь найти тексты песен! Введи /help что бы увидеть список команд."
                   );
                else if (e.Message.Text == "/help")
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: helpText);
                else if (e.Message.Text == "saysomething")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Trying *all the parameters* of `sendMessage` method",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithUrl(
                                "Check sendMessage method",
                                "https://core.telegram.org/bots/api#sendmessage"
                                )
                        )
                    );
                }
                else if (e.Message.Text == "/search") //answer on /search
                {
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Введи имя артиста, название песни или все вместе\n Например: `kendrick lamar humble` или `мамбл`",
                        parseMode: ParseMode.Markdown
                        );
                    botClient.OnMessage -= Bot_OnMessage; //change subscribing for continue search
                    botClient.OnMessage += Search;
                    Thread.Sleep(int.MaxValue);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Я не понимаю о чем ты говоришь, попробуй /help что бы увидеть список команд");
                }
            }
        }
        static async void Search(object sender, MessageEventArgs e) //realyzing Genius API
        {
            if (e.Message.Text == "/cancel")
            {
                botClient.OnMessage -= Search;
                botClient.OnMessage += Bot_OnMessage;

                await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: $"Последнее действие отменено");
            }
            else
            {
                try
                {
                    Console.WriteLine($"Received '{e.Message.Text}' in chat with {e.Message.From.Username}.");
                    var geniusClient = new GeniusClient("Gfx7YiVFLGVjdlyrbgQHvfcnEf1hT4Xad0UJ3s72zFN-RqjE-qTiGhuc2ya1BJjB");
                    var searchResult = await geniusClient.SearchClient.Search(textFormat: TextFormat.Dom, searchTerm: e.Message.Text);
                    var result = Result.FromJson(searchResult.Response[0].Result.ToString());
                    var songResult = await geniusClient.SongsClient.GetSong(TextFormat.Dom, result.Id.ToString());

                    HtmlWeb web = new HtmlWeb();
                    string lyric = web.Load(result.Url.ToString()).DocumentNode.SelectSingleNode("//div[@class='lyrics']").InnerText;
                    Song song = songResult.Response;
                    
                    
                    string info = $"*{song.Title}*\n*{result.PrimaryArtist.Name}*\n";
                    int count = 0;
                    if (song.FeaturedArtists.Count != 0)
                    {
                        info += "_Featuring by:_  ";
                        count = 0;
                        foreach (Artist artist in song.FeaturedArtists)
                        {
                            if (count == 0) info += "*" + artist.Name.Replace("*", "") + "*";
                            else info += " & " + "*" + artist.Name.Replace("*", "") + "*";
                            count++;
                        }
                        info += "\n";
                    }
                    if (song.ProducerArtists.Count != 0)
                    {
                        info += "_Produced by:_  ";
                        count = 0;
                        foreach (Artist artist in song.ProducerArtists)
                        {
                            if (count == 0) info += "*" + artist.Name.Replace("*", "") + "*";
                            else info += " & " + "*" + artist.Name.Replace("*", "") + "*";
                            count++;
                        }
                        info += "\n";
                    }
                    if (song.Album != null)
                    {
                        info += $"_Album_: *{song.Album.Name.Replace("*", "")}*\n";
                    }
                    info += $"_Release date_: *{song.ReleaseDate}*";
                    lyric = info + lyric;
                    lyric = lyric.Replace("[", "*[");
                    lyric = lyric.Replace("]", "]*");
                    lyric = lyric.Replace("&amp;", "&");
                    lyric = lyric.Replace("\n\n\n", "");
                    lyric += "Powered by: " + result.Url.ToString();

                    Console.WriteLine($"Give text on message '{e.Message.Text}': {result.FullTitle}");

                    await botClient.SendPhotoAsync(
                        chatId: e.Message.Chat,
                        photo: result.HeaderImageUrl.ToString());

                    if (lyric.Length > 4096)
                    {
                        int numOfMessages = (lyric.Length / 4096) + 1;
                        int pastIndex = 0;
                        int index = 0;
                        for (int i = 0; i < numOfMessages; i++)
                        {
                            if (i == 0)
                            {
                                index = lyric.LastIndexOf('\n', 4096);
                                await botClient.SendTextMessageAsync(
                                    chatId: e.Message.Chat,
                                    text: lyric.Substring(0, index),
                                    parseMode: ParseMode.Markdown);
                                pastIndex = index;
                            }
                            else if (i == numOfMessages - 1)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: e.Message.Chat,
                                    text: lyric.Substring(pastIndex, lyric.Length - pastIndex),
                                    parseMode: ParseMode.Markdown);
                            }
                            else
                            {
                                index += 4096;
                                await botClient.SendTextMessageAsync(
                                    chatId: e.Message.Chat,
                                    text: lyric.Substring(pastIndex, 4096),
                                    parseMode: ParseMode.Markdown);
                                pastIndex = index;
                            }
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat,
                          text: lyric,
                         parseMode: ParseMode.Markdown);
                    }

                    botClient.OnMessage -= Search;
                    botClient.OnMessage += Bot_OnMessage;
                }
                
                catch (Exception ex)
                {
                    Console.WriteLine(new string('-', 40));
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(new string('-', 40));
                    Console.WriteLine($"Error was in chat with {e.Message.From.Username}");
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Такой песни нету, попробуй еще раз");
                }
            }
        }
    }
}

