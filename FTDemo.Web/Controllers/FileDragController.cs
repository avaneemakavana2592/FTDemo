using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FTDemo.Common;
using FTDemo.Common.Models;
using FTDemo.Database.Interfaces;

namespace FTDemo.Web.Controllers
{
    public class FileDragController : Controller
    {
        private static IFileUpload _fileUpload;
        private static IWebHostEnvironment _environment;

        #region Constractor
        public FileDragController(IFileUpload fileUpload, IWebHostEnvironment webHostEnvironment)
        {
            _fileUpload = fileUpload;
            _environment = webHostEnvironment;
        }
        #endregion

        public IActionResult Index()
        {
            List<FileUploadModel> list = _fileUpload.GetAllFileUpload();
            return View(list);
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            FileUploadModel fileUploadModel = new FileUploadModel();
            fileUploadModel.File = file;
            fileUploadModel.WebHostEnvironmentPath = _environment.WebRootPath;
            _fileUpload.AddUpdateBranch(fileUploadModel);
            return RedirectToAction("Index");
        }

        public void StoreDownloadFile(string filename)
        {
            var decuploads = Path.Combine(_environment.WebRootPath, "uploads\\decrypted");
            var encuploads = Path.Combine(_environment.WebRootPath, "uploads\\encrypted");
            if (!Directory.Exists(decuploads))
            {
                Directory.CreateDirectory(decuploads);
            }
            string fileName = filename;
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
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Mode = CipherMode.CBC;


            try
            {
                using (FileStream fsCrypt = new FileStream(Path.Combine(encuploads, fileName), FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fsCrypt,
                        RMCrypto.CreateDecryptor(keyBytes, initVectorBytes), CryptoStreamMode.Read))
                    {
                        string outputFile = Path.Combine(decuploads, fileName);
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        {
                            int decodedata;
                            while ((decodedata = cs.ReadByte()) != -1)
                                fsOut.WriteByte((byte)decodedata);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Encryption failed!", "Error");
            }

        }
        public async Task<FileStreamResult> DownloadFile(string filename)
        {
            var decuploads = Path.Combine(_environment.WebRootPath, "uploads\\decrypted");
            var encuploads = Path.Combine(_environment.WebRootPath, "uploads\\encrypted");
            if (!Directory.Exists(decuploads))
            {
                Directory.CreateDirectory(decuploads);
            }
            string fileName = filename;
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
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Mode = CipherMode.CBC;
            try
            {
                var memory = new MemoryStream();
                using (FileStream fsCrypt = new FileStream(Path.Combine(encuploads, fileName), FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fsCrypt,
                        RMCrypto.CreateDecryptor(keyBytes, initVectorBytes), CryptoStreamMode.Read))
                    {
                        await cs.CopyToAsync(memory);
                    }
                }
                string contentType = GlobalCode.GetContentType(Path.Combine(encuploads, fileName));
                memory.Position = 0;
                return File(memory, contentType, fileName);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Encryption failed!", "Error");
            }
            return null;
        }
    }
}