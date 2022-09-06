/*
Created by: X
http://www.createdbyx.com/

The XMLDocContentProcessor library is licenced under the 
Creative Commons Attribution-NonCommercial-ShareAlike 2.5 Canada
http://creativecommons.org/licenses/by-nc-sa/2.5/ca/
*/

namespace XMLDocContentProcessor
{
    /// <summary>
    /// Provides a simple class for storing string data.
    /// </summary>
    public class DataContainer
    {
        private string data;

        public DataContainer(string data)
        {
            this.data = data;
        }

        public string Data
        {
            get { return data; }
        }
    }
}
