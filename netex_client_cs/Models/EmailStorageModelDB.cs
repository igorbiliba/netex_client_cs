using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Models
{
    public class EmailStorageModelDB
    {
        const string KEY = "email";
        public StorageModelDB storageDB;

        public string Get()
            => storageDB.Get(KEY);

        public void Update(string val)
            => storageDB.Set(KEY, val);
    }
}
