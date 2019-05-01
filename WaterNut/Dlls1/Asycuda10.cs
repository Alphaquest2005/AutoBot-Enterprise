using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Asycuda10
{
    public partial class ASYCUDA
    {
        public static bool CanLoadFromFile(string fileName)
        {
            return CanLoadFromFile(fileName, Encoding.UTF8);
        }

        public static bool CanLoadFromFile(string fileName, System.Text.Encoding encoding)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null,@"Asycuda10.xsd");

            XDocument doc = XDocument.Load(fileName);
            string msg = "";
            doc.Validate(schemas, (o, e) =>
            {
                msg += e.Message + Environment.NewLine;
            });
            if (string.IsNullOrEmpty(msg)) return true;
            return false;
        }

        // Display any warnings or errors.
       

        public static bool CanDeserialize(string xml)
        {
            System.IO.StringReader stringReader = null;
            try
            {
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema; 
                stringReader = new System.IO.StringReader(xml);
                return Serializer.CanDeserialize(System.Xml.XmlReader.Create(stringReader,settings));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }
    }
}
