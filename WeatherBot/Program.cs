﻿using MihaZupan;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace WeatherBot
{
    class Program
    {
        private static ITelegramBotClient client;

        public static string NameCity;
        public static float tempOfCity;
        public static string nameOfCity;


        public static void Main(string[] args)
        {
            string webHook = "https://a1111-7831a.s2.deploy-f.com";

            //Инициализация бота
            var proxy = new HttpToSocks5Proxy("198.46.205.105", 1080);
            client = new TelegramBotClient("1302247747:AAF4kJCZ7NAZmE0H649mtSsve6VpLUfix5Y") { Timeout = TimeSpan.FromSeconds(10)};

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"Bot_Id: {me.Id} \nBot_Name: {me.FirstName} ");

            //client.SetWebhookAsync(webHook).Wait();
            client.OnMessage += Bot_OnMessage;
            client.OnMessageEdited += Bot_OnMessage;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
            
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type == MessageType.Text)
            {
                if (message.Text == "/start")
                    return;

                NameCity = message.Text;
                Weather(NameCity);

                Console.WriteLine(message.Text);
                await client.SendTextMessageAsync(message.Chat.Id, $"Температура в {nameOfCity}: {Math.Round(tempOfCity)} °C");
            }

        }

        public static string Weather(string cityName)
        {
            string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&unit=metric&appid=2351aaee5394613fc0d14424239de2bd";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
            string response;

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }
            WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

            nameOfCity = weatherResponse.Name;
            tempOfCity = weatherResponse.Main.Temp - 273;

            return cityName;
        }
    }
}
