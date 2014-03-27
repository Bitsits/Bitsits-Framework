using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace GameDataLibrary
{
    public class GenerateData
    {
        public GenerateData()
        {
            object i = new object();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create("Item.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, i, "Item.xml");
            }
        }
    }
}
