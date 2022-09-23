
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

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
