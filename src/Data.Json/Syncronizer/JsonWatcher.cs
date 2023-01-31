using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Json.Syncronizer
{
    internal class JsonWatcher:FileSystemWatcher
    {
        public ReaderWriterLock ReaderWriterLock { get; private set; }
        = new ReaderWriterLock();
        public JsonWatcher(string path,DataSet dataSet,bool isDirectory)
            :base()
        {
           
            if (isDirectory)
            {
                base.Path = path;
                foreach (DataTable item in dataSet.Tables)
                {
                    base.Filters.Add($"{item.TableName}.json");
                }
            }
            else
            {
                base.Path = path;
                base.Filter=System.IO.Path.GetFileName(path);
            }
            base.Changed += JsonWatcher_Changed;
        }

        private void JsonWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
