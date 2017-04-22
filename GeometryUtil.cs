using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GISCore
{
    public static class GeometryUtil
    {
        /// <summary>
        /// This funtion will return an IPoint object from the input parameters and spatial ref
        /// </summary>
        /// <param name="x">the x coord (map coordinate)</param>
        /// <param name="y">the y coord (map coordinate)</param>
        /// <param name="spatialRef">the spatial ref to use for the new point</param>
        /// <returns>IPoint object</returns>
        public static IPoint ReturnPoint(double x, double y, ISpatialReference spatialRef)
        {
            IPoint point = new PointClass
            {
                X = x,
                Y = y
            };
            if (spatialRef != null) point.SpatialReference = spatialRef;

            return point;
        }

        /// <summary>
        /// This function takes in the envelope coordinates and returns an IEnvelope object
        /// </summary>
        /// <param name="xMin">the map coordinate xmin</param>
        /// <param name="yMin">the map coordinate ymin</param>
        /// <param name="xMax">the map coordinate xmax</param>
        /// <param name="yMax">the map coordinate ymax</param>
        /// <returns>IEnvelope object</returns>
        public static IEnvelope ReturnEnvelope(double xMin, double yMin, double xMax, double yMax)
        {
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(xMin, yMin, xMax, yMax);
            return envelope;
        }

        /// <summary>
        /// This function will zoom the map to point location at a specific extent
        /// </summary>
        /// <param name="map">the map</param>
        /// <param name="inPoint">the point to zoom to</param>
        /// <param name="xminFactor">the xmin map coordinate for the extent</param>
        /// <param name="yminFactor">the ymin map coordinate for the extent</param>
        /// <param name="xmaxFactor">the xmax map coordinate for the extent</param>
        /// <param name="ymaxFactor">the ymax map coordinate for the extent</param>
        public static void ZoomToPoint(IMap map, IPoint inPoint, double xminFactor,double yminFactor,double xmaxFactor,double ymaxFactor)
        {
            IActiveView activeView = map as IActiveView;
            if (activeView != null)
            {
                activeView.Extent = ReturnEnvelope(inPoint.X - xminFactor, inPoint.Y - yminFactor, inPoint.X + xmaxFactor, inPoint.Y + ymaxFactor);
                activeView.Extent.CenterAt(inPoint);
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }

        /// <summary>
        /// This function will return a centroid point from an input polygon feature
        /// </summary>
        /// <param name="inFeature"></param>
        /// <returns></returns>
        public static IPoint ReturnCentroidOfPoly(IFeature inFeature)
        {
            IPoint retVal = new ESRI.ArcGIS.Geometry.Point();
            IArea area = inFeature.Shape as IArea;
            area.QueryCentroid(retVal);

            return retVal;
        }
    }
}
