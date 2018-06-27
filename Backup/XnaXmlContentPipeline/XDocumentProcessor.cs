/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/
using Microsoft.Xna.Framework.Content.Pipeline;

namespace XMLDocContentProcessor
{
    [ContentProcessor(DisplayName = "XDocument Processor - Createdbyx.com")]
    public class XDocumentProcessor : ContentProcessor<DataContainer, DataContainer>
    {
        public override DataContainer Process(DataContainer input, ContentProcessorContext context)
        {
            // simply return the input object
            return input;
        }
    }
}
