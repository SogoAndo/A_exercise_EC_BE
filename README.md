# Fullness Stationery EC Backend

Fullness Stationery社の顧客向けECサービス用バックエンドです。

## 技術構成

- C# / .NET 10 / ASP.NET Core Web API
- DDDに基づくDomain、Application、Infrastructure、Presentationの4層
- PostgreSQL（管理側バックエンドと同じデータベースを共有）
- MSTest

## プロジェクト構成

```text
Apps/
  A_exercise_EC_BE.Domains/
  A_exercise_EC_BE.Applications/
  A_exercise_EC_BE.Infrastructures/
  A_exercise_EC_BE.Presentations/
Tests/
  A_exercise_EC_BE.Domains.Tests/
  A_exercise_EC_BE.Applications.Tests/
  A_exercise_EC_BE.Infrastructures.Tests/
  A_exercise_EC_BE.Presentations.Tests/
```

## 対象ユースケース

- UC001 顧客アカウント登録
- UC002 顧客ログイン
- UC003 カテゴリ別商品検索
- UC004 商品購入
- UC005 購入確定
- UC006 購入キャンセル
- UC007 購入履歴閲覧
- UC008 顧客ログアウト

APIは今後 `/api/ec/...` 配下に実装します。

## ローカルでの検証

```bash
dotnet restore A_exercise_EC_BE.slnx
dotnet build A_exercise_EC_BE.slnx
dotnet test A_exercise_EC_BE.slnx
```

EC APIのローカルHTTPポートは `5100` を使用します。

接続文字列や顧客用JWT署名鍵などの秘密情報はリポジトリへ保存せず、環境変数またはローカル専用設定から渡します。


# API一覧

## UC001 顧客アカウント登録

### ベースURL

```
/account
```

---

## 1. 入力画面初期表示情報取得

|項目|内容|
|---|---|
|エンドポイント|`GET /account/form`|
|HTTPメソッド|GET|
|コントローラー|RegisterCustomerAccountController|
|アクションメソッド|GetForm()|

### 概要

顧客アカウント登録画面の初期表示情報を取得します。

### レスポンス例（200）

```json
{
  "title": "顧客アカウント登録(入力)",
  "model": {}
}
```

### ステータスコード

|コード|内容|
|---|---|
|200|取得成功|
|500|システムエラー|

---

## 2. アカウント名重複チェック

|項目|内容|
|---|---|
|エンドポイント|`GET /account/validate/username?username={username}`|
|HTTPメソッド|GET|
|コントローラー|RegisterCustomerAccountController|
|アクションメソッド|ValidateUsername()|

### 概要

入力されたアカウント名が既に登録されているか確認します。

### クエリパラメータ

|項目|型|必須|
|---|---|---|
|username|string|○|

### レスポンス例（200）

```json
{
  "exists": false,
  "message": "使用できるアカウント名です"
}
```

### 重複時（409）

```json
{
  "code": "USERNAME_ALREADY_EXISTS",
  "exists": true,
  "message": "アカウント名は既に存在します"
}
```

### ステータスコード

|コード|内容|
|---|---|
|200|使用可能|
|400|入力値不正|
|409|アカウント名重複|
|500|システムエラー|

---

## 3. メールアドレス重複チェック

|項目|内容|
|---|---|
|エンドポイント|`GET /account/validate/mail-address?mailAddress={mailAddress}`|
|HTTPメソッド|GET|
|コントローラー|RegisterCustomerAccountController|
|アクションメソッド|ValidateMailAddress()|

### 概要

入力されたメールアドレスが既に登録されているか確認します。

### クエリパラメータ

|項目|型|必須|
|---|---|---|
|mailAddress|string|○|

### レスポンス例（200）

```json
{
  "exists": false,
  "message": "使用できるメールアドレスです"
}
```

### 重複時（409）

```json
{
  "code": "MAIL_ADDRESS_ALREADY_EXISTS",
  "exists": true,
  "message": "メールアドレスは既に存在します"
}
```

### ステータスコード

|コード|内容|
|---|---|
|200|使用可能|
|400|入力値不正|
|409|メールアドレス重複|
|500|システムエラー|

---

## 4. 入力内容確認

|項目|内容|
|---|---|
|エンドポイント|`POST /account/confirm`|
|HTTPメソッド|POST|
|コントローラー|RegisterCustomerAccountController|
|アクションメソッド|Confirm()|

