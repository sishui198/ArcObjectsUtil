using System;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GISCore
{
    public static class QueryUtil
    {
        /// <summary>
        /// This function will perform a query filter on a feature class using the input where clause.
        /// </summary>
        /// <param name="where">the where clause to use for the filter (ie: "ID = 2")</param>
        /// <param name="subFields">You can use the SubFields property to improve performance when using query filters. The performance gain comes from just fetching the field values 
        ///         that you require rather than all the data for each row. The default value for SubFields is "*", which indicates that all field values will be returned. Setting it 
        ///         back to this original (default) "*" can be done either by setting it to "*" or to "".
        ///         The SubFields property should be a comma-delimited list of the columns that are required. For example, to retrieve two fields named "OBJECTID" and "NAME", the property 
        ///         should be set to "OBJECTID, NAME" (the space is optional).
        ///         It isnt necessary to set the subfields when the query filter is used in a context in which no attribute values are fetched, for example, when selecting features
        /// </param>
        /// <param name="fc">the feature class to perform the query on</param>
        /// <returns>an ICursor object containing the results of the query</returns>
        public static ICursor PerfomQueryFilterByFeatureClass(string where, string subFields, IFeatureClass fc)
        {
            try
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = where;
                if (!string.IsNullOrEmpty(subFields))
                    qf.SubFields = subFields;
                return (ICursor) fc.Search(qf, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// This function performs a query filter on a table using the input where clause
        /// </summary>
        /// <param name="where">the where clause to use (ie: "ID = 2")</param>
        /// <param name="subFields">You can use the SubFields property to improve performance when using query filters. The performance gain comes from just fetching the field values 
        ///         that you require rather than all the data for each row. The default value for SubFields is "*", which indicates that all field values will be returned. Setting it 
        ///         back to this original (default) "*" can be done either by setting it to "*" or to "".
        ///         The SubFields property should be a comma-delimited list of the columns that are required. For example, to retrieve two fields named "OBJECTID" and "NAME", the property 
        ///         should be set to "OBJECTID, NAME" (the space is optional).
        ///         It isnt necessary to set the subfields when the query filter is used in a context in which no attribute values are fetched, for example, when selecting features
        /// </param>
        /// <param name="tbl">the table to use</param>
        /// <returns>an ICursor object containing the results of the query</returns>
        public static ICursor PerfomQueryFilterByTable(string where, string subFields, ITable tbl)
        {
            try
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = where;
                if (!string.IsNullOrEmpty(subFields))
                    qf.SubFields = subFields;
                return tbl.Search(qf, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// This function will find all the features which intersect, but which are exculed by the "WHERE" clause
        /// For example --> find all the streets that intersect, which do NOT have the same name as the selected street
        /// </summary>
        /// <param name="inFeature">the feature to use as the intersecting feature</param>
        /// <param name="fc">the feature class to intersect</param>
        /// <param name="where">the where clause to use as the filter</param>
        /// <returns>a feature cursor of resulting intersecting features</returns>
        public static IFeatureCursor PerformIntersectSpatialFilter(IFeature inFeature, IFeatureClass fc, string where)
        {
            try
            {
                ISpatialFilter pSpatialFilter = new SpatialFilterClass
                {
                    Geometry = inFeature.Shape,
                    GeometryField = fc.ShapeFieldName,
                    WhereClause = where,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                };
                return fc.Search(pSpatialFilter, false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// This function will return a point collection for intersections along a curve
        /// </summary>
        /// <param name="inFeature">the feature to intersect</param>
        /// <param name="inTopOp">the curve to use to intersect the feature</param>
        /// <returns>a point collection of intersecting points along the infeature</returns>
        public static IPointCollection PerformTopologicalIntersect(IFeature inFeature, ITopologicalOperator inTopOp)
        {
            try
            {
                IPolycurve pFeatureShape = inFeature.Shape as IPolycurve;
                IMultipoint pIntersection =
                    (IMultipoint) inTopOp.Intersect(pFeatureShape, esriGeometryDimension.esriGeometry0Dimension);
                return (IPointCollection) pIntersection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// This function use the "OpenRelationshipClass" function to open a relationship class in the passed in workspace. Then the relationship class uses the 
        /// "GetObjectsRelated" to return an ISet of the objects that are related to the specified input object.
        /// </summary>
        /// <param name="relatedTable">the relateionship table</param>
        /// <param name="obj">the object to find relationships for</param>
        /// <param name="fws">the feature workspace to work in</param>
        /// <returns>ISet of all relationships for passed in object</returns>
        public static ISet PerformRelatedObjectQuery(string relatedTable, IObject obj, IFeatureWorkspace fws)
        {
            try
            {
                IRelationshipClass pRelClass = fws.OpenRelationshipClass(relatedTable);
                return pRelClass.GetObjectsRelatedToObject(obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
