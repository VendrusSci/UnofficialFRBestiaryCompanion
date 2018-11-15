using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    class KnownFamiliarStore : IStorage<Familiar>
    {
        Familiar[] familiars;

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Familiar Fetch(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, Familiar q)
        {
            throw new NotImplementedException();
        }

        public Familiar[] FetchAll()
        {
            throw new NotImplementedException();
        }
    }
}
