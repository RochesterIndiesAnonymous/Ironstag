/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Xml;
using System.Xml.Linq;

namespace XMLDocContentProcessor
{
    [ContentTypeWriter]
    public class XDocumentWriter : ContentTypeWriter<DataContainer>
    {

        protected override void Write(ContentWriter output, DataContainer value)
        {
            // write the string data stored in value as a character array
            output.Write(value.Data.Length);
            output.Write(value.Data.ToCharArray());
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            // return the assembly-qualified name for the XmlDocument type.
            return typeof(XDocument).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // return the assembly-qualified name for the XMLDocumentReader type.
            return typeof(XDocumentReader).AssemblyQualifiedName;
        }
    }
}