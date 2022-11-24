﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.FileService
{
    public class FileService:IFileService
    {
        public async Task<string> UploadAsync(IFormFile file, string webRootPath)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(webRootPath, "assets/img", fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }

        public void Delete(string fileName, string webRootPath)
        {
            var path = Path.Combine(webRootPath, "assets/img", fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image/"))
            {
                return true;
            }

            return false;
        }

        public bool CheckSize(IFormFile file, int size)
        {
            if (file.Length / 1024 > size)
            {
                return false;
            }
            return true;
        }
    }
}
