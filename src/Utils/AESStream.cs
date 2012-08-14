using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace OpenMC.Utils
{
    public class AesStream : Stream
    {
        CryptoStream Encrypt;
        CryptoStream Decrypt;
	RSACryptoServiceProvider RSACrypt;

        public AesStream(Stream stream, byte[] key) {
            BaseStream = stream;
            Encrypt = new CryptoStream(stream, GenerateAES(key).CreateEncryptor(), CryptoStreamMode.Write);
            Decrypt = new CryptoStream(stream, GenerateAES(key).CreateDecryptor(), CryptoStreamMode.Read);
        }
        public Stream BaseStream { get; set; }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush() { BaseStream.Flush(); }

        public override long Length { get { throw new NotSupportedException(); } }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int ReadByte() { return Decrypt.ReadByte(); }

        public override int Read(byte[] buffer, int offset, int count)
      	{
            return Decrypt.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        public override void SetLength(long value){ throw new NotSupportedException(); }

        public override void WriteByte(byte b) { Encrypt.WriteByte(b); }

        public override void Write(byte[] buffer, int offset, int count) { Encrypt.Write(buffer, offset, count);}

        private RijndaelManaged GenerateAES(byte[] key)
        {
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.Mode = CipherMode.CFB;
            cipher.Padding = PaddingMode.None;
            cipher.KeySize = 128;
            cipher.FeedbackSize = 8;
            cipher.Key = key;
            cipher.IV = key;
            
            return cipher;
        }
    }
}
