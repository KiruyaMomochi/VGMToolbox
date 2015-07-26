﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriField
    {        
        public byte Type { set; get; }
        public string Name { set; get; }        
        public object Value { set; get; }
        public ulong Offset { set; get; }
        public ulong Size { set; get; }
    }

    public class CriUtfTocFileInfo
    {
        public string DirName { set; get; }
        public string FileName { set; get; }
        public uint FileSize { set; get; }
        public uint ExtractSize { set; get; }
        public ulong FileOffset { set; get; }
    }

    public class CriUtfTable
    {
        // thanks to hcs for all of this!!!

        #region Constants

        public static readonly byte[] SIGNATURE_BYTES = new byte[] { 0x40, 0x55, 0x54, 0x46 };
        public const string LCG_SEED_KEY = "SEED";
        public const string LCG_INCREMENT_KEY = "INC";

        public const byte COLUMN_STORAGE_MASK = 0xF0;
        public const byte COLUMN_STORAGE_PERROW = 0x50;
        public const byte COLUMN_STORAGE_CONSTANT = 0x30;
        public const byte COLUMN_STORAGE_ZERO = 0x10;

        // I suspect that "type 2" is signed
        public const byte COLUMN_TYPE_MASK = 0x0F;
        public const byte COLUMN_TYPE_DATA = 0x0B;
        public const byte COLUMN_TYPE_STRING = 0x0A;
        // 0x09 double? 
        public const byte COLUMN_TYPE_FLOAT = 0x08;
        // 0x07 signed 8byte? 
        public const byte COLUMN_TYPE_8BYTE = 0x06;
        public const byte COLUMN_TYPE_4BYTE2 = 0x05;
        public const byte COLUMN_TYPE_4BYTE = 0x04;
        public const byte COLUMN_TYPE_2BYTE2 = 0x03;
        public const byte COLUMN_TYPE_2BYTE = 0x02;
        public const byte COLUMN_TYPE_1BYTE2 = 0x01;
        public const byte COLUMN_TYPE_1BYTE = 0x00;

        #endregion

        #region Members

        public string SourceFile { set; get; }
        public long BaseOffset { set; get; }

        public byte[] MagicBytes { set; get; }
        public bool IsEncrypted { set; get; } 
        public uint TableSize { set; get; }

        public ushort Unknown1 { set; get; }

        public uint RowOffset { set; get; }
        public uint StringTableOffset { set; get; }
        public uint DataOffset { set; get; }

        public uint TableNameOffset { set; get; }
        public string TableName { set; get; }

        public ushort NumberOfFields { set; get; }
        
        public ushort RowSize { set; get; }
        public uint NumberOfRows { set; get; }

        public Dictionary<string, CriField>[] Rows { set; get; }

        public CriUtfReader UtfReader { set; get; }

        #endregion

        public void Initialize(FileStream fs, long offset)
        {
            this.SourceFile = fs.Name;
            this.BaseOffset = offset;
            this.MagicBytes = ParseFile.ParseSimpleOffset(fs, offset + 0, 4);

            // check if file is decrypted and get decryption keys if needed
            this.checkEncryption(fs);            
            
            if (ParseFile.CompareSegment(this.MagicBytes, 0, SIGNATURE_BYTES))
            {               
                // read header
                this.initializeUtfHeader(fs);

                // initialize rows
                this.Rows = new Dictionary<string, CriField>[this.NumberOfRows];

                // read schema
                if (this.TableSize > 0)
                {
                    this.initializeUtfSchema(fs, (this.BaseOffset + 0x20));
                }               
            }
            else
            {
                //Dictionary<string, byte> foo = GetKeysForEncryptedUtfTable(this.MagicBytes);
                throw new FormatException(String.Format("@UTF signature not found at offset <0x{0}>.", offset.ToString("X8")));           
            }
        
        }

        private void checkEncryption(FileStream fs)
        {
            if (ParseFile.CompareSegment(this.MagicBytes, 0, SIGNATURE_BYTES))
            {
                this.IsEncrypted = false;
                this.UtfReader = new CriUtfReader();
            }
            else
            {
                this.IsEncrypted = true;
                Dictionary<string, byte> lcgKeys = GetKeysForEncryptedUtfTable(this.MagicBytes);

                if (lcgKeys.Count != 2)
                {
                    throw new FormatException(String.Format("Unable to decrypt UTF table at offset: 0x{0}", this.BaseOffset.ToString("X8")));
                }
                else
                {
                    this.UtfReader = new CriUtfReader(lcgKeys[LCG_SEED_KEY], lcgKeys[LCG_INCREMENT_KEY], this.IsEncrypted);
                }

                this.MagicBytes = this.UtfReader.GetBytes(fs, this.BaseOffset, 4, 0);
            }

        }

        private void initializeUtfHeader(FileStream fs)
        {
            this.TableSize = this.UtfReader.ReadUint(fs, this.BaseOffset, 4);

            this.Unknown1 = this.UtfReader.ReadUshort(fs, this.BaseOffset, 8);

            this.RowOffset = (uint)this.UtfReader.ReadUshort(fs, this.BaseOffset, 0xA) + 8;
            this.StringTableOffset = this.UtfReader.ReadUint(fs, this.BaseOffset, 0xC) + 8;
            this.DataOffset = this.UtfReader.ReadUint(fs, this.BaseOffset, 0x10) + 8;

            this.TableNameOffset = this.UtfReader.ReadUint(fs, this.BaseOffset, 0x14);
            this.TableName = UtfReader.ReadAsciiString(fs, this.BaseOffset, this.StringTableOffset + this.TableNameOffset);

            this.NumberOfFields = this.UtfReader.ReadUshort(fs, this.BaseOffset, 0x18);

            this.RowSize = this.UtfReader.ReadUshort(fs, this.BaseOffset, 0x1A);
            this.NumberOfRows = this.UtfReader.ReadUint(fs, this.BaseOffset, 0x1C);        
        }

        private void initializeUtfSchema(FileStream fs, long schemaOffset)
        {
            long nameOffset;

            long constantOffset;
            
            long dataOffset;
            long dataSize;

            long rowDataOffset;
            long rowDataSize;

            long currentOffset = schemaOffset;
            long currentRowBase;
            long currentRowOffset = 0;
            
            CriField field;

            for (uint i = 0; i < this.NumberOfRows; i++)
            {
                //if (i == 0x2a2)
                //{
                //    int yuuuu = 1;
                //}                
                //try
                //{
                    currentOffset = schemaOffset;
                    currentRowBase = this.BaseOffset + this.RowOffset + (this.RowSize * i);
                    currentRowOffset = 0;
                    this.Rows[i] = new Dictionary<string, CriField>();

                    // parse fields
                    for (ushort j = 0; j < this.NumberOfFields; j++)
                    {
                        field = new CriField();

                        field.Type = ParseFile.ReadByte(fs, currentOffset);
                        nameOffset = ParseFile.ReadUintBE(fs, currentOffset + 1);
                        field.Name = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + nameOffset);

                        // each row will have a constant
                        if ((field.Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)
                        {
                            // capture offset of constant
                            constantOffset = currentOffset + 5;

                            // read the constant depending on the type
                            switch (field.Type & COLUMN_TYPE_MASK)
                            {
                                case COLUMN_TYPE_STRING:
                                    dataOffset = ParseFile.ReadUintBE(fs, constantOffset);
                                    field.Value = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + dataOffset);
                                    currentOffset += 4;
                                    break;
                                case COLUMN_TYPE_8BYTE:
                                    field.Value = ParseFile.ReadUlongBE(fs, constantOffset);
                                    currentOffset += 8;
                                    break;
                                case COLUMN_TYPE_DATA:
                                    dataOffset = ParseFile.ReadUintBE(fs, constantOffset);
                                    dataSize = ParseFile.ReadUintBE(fs, constantOffset + 4);
                                    field.Offset = (ulong)(this.BaseOffset + this.DataOffset + dataOffset);
                                    field.Size = (ulong)dataSize;
                                    field.Value = ParseFile.ParseSimpleOffset(fs, (long)field.Offset, (int)dataSize);
                                    currentOffset += 8;
                                    break;
                                case COLUMN_TYPE_FLOAT:
                                    field.Value = ParseFile.ReadFloatBE(fs, constantOffset);
                                    currentOffset += 4;
                                    break;
                                case COLUMN_TYPE_4BYTE2:
                                    field.Value = ParseFile.ReadInt32BE(fs, constantOffset);
                                    currentOffset += 4;
                                    break;
                                case COLUMN_TYPE_4BYTE:
                                    field.Value = ParseFile.ReadUintBE(fs, constantOffset);
                                    currentOffset += 4;
                                    break;
                                case COLUMN_TYPE_2BYTE2:
                                    field.Value = ParseFile.ReadInt16BE(fs, constantOffset);
                                    currentOffset += 2;
                                    break;
                                case COLUMN_TYPE_2BYTE:
                                    field.Value = ParseFile.ReadUshortBE(fs, constantOffset);
                                    currentOffset += 2;
                                    break;
                                case COLUMN_TYPE_1BYTE2:
                                    field.Value = ParseFile.ReadSByte(fs, constantOffset);
                                    currentOffset += 1;
                                    break;
                                case COLUMN_TYPE_1BYTE:
                                    field.Value = ParseFile.ReadByte(fs, constantOffset);
                                    currentOffset += 1;
                                    break;
                                default:
                                    throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));

                            } // switch (field.Type & COLUMN_TYPE_MASK)
                        }
                        else if ((field.Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_PERROW)
                        {
                            // read the constant depending on the type
                            switch (field.Type & COLUMN_TYPE_MASK)
                            {
                                case COLUMN_TYPE_STRING:
                                    rowDataOffset = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset);
                                    field.Value = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + rowDataOffset);
                                    currentRowOffset += 4;
                                    break;
                                case COLUMN_TYPE_8BYTE:
                                    field.Value = ParseFile.ReadUlongBE(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 8;
                                    break;
                                case COLUMN_TYPE_DATA:
                                    rowDataOffset = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset);
                                    rowDataSize = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset + 4);
                                    field.Offset = (ulong)(this.BaseOffset + this.DataOffset + rowDataOffset);
                                    field.Size = (ulong)rowDataSize;
                                    field.Value = ParseFile.ParseSimpleOffset(fs, (long)field.Offset, (int)rowDataSize);
                                    currentRowOffset += 8;
                                    break;
                                case COLUMN_TYPE_FLOAT:
                                    field.Value = ParseFile.ReadFloatBE(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 4;
                                    break;
                                case COLUMN_TYPE_4BYTE2:
                                    field.Value = ParseFile.ReadInt32BE(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 4;
                                    break;
                                case COLUMN_TYPE_4BYTE:
                                    field.Value = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 4;
                                    break;
                                case COLUMN_TYPE_2BYTE2:
                                    field.Value = ParseFile.ReadInt16BE(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 2;
                                    break;
                                case COLUMN_TYPE_2BYTE:
                                    field.Value = ParseFile.ReadUshortBE(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 2;
                                    break;
                                case COLUMN_TYPE_1BYTE2:
                                    field.Value = ParseFile.ReadSByte(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 1;
                                    break;
                                case COLUMN_TYPE_1BYTE:
                                    field.Value = ParseFile.ReadByte(fs, currentRowBase + currentRowOffset);
                                    currentRowOffset += 1;
                                    break;
                                default:
                                    throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));

                            } // switch (field.Type & COLUMN_TYPE_MASK)

                        } // if ((fields[i].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)

                        // add field to dictionary
                        this.Rows[i].Add(field.Name, field);

                        // move to next field
                        currentOffset += 5; //  sizeof(CriField.Type + CriField.NameOffset)

                    } // for (ushort j = 0; j < this.NumberOfFields; j++)
                //}
                //catch (Exception ex)
                //{
                //    int xxxx = 1;
                //}
            } // for (uint i = 0; i < this.NumberOfRows; i++)
        }

        public static object GetUtfFieldForRow(CriUtfTable utfTable, int rowIndex, string key)
        {
            object ret = null;

            if (utfTable.Rows.GetLength(0) > rowIndex)
            {
                if (utfTable.Rows[rowIndex].ContainsKey(key))
                {
                    ret = utfTable.Rows[rowIndex][key].Value;
                }
            }

            return ret;
        }

        public static ulong GetOffsetForUtfFieldForRow(CriUtfTable utfTable, int rowIndex, string key)
        {
            ulong ret = 0;

            if (utfTable.Rows.GetLength(0) > rowIndex)
            {
                if (utfTable.Rows[rowIndex].ContainsKey(key))
                {
                    ret = utfTable.Rows[rowIndex][key].Offset;
                }
            }

            return ret;
        }

        public static ulong GetSizeForUtfFieldForRow(CriUtfTable utfTable, int rowIndex, string key)
        {
            ulong ret = 0;

            if (utfTable.Rows.GetLength(0) > rowIndex)
            {
                if (utfTable.Rows[rowIndex].ContainsKey(key))
                {
                    ret = utfTable.Rows[rowIndex][key].Size;
                }
            }

            return ret;
        }

        public static Dictionary<string, byte> GetKeysForEncryptedUtfTable(byte[] encryptedUtfSignature)
        {
            Dictionary<string, byte> keys = new Dictionary<string, byte>();
            byte m, t, xor;

            byte[] xorBytes = new byte[SIGNATURE_BYTES.Length];
            bool keysFound = false;

            for (byte seed = 0; seed <= byte.MaxValue; seed++)
            {
                if (!keysFound)
                {
                    // match first char
                    if ((encryptedUtfSignature[0] ^ seed) == SIGNATURE_BYTES[0])
                    {
                        for (byte increment = 0; increment <= byte.MaxValue; increment++)
                        {
                            if (!keysFound)
                            {
                                m = (byte)(seed * increment);

                                if ((encryptedUtfSignature[1] ^ m) == SIGNATURE_BYTES[1])
                                {
                                    t = increment;

                                    for (int j = 2; j < SIGNATURE_BYTES.Length; j++)
                                    {
                                        m *= t;

                                        if ((encryptedUtfSignature[j] ^ m) != SIGNATURE_BYTES[j])
                                        {
                                            break;
                                        }
                                        else if (j == (SIGNATURE_BYTES.Length - 1))
                                        {
                                            keys.Add(LCG_SEED_KEY, seed);
                                            keys.Add(LCG_INCREMENT_KEY, increment);
                                            keysFound = true;
                                        }
                                    }
                                } // if ((encryptedUtfSignature[1] ^ m) == SIGNATURE_BYTES[1])
                            }
                            else
                            {
                                break;
                            } // if (!keysFound)
                        } // for (byte inc = 0; inc <= byte.MaxValue; inc++)
                    } // if ((encryptedUtfSignature[0] ^ mult) == SIGNATURE_BYTES[0])
                } // if (!keysFound)
                else
                {
                    break;
                }
            } // for (byte mult = 0; mult <= byte.MaxValue; mult++)

            return keys;
        }
    }

    public class CriUtfReader
    {
        public byte Seed { set; get; }
        public byte Increment { set; get; }
        public bool IsEncrypted { set; get; }

        public CriUtfReader()
        {
            this.IsEncrypted = false;
        }

        public CriUtfReader(byte seed, byte increment, bool isEncrypted)
        {
            this.Seed = seed;
            this.Increment = increment;
            this.IsEncrypted = isEncrypted;
        }

        public byte[] GetBytes(FileStream fs, long BaseOffset, int Size, long UtfOffset)
        {
            byte[] ret;
            byte xorByte;

            ret = ParseFile.ParseSimpleOffset(fs, BaseOffset + UtfOffset, Size);

            if (this.IsEncrypted)
            {
                xorByte = this.Seed;

                // catch up to this offset
                for (long j = 1; j < UtfOffset; j++)
                {
                    xorByte *= this.Increment;
                }

                // decrypt this offset
                for (long i = 0; i < Size; i++)
                {
                    xorByte *= this.Increment;
                    ret[i] ^= xorByte;
                }
            }
            
            return ret;
        }

        public string ReadAsciiString(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte encryptedByte;
            byte decryptedByte;

            StringBuilder asciiVal =  new StringBuilder();
            byte xorByte;
            long fileSize = fs.Length;

            if (this.IsEncrypted)
            {
                fs.Position = (BaseOffset + UtfOffset);

                // catch up to this offset
                xorByte = this.Seed;
                
                for (long j = 1; j < UtfOffset; j++)
                {
                    xorByte *= this.Increment;
                }

                for (long i = UtfOffset; i < (fileSize - (BaseOffset + UtfOffset)); i++)
                {
                    xorByte *= this.Increment;
                    encryptedByte = (byte)fs.ReadByte();
                    decryptedByte = (byte)(encryptedByte ^ xorByte);

                    if (decryptedByte == 0)
                    {
                        break;
                    }
                    else
                    {
                        asciiVal.Append(Convert.ToChar(decryptedByte));
                    }

                }
            }
            else
            {
                asciiVal.Append(ParseFile.ReadAsciiString(fs, BaseOffset + UtfOffset));
            }
            
            return asciiVal.ToString();
        }


        public byte ReadByte(FileStream fs, long BaseOffset, long UtfOffset)
        {
            return this.GetBytes(fs, BaseOffset, 1, UtfOffset)[0];
        }

        public SByte ReadSByte(FileStream fs, long BaseOffset, long UtfOffset)
        {
            return (SByte)this.GetBytes(fs, BaseOffset, 1, UtfOffset)[0];
        }

        public ushort ReadUshort(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte[] temp = this.GetBytes(fs, BaseOffset, 2, UtfOffset);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            return BitConverter.ToUInt16(temp, 0);
        }

        public short ReadShort(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte[] temp = this.GetBytes(fs, BaseOffset, 2, UtfOffset);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            return BitConverter.ToInt16(temp, 0);
        }

        public uint ReadUint(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte[] temp = this.GetBytes(fs, BaseOffset, 4, UtfOffset);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            return BitConverter.ToUInt32(temp, 0);
        }

        public int ReadInt(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte[] temp = this.GetBytes(fs, BaseOffset, 4, UtfOffset);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            return BitConverter.ToInt32(temp, 0);
        }

        public ulong ReadUlong(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte[] temp = this.GetBytes(fs, BaseOffset, 8, UtfOffset);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            return BitConverter.ToUInt64(temp, 0);
        }

        public float ReadFloat(FileStream fs, long BaseOffset, long UtfOffset)
        {
            byte[] temp = this.GetBytes(fs, BaseOffset, 4, UtfOffset);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            return BitConverter.ToSingle(temp, 0);
        }
    }
}
