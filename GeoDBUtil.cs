using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

namespace GISCore
{
    public static class GeoDBUtil
    {
        /// <summary>
        /// This function will take in a coded value domain and a code and will return the coded value domain string
        /// </summary>
        /// <param name="CVDomain">the ICodedValueDomain value to search</param>
        /// <param name="sCode">the code that you are looking for</param>
        /// <returns>the string associated with the CVD</returns>
        public static string GetDomainDesc(ICodedValueDomain CVDomain, string sCode)
        {
            string sDesc = "";
            for (int i = 0; i < CVDomain.CodeCount; i++)
            {
                if (CVDomain.Value[i].ToString() == sCode)
                {
                    sDesc = CVDomain.Name[i];
                    i = CVDomain.CodeCount;
                }
            }
            return sDesc;
        }

        /// <summary>
        /// Connect to an enterprise geodatabase using an sde connection file.
        /// </summary>
        /// <param name="connectionFile">The sde file containing the connection parameters</param>
        /// <returns>the workspace interface for the connected datase</returns>
        public static IWorkspace ConnectWithFile(string connectionFile)
        {
            IWorkspaceFactory pWkspFact = new SdeWorkspaceFactoryClass();
            return pWkspFact.OpenFromFile(connectionFile, 0);
        }
    }
}
