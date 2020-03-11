using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Models
{
    public class ProxyStorageModelMixed
    {
        string registerKeyPrefix;
        string dbKeyPrefix;
        string key;

        public ProxyStorageModelMixed(string registerKeyPrefix, string dbKeyPrefix, string key)
        {
            this.registerKeyPrefix = registerKeyPrefix;
            this.dbKeyPrefix       = dbKeyPrefix;
            this.key               = key;
        }

        public StorageModelDB  storageDB;
        public StorageModelREG storageREG;

        public string Get()
        {
            string val = storageREG.Get(registerKeyPrefix + key);            
            if (val != "") return val;

            return storageDB.Get(dbKeyPrefix + key);
        }

        public void Update(string val)
        {
            storageREG.Set(registerKeyPrefix + key, val);
            storageDB.Set(dbKeyPrefix + key, val);
        }
    }
}
