using MyPhotos.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotos.Services.ImageService
{
    public class ImageFinder
    {
        public  List<FileModel> GetFiles(string path, List<FileModel> previousfiles, Func<FileModel,bool> onFileAddUpdate,Func<string,bool> logger)
        {
           
            path = path.ToLower();
            List<FileModel> addedFiles = new List<FileModel>();
            string [] dirs =  System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            logger(string.Format("Scan directories  : {0}. Total Dirs Found : {1}", path, dirs.Length));
                
            foreach (var item in dirs)
            {
                
                List<string> files = new List<string>();
                var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                foreach (var filter in filters)
                {
                    files.AddRange(Directory.GetFiles(item, String.Format("*.{0}", filter), SearchOption.TopDirectoryOnly));
                }
                logger(string.Format("Scanning files for directory : {0}. Total Files Found : {1}", item, files.Count));
                long count = 0;
                foreach (var filePath in files)
                {
                    var fileHash = Hashing.GetFileHash(filePath);
                    var absPath = filePath.ToLower().Replace(path, "");
                    var file = previousfiles.FirstOrDefault(f => f.file_hash == fileHash);
                    if (file != null)
                    {
                        if (file.file_paths.IndexOf(absPath) != -1)
                        {
                            file.file_paths.Add(absPath);
                            file.IsUpdated = true;
                            addedFiles.Add(file);
                            onFileAddUpdate(file);
                            previousfiles.Remove(file);
                        }
                    }
                    else if (addedFiles.FirstOrDefault(f => f.file_hash == fileHash) != null)
                    {
                        var fileUpdate = addedFiles.FirstOrDefault(f => f.file_hash == fileHash);
                        fileUpdate.file_paths.Add(absPath);
                        onFileAddUpdate(fileUpdate);
                    }
                    else
                    {
                        var newfile = new FileModel() { file_hash = fileHash, IsAdded = true };
                        newfile.file_paths.Add(absPath);
                        addedFiles.Add(newfile);
                        onFileAddUpdate(newfile);
                    }
                    count++;
                   // logger(string.Format("File Scan Path: {0} {1} of {2}", item, count, files.Count));

                }
                
            }
            return addedFiles;
        }
    }
}
