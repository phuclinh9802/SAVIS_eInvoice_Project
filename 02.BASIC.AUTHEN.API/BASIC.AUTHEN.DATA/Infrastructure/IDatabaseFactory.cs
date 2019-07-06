using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.DATA
{
    public interface IDatabaseFactory 
    {
        EFEntities GetDbContext();
    }
}
