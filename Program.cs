using DealMeet.Core;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DealMeet.Bot;

class Program
{
    static void Main()
    {
        TelegramBotClient botClient = new("7614679805:AAHMVbGlbYSOkPj_Kh75nrnvkWO9IGBjU_g");
        Console.WriteLine("Starting bot");
        botClient.StartReceiving(Start, Error);
        Console.ReadLine();
    }

    private static async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;

        if (!string.IsNullOrEmpty(message?.Text))
        {
            Console.WriteLine($"{message.From.FirstName}: {message.Text}");

            if (message.Chat.Username == "")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Установите Username");
                
                return;
            }
            
            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "Авторизироваться" },
                    new KeyboardButton[] { "Зарегистрироватья" },
                    new KeyboardButton[] { "Отмена" },
                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие:",
                    replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
        }

        if (message.Text == "Зарегистрироватья")
        {
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
                HttpResponseMessage response;

                UserDto dto = new()
                {
                    FirstName = message.From?.FirstName,
                    LastName = message.From?.LastName,
                    Email = message.Chat.Username,
                };
                
                var jsonContent = JsonConvert.SerializeObject(dto);
                // Создание HTTP-запроса
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                await client.PostAsync(
                    $"https://90.156.254.193:8080/auth/register-tg-user",
                    content, cancellationToken);
            }
            
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы успешно зарегистрировались");
        }

        if (message.Text == "Авторизироваться")
        {
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
                HttpResponseMessage response;

                UserDto dto = new()
                {
                    FirstName = message.From?.FirstName,
                    LastName = message.From?.LastName,
                    Email = message.Chat.Username,
                };
                
                var jsonContent = JsonConvert.SerializeObject(dto);
                // Создание HTTP-запроса
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                await client.PostAsync(
                    $"https://90.156.254.193:8080/auth/login-tg-user",
                    content, cancellationToken);
            }
            
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы успешно авторизировались");
        }
        
        if (message.Text == "Отмена")
        {
            return;
        }
    }
    
    private static Task Error(ITelegramBotClient botClient, Exception exception, HandleErrorSource errorSource, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.InnerException?.Message);
        
        return Task.CompletedTask;
    }
}