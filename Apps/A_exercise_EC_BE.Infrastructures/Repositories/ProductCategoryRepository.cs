using Microsoft.EntityFrameworkCore;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
namespace A_exercise_EC_BE.Infrastructures.Repositories;
/// <summary>
///  ドメインオブジェクト:商品カテゴリのCRUD操作インターフェイスの実装
/// </summary>
public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _context;
    private readonly ProductCategoryEntityAdapter _adapter;
    /// <summary>
    /// コンストラクタ 
    /// </summary>
    /// <param name="context">アプリケーション用データベースコンテキスト</param>
    /// <param name="adapter">ドメインオブジェクト:ProductCategoryとProductCategoryEntityの相互変換クラス</param> 
    public ProductCategoryRepository(
        AppDbContext context,
        ProductCategoryEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// すべての商品カテゴリを取得する
    /// </summary>
    /// <returns>ProductCategoryのリスト</returns>
    public async Task<List<ProductCategory>> FindAllAsync()
    {
        try
        {
            // すべての商品カテゴリを取得する
            var entities = await _context.ProductCategories
                .AsNoTracking().ToListAsync(); //追跡データをもらわない形でEntityが入ったリストで受けとる
            // ProductCategoryのリストを生成する
            var categories = new List<ProductCategory>();
            foreach (var entity in entities)
            {
                // ProductCategoryEntityからProductCategoryを復元する
                categories.Add(await _adapter.RestoreAsync(entity));
            }
            return categories;
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする（DBアクセス不可など）
            // 例外をもう一度,exとして投げることを「再スロー」という
            // DBにアクセスできない原因別に本当は違う例外を投げるけれど、それを全部設定するのは大変なのでラップしてスロー
            // ,ex を入れることで、開発側は何のエラーなのかを識別可能
            throw new InternalException("すべての商品カテゴリ取得時に予期しないエラーが発生しました。", ex);
        }
    }
}