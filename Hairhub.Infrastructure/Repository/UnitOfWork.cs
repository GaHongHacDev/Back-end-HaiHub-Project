using Hairhub.Service.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Infrastructure.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
        private Dictionary<Type, object> _repositories;
        public DbContext Context { get; }

        public UnitOfWork()
        {
            Context = new HaiHubDbContext();
            _repositories = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public int Commit()
        {
            TrackChanges();
            return Context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            TrackChanges();
            return await Context.SaveChangesAsync();
        }

        private void TrackChanges()
        {
            var validationErrors = Context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {

            try
            {
                _repositories ??= new Dictionary<Type, object>();
                if (_repositories.TryGetValue(typeof(TEntity), out object repository))
                {
                    return (IGenericRepository<TEntity>)repository;
                }

                repository = new GenericRepository<TEntity>(Context);
                _repositories.Add(typeof(TEntity), repository);
                return (IGenericRepository<TEntity>)repository;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;

        }

        private Type GetClassImplementingInterface(Type interfaceType)
        {
            var genericType = interfaceType.GenericTypeArguments.First();

            return Assembly.GetExecutingAssembly().GetTypes()
                                    .First(t => t.IsClass == true && t.IsAbstract == false
                                            && (t.GetInterface(interfaceType.Name)
                                                ?.GetGenericArguments()
                                                ?.Any(a => a.Name == genericType.Name) ?? false));
        }
    }
}
