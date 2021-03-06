﻿using MtarTool.Core.Common;
using MtarTool.Core.Mtar;
using MtarTool.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool
{
    class Program
    {
        private static bool numberNames;
        private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(MtarFile), typeof(MtarFile2) });

        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                string path = Path.GetFullPath(args[0]);
                bool outputHashes = false;
                if(args.Length > 1)
                {
                    if(args[1] == "-n")
                    {
                        numberNames = true;
                    } //if ends

                    if(args[1].ToLower()=="-outputhashes" || args[1].ToLower() == "-o") {
                        outputHashes = true;
                    }
                } //if ends

                if(Path.GetExtension(path) == ".mtar")
                {
                    HashSet<string> uniquePathHashes = new HashSet<string>();
                    if(GetMtarType(path) == 1)
                    {
                        MtarFile mtarfile = ReadArchive<MtarFile>(path, outputHashes);
                        foreach (MtarGaniFile entry in mtarfile.files) {
                            ulong pathHash = entry.hash & 0x3FFFFFFFFFFFF;//tex TODO is this right for GZ?
                            uniquePathHashes.Add(pathHash.ToString("x"));
                        }
                    } //if ends
                    else
                    {
                        MtarFile2 mtarfile = ReadArchive<MtarFile2>(path, outputHashes);
                        foreach (MtarGaniFile2 entry in mtarfile.files) {
                            ulong pathHash = entry.hash & 0x3FFFFFFFFFFFF;//tex TODO is this right for GZ?
                            uniquePathHashes.Add(pathHash.ToString("x"));
                        }
                    } //else ends
                    if (outputHashes) {
                        List<string> pathHashes = uniquePathHashes.ToList<string>();
                        pathHashes.Sort();
                        string fileDirectory = Path.GetDirectoryName(path);
                        string pathHashesOutputPath = Path.Combine(fileDirectory, string.Format("{0}_pathHashes.txt", Path.GetFileName(path)));
                        File.WriteAllLines(pathHashesOutputPath, pathHashes.ToArray<string>());
                    }
                } //if ends
                else if(Path.GetExtension(path) == ".xml")
                {
                    WriteArchive(path);
                } //else if ends
            } //if ends
        } //method Main ends

        static int GetMtarType(string path)
        {
            using (FileStream input = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

                input.Position = 0x28;
                uint offset = reader.ReadUInt32();

                reader.BaseStream.Position = offset;

                if (reader.ReadUInt32() == 0xBFCA2D2)
                    return 1;

                return 2;
            } //using ends
        } //method GetMtarType ends

        static T ReadArchive<T>(string path, bool skipWrite = false) where T : ArchiveFile, new()
        {
            string directory = Path.GetDirectoryName(path);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).Substring(1);
            string outputPath = directory + @"\" + nameWithoutExtension + "_" + extension + @"\";
            string xmlOutputPath = path + ".xml";

            using (FileStream input = new FileStream(path, FileMode.Open))
            using (FileStream xmlOutput = new FileStream(xmlOutputPath, FileMode.Create))
            {
                T file = new T();

                file.numberNames = numberNames;

                file.name = Path.GetFileName(path);
                file.Read(input);
                if (skipWrite == false) 
                {
                file.Export(input, outputPath);
                    NameResolver.WriteOutputList();
                }
                xmlSerializer.Serialize(xmlOutput, file);

                return file;
            } //using ends
        } //method ReadArchive ends

        static void WriteArchive(string path)
        {
            string outputPath = path.Replace(".xml", "");

            using (FileStream xmlInput = new FileStream(path, FileMode.Open))
            using (FileStream output = new FileStream(outputPath, FileMode.Create))
            {
                ArchiveFile archiveFile = xmlSerializer.Deserialize(xmlInput) as ArchiveFile;

                archiveFile.Import(output, outputPath);
            } //using ends
        } //method WriteArchive ends
    } //class Program ends
} //namespace MtarTool ends
