# PasswordManager_Encryption

RU | EN

---

# RU

## PasswordManager_Encryption — WPF приложение

Простое WPF-приложение для сохранения, редактирования и управления паролями. Все пароли шифруются с использованием AES-256 (CBC) с безопасным хранением IV и солью.

### Возможности
- Сохранение записей (имя, логин, пароль, URL, заметки)
- Редактирование и удаление записей
- Поиск по записям
- Копирование пароля в буфер обмена
- AES-256 шифрование локального файла данных

### Безопасность
- Используется AES-256 для шифрования данных на диске.
- Пароль-мастер (master password) используется для генерации ключа через PBKDF2 (или другой KDF) с солью и большим числом итераций.
- IV (инициализационный вектор) генерируется случайно для каждой записи/файла и хранится вместе с зашифрованными данными.

> Важно: приложение хранит зашифрованные данные локально; безопасность зависит от силы master password и защиты машины пользователя.

### Установка и сборка
1. Клонируйте репозиторий:
   git clone https://github.com/Jerka2009/PasswordManager_Encryption.git
2. Откройте решение в Visual Studio (версия 2019/2022).
3. Соберите проект (Release).
4. Запустите приложение.

### Использование
1. При первом запуске задайте master password — он будет использоваться для шифрования/дешифрования базы.
2. Добавляйте записи: название, логин, пароль, URL, заметки.
3. Пароли шифруются автоматически при сохранении.

### Настройка AES-256
- Алгоритм: AES-256 (например, AES CBC или GCM — по желанию можно использовать GCM для целостности).
- Генерация ключа: PBKDF2(HMAC-SHA256) с уникальной солью и не менее 100 000 итераций (настраиваемо).
- IV: 16 байт, криптографически случайные, для каждого шифрования своим IV.
- Хранение: в зашифрованном контейнере/файле хранится соль, IV и зашифрованные данные.

### Лицензия
Добавьте лицензию в репозиторий (например, MIT) по желанию.

---

# EN

## PasswordManager_Encryption — WPF application

A simple WPF application for storing, editing and managing passwords. All passwords are encrypted using AES-256 (CBC) with secure handling of IV and salt.

### Features
- Store entries (title, username, password, URL, notes)
- Edit and delete entries
- Search entries
- Copy password to clipboard
- AES-256 encryption of local data file

### Security
- AES-256 is used to encrypt data at rest.
- Master password is used to derive the encryption key via PBKDF2 (or another KDF) with salt and high iteration count.
- IV (initialization vector) is randomly generated per record/file and stored alongside the ciphertext.

> Note: The application stores encrypted data locally; security depends on the strength of the master password and the user's machine protection.

### Installation & Build
1. Clone the repository:
   git clone https://github.com/Jerka2009/PasswordManager_Encryption.git
2. Open the solution in Visual Studio (2019/2022 recommended).
3. Build the project (Release).
4. Run the application.

### Usage
1. On first run, set a master password — it will be used to encrypt/decrypt the database.
2. Add entries with title, username, password, URL, and notes.
3. Passwords are encrypted automatically on save.

### AES-256 configuration
- Algorithm: AES-256 (e.g., AES-CBC or AES-GCM — GCM recommended if integrity/authentication is required).
- Key derivation: PBKDF2(HMAC-SHA256) with a unique salt and at least 100,000 iterations (configurable).
- IV: 16 bytes, cryptographically random, unique per encryption.
- Storage: the encrypted container/file stores salt, IV and ciphertext (consider including an HMAC or use AES-GCM for authentication).

### License
Add a license to the repository (e.g., MIT) if desired.

---

Если хотите, могу:
- Добавить пример конфигурации KDF/параметров в коде.
- Сгенерировать файл LICENSE (MIT) и поместить его в репозиторий.
- Написать инструкцию по миграции/экспорту/импорту базы.
