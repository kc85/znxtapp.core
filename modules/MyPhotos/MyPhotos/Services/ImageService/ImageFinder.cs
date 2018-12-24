using MyPhotos.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Consts;

namespace MyPhotos.Services.ImageService
{
    public class ImageFinder
    {
        public static string []  ImageTypes=  null;
        public ImageFinder()
        {
            ImageTypes = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
        }

        private JObject GetFileData(string fileHash, IDBService dbProxy)
        {
            JObject filter = new JObject();
            filter[ImageProcessor.FILE_HASH] = fileHash;
            return dbProxy.FirstOrDefault(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString());
        }
        public  void GetFiles(string path,string folderPath, IDBService dbProxy, List<FileModel> previousfiles, Func<FileModel,bool> onFileAddUpdate,Func<string,bool> logger)
        {
            path = path.ToLower();
            var folderScanPath = string.Format("{0}{1}", path.ToLower(), folderPath.ToLower());
            List<string> dirs = new List<string>();
            dirs.Add(folderScanPath);
            dirs.AddRange(System.IO.Directory.GetDirectories(folderScanPath, "*", SearchOption.AllDirectories));

            logger(string.Format("Scan directories  : {0}. Total Dirs Found : {1}", folderScanPath, dirs.Count));

            foreach (var item in dirs)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(item);

                    JObject dir = new JObject();
                    dir[ImageProcessor.PATH] = di.FullName;

                    

                    List<string> files = new List<string>();

                    foreach (var filter in ImageTypes)
                    {
                        files.AddRange(Directory.GetFiles(item, String.Format("*.{0}", filter), SearchOption.TopDirectoryOnly));
                    }
                    logger(string.Format("Scanning files for directory : {0}. Total Files Found : {1}", item, files.Count));

                    var dirDBObject = dbProxy.FirstOrDefault(ImageProcessor.MYPHOTO_DIR_SCAN_COLLECTION, dir.ToString());
                    if (dirDBObject != null)
                    {
                        if (dir[ImageProcessor.COUNT]!=null && int.Parse(dirDBObject[ImageProcessor.COUNT].ToString()) == files.Count)
                        {
                            logger(string.Format("Scan Skipped... Count of files same for directory : {0}. Total Files Found : {1}", item, files.Count));
                            continue;
                        }
                    }
                  
                    long count = 0;
                    foreach (var filePath in files)
                    {
                        try
                        {
                            var fileHash = Hashing.GetFileHash(filePath);
                            logger(string.Format("Scaning file : FileHash : {0}, {1}", fileHash, filePath));

                            var absPath = filePath.ToLower().Replace(path, "");
                            var file = previousfiles.FirstOrDefault(f => f.file_hash == fileHash);
                            var dbFileData = GetFileData(fileHash, dbProxy);
                            if (file != null)
                            {
                                if (file.file_paths.IndexOf(absPath) == -1)
                                {
                                    file.file_paths.Add(absPath);
                                    file.IsUpdated = true;
                                    onFileAddUpdate(file);
                                    previousfiles.Remove(file);
                                }
                            }
                            else if (dbFileData != null)
                            {
                                file = new FileModel();
                                file.file_hash = fileHash;
                                file.IsUpdated = true;
                                file.file_paths.Add(absPath);
                                onFileAddUpdate(file);
                            }
                            else
                            {
                                var newfile = new FileModel() { file_hash = fileHash, IsAdded = true };
                                newfile.file_paths.Add(absPath);
                                onFileAddUpdate(newfile);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger(string.Format("Error in file {0}, Error {1} : {2}", filePath, ex.Message, ex.StackTrace));
                        }

                        count++;
                    }

                    if (dirDBObject == null)
                    {
                        dir[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                        dir[ImageProcessor.COUNT] = files.Count;
                        dbProxy.Write(ImageProcessor.MYPHOTO_DIR_SCAN_COLLECTION, dir);
                    }
                    else
                    {
                        dir[CommonConst.CommonField.DISPLAY_ID] = dirDBObject[CommonConst.CommonField.DISPLAY_ID];
                        dir[ImageProcessor.COUNT] = files.Count;
                        JObject filter = new JObject();
                        filter[CommonConst.CommonField.DISPLAY_ID] = dirDBObject[CommonConst.CommonField.DISPLAY_ID];
                        dbProxy.Update(ImageProcessor.MYPHOTO_DIR_SCAN_COLLECTION, filter.ToString(), dir, false, MergeArrayHandling.Replace);
                    }

                }
                catch (Exception ex)
                {
                    logger(string.Format("Error in Folder {0}, Error {1}", item, ex.Message));
                }

            }
        }
    }
}
