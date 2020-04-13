using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealProxyTest
{
    [Log]
    public class Repository : ContextBoundObject, IRepository
    {
        public Repository()
        {
            Console.WriteLine("Constructed");
        }

        public void Add(Customer entity, out int test, string test2 = "test2")
        {
            Console.WriteLine("Adding {0}", entity);
            test = 10;
        }
        public void Delete(Customer entity)
        {
            Console.WriteLine("Deleting {0}", entity);
        }
        public void Update(Customer entity)
        {
            Console.WriteLine("Updating {0}", entity);
        }
        public IEnumerable<Customer> GetAll()
        {
            Console.WriteLine("Getting entities");
            return null;
        }
        public Customer GetById(int id)
        {
            Console.WriteLine("Getting entity {0}", id);
            return default(Customer);
        }
    }
}
