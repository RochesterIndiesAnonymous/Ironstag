/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/
using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using XMLDocContentProcessor.Properties;

namespace XMLDocContentProcessor
{

    [ContentImporter(".xml", DefaultProcessor = "XDocumentProcessor", DisplayName = "XDocument Importer - Createdbyx.com")]
    public class XDocumentImporter : ContentImporter<DataContainer>
    {
        public override DataContainer Import(string filename, ContentImporterContext context)
        {
            // declare a varible for storing the data contained in the file
            string data = string.Empty;

            // report that asset processing has begun
            context.Logger.PushFile(filename);

            // try to read the file
            try
            {
                // read all file data as a string
                data = File.ReadAllText(filename);
            }
            catch (Exception ex)
            {
                // throw a InvalidContentException exception that reports the problem
                throw new InvalidContentException(Resources.CouldNotReadFile,
                                  new ContentIdentity(Path.GetFullPath(filename)), ex);
            }
            finally
            {
                // report that asset processing has completed
                context.Logger.PopFile();
            }

            // return the string data that was read as a DataContainer object.
            return new DataContainer(data);
        }
    }
}
