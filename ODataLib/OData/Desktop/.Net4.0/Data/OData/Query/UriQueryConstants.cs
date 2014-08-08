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

namespace Microsoft.Data.OData.Query
{
    /// <summary>
    /// Constant values related to the URI query syntax.
    /// </summary>
    internal static class UriQueryConstants
    {
        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        internal const string MetadataSegment = "$metadata";

        /// <summary>A segment name in a URI that indicates a plain primitive value is being requested.</summary>
        internal const string ValueSegment = "$value";

        /// <summary>A segment name in a URI that indicates batch is being requested.</summary>
        internal const string BatchSegment = "$batch";

        /// <summary>A segment name in a URI that indicates that this is an entity reference link operation.</summary>
        internal const string LinkSegment = "$links";

        /// <summary>A segment name in a URI that indicates that this is a count operation.</summary>
        internal const string CountSegment = "$count";

        /// <summary>A filter query option name.</summary>
        internal const string FilterQueryOption = "$filter";

        /// <summary>An order by query option name.</summary>
        internal const string OrderByQueryOption = "$orderby";

        /// <summary>A select query option name.</summary>
        internal const string SelectQueryOption = "$select";

        /// <summary>An expand query option name.</summary>
        internal const string ExpandQueryOption = "$expand";

        /// <summary>A skip query option name.</summary>
        internal const string SkipQueryOption = "$skip";

        /// <summary>A valid value to denote all-properties access.</summary>
        internal const string Star = "*";

        /// <summary>A top query option name.</summary>
        internal const string TopQueryOption = "$top";

        /// <summary>A inline-count query option name.</summary>
        internal const string InlineCountQueryOption = "$inlinecount";

        /// <summary>A format query option name.</summary>
        internal const string FormatQueryOption = "$format";
    }
}
