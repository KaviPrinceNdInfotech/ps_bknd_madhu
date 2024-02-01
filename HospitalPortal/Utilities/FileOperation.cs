using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HospitalPortal.Utilities
{
    public static class FileOperation
    {
        public static string UploadImage(HttpPostedFileBase File, string folderName)
        {
            var allowedExtensions = new[] { ".jpeg", ".jpg", ".png",  ".pdf" };
            return UploadFile(File, folderName, allowedExtensions);
        }

        public static bool CheckAllowedFiles(HttpPostedFileBase[] files,string[] allowedExts)
        {
            
            foreach(var file in files)
            {
                var filename = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(filename);
                if(!allowedExts.Contains(ext))
                {
                    return false; 
                }
            }
            return true;
        }

        public static string UploadFile(HttpPostedFileBase File, string folderName, string[] allowedExtensions)
        {
            DateTime dt = DateTime.Now;
            //var allowedExtensions = new[] { ".pdf" };

            string savedFileName = Guid.NewGuid().ToString() + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + dt.Millisecond + dt.Second;

            string ImageName = Path.GetFileName(File.FileName);


            string ext = Path.GetExtension(ImageName);

            string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/" + folderName + "/" + savedFileName + ext);

            if (!allowedExtensions.Contains(ext))
            {
                return "not allowed";
            }
            // save image in folder
            File.SaveAs(physicalPath);
            return savedFileName + ext;
        }

        public static bool DeleteFile(string fileNameWithPath)
        {

            System.IO.FileInfo file = new System.IO.FileInfo(fileNameWithPath);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            return false;
        }


        public static string UploadFileWithBase64(string folderName, string imageName, string base64Image, string[] extensions)
           {
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            string[] arr = imageName.Split('.');
            string ext = "." + arr[1];
            if (!extensions.Contains(ext))
                return "not allowed";
            string fileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + Guid.NewGuid().ToString();
            var filePath = HttpContext.Current.Server.MapPath("~/" + folderName);
            File.WriteAllBytes(filePath + "/" + fileName + ext, imageBytes);
            return fileName + ext;
        }


    }
}