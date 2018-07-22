using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotos.Model
{
    public class FileModel : ICloneable
    {
        public string file_hash { get; set; }
        public List<string> file_paths { get; set; }
        public bool IsUpdated = false;
        public bool IsDeleted = false;
        public bool IsAdded = false;
        public FileModel()
        {
            file_paths = new List<string>();
        }


        public object Clone()
        {
            FileModel data = new FileModel();
            data.file_hash = this.file_hash;
            data.IsDeleted = this.IsDeleted;
            data.IsAdded = this.IsAdded;
            data.IsUpdated = this.IsUpdated;
            data.file_paths = new List<string>();
            foreach (var item in file_paths)
            {
                data.file_paths.Add(item);
            }
            return data;

        }
    }
}
