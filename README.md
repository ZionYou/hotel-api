# fork on 私人專案 https://github.com/oewsj/hotel_full

# Hotel API & DataBase Design

## Introduce - 專案介紹
這個後端專案使用 C# + ASP.NET Core WebAPI + MySQL 建立一個 Hotel API，主要用於於管理飯店會員的相關操作，包括註冊、登入、資料查詢及更新，並且支援 Google OAuth 2.0 登入及 JWT 驗證。

## Outline - 目錄
- Features - 專案功能
- Enviroment - 環境建置與需求
- API - 說明API的功能
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

## API - 說明API的功能
### API路徑 https://sunsetvalley.us.to/ + api
### 測試帳號 
- Account： test1@mail.com
- Password： 111111111

### Advertisement

- `GET /api/Advertisement/AdvertisementInfo` - 獲取廣告資訊
- `GET /api/Advertisement/AdvertisementForBanner` - 獲取橫幅廣告
- `POST /api/Advertisement` - 新增廣告
- `PUT /api/Advertisement` - 更新廣告
- `DELETE /api/Advertisement` - 刪除廣告

### Ecpay

- `POST /api/Ecpay/goToEcpay` - 前往綠界支付
- `POST /api/Ecpay/fetchEcpay` - 獲取綠界支付結果

### HotelInfo

- `GET /api/HotelInfo` - 獲取飯店資訊
- `PUT /api/HotelInfo` - 更新飯店資訊

### Member

- `GET /api/Member/secure` - 獲取安全資訊
- `GET /api/Member` - 獲取所有會員
- `GET /api/Member/{id}` - 獲取特定會員
- `PUT /api/Member/{id}` - 更新會員資訊
- `DELETE /api/Member/{id}` - 刪除會員
- `POST /api/Member/register` - 會員註冊
- `POST /api/Member/login` - 會員登入
- `POST /api/Member/google-login` - Google登入
- `POST /api/Member/refresh-token` - 更新令牌

### News

- `GET /api/News/AllNewsForEnd` - 獲取所有新聞(後台)
- `GET /api/News` - 獲取所有新聞(前台)
- `POST /api/News` - 新增新聞
- `GET /api/News/{id}` - 獲取特定新聞
- `PUT /api/News/{id}` - 更新新聞
- `DELETE /api/News/{id}` - 刪除新聞

### Product

- `GET /api/Product` - 獲取所有產品
- `POST /api/Product` - 新增產品
- `DELETE /api/Product` - 刪除產品
- `GET /api/Product/Product/{typeId}` - 獲取特定類型的產品
- `GET /api/Product/Product/single/{id}` - 獲取單一產品
- `PUT /api/Product/{id}` - 更新產品

### ProductOrder

- `GET /api/ProductOrder` - 獲取所有訂單
- `POST /api/ProductOrder` - 新增訂單
- `PUT /api/ProductOrder` - 更新訂單
- `DELETE /api/ProductOrder` - 刪除訂單
- `GET /api/ProductOrder/search/{id}` - 搜尋訂單
- `GET /api/ProductOrder/order/{id}` - 獲取特定訂單
- `PUT /api/ProductOrder/OrderDetail` - 更新訂單細節

### ProductType

- `GET /api/ProductType` - 獲取所有產品類型
- `POST /api/ProductType` - 新增產品類型
- `PUT /api/ProductType` - 更新產品類型
- `DELETE /api/ProductType` - 刪除產品類型
- `GET /api/ProductType/{id}` - 獲取特定產品類型

### RoomType

- `GET /api/RoomType` - 獲取所有房型
- `GET /api/RoomType/{id}` - 獲取特定房型
- `POST /api/RoomType/InsertRoomType` - 新增房型
- `PUT /api/RoomType/UpdateRoomType/{id}/{type}` - 更新房型
- `DELETE /api/RoomType/DeleteRoomType/{id}` - 刪除房型

### 認證

大多數端點需要認證。請在請求頭中包含有效的JWT令牌:

```
Authorization: Bearer <your_token_here>
```

### 錯誤處理

API使用標準的HTTP狀態碼來表示請求的結果。在發生錯誤時,響應體將包含更多詳細信息。

### 範例請求

以下是使用curl進行API請求的示例:

```bash
# 獲取所有產品
curl -X GET "https://sunsetvalley.us.to/api/Product" -H "Authorization: Bearer YOUR_TOKEN"

# 新增會員
curl -X POST "https://sunsetvalley.us.to/api/Member/register" \
     -H "Content-Type: application/json" \
     -d '{"name":"John Doe","email":"john@example.com","password":"securepass"}'

## Contributor - 開發人員與工作分配
* ZionYou - 資料庫設計、開發API
* junior155235 - PM、開發API

