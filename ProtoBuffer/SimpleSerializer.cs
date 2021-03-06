﻿using System;
using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;
using ProtoBuf;

namespace ProtoBuffer
{
    public class SimpleSerializer : ISimpleSerializer
    {
        private const int GZIP_BUFFER_SIZE = 64*1024;

        /// <summary>
        ///     Saves item to file
        /// </summary>
        /// <param name="item">Item to be saved</param>
        /// <param name="filePath">Destination filepath</param>
        /// <param name="overWriteExistingFile"></param>
        /// <param name="gzipCompress">Use gzip compression</param>
        /// <returns>Saved filepath</returns>
        public string SaveToFile(
                                 [NotNull] object item,
                                 [NotNull] string filePath,
                                 bool overWriteExistingFile = false,
                                 bool gzipCompress = false)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            if (!filePath.EndsWith(".bin"))
                throw new ArgumentException("filePath must end with .bin");

            if (overWriteExistingFile && File.Exists(filePath))
                throw new ArgumentException("file already exists");

            var byteArray = ToByteArray(item, gzipCompress);

            File.WriteAllBytes(filePath, byteArray);

            return filePath;
        }


        /// <summary>
        ///     Transforms item to protobuf string
        /// </summary>
        /// <param name="item">Item to be serialized</param>
        /// <param name="gzipCompress">Use gzip compression</param>
        /// <returns>String serialization of the item</returns>
        public byte[] ToByteArray(
                                  [NotNull] object item,
                                  bool gzipCompress = false)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using (var ms = new MemoryStream())
            {
                if (gzipCompress)
                {
                    using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                    using (var bs = new BufferedStream(gzip, GZIP_BUFFER_SIZE))
                    {
                        Serializer.Serialize(bs, item);
                    } //flush gzip                                     
                }
                else
                {
                    Serializer.Serialize(ms, item);
                }

                return ms.ToArray();
            }
        }
    }
}