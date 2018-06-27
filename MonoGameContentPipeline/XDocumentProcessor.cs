/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGameContentPipeline
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
