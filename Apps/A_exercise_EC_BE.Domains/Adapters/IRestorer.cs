namespace A_exercise_EC_BE.Domains.Adapters;

/// <summary>
/// 永続化用オブジェクトからドメインオブジェクトを復元する。
/// </summary>
public interface IRestorer<TDomain, in TTarget>
{
    Task<TDomain> RestoreAsync(TTarget target);
}
