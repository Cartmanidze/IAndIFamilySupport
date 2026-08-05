# IAndIFamilySupport

Telegram-бот технической поддержки. Проект работает на .NET 9 и получает обновления через HTTPS webhook.

## Безопасность

- Не сохраняйте токен Telegram в Git.
- Передавайте токен и адрес webhook через переменные окружения.
- Если токен когда-либо попадал в репозиторий, отзовите его через `@BotFather`.
- Для webhook используйте отдельный домен с действующим TLS-сертификатом.

## Настройки

Приложение читает две обязательные переменные:

```text
TelegramBot__Token=<новый токен>
TelegramBot__WebhookUrl=https://bot.example.com/telegram/update
```

Без них приложение завершится при запуске с понятной ошибкой.

## Проверка

```bash
dotnet test tests/IAndIFamilySupport.API.Tests/IAndIFamilySupport.API.Tests.csproj -c Release
dotnet build IAndIFamilySupport.API.csproj -c Release
```

## Публикация для Linux

```bash
dotnet publish IAndIFamilySupport.API.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -o publish/linux-x64
```

Файлы из `publish/linux-x64` размещаются в `/opt/iandifamilysupport`. Пример службы находится в `deploy/iandifamilysupport.service`.

Секреты хранятся на сервере в `/etc/iandifamilysupport.env` с правами `600`. Пример содержимого находится в `deploy/iandifamilysupport.env.example`.

Перед приложением нужен обратный прокси с HTTPS. Пример для Caddy находится в `deploy/Caddyfile.example`.
