using Microsoft.EntityFrameworkCore;
using TheGameVoice.Domain.Common.Interfaces;

namespace TheGameVoice.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilters(
        this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete)
                .IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ModelBuilderExtensions)
                    .GetMethod(nameof(GetSoftDeleteFilter),
                        System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                var filter = method.Invoke(null, Array.Empty<object>());

                entityType.SetQueryFilter(
                    (System.Linq.Expressions.LambdaExpression)filter!);
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression
        GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDelete
    {
        return (TEntity x) => x.DeletedAt == null;
    }
}