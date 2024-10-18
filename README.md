# fork on 私人專案 https://github.com/oewsj/hotel_full

# Hotel API & DataBase Design

## Introduce - 專案介紹
這個後端專案使用 C# + ASP.NET Core WebAPI + MySQL 建立一個 Hotel API，主要用於於管理飯店會員的相關操作，包括註冊、登入、資料查詢及更新，並且支援 Google OAuth 2.0 登入及 JWT 驗證。

## Outline - 目錄
- Features - 專案功能
- Enviroment - 環境建置與需求
- Installing - 專案安裝流程
- Contributor - 開發人員與工作分配

## Features - 專案功能
- 使用 JWT 驗證系統進行使用者身份驗證。
- Google OAuth 2.0 身份驗證集成。
- 支援多種 API，包含會員登入、房間資訊管理、訂房等功能。
- 支援 MySQL 資料庫，使用 Entity Framework 進行 ORM 管理。
- API 文件透過 Swagger 自動生成。

## Enviroment - 環境建置與需求
### 伺服器（Server）
- .NET - 8.0
- ASP.NET Core Web API - 8.0.8
- EntityFrameworkCore - 8.0.8
- EntityFrameworkCore.Design - 8.0.8
- EntityFrameworkCore.MySQL - 8.0.2
- Pomelo.EntityFrameworkCore.MySQL - 8.0.2
- BCrypt.Net-Next - 4.0.3
- Swashbuckle.AspNetCore - 6.7.2
- System.IdentityModel.Tokens.Jwt - 8.0.2
- Google.Apis.Auth - 1.68.0
- jose-jwt - 5.0.0
- JWT - 10.1.1
- Microsoft.AspNetCore.Authentication.Google - 8.0.8
- Microsoft.AspNetCore.Authentication.JwtBearer - 8.0.8
- Portable.BouncyCastle - 1.9.0
- System.Drawing.Common - 8.0.8

### 資料庫（Database）
- MySQL - 8.0.5

### 身份驗證（Authentication）
- JWT 驗證
- Google OAuth 2.0 身份驗證

## Installing - 專案安裝流程
1. 將專案從 GitHub fork 到本地：
   ```bash
   git clone https://github.com/oewsj/hotel_full

2. 安裝所需的套件：
  ```bash
   dotnet restore

3. 啟動專案：
  ```bash
    dotnet run

## Contributor - 開發人員與工作分配
* ZionYou - 資料庫設計、開發API
* junior155235 - PM、開發API