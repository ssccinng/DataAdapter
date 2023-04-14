namespace DataAdapter
{
    public class DataInfoAttribute : Attribute
    {
        public int Offset { get; set; }
        public int ArrayLength { get; set; }
        public int DataLength { get; set; }
        public DataInfoAttribute(int offset, int arrayLength = 0, int dataLength = 0)
        {
            Offset = offset;
            ArrayLength = arrayLength;
            DataLength = dataLength;
        }
    }
}