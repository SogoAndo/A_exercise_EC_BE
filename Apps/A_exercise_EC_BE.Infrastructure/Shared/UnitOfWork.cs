using Microsoft.EntityFrameworkCore.Storage;
using A_exercise_EC_BE.Application.Usecases;
using A_exercise_EC_BE.Infrastructure.Contexts;
namespace A_exercise_EC_BE.Infrastructure.Shared;
/// <summary>
/// Unit of Workパターンを利用したトランザクション制御インターフェイスの実装
/// </summary>

// トランザクション→全部成功 or 全部失敗のどちらかに振り分ける仕組み
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context">アプリケーション用DbContext</param>
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// トランザクションを開始する
    /// </summary>
    /// <returns>なし</returns>
    public async Task BeginAsync()
    {
        // トランザクションがなければ開始する
        if (_transaction is null)
        {
            // トランザクションを開始する
            _transaction = await _context.Database.BeginTransactionAsync();
        }
    }

    /// <summary>
    /// トランザクションをコミットする
    /// </summary>
    /// <returns>なし</returns>
    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            // トランザクションをコミットする
            await _transaction.CommitAsync();
            // トランザクションを破棄する
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// トランザクションをロールバックする
    /// </summary>
    /// <returns>なし</returns>
    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            // トランザクションをロールバックする
            await _transaction.RollbackAsync();
            // トランザクションを破棄する
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}