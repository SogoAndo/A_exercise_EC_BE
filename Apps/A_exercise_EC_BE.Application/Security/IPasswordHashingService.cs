namespace A_exercise_EC_BE.Application.Security;

/// <summary>
/// パスワードのハッシュ化を行うサービス
/// </summary>
public interface IPasswordHashingService
{
    /// <summary>
    /// 平文パスワードをハッシュ化する
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <returns>ハッシュ化済みパスワード</returns>
    string Hash(string password);

    /// <summary>
    /// 平文パスワードとハッシュ値を照合する
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <param name="hashedPassword">ハッシュ化済みパスワード</param>
    /// <returns>一致する場合true</returns>
    bool Verify(
        string password,
        string hashedPassword);
}