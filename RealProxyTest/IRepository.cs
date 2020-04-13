using System.Collections.Generic;

namespace RealProxyTest
{
    public interface IRepository
    {
        void Add(Customer entity, out int test, string test2 = "test2");
        void Delete(Customer entity);
        void Update(Customer entity);
        IEnumerable<Customer> GetAll();
        Customer GetById(int id);
    }
}