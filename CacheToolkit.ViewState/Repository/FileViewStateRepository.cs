using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheToolkit.ViewState.Model;
using System.IO;

namespace CacheToolkit.ViewState.Repository
{
    public class FileViewStateRepository : IViewStateRepository
    {
        string BasePath;
        public FileViewStateRepository(string basePath)
        {
            this.BasePath = basePath;
        }
        public ViewStateInfo GetViewState(string id)
        {
            ViewStateInfo info;
            string path = BasePath + Path.DirectorySeparatorChar + id + ".dat";
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream))
            {
                info = new ViewStateInfo(reader.ReadToEnd())
                {
                    Id = id,
                    CreateTime = DateTimeOffset.Now,
                    LastVisitTime = DateTimeOffset.Now,
                };
                reader.Close();
                stream.Close();
            }
            return info;
        }

        public string SaveViewState(ViewStateInfo info)
        {
            string path = BasePath + Path.DirectorySeparatorChar + info.Id + ".dat";
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(info.Value);
                writer.Close();
                stream.Close();
            }
            return info.Id;
        }
    }
}
