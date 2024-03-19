using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> _data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            _data = new List<T>(data);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_data.FirstOrDefault(x => x.Id == id));
        }

        public  Task<T> AddAsync(T value)
        {
            if (_data.Any(x => x.Id == value.Id))
            {
                throw new InvalidOperationException("Item with the same Id already exists");
            }
            _data.Add(value);
            return Task.FromResult(value);
        }

        public Task UpdateAsync(T value)
        {
            if (_data.FirstOrDefault(x => x.Id == value.Id) is { } item)
            {
                item = value;
                return Task.CompletedTask;
            }
            else
            {
                throw new InvalidOperationException("Item not found");
            }
        }

        public Task DeleteAsync(T value)
        {
            _data.RemoveAll(x => x.Id == value.Id);
            return Task.CompletedTask;
        }
    }
}