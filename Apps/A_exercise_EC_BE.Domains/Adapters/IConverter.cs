namespace A_exercise_EC_BE.Domains.Adapters;

/// <summary>
/// DomainObjectをEntityに変換するアダプターのインターフェース
/// </summary>
public interface IConverter<TDomain, TTarget>
{
    Task<TTarget> ConvertAsync(TDomain domain);
}