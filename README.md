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

※実際の `Product` クラスのプロパティに合わせて修正してください。

### ステータスコード

|コード|内容|
|---|---|
|200|検索成功|
|401|未認証|
|500|システムエラー|