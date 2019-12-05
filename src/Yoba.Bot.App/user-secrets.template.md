https://docs.microsoft.com/ru-ru/aspnet/core/security/app-secrets

dotnet user-secrets init
dotnet user-secrets set "YobaBot:ConnectionString" "Data Source=C:\tmp\YobaDB.sqlite3;BinaryGUID=False"
dotnet user-secrets set "YobaBot:TelegramToken" "QWERTY"
dotnet user-secrets set "YobaBot:GroupChatId:0" "-123"
dotnet user-secrets set "YobaBot:Proxy:Host" "proxy.com"
dotnet user-secrets set "YobaBot:Proxy:Port" "1080"
dotnet user-secrets set "YobaBot:Proxy:Login" "login@proxy.com"
dotnet user-secrets set "YobaBot:Proxy:Password" "password"
dotnet user-secrets list

or edit %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json