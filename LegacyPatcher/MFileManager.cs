using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace LegacyPatcher
{
    public enum MFileType
    {
        FILETYPE_LEGACY_CLIENT_PATCH  = 1
    }
    public class MFileManager
    {
        #region FILE_CONSTANT
        private const string TEMP_FILE = "patchTemp.m";
        private const string ZIP_TEMP_FILE = "patchTempZip.m";
        private const string UNZIP_TEMP_FILE = "patchTempUnzip.m";
        private const string UNZIPPED_TEMP_FILE = "patchTemplateUnzipped.m";
        private const string TEMP_DATA_FILE = "patchTempData.m";

        public const int M_HEADER_SIZE = 2;
        public const int M_HEADER_OFFSET = 0;
        public const int M_FILETABLE_SIZE = sizeof(int);
        public const int M_FILETABLE_OFFSET = M_HEADER_OFFSET + M_HEADER_SIZE;
        public const int M_BUILD_SIZE = sizeof(int);
        public const int M_BUILD_OFFSET = M_FILETABLE_OFFSET + M_FILETABLE_SIZE;
        public const int M_DATA_LENGTH_SIZE = sizeof(int);
        public const int M_DATA_LENGTH_OFFSET = M_BUILD_OFFSET + M_BUILD_SIZE;
        public const int M_DATA_OFFSET = M_DATA_LENGTH_OFFSET + M_DATA_LENGTH_SIZE;
        #endregion
        public static void Test()
        {
        }
        public static void PackPatch(MFileType type, List<PatchFileInfo> files, uint gamebuild, string root, string patch)
        {
            byte[] emptyByte = new byte[1];
            emptyByte[0] = (byte)0;

            if (File.Exists(patch))
                File.Delete(patch);
            
            int offset = 0;

            // Write header (2bytes) - 0-1
            FileStream fileStream = File.Create(patch);
            fileStream.Seek(offset, SeekOrigin.Begin);
            byte[] header = CreateHeader(type);
            offset = WriteToStream(fileStream, header, offset);

            // Write blank filetable offset (4bytes) 2-5
            byte[] filetableOffset = BitConverter.GetBytes((int)0);
            offset = WriteToStream(fileStream, filetableOffset, offset);

            // Write build string (4bytes) 6-9
            offset = WriteToStream(fileStream, BitConverter.GetBytes(gamebuild), offset);

            // Write Compressed data length (4bytes) 10-13
            byte[] dataLength = BitConverter.GetBytes((int)0);
            offset = WriteToStream(fileStream, dataLength, offset);

            // Build temp file
            FileStream tempFile = File.Create(TEMP_FILE);
            int tempOffset = 0;
            foreach (PatchFileInfo file in files)
            {
                string fileFullName = root + "\\" + (file.FilePath == "" ? file.FileName : file.FilePath + "\\" + file.FileName);
                if (File.Exists(fileFullName))
                {
                    file.FileOffset = tempOffset;
                    byte[] fileBytes = File.ReadAllBytes(fileFullName);
                    tempOffset = WriteToStream(tempFile, fileBytes, tempOffset);
                    file.FileLength = fileBytes.Length;
                }
            }
            tempFile.Flush();
            tempFile.Close();

            // write datafile
            byte[] dataBytes = File.ReadAllBytes(TEMP_FILE);
            offset = WriteToStream(fileStream, dataBytes, offset);

            // delete temp datafile
            File.Delete(TEMP_FILE);

            // Write datafile length
            WriteToStream(fileStream, BitConverter.GetBytes(dataBytes.Length), M_DATA_LENGTH_OFFSET);

            // filetable
            // Write filetable offset (start at 4)
            WriteToStream(fileStream, BitConverter.GetBytes(offset), M_FILETABLE_OFFSET);

            // Write filetable size info (for further development)
            offset = WriteToStream(fileStream, BitConverter.GetBytes((int)files.Count * 16), offset);

            // format (16bytes) - FileOffset(4)-FileSize(4)-FileNameStringOffset(4)-FilePathStringOffset(4)
            foreach (PatchFileInfo file in files)
            {
                offset = WriteToStream(fileStream, BitConverter.GetBytes(file.FileOffset), offset);
                offset = WriteToStream(fileStream, BitConverter.GetBytes(file.FileLength), offset);
                file.FileNameSaveOffset = offset;
                offset = WriteToStream(fileStream, BitConverter.GetBytes((int)0), offset);
                file.FilePathSaveOffset = offset;
                offset = WriteToStream(fileStream, BitConverter.GetBytes((int)0), offset);
            }

            // Write strings
            foreach (PatchFileInfo file in files)
            {
                WriteToStream(fileStream, BitConverter.GetBytes(offset), file.FileNameSaveOffset);
                offset = WriteToStream(fileStream, Encoding.UTF8.GetBytes(file.FileName), offset);
                offset = WriteToStream(fileStream, emptyByte, offset);
                WriteToStream(fileStream, BitConverter.GetBytes(offset), file.FilePathSaveOffset);
                offset = WriteToStream(fileStream, Encoding.UTF8.GetBytes(file.FilePath), offset);
                offset = WriteToStream(fileStream, emptyByte, offset);
            }

            // All done
            fileStream.Flush();
            fileStream.Close();
        }
        public static void UnpackPatch(string destinationDir, string patch)
        {
            List<PatchFileInfo> files = new List<PatchFileInfo>();

            FileStream patchStream = File.OpenRead(patch);

            // Get filetable
            // search for filetable size data
            int filetableOffset = ReadIntFromStream(patchStream, M_FILETABLE_OFFSET);

            // get filetable size
            int filetableSize = ReadIntFromStream(patchStream, filetableOffset);

            // get filetable
            byte[] filetableBytes = ReadBytesFromStream(patchStream, filetableOffset + 4, filetableSize);

            for (int index = 0; index * 16 < filetableSize; index++)
            {
                PatchFileInfo file = new PatchFileInfo();

                // fileoffset - filesize - filenamestringoffset - filepathstringoffset
                byte[] fileOffsetBytes = new byte[4];
                byte[] fileSizeBytes = new byte[4];
                byte[] fileNameBytes = new byte[4];
                byte[] filePathBytes = new byte[4];

                for (int i = 0; i != 4; i++)
                {
                    fileOffsetBytes[i] = filetableBytes[index * 16 + i];
                    fileSizeBytes[i] = filetableBytes[index * 16 + 4 + i];
                    fileNameBytes[i] = filetableBytes[index * 16 + 8 + i];
                    filePathBytes[i] = filetableBytes[index * 16 + 12 + i];
                }

                file.FileOffset = BitConverter.ToInt32(fileOffsetBytes, 0);
                file.FileLength = BitConverter.ToInt32(fileSizeBytes, 0);
                int fileNameOffset = BitConverter.ToInt32(fileNameBytes, 0);
                file.FileName = ReadStringFromStream(patchStream, fileNameOffset);
                int filePathOffset = BitConverter.ToInt32(filePathBytes, 0);
                file.FilePath = ReadStringFromStream(patchStream, filePathOffset);

                files.Add(file);
            }

            // datafile
            int dataFileSize = ReadIntFromStream(patchStream, M_DATA_LENGTH_OFFSET);
            byte[] dataFileBytes = ReadBytesFromStream(patchStream, M_DATA_OFFSET, dataFileSize);
            FileStream dataFile = File.Create(TEMP_DATA_FILE);
            dataFile.Write(dataFileBytes, 0, dataFileBytes.Length);

            // split files and lead them to their destiny.
            foreach (PatchFileInfo file in files)
            {
                string dir = file.FilePath == "" ? destinationDir : destinationDir + "\\" + file.FilePath;
                string filePath = dir + "\\" + file.FileName;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!File.Exists(filePath))
                    File.Delete(filePath);

                FileStream fileStream = File.Create(filePath);
                byte[] fileBytes = ReadBytesFromStream(dataFile, file.FileOffset, file.FileLength);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Flush();
                fileStream.Close();
                // put it to the right place
                // 1234567
            }

            // delete datafile
            dataFile.Close();
            File.Delete(TEMP_DATA_FILE);
        }
        private static byte[] GetZippedBytesOfFile(string file)
        {
            MemoryStream inputMemStream = new MemoryStream(File.ReadAllBytes(file));
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
            zipStream.SetLevel(3);
            ZipEntry newEntry = new ZipEntry(ZIP_TEMP_FILE);
            newEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(newEntry);
            StreamUtils.Copy(inputMemStream, zipStream, new byte[4096]);
            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
            zipStream.Close();
            byte[] byteArrayOut = outputMemStream.ToArray();
            return byteArrayOut;
        }
        private static byte[] GetUnzippedBytesOfFile(FileStream fileStream)
        {
            ZipFile zf = new ZipFile(fileStream);
            byte[] buffer = new byte[4096];
            foreach (ZipEntry z in zf)
            {
                Stream zipStream = zf.GetInputStream(z);
                using (FileStream unzippedStream = File.Create(UNZIPPED_TEMP_FILE))
                {
                    StreamUtils.Copy(zipStream, unzippedStream, buffer);
                }
                if (zf != null)
                {
                    zf.IsStreamOwner = true;
                    zf.Close();
                }
            }
            return File.ReadAllBytes(UNZIPPED_TEMP_FILE);
        }
        public static byte[] ReadBytesFromStream(FileStream fileStream, int offset, int length)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = new byte[length];
            fileStream.Read(buffer, 0, length);
            return buffer;
        }
        public static int ReadIntFromStream(FileStream fileStream, int offset)
        {
            byte[] intBytes = new byte[sizeof(int)];
            fileStream.Seek(offset, SeekOrigin.Begin);
            for (int i = 0; i != intBytes.Length; i++)
            {
                intBytes[i] = (byte)fileStream.ReadByte();
            }
            return BitConverter.ToInt32(intBytes, 0);
        }
        public static string ReadStringFromStream(FileStream fileStream, int offset)
        {
            byte[] bufferBytes = new byte[4096];
            fileStream.Seek(offset, SeekOrigin.Begin);
            int stringLength = 0;
            for (int i = 0; i != fileStream.Length; i++)
            {
                int b = fileStream.ReadByte();
                if (b == 0 || b == -1)
                    break;
                else
                    bufferBytes[i] = (byte)b;
                stringLength++;
            }
            byte[] stringBytes = new byte[stringLength];
            Buffer.BlockCopy(bufferBytes, 0, stringBytes, 0, stringLength);
            string s = Encoding.UTF8.GetString(stringBytes);
            return s.Trim();
        }
        private static byte[] CreateHeader(MFileType type)
        {
            byte[] headerbytes = new byte[2];
            string magic = "m" + (int)type;
            headerbytes = Encoding.UTF8.GetBytes(magic);
            return headerbytes;
        }
        private static int WriteToStream(FileStream stream, byte[] data, int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            stream.Write(data, 0, data.Length);
            return offset + data.Length;
        }
    }
    public class PatchFileInfo
    {
        public string FileName { get; set; }
        public int FileNameSaveOffset { get; set; }
        public string FilePath { get; set; }
        public int FilePathSaveOffset { get; set; }
        public int FileOffset { get; set; }
        public int FileLength { get; set; }
    }
}
