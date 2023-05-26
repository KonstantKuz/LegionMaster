using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using LegionMaster.Config.Csv;
using Mono.Csv;
using NUnit.Framework;

public class DeserializeCsv
{
    [Test]
    public void SimpleReadCsv()
    {
        List<List<string>> table = new List<List<string>>();
        new CsvFileReader(new MemoryStream(Encoding.UTF8.GetBytes("1,2\n3,4"))).ReadAll(table);
        Assert.That(table[0][0], Is.EqualTo("1"));
        Assert.That(table[0][1], Is.EqualTo("2"));
        Assert.That(table[1][0], Is.EqualTo("3"));
        Assert.That(table[1][1], Is.EqualTo("4"));        
    }

    [DataContract]
    private class TestConfig
    {
        [DataMember]
        public string StrField;
        [DataMember]
        public int IntField;
        [DataMember(Name = "FloatField")]
        public float FField;
    }

    [Test]
    public void ReadSingleObject()
    {
        const string data = @"StrField,some text
IntField,42
FloatField,3.14";
        var rez = new CsvSerializer().ReadSingleObject<TestConfig>(ToMemoryStream(data));
        Assert.That(rez.StrField, Is.EqualTo("some text"));
        Assert.That(rez.IntField, Is.EqualTo(42));
        Assert.That(rez.FField, Is.EqualTo(3.14f));
    }

