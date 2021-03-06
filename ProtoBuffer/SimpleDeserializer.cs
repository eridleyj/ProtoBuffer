﻿using System;
using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;
using ProtoBuf;

namespace ProtoBuffer
{
    public class SimpleDeserializer : ISimpleDeserializer
    {        
        /// <summary>
        ///     Deserializes from file
        /// </summary>
        /// <typeparam name="T">Type to deserialize into</typeparam>
        /// <param name="filePath">Filepath for deserialization</param>
        /// <param name="gzipDecompress">Use gzip decompression, if your data is serialized with gzip</param>
        /// <returns>File deserialized into type</returns>
        public T FromFile<T>(
                             [NotNull] string filePath,
                             bool gzipDecompress = false)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var readAllBytes = File.ReadAllBytes(filePath);

            return FromByteArray<T>(readAllBytes, gzipDecompress);
        }

        /// <summary>
        ///     Deserializes from byte array
        /// </summary>
        /// <typeparam name="T">Type to deserialize into</typeparam>
        /// <param name="value">Byte-array to be deserialized</param>
        /// <param name="gzipDecompress">Use gzip decompression, if your data is serialized with gzip</param>
        /// <returns>Byte-array deserialized into type</returns>
        public T FromByteArray<T>(
                                  [NotNull] byte[] value,
                                  bool gzipDecompress = false)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            using (var ms = new MemoryStream(value))
            {
                if (gzipDecompress)
                {
                    using (var gzip = new GZipStream(ms, CompressionMode.Decompress, true))
                    {
                        return Serializer.Deserialize<T>(gzip);
                    }
                }

                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}