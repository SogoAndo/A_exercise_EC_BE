using A_exercise_EC_BE.Presentations.Configs;

var builder = WebApplication.CreateBuilder(args);

// ControllerをDIコンテナへ登録する
builder.Services.AddControllers();

// Application、Infrastructureなどの依存関係を登録する
builder.Services.AddApplicationDependencies(
    builder.Configuration);

// Swagger生成に必要なサービスを登録する
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// appsettings.jsonの設定値を取得する
var swaggerEnabled =
    builder.Configuration.GetValue<bool>(
        "Swagger:Enabled");

// 開発環境、または設定で明示的に有効化した場合のみ
// Swaggerを公開する
if (app.Environment.IsDevelopment()
    || swaggerEnabled)
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 現在の本番環境はNginxもHTTP運用のため、
// ProductionではHTTPSへリダイレクトしない
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// JWT認証を実装するときに追加する
app.UseAuthentication();
app.UseAuthorization();

// ControllerのURLを有効化する
app.MapControllers();

app.Run();