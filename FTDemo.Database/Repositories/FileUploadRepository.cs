using FTDemo.Database.Data;
using System;
using System.Collections.Generic;
using System.Text;
using FTDemo.Database.Interfaces;
using FTDemo.Database.Entities;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using FTDemo.Common;
using FTDemo.Common.Models;

namespace FTDemo.Database.Repositories
{
    public class FileUploadRepository : IFileUpload
    {
        #region Member Declaration
        private static SkillTestContext _context;

        #endregion

        #region Constractor
        public FileUploadRepository(SkillTestContext context)
        {
            _context = context;
        }
        #endregion

        #region Add Update Branch
        public bool AddUpdateBranch(FileUploadModel fileUploadModel)
        {
            tblFileUpload tblFileUpload = new tblFileUpload();

            if (fileUploadModel.File != null)
            {
                tblFileUpload.FileName = SaveFile(fileUploadModel);
            }

            if (!string.IsNullOrEmpty(tblFileUpload.FileName))
            {
                tblFileUpload.CreatedDate = DateTime.Now;
                _context.tblFileUpload.Add(tblFileUpload);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

      
        #endregion

        #region Save Branch Image
        public string SaveFile(FileUploadModel fileUploadModel)
        {
            if (fileUploadModel.File != null)
            {
                var encuploads = Path.Combine(fileUploadModel.WebHostEnvironmentPath, "uploads\\encrypted");
                if (!Directory.Exists(encuploads))
                {
                    Directory.CreateDirectory(encuploads);
                }

                string GUID = Guid.NewGuid().ToString();
                string fileName = fileUploadModel.File.FileName;
                string fileExtension = Path.GetExtension(fileName);
                fileName = GUID + fileExtension;

                int keySize = Constants.EncryptionKeys.keySize;
                string hashAlgorithm = Constants.EncryptionKeys.hashAlgorithm;
                string passPhrase = Constants.EncryptionKeys.passPhrase;
                string saltValue = Constants.EncryptionKeys.saltValue;
                string initVector = Constants.EncryptionKeys.initVector;

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);


                var _password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, 2);

                byte[] keyBytes = _password.GetBytes(keySize / 8);
                UnicodeEncoding UE = new UnicodeEncoding();
                //byte[] key = UE.GetBytes(password);
                string cryptFile = Path.Combine(encuploads, fileName);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.Mode = CipherMode.CBC;
                try
                {
                    using (FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create))
                    {
                        using (CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(keyBytes, initVectorBytes), CryptoStreamMode.Write))
                        {
                            using (Stream fsIn = fileUploadModel.File.OpenReadStream())
                            {
                                int data;
                                while ((data = fsIn.ReadByte()) != -1)
                                    cs.WriteByte((byte)data);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Encryption failed!", "Error");
                }
                return fileName;
            }
            return string.Empty;
        }
        #endregion

        public FileUploadModel GetFileUploadById(int Id)
        {
            var fileUpload = (from f in _context.tblFileUpload
                              where f.FileId == Id
                              select new FileUploadModel
                              {
                                  FileId = f.FileId,
                                  FileName = f.FileName,
                                  CreatedDate = f.CreatedDate
                              }).FirstOrDefault();
            return fileUpload;
        }

        public List<FileUploadModel> GetAllFileUpload()
        {
            var _list = (from f in _context.tblFileUpload
                              orderby f.CreatedDate
                              select new FileUploadModel
                              {
                                  FileId = f.FileId,
                                  FileName = f.FileName,
                                  CreatedDate = f.CreatedDate
                              }).ToList();
            return _list;
        }
    }
}