### 概要

入力された顧客アカウント情報を確認し、確認画面表示用の情報を返します。

### リクエスト例

```json
{
  "name": "末永 浩平",
  "kana": "スエナガ コウヘイ",
  "address1": "埼玉県川口市",
  "address2": "上青木西2-3-306",
  "phoneNumber": "070-8015-4728",
  "mailAddress": "suenaga_1028@example.co.jp",
  "username": "suenaga",
  "password": "passsuenaga"
}
```

### レスポンス

入力内容確認用の `RegisterCustomerAccountConfirmViewModel` を返却します。

### ステータスコード

|コード|内容|
|---|---|
|200|確認成功|
|400|入力値不正|
|409|アカウント名またはメールアドレス重複|
|500|システムエラー|

---

## 5. 顧客アカウント登録

|項目|内容|
|---|---|
|エンドポイント|`POST /account/complete`|
|HTTPメソッド|POST|
|コントローラー|RegisterCustomerAccountController|
|アクションメソッド|Complete()|

### 概要

顧客アカウントを登録し、登録完了情報を返します。

### リクエスト例

```json
{
  "name": "末永 浩平",
  "kana": "スエナガ コウヘイ",
  "address1": "埼玉県川口市",
  "address2": "上青木西2-3-306",
  "phoneNumber": "070-8015-4728",
  "mailAddress": "suenaga_1028@example.co.jp",
  "username": "suenaga",
  "password": "passsuenaga"
}
```

### レスポンス例（201）

```json
{
  "customerUuid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "name": "末永 浩平",
  "username": "suenaga",
  "mailAddress": "suenaga_1028@example.co.jp"
}
```

※実際の `RegisterCustomerAccountCompleteViewModel` の項目に合わせて修正してください。

### ステータスコード

|コード|内容|
|---|---|
|201|登録成功|
|400|入力値不正|
|409|アカウント名またはメールアドレス重複|
|500|システムエラー|



# API一覧

## UC002 顧客ログイン

### ベースURL

```
/
```

---

## 1. 顧客ログイン

|項目|内容|
|---|---|
|エンドポイント|`POST /login`|
|HTTPメソッド|POST|
|コントローラー|LoginCustomerController|
|アクションメソッド|LoginAsync()|

### 概要

メールアドレスとパスワードで顧客認証を行い、認証に成功した場合はアクセストークンを発行します。

### リクエスト例

```json
{
  "mailAddress": "suenaga_1028@example.co.jp",
  "password": "passsuenaga"
}
```

