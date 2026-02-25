
using DataContext;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Services.Interfaces;
using Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using DataContext.model;
using System.Speech.Synthesis;
using System.Media;



namespace MyProject
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddSingleton<IContext>(new GlottieContext(connection));
            builder.Services.AddSingleton<IOpenAi, Chat>(); // Chat עם HttpClient
            builder.Services.AddAutoMapper(typeof(MapperProfile));
            builder.Services.AddServices();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact",
                    p => p.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });


            //var ttsService = new Speech();

            //// 1. השמעת טקסט עם קול ברירת מחדל
            //ttsService.Speak("Hello! This is the default voice.");

            //// 2. השמעת טקסט עם קול מסוים (אם קיים)
            //ttsService.Speak("Hello! This is David's voice.", "Microsoft David Desktop");

            //// 3. הצגת כל הקולות המותקנים במערכת
            //var voices = ttsService.GetInstalledVoices();
            //Console.WriteLine("Installed voices:");
            //foreach (var voice in voices)
            //{
            //    Console.WriteLine(voice);
            //}

            //// 4. בחירה דינמית של קול על ידי המשתמש
            //Console.WriteLine("Enter the name of the voice you want to use:");
            //string chosenVoice = Console.ReadLine();
            //Console.WriteLine("Enter the text to speak:");
            //string text = Console.ReadLine();
            //ttsService.Speak(text, chosenVoice);

            //Console.WriteLine("Done. Press any key to exit.");
            //Console.ReadKey();
            //ttsService.Speak("Hello! This is a free text-to-speech example in English.");

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowReact");
            app.UseHttpsRedirection();


            //authorize -אימות 
            //authenticate -הרשאות גישה

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
