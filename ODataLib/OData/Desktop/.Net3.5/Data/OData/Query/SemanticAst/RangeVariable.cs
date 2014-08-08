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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using System;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// A RangeVariable, which represents an iterator variable either over a collection, either of entities or not.
    /// Exists outside of the main SemanticAST, but hooked in via a RangeVariableReferenceNode (either Non-Entity or Entity).
    /// </summary>
    public abstract class RangeVariable : ODataAnnotatable
    {
        /// <summary>
        /// Gets the name of the associated rangeVariable.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the type of entity referenced by this rangeVariable
        /// </summary>
        public abstract IEdmTypeReference TypeReference { get; }

        /// <summary>
        /// Gets the kind of this rangeVariable.
        /// </summary>
        public abstract int Kind { get; }
    }
}
