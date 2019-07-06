using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.DATA
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly EFEntities dataContext;
        public string Id { get; set; }
        public DatabaseFactory()
        {
            dataContext = new EFEntities();

            // Get randomize Id
            var random = new Random(DateTime.Now.Millisecond);
            Id = random.Next(1000000).ToString();
        }

        public EFEntities GetDbContext()
        {
            return dataContext;
        }
    }
}
