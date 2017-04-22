using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace GISCore
{
    public static class SymbolUtil
    {
        /// <summary>
        /// This function will return a simple fill symbol filled with the color that is passed in.
        /// </summary>
        /// <param name="color">The ESRI.ArcGIS.Display.IColor object to use as the color</param>
        /// <returns>ISimpleFillSymbol as ISymbol</returns>
        public static ISymbol ReturnSimpleFillSymbol(IColor color)
        {
            ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass
            {
                Color = color
            };

            ISymbol symbol = simpleFillSymbol as ISymbol;
            symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            return symbol;
        }

        /// <summary>
        /// This function will return a simple line symbol with the width and color that is passed in.
        /// </summary>
        /// <param name="color">The ESRI.ArcGIS.Display.IColor object to use as the color</param>
        /// <param name="width">The width of the line</param>
        /// <returns>ISimpleLineSymbol as ISymbol</returns>
        public static ISymbol ReturnLineSymbol(IColor color, int width)
        {
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass
            {
                Width = width,
                Color = color
            };

            ISymbol symbol = simpleLineSymbol as ISymbol; // Dynamic Cast
            symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            return symbol;
        }

        /// <summary>
        /// This function will return a simple marker symbol with the size, color and style that is passed in.
        /// </summary>
        /// <param name="style">the marker symbol style to use from the esriSimpleMarkerStyle enum (ieL: circle, cross, diamond, etc)</param>
        /// <param name="size">the size of the symbol to be rendered</param>
        /// <param name="color">The ESRI.ArcGIS.Display.IColor object to use as the color</param>
        /// <returns>ISimpleMarkerSymbol as ISymbol</returns>
        public static ISymbol ReturnSimpleMarkerSymbol(esriSimpleMarkerStyle style, int size, IColor color)
        {
            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass
            {
                Style = style,
                Size = size,
                Color = color
            };

            ISymbol symbol = simpleMarkerSymbol as ISymbol; 
            symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            return symbol;
        }

        /// <summary>
        /// This function will return a picture marker symbol using the bmp, transparency color and size which are passed in
        /// </summary>
        /// <param name="bmp">the bitmap image to use</param>
        /// <param name="rgbColor">the color to use a transparency</param>
        /// <param name="dblSize">the size to make the picture marker symbol</param>
        /// <returns>a picture marker symbol (IPictureMarkerSymbol)</returns>
        public static IPictureMarkerSymbol ReturnPictureMarkerSymbol(System.Drawing.Bitmap bmp, IRgbColor rgbColor, double dblSize)
        {
            // Create the Marker and assign properties.
            IPictureMarkerSymbol pictureMarkerSymbol = new PictureMarkerSymbolClass
            {
                Picture = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(bmp) as stdole.IPictureDisp,
                Angle = 0,
                BitmapTransparencyColor = rgbColor,
                Size = dblSize,
                XOffset = 0,
                YOffset = 0
            };
            return pictureMarkerSymbol;
        }

        /// <summary>
        /// this function will return an ESRI rgb color object (ESRI.ArcGIS.Display.IRgbColor)
        /// </summary>
        /// <param name="red">the value to use for Red</param>
        /// <param name="green">the value to use for Green</param>
        /// <param name="blue">the value to use for Blue</param>
        /// <returns>an ESRI.ArcGIS.Display.IRgbColor object</returns>
        public static IRgbColor ReturnRGB(int red, int green, int blue)
        {
            IRgbColor rgbColor = new RgbColorClass
            {
                Red = red,
                Green = green,
                Blue = blue,
                UseWindowsDithering = true
            };

            return rgbColor;
        }

        /// <summary>
        /// This function takes in two ESRI.ArcGIS.Display.IRgbColor object for the color and outline, then returns a marker element
        /// </summary>
        /// <param name="color">the color to make the marker element</param>
        /// <param name="outline">the outline color of the marker element</param>
        /// <returns>a simple marker symbol as a marker element</returns>
        public static IMarkerElement ReturnMarkerElement(IRgbColor color, IRgbColor outline)
        {
            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass
            {
                Color = color,
                Outline = true,
                OutlineColor = outline,
                Size = 30,
                Style = esriSimpleMarkerStyle.esriSMSCircle
            };

            IMarkerElement markerElement = new MarkerElementClass
            {
                    Symbol = simpleMarkerSymbol
            };

            return markerElement;
        }

        /// <summary>
        /// This function will flash the geometry on the map as an indicator of its location
        /// </summary>
        /// <param name="activeView">the map's active view</param>
        /// <param name="geom">the IGeometry object to flash</param>
        /// <param name="size">the size to make the flashing object</param>
        /// <param name="interval">how long to keep the flash object visible</param>
        /// <param name="numFlash">the number of times to flash</param>
        public static void FlashGeometry(IActiveView activeView, IGeometry geom, double size, short interval, short numFlash)
        {
            if (activeView != null)
            {
                // this will refresh the display, so that the zoom happens before the flash
                activeView.ScreenDisplay.UpdateWindow();

                IColor pColor = new RgbColorClass();
                pColor.RGB = 255;
                IMMMapUtilities pMapUtils = new mmMapUtilsClass();
                pMapUtils.FlashGeometry(geom, activeView.ScreenDisplay, pColor, size, interval, numFlash);
            }
        }

        /// <summary>
        /// This function will clear all map selections from the map
        /// </summary>
        /// <param name="map">the map</param>
        public static void ClearMapSelection(IMap map)
        {
            IActiveView pActiveView = map as IActiveView;
            if (map.SelectionCount > 0)
            {
                if (pActiveView != null)
                {
                    pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    map.ClearSelection();
                    pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                }

            }
        }
    }
}
