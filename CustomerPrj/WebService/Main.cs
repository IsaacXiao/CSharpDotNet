
var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
//为了运行测试JS添加了一点让服务器端能支持CORS的代码
=======
//为了运行测试JS添加了一点设置服务器端能支持CORS的代码
>>>>>>> 09ac0e11c2e9a7425ca148a1f8614d1a3e8745fc

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

//若不需要初始样本数据就把这行注释掉
CustomerModel.InitTestSamples();

app.Run();
