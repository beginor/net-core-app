using System;
using Beginor.AppFx.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace Beginor.AppFx.Services {

    public interface IBaseService<TModel> where TModel : IBaseEntity<string> {
        Task<IList<TModel>> GetAllAsync();
        Task<TModel> GetByIdAsync(string id);
        Task CreateAsync(TModel model);
        Task UpdateAsync(string id, TModel model);
        Task DeleteAsync(string id);
    }

    public abstract class BaseService<TRepository, TEntity, TModel, TId>
        : IBaseService<TModel>
        where TId : IComparable<TId>
        where TModel : class, IBaseEntity<string>
        where TEntity : class, IBaseEntity<TId>
        where TRepository : class, IRepository<TEntity, TId> {

        protected TRepository Repository { get; private set; }

        protected BaseService(TRepository repository) {
            Repository = repository;
        }

        protected virtual string ConvertIdToString(TId id) {
            return id.ToString();
        }

        protected abstract TId ConvertIdFromString(string id);

        public async Task<IList<TModel>> GetAllAsync() {
            var entities = await Repository.GetAllAsync();
            return Mapper.Map<IList<TModel>>(entities);
        }

        public async Task<TModel> GetByIdAsync(string id) {
            Argument.NotNullOrEmpty(id, nameof(id));
            var entity = await Repository.GetByIdAsync(ConvertIdFromString(id));
            return Mapper.Map<TModel>(entity);
        }

        public async Task CreateAsync(TModel model) {
            Argument.NotNull(model, nameof(model));
            var entity = Mapper.Map<TEntity>(model);
            entity = await Repository.SaveAsync(entity);
            Mapper.Map(entity, model);
        }

        public async Task UpdateAsync(string id, TModel model) {
            Argument.NotNullOrEmpty(id, nameof(id));
            Argument.NotNull(model, nameof(model));
            var entity = await Repository.GetByIdAsync(ConvertIdFromString(id));
            if (entity == null) {
                throw new InvalidOperationException(
                    $"{typeof(TEntity).Name} with {id} is null!"
                );
            }
            Mapper.Map(model, entity);
            await Repository.UpdateAsync(entity);
            Mapper.Map(entity, model);
        }

        public async Task DeleteAsync(string id) {
            Argument.NotNullOrEmpty(id, nameof(id));
            await Repository.DeleteAsync(ConvertIdFromString(id));
        }

    }

}
