using Tournament.Application.Common.Interfaces;
using Tournament.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tournament.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
	private readonly IDateTime _dateTime;
	private readonly ICurrentUserService _currentUserService;

	public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService,IDateTime dateTime)
	{
		_dateTime = dateTime;
		_currentUserService=currentUserService;
	}

	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		UpdateEntities(eventData.Context);

		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
	{
		UpdateEntities(eventData.Context);

		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	public void UpdateEntities(DbContext? context)
	{
		if (context == null) return;

		foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreatedBy = _currentUserService.UserName;
				entry.Entity.Created = _dateTime.Now;
			} 

			if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
			{
				entry.Entity.LastModifiedBy = _currentUserService.UserName;
				entry.Entity.LastModified = _dateTime.Now;
			}
		}
	}
}

public static class Extensions
{
	public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
		entry.References.Any(r => 
				r.TargetEntry != null && 
				r.TargetEntry.Metadata.IsOwned() && 
				(r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}

