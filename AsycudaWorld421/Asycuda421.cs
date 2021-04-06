using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Asycuda421
{
    public partial class ASYCUDA
    {
        public static bool CanLoadFromFile(string fileName)
        {
            return CanLoadFromFile(fileName, Encoding.UTF8);
        }

        public static bool CanLoadFromFile(string fileName, Encoding encoding)
        {
            var schemas = new XmlSchemaSet();
            schemas.Add(null, @"Asycuda421.xsd");

            var doc = XDocument.Load(fileName);
            var msg = "";
            doc.Validate(schemas, (o, e) => { msg += e.Message + Environment.NewLine; });
            if (string.IsNullOrEmpty(msg)) return true;
            return false;
        }

        public static bool CanDeserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return Serializer.CanDeserialize(XmlReader.Create(stringReader));
            }
            finally
            {
                if (stringReader != null) stringReader.Dispose();
            }
        }
    }

    public partial class Value_declaration_form
    {
        public static bool CanLoadFromFile(string fileName)
        {
            return CanLoadFromFile(fileName, Encoding.UTF8);
        }

        public static bool CanLoadFromFile(string fileName, Encoding encoding)
        {
            var schemas = new XmlSchemaSet();
            schemas.Add(null, @"C71.xsd");

            var doc = XDocument.Load(fileName);
            var msg = "";
            doc.Validate(schemas, (o, e) => { msg += e.Message + Environment.NewLine; });
            if (string.IsNullOrEmpty(msg)) return true;
            return false;
        }

        public static bool CanDeserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return Serializer.CanDeserialize(XmlReader.Create(stringReader));
            }
            finally
            {
                if (stringReader != null) stringReader.Dispose();
            }
        }
    }

    public partial class Licence
    {
        public static bool CanLoadFromFile(string fileName)
        {
            return CanLoadFromFile(fileName, Encoding.UTF8);
        }

        public static bool CanLoadFromFile(string fileName, Encoding encoding)
        {
            var schemas = new XmlSchemaSet();
            schemas.Add(null, @"License.xsd");

            var doc = XDocument.Load(fileName);
            var msg = "";
            doc.Validate(schemas, (o, e) => { msg += e.Message + Environment.NewLine; });
            if (string.IsNullOrEmpty(msg)) return true;
            return false;
        }

        public static bool CanDeserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return Serializer.CanDeserialize(XmlReader.Create(stringReader));
            }
            finally
            {
                if (stringReader != null) stringReader.Dispose();
            }
        }
    }
}