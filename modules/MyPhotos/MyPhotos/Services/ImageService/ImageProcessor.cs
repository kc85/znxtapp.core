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
        public const string FILE_HASH = "file_hash";
        public const string USERS = "users";
        public const string CHANGESET_NO = "changeset_no";
        public const string FILE_PATHS = "file_paths";
        public const string FILE_HASHS = "file_hashs";
        public const string IMAGES = "images";
        public const string TAGS = "tags";
        private const string KEY = "key";
        private const string VALUE = "value";
        public const string METADATA = "metadata";
        public const string IMAGE_S_BASE64 = "image_s_base64";
        public const string IMAGE_M_BASE64 = "image_m_base64";
        public const string IMAGE_L_BASE64 = "image_l_base64";
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
        public const string DEFAULT_USER = "user";
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

        private List<FileModel> _existingFiles = new List<FileModel>();

        public void Scan(string baseFolderPath, IDBService dbProxy, Func<string, bool> logger)
        {
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
             ifm.GetFiles(baseFolderPath,dbProxy, previousfiles, (FileModel file) => {
                if (file.IsUpdated)
                {
                    UpdateFile(file, dbProxy);
                   
                }
                else
                {
                    AddFile(file, baseFolderPath, dbProxy);
                   
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
                    gallery[CommonConst.CommonField.NAME] = item;
                    gallery[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                    AddDefaultAuthUser(gallery);
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


        private void AddMetaData(Image image, JObject fileObj)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            if (fileObj[METADATA] == null)
            {
                fileObj[METADATA] = new JArray();
            }
            foreach (var propItem in image.PropertyItems)
            {
                JObject data = new JObject();
                data[KEY] = propItem.Type.ToString();
                data[VALUE] = encoding.GetString(propItem.Value);
                (fileObj[METADATA] as JArray).Add(data);
            }
        }

        private void AddPath(JObject fileObj, FileModel fileModel)
        {
            fileObj[FILE_PATHS] = new JArray();
            foreach (var path in fileModel.file_paths)
            {
                (fileObj[FILE_PATHS] as JArray).Add(path);
            }
        }

        private void AddTags(JObject fileObj, FileModel fileModel)
        {
            fileObj[TAGS] = new JArray();
            foreach (var path in fileModel.file_paths)
            {
                var splitData = path.Split('\\');
                for (int count = 0; count < splitData.Length-1; count++ )
                {
                    if (!string.IsNullOrEmpty(splitData[count]))
                    {
                        (fileObj[TAGS] as JArray).Add(splitData[count]);
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

        private JObject AddFile(FileModel fileModel, string baseFolderPath, IDBService dbProxy)
        {
            if (fileModel.IsAdded)
            {
                JObject fileData = new JObject();
                var path = string.Concat(baseFolderPath, "\\", fileModel.file_paths.First());

                using (var image = ImageGalleryHelper.GetImageBitmapFromFile(path))
                    {
                        
                        fileData[FILE_HASH] = fileModel.file_hash;
                        ImageGalleryHelper.ProcessImage(fileData, image);
                        fileData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                        AddPath(fileData, fileModel);
                        AddTags(fileData, fileModel);
                        AddDefaultAuthUser(fileData);
                        AddMetaData(image, fileData);
                        GetDateTaken(fileData, path);
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

        private void AddDefaultAuthUser(JObject fileData)
        {
            fileData[AUTH_USERS] = new JArray();
            (fileData[AUTH_USERS] as JArray).Add(DEFAULT_USER);
        }

        private string GetDateTaken(JObject fileObj, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource img = BitmapFrame.Create(fs);
                BitmapMetadata md = (BitmapMetadata)img.Metadata;
                string date = md.DateTaken;
                fileObj[PHOTO_DATE_TAKEN] = date;
                DateTime dt = new DateTime();
                if (DateTime.TryParse(date, out dt))
                {
                    fileObj[PHOTO_DATE_TAKEN_TIME_STAMP] = CommonUtility.GetUnixTimestamp(dt);
                }

                JObject data = new JObject();
                data[KEY] = "CameraManufacturer";
                data[VALUE] = md.CameraManufacturer;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "CameraModel";
                data[VALUE] = md.CameraModel;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "Comment";
                data[VALUE] = md.Comment;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "Copyright";
                data[VALUE] = md.Copyright;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "Format";
                data[VALUE] = md.Format;
                (fileObj[METADATA] as JArray).Add(data);
                
                if (md.Keywords != null)
                {
                    data = new JObject();
                    data[KEY] = "Keywords";
                    data[VALUE] = string.Join(",", md.Keywords);
                    (fileObj[METADATA] as JArray).Add(data);                    
                }

                data = new JObject();
                data[KEY] = "Location";
                data[VALUE] = md.Location;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "Rating";
                data[VALUE] = md.Rating;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "Subject";
                data[VALUE] = md.Subject;
                (fileObj[METADATA] as JArray).Add(data);

                data = new JObject();
                data[KEY] = "Title";
                data[VALUE] = md.Title;
                (fileObj[METADATA] as JArray).Add(data);

                return date;
            }
        }

        private void UpdateFile(FileModel fileModel,IDBService dbProxy)
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
                AddPath(fileData, fileModel);
                AddTags(fileData, fileModel);

                dbProxy.Update(MYPHOTO_COLLECTION, filter.ToString(), fileData, false, MergeArrayHandling.Union);
                AddGallery(fileData, dbProxy);
            }
        }
      
    }
}
