/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/
using System;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using XMLDocContentProcessor.Properties;

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
            XDocument doc;

            // try loading the data
            try
            {
                // attempt to load the data stored in the sb object into the Doc object by
                // wrapping the data in a StringReader object
                doc = XDocument.Parse(sb.ToString());
            }
            catch (Exception ex)
            {
                // throw a InvalidContentException exception reporting the error
                throw new InvalidContentException(Resources.BadFileData, ex);
            }

            // return the reference the Doc varible
            return doc;
        }
    }
}
