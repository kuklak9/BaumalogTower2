using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS
{
    public class PLCUtilities
    {
        public class Loader3015
        {
            public const String LU_CUT_PREFIX = "(C)";
        }


        public static bool GetBoolValueFromShortArray(short[] arr, int nr)
        {

            int indexNr = nr / 16;
            int bitNr = nr % 16;
            return GetBoolValueFromShortArray(arr, indexNr, bitNr);
        }


        public static bool GetBoolValueFromShortArray(short[] arr, int nr, int bit)
        {
            try
            {
                return ((arr[nr] >> bit) & 1) == 1;
            }
            catch
            {
                return false;
            }

        }
        public static bool GetBoolValueFromIntArray(int[] arr, int nr)
        {
            int indexNr = nr / 32;
            int bitNr = nr % 32;
            return GetBoolValueFromIntArray(arr, indexNr, bitNr);
        }
        public static bool GetBoolValueFromIntArray(int[] arr, int nr, int bit)
        {
            try
            {
                return ((arr[nr] >> bit) & 1) == 1;
            }
            catch
            {
                return false;
            }
        }

        public static int GetIntDataFromPLCValue(STR.PLCValue plcV)
        {
            int rv = 0;
            try
            {
                rv = Convert.ToInt32((Int16)plcV.Value);

            }
            catch
            {

            }
            return rv;
        }

        public static STR.STR_IO[] GetIOTableFromPLCValue(STR.PLCValue plcV)
        {
            STR.STR_IO[] rv = null;
            try
            {
                rv = (STR.STR_IO[])plcV.Value;
            }
            catch { }
            return rv;
        }


        public static STR.STR_IO[] GetIOTableFromUintArr(uint[] val)
        {
            int arrCount = 0;
            int bitCount = 0;
            STR.STR_IO[] io = new STR.STR_IO[val.Count() * 32 / 3];
            try
            {
                for (int i = 0; i < io.Count(); i++)
                {
                    io[i] = new STR.STR_IO()
                    {
                        Signal = GetBitValue(val, ref arrCount, ref bitCount),
                        ForceOn = GetBitValue(val, ref arrCount, ref bitCount),
                        ForceOff = GetBitValue(val, ref arrCount, ref bitCount)
                    };
                }

                return io;
            }
            catch { return null; }
        }

        private static bool GetBitValue(uint[] val, ref int arrCount, ref int bitCount)
        {
            bool value = new bool();

            value = (val[arrCount] >> bitCount & 1) == 1;

            bitCount++;

            if (bitCount > 31)
            {
                arrCount++;
                bitCount = 0;
            }

            return value;
        }



        public static short[] GetShortTableFromPLCValue(STR.PLCValue plcV)
        {
            short[] rv = null;

            try
            {
                if (plcV.ValueType == typeof(short[]))
                    rv = (short[])plcV.Value;
            }
            catch { }

            return rv;

        }



        public static bool[] GetBoolTableFromPLCValue(STR.PLCValue plcV)
        {
            bool[] rv = null;

            try
            {
                if (plcV.ValueType == typeof(bool[]))
                    rv = (bool[])plcV.Value;
                else if (plcV.ValueType == typeof(Int16))
                {
                    rv = new bool[32];
                    System.Collections.BitArray bi = new System.Collections.BitArray(new int[] { (Int16)plcV.Value });
                    bi.CopyTo(rv, 0);
                }
                else if(plcV.ValueType == typeof(UInt32) && plcV.ValueTypeArrayItemCount > 0) //uint[]
                {
                    rv = new bool[32 * (plcV.ValueTypeArrayItemCount + 1)];

                    int j = 0;
                    int k = 0;

                    for(int i = 0; i < rv.Length; i++)
                    {
                        rv[i] = (((UInt32[])plcV.Value)[j] >> k & 1) == 1;

                        k++;
                        if (k >= 32)
                        {
                            j++;
                            k = 0;
                        }    
                    }
                }
                else if(plcV.ValueType == typeof(UInt32))//uint
                {
                    rv = new bool[32];
                    for (int i = 0; i < rv.Length; i++)
                        rv[i] = ((UInt64)plcV.Value >> i & 1) == 1;
                }
            }
            catch { }

            return rv;
        }


        public static bool[] GetBoolTableFromUInt32(uint value)
        {
            try
            {
                bool[] rv = new bool[64];

                int val = Int32.Parse("0");

                for (int i = 0; i < 64; i++)
                    rv[i] = (value >> i & 1) == 1;

                return rv;
            }
            catch
            {
                return null;
            }
        }

        public static int[] IOToIntTable(BTPTwinCatADS.STR.STR_IO[] io)
        {
            // 1 on
            // 2 force on
            // 0 off
            // -1 force off
            try
            {
                int[] t = new int[io.Length];

                for (int i = 0; i < io.Length; i++)
                {
                    t[i] = 0;
                    if (io[i].Signal)
                    {
                        if (io[i].ForceOn)
                            t[i] = 2;
                        else
                            t[i] = 1;
                    }
                    else
                    {
                        if (io[i].ForceOff)
                            t[i] = -1;
                        else
                            t[i] = 0;
                    }


                }

                return t;
            }
            catch
            {
                return null;
            }
        }


    }
}

