//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Spatial
{
    using System;
    using System.Spatial;

    /// <summary>
    /// Adapter from the type washed API to Geography, where it assumes that coord1 is Longitude.
    /// </summary>
    internal class TypeWashedToGeographyLongLatPipeline : TypeWashedPipeline
    {
        /// <summary>
        /// The pipeline to redirect the calls to
        /// </summary>
        private readonly GeographyPipeline output;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The pipeline to redirect the calls to</param>
        public TypeWashedToGeographyLongLatPipeline(SpatialPipeline output)
        {
            this.output = output;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is geography.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is geography; otherwise, <c>false</c>.
        /// </value>
        public override bool IsGeography
        {
            get { return true; }
        }

        /// <summary>
        /// Set the coordinate system based on the given ID
        /// </summary>
        /// <param name="epsgId">The coordinate system ID to set. Null indicates the default should be used</param>
        internal override void SetCoordinateSystem(int? epsgId)
        {
            var coordinateSystem = CoordinateSystem.Geography(epsgId);
            this.output.SetCoordinateSystem(coordinateSystem);
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        internal override void Reset()
        {
            output.Reset();
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        internal override void BeginGeo(SpatialType type)
        {
            this.output.BeginGeography(type);
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="coordinate1">1st Coordinate</param>
        /// <param name="coordinate2">2nd Coordinate</param>
        /// <param name="coordinate3">3rd Coordinate</param>
        /// <param name="coordinate4">4th Coordinate</param>
        internal override void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            this.output.BeginFigure(new GeographyPosition(coordinate2, coordinate1, coordinate3, coordinate4));
        }

        /// <summary>
        /// Draw a line to a point in the specified coordinate
        /// </summary>
        /// <param name="coordinate1">1st Coordinate</param>
        /// <param name="coordinate2">2nd Coordinate</param>
        /// <param name="coordinate3">3rd Coordinate</param>
        /// <param name="coordinate4">4th Coordinate</param>
        internal override void LineTo(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            this.output.LineTo(new GeographyPosition(coordinate2, coordinate1, coordinate3, coordinate4));
        }

        /// <summary>
        /// Ends the current figure
        /// </summary>
        internal override void EndFigure()
        {
            this.output.EndFigure();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        internal override void EndGeo()
        {
            this.output.EndGeography();
        }
    }
}
