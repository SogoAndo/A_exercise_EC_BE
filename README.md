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
  A_exercise_EC_BE.Domain/
  A_exercise_EC_BE.Application/
  A_exercise_EC_BE.Infrastructure/
  A_exercise_EC_BE.Presentation/
Tests/
  A_exercise_EC_BE.Domain.Tests/
  A_exercise_EC_BE.Application.Tests/
  A_exercise_EC_BE.Infrastructure.Tests/
  A_exercise_EC_BE.Presentation.Tests/
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