### レスポンス例（200）

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.xxxxxxxxxxxxxxxxxxxxxxxxx",
  "expiresAt": "2026-07-24T18:30:00Z"
}
```

### 認証失敗（401）

```json
{
  "code": "AUTHENTICATION_FAILED",
  "message": "メールアドレスまたはパスワードが正しくありません。"
}
```
### ステータスコード

|コード|内容|
|---|---|
|200|ログイン成功（アクセストークン発行）|
|400|入力値不正|
|401|認証失敗（メールアドレスまたはパスワード誤り）|
|500|システムエラー|


# API一覧

## UC003 商品カテゴリー検索

### ベースURL

```
/product/search
```

---

## 1. 商品カテゴリー検索

|項目|内容|
|---|---|
|エンドポイント|`GET /product/search?productCategoryUuid={productCategoryUuid}`|
|HTTPメソッド|GET|
|コントローラー|SearchProductByCategoryController|
|アクションメソッド|Search()|

### 概要

指定した商品カテゴリUUIDに属する商品一覧を取得します。

### クエリパラメータ

|項目|型|必須|
|---|---|---|
|productCategoryUuid|UUID|○|

### レスポンス例（200）

```json
[
  {
    "productUuid": "11111111-1111-1111-1111-111111111111",
    "name": "赤ペン",
    "price": 120,
    "imageUrl": "https://example.com/images/red-pen.png",
    "category": {
      "categoryUuid": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "name": "文房具"
    },
    "stock": {
      "stock": 10
    }
  },
  {
    "productUuid": "22222222-2222-2222-2222-222222222222",
    "name": "青ペン",
    "price": 120,
    "imageUrl": "https://example.com/images/blue-pen.png",
    "category": {
      "categoryUuid": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "name": "文房具"
    },
    "stock": {
      "stock": 8
    }
  }
]
```

### ステータスコード

|コード|内容|
|---|---|
|200|検索成功|
|401|未認証|
|500|システムエラー|


# API一覧

## UC004 商品詳細取得

### ベースURL

```
/products/detail
```

---

## 1. 商品詳細取得

|項目|内容|
|---|---|
|エンドポイント|`GET /products/detail/{productId}`|
|HTTPメソッド|GET|
|コントローラー|ProductDetailController|
|アクションメソッド|GetAsync()|

### 概要

指定された商品UUIDをもとに、商品の詳細情報と現在の在庫数を取得します。

商品が存在する場合、商品詳細情報を返却します。

### パスパラメータ

|項目|型|必須|
|---|---|---|
|productId|UUID|○|

### リクエスト例

```
GET /products/detail/72f394dd-f316-4a76-9c51-0de1af990991
```

### レスポンス例（200）

```json
{
  "productUuid": "72f394dd-f316-4a76-9c51-0de1af990991",
  "productName": "橙ペン",
  "price": 120,
  "productImage": "https://localhost:5126/images/products/a07595ae-b8c1-46c3-80d1-d77724ada1e6.png",
  "stockQuantity": 10
}
```


### ステータスコード

|コード|内容|
|---|---|
|200|商品詳細取得成功|
|404|商品が存在しない|
|500|システムエラー|

# API一覧

## UC005 購入確定

### ベースURL

```
/purchase
```

---

## 1. 購入確定

|項目|内容|
|---|---|
|エンドポイント|`POST /purchase/complete`|
|HTTPメソッド|POST|
|コントローラー|ConfirmPurchaseController|
|アクションメソッド|CompleteAsync()|

### 概要

認証済み顧客の購入を確定します。

支払い方法と購入商品情報を受け取り、注文情報を作成します。
購入確定成功後、確定した注文情報を返却します。

本APIは顧客JWT認証が必要です。

### 認証

```
Authorization: Bearer {customer access token}
```

### リクエスト例

```json
{
  "paymentMethodId": 4,
  "items": [
    {
      "productUuid": "10000000-0000-0000-0000-000000000001",
      "quantity": 4
    }
  ]
}
```


### レスポンス例（201）

```json
{
  "completeMessage": "購入が完了しました",
  "orderUuid": "66aa718d-8d59-46c5-876d-9eca090b122e",
  "orderDate": "2026/07/27 17:18:38",
  "totalPrice": 480
}
```

### ステータスコード

|コード|内容|
|---|---|
|201|購入確定成功|
|400|入力値不正|
|401|未認証・JWT認証情報不正|
|404|商品または購入対象なし|
|500|システムエラー|



# API一覧

## UC007 購入履歴閲覧

### ベースURL

```
/purchase/history
```

---

## 1. 購入履歴一覧取得

|項目|内容|
|---|---|
|エンドポイント|`GET /purchase/history`|
|HTTPメソッド|GET|
|コントローラー|PurchaseHistoryListController|
|アクションメソッド|GetAsync()|

### 概要

認証済み顧客の購入履歴一覧を取得します。

購入日時、注文番号、購入金額などの購入履歴情報を返却します。

本APIは顧客JWT認証が必要です。

### 認証

```
Authorization: Bearer {customer access token}
```

### リクエスト例

```http
GET /purchase/history
Authorization: Bearer {customer access token}
```

### レスポンス例（200）

```json
{
  "orderUuid": "66aa718d-8d59-46c5-876d-9eca090b122e",
  "orderDate": "2026/07/27 17:18:38",
  "orderStatusId": 1,
  "orderStatusName": "受付",
  "orderItems": [
    {
      "productUuid": "10000000-0000-0000-0000-000000000001",
      "productName": "水性ボールペン(黒)",
      "price": 120,
      "quantity": 4,
      "subtotal": 480
    }
  ],
  "totalPrice": 480
}
```

### ステータスコード

|コード|内容|
|---|---|
|200|購入履歴取得成功|
|401|未認証・JWT認証情報不正|
|500|システムエラー|


# API一覧

## UC007 購入履歴閲覧

### ベースURL

```
/purchase/history
```

---

## 2. 購入履歴詳細取得

|項目|内容|
|---|---|
|エンドポイント|`GET /purchase/history/{orderUuid}`|
|HTTPメソッド|GET|
|コントローラー|PurchaseHistoryDetailController|
|アクションメソッド|GetAsync()|

### 概要

認証済み顧客自身の購入履歴詳細を取得します。

指定した注文UUIDに一致する購入履歴が存在する場合、購入商品の詳細情報を返却します。

本APIは顧客JWT認証が必要です。

### 認証

```
Authorization: Bearer {customer access token}
```

### パスパラメータ

|項目|型|必須|
|---|---|---|
|orderUuid|UUID|○|

### リクエスト例

```http
GET /purchase/history/66aa718d-8d59-46c5-876d-9eca090b122e
Authorization: Bearer {customer access token}
```

### レスポンス例（200）

```json
{
  "orderList": [
    {
      "orderUuid": "66aa718d-8d59-46c5-876d-9eca090b122e",
      "orderDate": "2026/07/27 17:18:38",
      "orderStatus": "受付",
      "totalPrice": 480,
      "detailUrl": "/purchase/history/66aa718d-8d59-46c5-876d-9eca090b122e"
    }
  ],
  "message": null
}
```

### レスポンス例（404）

```json
{
  "message": "購入履歴が見つかりませんでした。"
}
```

### ステータスコード

|コード|内容|
|---|---|
|200|購入履歴詳細取得成功|
|401|未認証・JWT認証情報不正|
|404|購入履歴が存在しない|
|500|システムエラー|


# API一覧

## UC008 顧客ログアウト

### ベースURL

```
/
```

---

## 1. 顧客ログアウト

|項目|内容|
|---|---|
|エンドポイント|`POST /logout`|
|HTTPメソッド|POST|
|コントローラー|LogoutCustomerController|
|アクションメソッド|LogoutAsync()|

### 概要

認証済み顧客のログアウトを実行します。

ログアウト処理を実行し、ログアウト結果を返却します。

本APIは顧客JWT認証が必要です。

### 認証

```
Authorization: Bearer {customer access token}
```

### リクエスト例

```http
POST /logout
Authorization: Bearer {customer access token}
```

### レスポンス例（200）

```json
{
  "loggedOut": true
}
```


### ステータスコード

|コード|内容|
|---|---|
|200|ログアウト成功|
|401|未認証・JWT認証情報不正|
|500|システムエラー|



# API一覧

## 商品カテゴリ取得

### ベースURL

```
/product-category
```

---

## 1. 商品カテゴリプルダウン取得

|項目|内容|
|---|---|
|エンドポイント|`GET /product-category/options`|
|HTTPメソッド|GET|
|コントローラー|ProductCategoryController|
|アクションメソッド|FindAllOptionsAsync()|

### 概要

商品カテゴリのプルダウン項目一覧を取得します。

商品カテゴリUUIDとカテゴリ名を取得し、商品検索画面などのプルダウン表示に使用します。

### リクエスト例

```http
GET /product-category/options
```

### レスポンス例（200）

```json
[
  {
    "value": "e50d978b-b73d-4afb-8e85-ace9cf1e12a7",
    "label": "文房具"
  },
  {
    "value": "ae4ed829-7017-4972-8187-59384e0b5627",
    "label": "雑貨"
  },
  {
    "value": "707c67f1-8f9a-457f-af39-f99c66085c45",
    "label": "パソコン周辺機器"
  },
  {
    "value": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "label": "テストカテゴリ"
  }
]
```

### ステータスコード

|コード|内容|
|---|---|
|200|商品カテゴリ取得成功|
|500|システムエラー|


# API一覧

## 支払い方法取得

### ベースURL

```
/payment-method
```

---

## 1. 支払い方法プルダウン取得

|項目|内容|
|---|---|
|エンドポイント|`GET /payment-method/options`|
|HTTPメソッド|GET|
|コントローラー|PaymentMethodController|
|アクションメソッド|FindAllAsync()|

### 概要

支払い方法のプルダウン項目一覧を取得します。

支払い方法IDと支払い方法名を取得し、購入画面などのプルダウン表示に使用します。

### リクエスト例

```http
GET /payment-method/options
```

### レスポンス例（200）

```json
[
  {
    "value": 1,
    "label": "クレジットカード"
  },
  {
    "value": 2,
    "label": "PayPay"
  },
  {
    "value": 3,
    "label": "コンビニ払い"
  },
  {
    "value": 4,
    "label": "銀行振込"
  }
]
```


### ステータスコード

|コード|内容|
|---|---|
|200|支払い方法取得成功|
|500|システムエラー|