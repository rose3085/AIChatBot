using AIChatBot;
using Microsoft.SemanticKernel;
using AIChatBot.Helper;
using AIChatBot.Interface;
using AIChatBot.Data;
using Microsoft.EntityFrameworkCore;
using AIChatBot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//builder.Services.AddKernel()

//    .AddOpenAIChatCompletion("gpt-3.5-turbo",
//                    builder.Configuration["AI:OpenAI:ApiKey"]);

builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddScoped<ILLMFormatter, LLMFormatter>();
builder.Services.AddScoped<ITextTokenizer,TextTokenizer>();
builder.Services.AddScoped<IImageFormatter, ImageFormatter>();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "ReactPolicy",
        builder =>
        {
            builder
                .WithOrigins("https://localhost:3000/")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
        }
    );
});


var app = builder.Build();




//app.MapGet("/chat",async (string question, ILLMFormatter formatter) =>
//{

//    var response = await formatter.FormatMessage(question);
//    return Results.Ok(response);
//});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("ReactPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();

