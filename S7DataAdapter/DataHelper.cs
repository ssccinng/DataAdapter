using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAdapter
{
    public static class DataHelper
    {
        public static T GetDataBlock<T> (Span<byte> data, bool littleEndian = true) where T : new() 
        {
            T dataBlock = new T();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var dataInfo = property.GetCustomAttributes(typeof(DataInfoAttribute), false).FirstOrDefault() as DataInfoAttribute;
                if (dataInfo != null)
                {
                    var dataLength = dataInfo.DataLength;
                    if (dataLength == 0)
                    {
                        dataLength = Marshal.SizeOf(property.PropertyType);
                    }
                    var arrayLength = dataInfo.ArrayLength;
                    if (arrayLength == 0)
                    {
                        arrayLength = 1;
                    }
                    var offset = dataInfo.Offset;
                    var span = data.Slice(offset, dataLength * arrayLength);
                    if (property.PropertyType.IsArray)
                    {
                        var elementType = property.PropertyType.GetElementType();
                        var array = Array.CreateInstance(elementType, arrayLength);
                        for (int i = 0; i < arrayLength; i++)
                        {
                            if (elementType == typeof(string)) {
                                var value = Encoding.ASCII.GetString(span.ToArray());
                                array.SetValue(value, i);
                                continue;
                            }
                            var element = GetDataBlock(elementType, span.Slice(i * dataLength, dataLength), littleEndian);
                            array.SetValue(element, i);
                        }
                        property.SetValue(dataBlock, array);
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        // 根据dataLength获取string
                        var value = Encoding.ASCII.GetString(span.ToArray());
                        property.SetValue(dataBlock, value);
                    }
                    else
                    {
                        var value = GetDataBlock(property.PropertyType, span, littleEndian);
                        property.SetValue(dataBlock, value);
                    }
                }
            }

            return dataBlock;

        }
        public static Object BytesToStruct(Byte[] bytes, Type strcutType) 
        { 
            Int32 size = Marshal.SizeOf(strcutType); 
            IntPtr buffer = Marshal.AllocHGlobal(size); 
            try 
            { 
                Marshal.Copy(bytes, 0, buffer, size); 
                return Marshal.PtrToStructure(buffer, strcutType); 
            } 
            finally 
            { 
                Marshal.FreeHGlobal(buffer); 
            } 
        }
        private static object GetDataBlock(Type elementType, Span<byte> span, bool littleEndian)
        {
            if (!littleEndian) span.Reverse();
            return BytesToStruct(span.ToArray(), elementType);
        }

        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[540];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
    }
}
