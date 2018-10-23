using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Marshall
{

    public static class ExtensionUtils
    {
        public static IEnumerable<T> CopyOfRange<T>(this IEnumerable<T> src, int start, int end)
        {
            var len = end - start;
            for (int i = 0; i < len; i++)
            {
                yield return src.ElementAt(start + i);
            }
        }
    }

    public class Utils
    {
        public static readonly short POLYNOMIAL_CCITT = 0x1021;   /* x^16 + x^12 + x^5 + 1 */
        public static readonly short SEED_CCITT = 0;




        static public int byteArrToInteger(byte[] arr, int offset)
        {
            int intValue = 0;
            if ((arr.Length - offset) > 0) intValue = (((int)arr[offset + 0]) & 0xff);
            if ((arr.Length - offset) > 1) intValue = intValue + (((int)(arr[offset + 1]) & 0xff) << 8);
            if ((arr.Length - offset) > 2) intValue = intValue + (((int)(arr[offset + 2]) & 0xff) << 16);
            if ((arr.Length - offset) > 3) intValue = intValue + (((int)(arr[offset + 3]) & 0xff) << 24);

            return intValue;
        }

        /*******************/

        static public short byteArrToShort(byte[] arr, int offset)
        {
            short shortValue = 0;
            if ((arr.Length - offset) > 0) shortValue = (short)(arr[offset + 0] & 0xff);
            if ((arr.Length - offset) > 1) shortValue = (short)(shortValue + (((short)(arr[offset + 1]) & 0xff) << 8));

            return shortValue;
        }

        static public byte byteArrToByte(byte[] arr, int offset)
        {
            return arr[offset];
        }

        static public int byteToByteArray(byte[] arr, int offset, byte value)
        {
            arr[offset + 0] = (byte)(value);

            return 1;
        }
        /*******************/
        static public void shortToByteArray(byte[] arr, int offset, short value)
        {
            arr[offset + 0] = (byte)(value);
            arr[offset + 1] = (byte)(value >> 8);
        }

        static public int intToByteArray(byte[] arr, int offset, int value)
        {
            arr[offset + 0] = (byte)(value >> 0);
            arr[offset + 1] = (byte)(value >> 8);
            arr[offset + 2] = (byte)(value >> 16);
            arr[offset + 3] = (byte)(value >> 24);

            return 4;
        }

        static public int stringToByteArray(byte[] arr, int offset, String str, int max_len)
        {
            byte[] str_byte_arr = new byte[0];
            try
            {
                //str_byte_arr = str.getBytes("US-ASCII"); TODO:FIX

                max_len = str_byte_arr.Length < max_len ? str_byte_arr.Length : max_len;

                for (int i = 0; i < str_byte_arr.Length; i++)
                {
                    arr[offset++] = str_byte_arr[i];
                }
            }
            catch (Exception e)
            {
            }

            return str_byte_arr.Length;
        }

        static public String byteArrToString(byte[] arr, int offset, int max_len)
        {

            int str_Length = max_len;
            for (int i = offset; i < offset + str_Length; i++)
            {
                if (arr[i] == 0 || arr[i] == -1)
                {
                    str_Length = i;
                    break;
                }
            }
            return "";
            //return new String(arr, offset, str_Length); TODO: Fix
        }


        static public int subByteArrToByteArray(byte[] arr, int offset, byte[] sub_arr)
        {
            for (int i = 0; i < sub_arr.Length; i++)
            {
                arr[offset++] = sub_arr[i];
            }

            return sub_arr.Length;
        }


        static public int byteArrToSubByteArray(byte[] arr, int offset, byte[] sub_arr)
        {
            for (int i = 0; i < sub_arr.Length; i++)
            {
                sub_arr[i] = arr[offset++];
            }

            return sub_arr.Length;
        }

        /**
         * @brief  :This function Return CRC-CCITT calculation based on current CRC value and a given byte
         * @param  :Crc  - Current CRC
         *          Byte - Current byte to insert to the Crc
         * @retval :New CRC
         */

        public static short CRC_CCITT(short crc, byte Byte)
        {
            byte i;
            short temp, shortC, currentCRC = SEED_CCITT;

            shortC = (short)((short)Byte & 0x00FF);
            temp = (short)(((crc >> 8) ^ shortC) << 8);
            for (i = 0; i < 8; i++)
            {
                if (((currentCRC ^ temp) & 0x8000) != 0)
                    currentCRC = (short)((currentCRC << 1) ^ POLYNOMIAL_CCITT); /* 1010.0000 0000.0001 = x^16 + x^15 + x^13 + 1 */
                else
                    currentCRC <<= 1;
                temp <<= 1;
            }

            return (short)((crc << 8) ^ currentCRC);
        }

        public static short calcCRC(byte[] pData, int Len, short seed)
        {
            int i;
            short crc = seed;

            for (i = 0; i < Len; i++)
                crc = CRC_CCITT(crc, pData[i]);

            return crc;
        }


        private static byte[] HexStringToByteArray(String s)
        {
            int len = s.Length;
            byte[] data = new byte[len / 2];
            for (int i = 0; i < len; i += 2)
            {
                //data[i / 2] = (byte)((Character.digit(s.charAt(i), 16) << 4) + Character.digit(s.charAt(i + 1), 16)); TODO: Fix
            }
            return data;
        }


        private static String ByteArrayToHexString(byte[] bytes)
        {
            char[] hexArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            char[] hexChars = new char[bytes.Length * 2];
            int v;
            for (int j = 0; j < bytes.Length; j++)
            {
                v = bytes[j] & 0xFF;
                //hexChars[j * 2] = hexArray[v >>> 4]; TODO: FIX
                hexChars[j * 2 + 1] = hexArray[v & 0x0F];
            }
            return new String(hexChars);
        }


    }

}
