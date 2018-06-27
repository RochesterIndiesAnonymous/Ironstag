/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;

namespace XMLDocContentProcessor
{
    public class XDocumentReader : ContentTypeReader<XDocument>
    {
        protected override XDocument Read(ContentReader input, XDocument existingInstance)
        {

            // calc the number of characters that will be read
            int count = input.ReadInt32();
         
            // create a string reader to store the data that will be read
            StringBuilder sb = new StringBuilder(count, count);

            // read "count" number of characters from the input object and append the data
            // to the StringBuilder varible
            sb.Append(input.ReadChars(count));

            // create a new XmlDocument object
            XDocument doc = XDocument.Parse(sb.ToString());

            // return the reference the Doc varible
            return doc;
        }
    }
}