public static class PLCUtilities_Generic
{
    /// <summary>
    /// Odczytuje zmienne PLC z BinaryReader i zwraca wartość z odpowiednim typem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Zmienna wynikowa</param>
    /// <param name="binaryReader">Reader do odczytywania bajtów</param>
    /// <param name="count">Ilość wierszy w tablicy</param>
    /// <param name="bytesLenght">Długość bajtów dla pojedynczej zmiennej</param>
    /// <returns></returns>
    public static T[] ReadPLCValue<T>(T source, System.IO.BinaryReader binaryReader, int count, int bytesLenght)
    {
        if (bytesLenght == 0)
            return null;

        if (count == 0)
            count = 1;

        byte[] data = binaryReader.ReadBytes(count * bytesLenght);


        T[] returnArray = new T[count];

        for (int i = 0; i < count * bytesLenght; i += bytesLenght)
        {
            returnArray[i / bytesLenght] = GetValueFromBytes(source, data, i, bytesLenght);
        }
        return returnArray;
    }

    /// <summary>
    /// Odczutuje zmienne PLC z odpowiednio pokrojonej tablicy byte[]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Zmienna wynikowa</param>
    /// <param name="data">Odczytane bajty</param>
    /// <param name="count">Ilość wierszy w tablici</param>
    /// <param name="bytesLenght">Długość bajtów dla pojedynczej zmiennej</param>
    /// <returns></returns>
    public static T[] ReadPlcValue_ByBytesArr<T>(T source, byte[] data, int count, int bytesLenght)
    {
        if (bytesLenght == 0)
            return null;


        if (count == 0)
            count = 1;

        T[] returnArray = new T[count];

        var type = source.GetType().IsArray ? source.GetType().GetElementType() : source.GetType();

        for (int i = 0; i < count * bytesLenght; i += bytesLenght)
        {
            returnArray[i / bytesLenght] = GetValueFromBytes(source, data, i, bytesLenght);
        }
        return returnArray;
    }

    /// <summary>
    /// Odczytuje wartość z tablicy byte
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <param name="bytesLenght"></param>
    /// <returns></returns>
    private static T GetValueFromBytes<T>(this T source, byte[] data, int index, int bytesLenght)
    {
        dynamic rv = source;

        if (source.GetType() == typeof(bool))
            rv = data[index] == 1;
        else if (source is byte)
        {
            rv = data[index];
        }
        else if (source is sbyte)
        {
            rv = Convert.ToSByte(data[index]);
        }
        else if (source is short)
        {
            rv = BitConverter.ToInt16(data, index);
        }
        else if (source is ushort)
        {
            rv = BitConverter.ToUInt16(data, index);
        }
        else if (source is int)
        {
            rv = BitConverter.ToInt32(data, index);
        }
        else if (source is uint)
        {
            rv = BitConverter.ToUInt32(data, index);
        }
        else if (source is string)
        {
            rv = Encoding.UTF8.GetString(data, 0, bytesLenght).Replace("\0", "");
        }

        return rv;
    }

    /// <summary>
    /// Zwraca fragment tablicy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Tablica źródłowa</param>
    /// <param name="index">Index początkowy</param>
    /// <param name="length">Długość tabliy wynikowej</param>
    /// <returns></returns>
    public static T[] Slice<T>(this T[] source, int index, int length)
    {
        T[] slice = new T[length];
        Array.Copy(source, index, slice, 0, length);
        return slice;
    }

    /// <summary>
    /// Zwraca prefix przed "HMI", bez kropki
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string GetValuePrefix(this string source)
    {
        try
        {
            int index = source.IndexOf("HMI");

            return source.Substring(0, index - 1);
        }
        catch
        {
            return "??";
        }
    }

    public static string GetValueProgramSource(this string source)
    {
        try
        {
            int dotIndex = source.IndexOf('.');

            string rv = source.PadLeft(dotIndex);

            return rv;
        }catch
        {
            return "??";
        }
    }
}