    private static MemoryStream ToMemoryStream(string data)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(data));
    }

    [Test]
    public void ReadObjectArray()
    {
        const string data = @"StrField,IntField,FloatField,
aaa,1,1
bbb,2,2.5
c,3,3.0";
        var rez = new CsvSerializer().ReadObjectArray<TestConfig>(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.That(rez[0].StrField, Is.EqualTo("aaa"));
        Assert.That(rez[1].IntField, Is.EqualTo(2));
        Assert.That(rez[1].FField, Is.EqualTo(2.5f));
        Assert.That(rez[2].StrField, Is.EqualTo("c"));
        Assert.That(rez[2].FField, Is.EqualTo(3.0f));
    }

    [Test]
    public void ReadObjectDictionary()
    {
        const string data = @",StrField,IntField,FloatField,
A,aaa,1,1
B,bbb,2,2.5
C,c,3,3.0";        
        var rez = new CsvSerializer().ReadObjectDictionary<TestConfig>(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.That(rez["A"].StrField, Is.EqualTo("aaa"));
        Assert.That(rez["B"].IntField, Is.EqualTo(2));
        Assert.That(rez["B"].FField, Is.EqualTo(2.5f));
        Assert.That(rez["C"].StrField, Is.EqualTo("c"));
        Assert.That(rez["C"].FField, Is.EqualTo(3.0f));
    }

    private class StructWithStructInside
    {
        [DataMember]
        public TestConfig Inner;

        [DataMember] 
        public int MyField;
    }

    [Test]
    public void NestedStruct()
    {
        const string data = @"StrField,some text
IntField,42
FloatField,3.14
MyField,8";        
        
        var rez = new CsvSerializer().ReadSingleObject<StructWithStructInside>(ToMemoryStream(data));
        Assert.That(rez.Inner.StrField, Is.EqualTo("some text"));
        Assert.That(rez.Inner.IntField, Is.EqualTo(42));
        Assert.That(rez.Inner.FField, Is.EqualTo(3.14f));
        Assert.That(rez.MyField, Is.EqualTo(8));
    }

    private class CustomSerializeStruct: ICustomCsvSerializable
    {
        [DataMember] 
        public int IntField;

        public string IntAsStringField;

        public void Deserialize(Func<string, string> data)
        {
            IntAsStringField = data("IntField");
        }
    }

    [Test]
    public void CustomSerialization()
    {
        const string data = @"IntField,42";  
        var rez = new CsvSerializer().ReadSingleObject<CustomSerializeStruct>(ToMemoryStream(data));
        Assert.That(rez.IntField, Is.EqualTo(42));
        Assert.That(rez.IntAsStringField, Is.EqualTo("42"));
    }

    private class StructWithPrivateField
    {
        [DataMember(Name = "IntField")]
        private int _intField;

        public int IntField => _intField;
    }

    [Test]
    public void PrivateFields()
    {
        const string data = @"IntField,42";  
        var rez = new CsvSerializer().ReadSingleObject<StructWithPrivateField>(ToMemoryStream(data));
        Assert.That(rez.IntField, Is.EqualTo(42));
    }

    private enum TestEnum
    {
        Bad,
        Good
    }

    private class StructWithEnum
    {
        [DataMember]
        public TestEnum EnumField;
    }

    [Test]
    public void EnumFields()
    {
        const string data = @"EnumField,Good";  
        var rez = new CsvSerializer().ReadSingleObject<StructWithEnum>(ToMemoryStream(data));
        Assert.That(rez.EnumField, Is.EqualTo(TestEnum.Good));
    }

    private class OneFieldStruct
    {
        [DataMember]
        public int IntField;
    }

    [Test]
    public void NestedTable()
    {
        const string data = @"Key,IntField
A,1
,2
B,1
C,3
,4
,5";       
        var rez = new CsvSerializer().ReadNestedTable<OneFieldStruct>(ToMemoryStream(data));
        Assert.That(rez.Count, Is.EqualTo(3));
        
        Assert.That(rez["A"].Count, Is.EqualTo(2));
        Assert.That(rez["A"][0].IntField, Is.EqualTo(1));
        Assert.That(rez["A"][1].IntField, Is.EqualTo(2));
        
        Assert.That(rez["B"].Count, Is.EqualTo(1));
        Assert.That(rez["B"][0].IntField, Is.EqualTo(1));

        Assert.That(rez["C"].Count, Is.EqualTo(3));
        Assert.That(rez["C"][0].IntField, Is.EqualTo(3));
        Assert.That(rez["C"][1].IntField, Is.EqualTo(4));
        Assert.That(rez["C"][2].IntField, Is.EqualTo(5));
    }

    [Test]
    public void EmptyFieldToDefaultValue()
    {
        const string data = @"IntField,";
        var rez = new CsvSerializer().ReadSingleObject<OneFieldStruct>(ToMemoryStream(data));
        Assert.That(rez.IntField, Is.EqualTo(0));
    }

    [Test]
    public void TrailingSpacesInHeader()
    {
        const string data = @"IntField   ,42";
        var rez = new CsvSerializer().ReadSingleObject<OneFieldStruct>(ToMemoryStream(data));
        Assert.That(rez.IntField, Is.EqualTo(42));
    }

    [Test]
    public void LeadingSpacesInHeader()
    {
        const string data = @"     IntField,42";
        var rez = new CsvSerializer().ReadSingleObject<OneFieldStruct>(ToMemoryStream(data));
        Assert.That(rez.IntField, Is.EqualTo(42));
    }
    
    [Test]
    public void TrailingSpacesInValues()
    {
        const string data = @"StrField,some text   
IntField,42   
FloatField,3.14    ";
        var rez = new CsvSerializer().ReadSingleObject<TestConfig>(ToMemoryStream(data));
        Assert.That(rez.StrField, Is.EqualTo("some text"));
        Assert.That(rez.IntField, Is.EqualTo(42));
        Assert.That(rez.FField, Is.EqualTo(3.14f));
    }

    [Test]
    public void OnlySpacesInValues()
    {
        const string data = @"StrField,    
IntField,    
FloatField,   ";
        var rez = new CsvSerializer().ReadSingleObject<TestConfig>(ToMemoryStream(data));
        Assert.That(rez.StrField, Is.Null);
        Assert.That(rez.IntField, Is.EqualTo(0));
        Assert.That(rez.FField, Is.EqualTo(0));
    }    
}
