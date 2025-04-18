using AIChatBot;
using Microsoft.SemanticKernel;
using AIChatBot.Helper;
using AIChatBot.Interface;

var builder = WebApplication.CreateBuilder(args);



//builder.Services.AddKernel()

//    .AddOpenAIChatCompletion("gpt-3.5-turbo",
//                    builder.Configuration["AI:OpenAI:ApiKey"]);

builder.Services.AddScoped<ILLMFormatter, LLMFormatter>();

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

