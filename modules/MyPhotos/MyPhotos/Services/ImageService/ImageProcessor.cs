using MyPhotos.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Consts;
using System.IO;
using System.Windows.Media.Imaging;

namespace MyPhotos.Services.ImageService
{
    public class ImageProcessor
    {
        public const string DEFAULT_OWNER = "sys_admin";
        public const string OWNER = "owner";
        public const string DISPLAY_NAME = "display_name";
        public const string DESCRIPTION = "description";
        public const string FILE_HASH = "file_hash";
        public const string USERS = "users";
        public const string CHANGESET_NO = "changeset_no";
        public const string FILE_PATHS = "file_paths";
        public const string FILE_HASHS = "file_hashs";
        public const string IMAGES = "images";
        public const string TAGS = "tags";
        public const string KEY = "key";
        public const string VALUE = "value";
        public const string METADATA = "metadata";
        public const string IMAGE_S_BASE64 = "image_s_base64";
        public const string IMAGE_M_BASE64 = "image_m_base64";
        public const string IMAGE_L_BASE64 = "image_l_base64";
        public const string IMAGE_KEY_VALUE_BUCKET = "my_photo_bucket";

        public const string IMAGE_ROTATE= "rotate";
        
        private const string IMAGE_S_URL = "image_s_url";
        private const string IMAGE_M_URL = "image_m_url";
        private const string IMAGE_L_URL = "image_l_url";

        public const string IMAGE_S_SIZE = "image_s_size";
        public const string IMAGE_M_SIZE = "image_m_size";
        public const string IMAGE_L_SIZE = "image_l_size";
        public const string COUNT = "count";
        public const string WIDTH = "width";
        public const string HEIGHT = "height";

        public const string MYPHOTO_COLLECTION = "my_photo";
        public const string MYPHOTO_GALLERY_COLLECTION = "my_photo_gallery";
        public const string MYPHOTO_DIR_SCAN_COLLECTION = "my_photo_dir_scan";
        public const string MYPHOTO_IMAGE_VIEW_COLLECTION = "my_photo_image_view";
        public const string MYPHOTO_IMAGE_LIKE_COLLECTION = "my_photo_image_like";
        public const string MYPHOTO_IMAGE_BOOKMARK_COLLECTION = "my_photo_user_bookmark";

        public const string PHOTO_DATE_TAKEN = "date_taken";
        public const string AUTH_USERS = "auth_users";
        public const string DEFAULT_USER = "sys_admin";
        public const string PHOTO_DATE_TAKEN_TIME_STAMP = "date_taken_timestamp";
        public const string GALLERY_THUMBNAIL = "thumbnail";
        public const string GALLERY_THUMBNAIL_IMAGE = "thumbnail_image";
        
        public const string GALLERY_ID = "galleryid";
        
        public const string FILES_COUNT = "files_count";
        public const string VIEWS_COUNT = "views_count";
        public const string LIKES_COUNT = "likes_count";
        public const string IS_LIKED= "is_liked";
        public const string IS_BOOKMARKED = "is_bookmarked";
        public const string PATH = "path";
        public const string RELATED_FILES = "related_files";

        public static string GetFileKey(string type, string fileHash)
        {
            return string.Format("{0}-{1}", type, fileHash);
        }
        private List<FileModel> _existingFiles = new List<FileModel>();

