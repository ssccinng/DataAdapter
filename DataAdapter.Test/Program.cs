// See https://aka.ms/new-console-template for more information
using DataAdapter;

Console.WriteLine("Hello, World!");
var aa = DataHelper.GetDataBlock<MyClass>(new byte[8] { 1, 0, 0, 0, 0, 1, 0, 0 });
Console.WriteLine(aa.A);
class MyClass
{
    [DataInfo(0)]
    public int A { get; set; }
    [DataInfo(5)]
    public byte B { get; set; }
}