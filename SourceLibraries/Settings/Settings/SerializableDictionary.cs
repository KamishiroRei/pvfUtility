using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Settings;

[Serializable]
public class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable
{
	private XmlSerializer _keySerializer;

	private XmlSerializer _valueSerializer;

	protected XmlSerializer ValueSerializer => _valueSerializer ?? (_valueSerializer = new XmlSerializer(typeof(TVal)));

	private XmlSerializer KeySerializer => _keySerializer ?? (_keySerializer = new XmlSerializer(typeof(TKey)));

	public SerializableDictionary()
	{
	}

	public SerializableDictionary(IDictionary<TKey, TVal> dictionary)
		: base(dictionary)
	{
	}

	public SerializableDictionary(IEqualityComparer<TKey> comparer)
		: base(comparer)
	{
	}

	public SerializableDictionary(int capacity)
		: base(capacity)
	{
	}

	public SerializableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
		: base(dictionary, comparer)
	{
	}

	public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		: base(capacity, comparer)
	{
	}

	protected SerializableDictionary(SerializationInfo info, StreamingContext context)
	{
		int @int = info.GetInt32("itemsCount");
		for (int i = 0; i < @int; i++)
		{
			KeyValuePair<TKey, TVal> keyValuePair = (KeyValuePair<TKey, TVal>)info.GetValue(string.Format(CultureInfo.InvariantCulture, "Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
			Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("itemsCount", base.Count);
		int num = 0;
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<TKey, TVal> current = enumerator.Current;
			info.AddValue(string.Format(CultureInfo.InvariantCulture, "Item{0}", num), current, typeof(KeyValuePair<TKey, TVal>));
			num++;
		}
	}

	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<TKey, TVal> current = enumerator.Current;
			writer.WriteStartElement("item");
			writer.WriteStartElement("key");
			KeySerializer.Serialize(writer, current.Key);
			writer.WriteEndElement();
			writer.WriteStartElement("value");
			ValueSerializer.Serialize(writer, current.Value);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}

	void IXmlSerializable.ReadXml(XmlReader reader)
	{
		if (!reader.IsEmptyElement)
		{
			if (reader.NodeType == XmlNodeType.Element && !reader.Read())
			{
				throw new XmlException("Error in Deserialization of SerializableDictionary");
			}
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");
				reader.ReadStartElement("key");
				TKey key = (TKey)KeySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement("value");
				TVal value = (TVal)ValueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadEndElement();
				Add(key, value);
				reader.MoveToContent();
			}
			if (reader.NodeType != XmlNodeType.EndElement)
			{
				throw new XmlException("Error in Deserialization of SerializableDictionary");
			}
			reader.ReadEndElement();
		}
	}

	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}
}