        public void Scan(string baseFolderPath, string folderPath, IDBService dbProxy, IKeyValueStorage keyValueStorage, Func<string, bool> logger)
        {

            DirectoryInfo di = new DirectoryInfo(string.Format("{0}{1}", baseFolderPath,folderPath));
            if (!di.Exists) throw new FileNotFoundException(string.Format("folder not found : {0}{1}", baseFolderPath, folderPath));
            ImageFinder ifm = new ImageFinder();

            var existingFiles = GetDBFiles(dbProxy);
            List<FileModel> previousfiles = new List<FileModel>();
            List<FileModel> previousCopy = new List<FileModel>();
            foreach (var item in existingFiles)
            {
                previousfiles.Add((FileModel)item.Clone());
                previousCopy.Add((FileModel)item.Clone());
            }
            logger(string.Format("Scanning files..{0}", baseFolderPath));
             ifm.GetFiles(baseFolderPath, folderPath, dbProxy, previousfiles, (FileModel file) => {
                if (file.IsUpdated)
                {
                    UpdateFile(file, dbProxy, logger);
                   
                }
                else
                {
                    AddFile(file, baseFolderPath, dbProxy,keyValueStorage, logger);
                   
                }
                return true;
            },logger);

            logger(string.Format("Deleting file ..Count {0}", previousfiles.Count));
            // DeleteFile(previousfiles, dbProxy);

            logger("Completed");

        }
        private void AddGallery(JObject fileData, IDBService dbProxy)
        {
            var fileHash = fileData[FILE_HASH].ToString();
            foreach (var item in fileData[TAGS])
            {
                
                JObject filter = new JObject();
                filter[CommonConst.CommonField.NAME] = item.ToString();
                var gallery = dbProxy.FirstOrDefault(MYPHOTO_GALLERY_COLLECTION, filter.ToString());
                if (gallery == null)
                {
                    gallery = new JObject();
                    gallery[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                    gallery[DISPLAY_NAME] = gallery[CommonConst.CommonField.NAME] = item;
                    gallery[DESCRIPTION] = "";
                    gallery[OWNER] = DEFAULT_OWNER;
                    ImageGalleryHelper.AddDefaultAuthUser(gallery);
                    gallery[GALLERY_THUMBNAIL] = fileHash;
                    gallery[FILE_HASHS] = new JArray();
                    (gallery[FILE_HASHS] as JArray).Add(fileHash);
                    gallery[FILES_COUNT] = (gallery[FILE_HASHS] as JArray).Count;
                    dbProxy.Write(MYPHOTO_GALLERY_COLLECTION, gallery);
                }
                else
                {
                    if ((gallery[FILE_HASHS] as JArray).FirstOrDefault(f => f.ToString() == fileHash) == null)
                    {
                        (gallery[FILE_HASHS] as JArray).Add(fileHash);
                        gallery[FILES_COUNT] = (gallery[FILE_HASHS] as JArray).Count;
                        dbProxy.Update(MYPHOTO_GALLERY_COLLECTION, filter.ToString(), gallery, false, MergeArrayHandling.Replace);

                    }
                }
            }
        }


       

        private void DeleteFile(List<FileModel> files,IDBService dbProxy)
        {
            foreach (var item in files)
            {
                JObject filter = new JObject();
                filter[FILE_HASH] = item.file_hash;
                dbProxy.Delete(MYPHOTO_COLLECTION, filter.ToString());
            }
        }

        public JArray GetFiles(IDBService dbProxy)
        {
            return dbProxy.Get(MYPHOTO_COLLECTION, "{}", new List<string> { FILE_HASH, FILE_PATHS });
        }

        private List<FileModel> GetDBFiles(IDBService dbProxy)
        {
            _existingFiles = new List<FileModel>();
            foreach (var item in GetFiles(dbProxy))
            {
                _existingFiles.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<FileModel>(item.ToString()));
            }
            return _existingFiles;
        }

        private JObject AddFile(FileModel fileModel, string baseFolderPath, IDBService dbProxy, IKeyValueStorage keyValueStorage, Func<string, bool> logger)
        {
            if (fileModel.IsAdded)
            {
                JObject fileData = new JObject();
                var path = string.Concat(baseFolderPath, "\\", fileModel.file_paths.First());

                using (var image = ImageGalleryHelper.GetImageBitmapFromFile(path))
                {
                    fileData = ImageGalleryHelper.CreateFileDataJObject(fileModel, path, image, keyValueStorage);
                }

                if (!dbProxy.Write(MYPHOTO_COLLECTION, fileData))
                {
                    throw new Exception("Unable to add data to db");
                }

                Console.WriteLine("Added file" + fileData[FILE_PATHS].ToString());
                fileData.Remove(IMAGE_S_BASE64);
                fileData.Remove(IMAGE_M_BASE64);
                fileData.Remove(IMAGE_L_BASE64);
                _existingFiles.Add(fileModel);
                AddGallery(fileData, dbProxy);
                return fileData;
            }
            else
            {
                return null;
            }
        }

        

      
        private void UpdateFile(FileModel fileModel,IDBService dbProxy, Func<string, bool> logger)
        {
            if (fileModel.IsUpdated)
            {
                JObject filter = new JObject();
                filter[FILE_HASH] = fileModel.file_hash;
                var fileData = new JObject();
                fileData[FILE_HASH] = fileModel.file_hash;
                fileData[FILE_PATHS] = new JArray();
                foreach (var item in fileModel.file_paths)
                {
                    (fileData[FILE_PATHS] as JArray).Add(item);
                }
                ImageGalleryHelper.AddPath(fileData, fileModel);
                ImageGalleryHelper.AddTags(fileData, fileModel);

                dbProxy.Update(MYPHOTO_COLLECTION, filter.ToString(), fileData, false, MergeArrayHandling.Union);
                AddGallery(fileData, dbProxy);
            }
        }
      
    }
}
