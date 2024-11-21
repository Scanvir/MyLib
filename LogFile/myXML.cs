using System;
using System.Xml;

namespace MyLib
{
    public class myXML
    {
		public XmlDocument NewDocument(String rootName, string encoding = "UTF-8")
        {
			XmlDocument document = new XmlDocument();

			XmlDeclaration xmldecl;
			xmldecl = document.CreateXmlDeclaration("1.0", null, null);
			xmldecl.Encoding = encoding;

			// Add the new node to the document.
			document.LoadXml("<" + rootName + " />");
			XmlElement root = document.DocumentElement;
			document.InsertBefore(xmldecl, root);

			return document;
		}
		public XmlNode AddRootNode(XmlDocument document, String name)
		{
			XmlNode element = document.CreateElement(name);
			document.DocumentElement.AppendChild(element);
			return element;
		}
		public XmlNode AddNode(XmlDocument document, XmlNode element, String name)
		{
			XmlNode subelement = document.CreateElement(name);
			element.AppendChild(subelement);
			return subelement;
		}
		public void AddAtribute(XmlDocument document, XmlNode element, String name, String value)
		{
			XmlAttribute attribute = document.CreateAttribute(name);
			attribute.Value = value;
			element.Attributes.Append(attribute);
		}
	}
}
