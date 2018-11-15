using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    class OwnedFamiliarStore : IStorage<OwnedFamiliar>
    {
        OwnedFamiliar[] OwnedFamiliars;

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public OwnedFamiliar Fetch(int id)
        {
            throw new NotImplementedException();
        }

        public OwnedFamiliar[] FetchAll()
        {
            throw new NotImplementedException();
        }

        public void Update(int id, OwnedFamiliar q)
        {
            throw new NotImplementedException();
        }
    }
}
