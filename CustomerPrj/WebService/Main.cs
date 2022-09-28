
var builder = WebApplication.CreateBuilder(args);

//为了运行测试JS添加了一点让服务器端能支持CORS的代码

string[] urls = new[] { "http://localhost:7292", "https://localhost:7292" };


builder.Services.AddCors(options =>
    options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
    .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
    }

app.UseCors();
app.UseHttpsRedirection();
app.UseDefaultFiles();//
app.UseStaticFiles();   //
app.UseAuthorization();
app.MapControllers();


CustomerModel.InitTestSamples();

app.Run();
