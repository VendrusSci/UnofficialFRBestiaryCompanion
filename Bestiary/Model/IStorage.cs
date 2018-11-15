using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    interface IStorage<Q>
    {
        Q Fetch(int id);

        void Update(int id, Q q);

        void Delete(int id);

        Q[] FetchAll();
    }
}
