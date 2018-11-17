using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    public interface ICRUD<Q>
    {
        Q Fetch();

        void Update(Action<Q> update);

        void Delete();
    }
}
