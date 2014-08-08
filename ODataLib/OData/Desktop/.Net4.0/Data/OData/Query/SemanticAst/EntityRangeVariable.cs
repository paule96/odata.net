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
    using Microsoft.Data.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// A RangeVariable inside an any or all expression that doesn't refer to an entity set
    /// </summary>
    public sealed class EntityRangeVariable : RangeVariable
    {
        /// <summary>
        ///  The name of the associated any/all parameter (null if none)
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The Entity collection that this rangeVariable node iterates over
        /// </summary>
        private readonly EntityCollectionNode entityCollectionNode;

        /// <summary>
        /// The Entity set of the collection this node iterates over.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The entity type of each item in the collection that this range variable iterates over.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// Creates a <see cref="EntityRangeVariable"/>.
        /// </summary>
        /// <param name="name"> The name of the associated any/all parameter (null if none)</param>
        /// <param name="entityType">The entity type of each item in the collection that this range variable iterates over.</param>
        /// <param name="entityCollectionNode">The Entity collection that this rangeVariable node iterates over</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or entityType is null.</exception>
        public EntityRangeVariable(string name, IEdmEntityTypeReference entityType, EntityCollectionNode entityCollectionNode)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.name = name;
            this.entityTypeReference = entityType;
            this.entityCollectionNode = entityCollectionNode;
            this.entitySet = entityCollectionNode != null ? entityCollectionNode.EntitySet : null;
        }

        /// <summary>
        /// Creates a <see cref="EntityRangeVariable"/>.
        /// </summary>
        /// <param name="name"> The name of the associated any/all parameter (null if none)</param>
        /// <param name="entityType">The entity type of each item in the collection that this range variable iterates over.</param>
        /// <param name="entitySet">The Entity set of the collection this node iterates over.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or entityType is null.</exception>
        public EntityRangeVariable(string name, IEdmEntityTypeReference entityType, IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.name = name;
            this.entityTypeReference = entityType;
            this.entityCollectionNode = null;
            this.entitySet = entitySet;
        }

        /// <summary>
        /// Gets the name of the associated any/all parameter (null if none)
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the Entity collection that this rangeVariable node iterates over
        /// </summary>
        public EntityCollectionNode EntityCollectionNode
        {
            get { return this.entityCollectionNode; }
        }

        /// <summary>
        /// Gets the Entity set of the collection this node iterates over.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the entity type of each item in the collection that this range variable iterates over.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the entity type of each item in the collection that this range variable iterates over.
        /// </summary>
        public IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public override int Kind
        {
            get { return RangeVariableKind.Entity; }
        }
    }
}
