using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace GISCore
{
    public static class LayerUtil
    {

        /// <summary>
        /// This function will check to make sure that the passed in layer is a valid SDE layer.
        /// It ensures that the layer is not:
        ///      and that it is not 
        ///         1). an XY event layer
        ///         2). in a relationship with a RESPONDER layer
        /// </summary>
        /// <param name="pLayer">the layer to validate</param>
        /// <returns>true / false (is valid / not valid)</returns>
        public static bool IsValidGisLayer(ILayer pLayer)
        {
            if (pLayer.Valid && pLayer is IGeoFeatureLayer)
            {
                // Skip the feature if it is an XY Event Source
                IGeoFeatureLayer pGfLayer = pLayer as IGeoFeatureLayer;
                if (pGfLayer.FeatureClass is IXYEventSource)
                    return false;

                // Skip the feature if it is joined to a Responder table
                IDisplayTable pDispTable = pLayer as IDisplayTable;
                if (pDispTable != null)
                {
                    ITable pTable = pDispTable.DisplayTable;
                    var table = pTable as IRelQueryTable;
                    if (table != null)
                    {
                        IRelQueryTable pRelQueryTable = table;
                        IObjectClass pClass = pRelQueryTable.DestinationTable as IObjectClass;
                        if (pClass == null)
                            return false;
                    }
                }
                //// See if the feature name is for a Service Location
                //string sName = pGfLayer.FeatureClass.AliasName;
                //if (sName == layerName)
                return true;
            }
            return false;
        }

        /// <summary>
        /// This function will return an ILayer object from a map by searching for its layerName
        /// </summary>
        /// <param name="map">The map to search</param>
        /// <param name="layerName">the layer name to find</param>
        /// <returns>an ILayer object --> the layer or null</returns>
        public static ILayer ReturnLayerByName(IMap map, string layerName)
        {
            IEnumLayer pEnumLayer = map.get_Layers();
            ILayer pLayer = pEnumLayer.Next();

            while (pLayer != null)
            {
                if (pLayer.Name == layerName)
                {
                    return pLayer;
                }
               pLayer = pEnumLayer.Next();
            }
            return null;
        }

        /// <summary>
        /// This function will search the map for a service location and then return it
        /// </summary>
        /// <param name="map">the map</param>
        /// <returns>the service location layer</returns>
        public static ILayer ReturnServiceLocationLayerByUtility(IMap map)
        {
            IEnumLayer enumLayer = map.get_Layers(null, true);
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                if (IsValidGisLayer(layer) && layer is IGeoFeatureLayer)
                {
                    string sName = ((IDataset)layer).BrowseName;

                    if (sName == "Service")
                    {
                        return layer;
                    }
                } 
                layer = enumLayer.Next();
            }
            return null;
        }

        /// <summary>
        /// This function will search the map for a service and then return it
        /// </summary>
        /// <param name="map">the map</param>
       /// <returns>the service  layer</returns>
        public static ILayer ReturnServiceLocationLayerByUtility(IMap map)
        {
            IEnumLayer enumLayer = map.get_Layers(null, true);
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                if (IsValidGisLayer(layer) && layer is IGeoFeatureLayer)
                {
                    string sName = ((IDataset)layer).BrowseName;

                        if (sName == "Service")
                        {
                            return layer;
                        }
                }
                layer = enumLayer.Next();
            }
            return null;
        }

        /// <summary>
        /// This function return the layer that matches the given feature name.
        /// The feature name is case sensitive and should be equal to the value (excluding the
        ///     schema owner) displayed in the Name field shown on the General tab when you examine
        ///     the properties of the feature in ArcCatalog. It is also the value shown as the
        ///     Feature Class on the Source tab when you examine the properties of a layer in
        ///     ArcMap (again, excluding the schema owner). Returns null if no layer is found.
        /// </summary>
        /// <param name="featureName">the feature class name to find</param>
        /// <param name="map">the map</param>
        /// <returns>the layer</returns>
        public static ILayer GetLayerForFeatureClass(string featureName, IMap map)
        {
            try
            {
                if (map.LayerCount == 0)
                    return null;

                string sFeatureName;
                IEnumLayer pEnumLayer = map.get_Layers();
                ILayer pLayer = pEnumLayer.Next();
                while (pLayer != null)
                {
                    if (IsValidGisLayer(pLayer))
                    {
                        IDataset pDataset = (IDataset)pLayer;
                        esriWorkspaceType pWSType = pDataset.Workspace.WorkspaceFactory.WorkspaceType;
                        if (pWSType != esriWorkspaceType.esriRemoteDatabaseWorkspace)
                            sFeatureName = featureName;
                        else
                            sFeatureName = featureName;
                        if (pDataset.BrowseName.ToUpper() == sFeatureName.ToUpper())
                            return pLayer;
                    }
                    pLayer = pEnumLayer.Next();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// This function will select a feature on the map
        /// </summary>
        /// <param name="map">the map</param>
        /// <param name="gfLayer">the layer on which to select the feature</param>
        /// <param name="selectedFeature">the feature that you would like to select</param>
        public static void SelectFeature(IMap map,IGeoFeatureLayer gfLayer, IFeature selectedFeature)
        {
            IActiveView activeView = map as IActiveView;
            if (activeView != null)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);  // do refresh to get ready for selection
                map.SelectFeature(gfLayer, selectedFeature);
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);  // do refresh after selection ... yes, I know, but this is AO.
            }
        }
    }
}
