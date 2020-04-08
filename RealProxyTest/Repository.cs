using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealProxyTest
{
    [Log]
    public class Repository<T> : ContextBoundObject, IRepository<T>
    {
        public Repository()
        {
            Console.WriteLine("Constructed");
        }

        public void Add(T entity)
        {
            Console.WriteLine("Adding {0}", entity);
        }
        public void Delete(T entity)
        {
            Console.WriteLine("Deleting {0}", entity);
        }
        public void Update(T entity)
        {
            Console.WriteLine("Updating {0}", entity);
        }
        public IEnumerable<T> GetAll()
        {
            Console.WriteLine("Getting entities");
            return null;
        }
        public T GetById(int id)
        {
            Console.WriteLine("Getting entity {0}", id);
            return default(T);
        }
    }
}
