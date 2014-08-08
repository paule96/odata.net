//   WCF Data Services Entity Framework Provider for OData ver. 1.0.0
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

namespace System.Data.Services.Providers
{
    #region Namespaces

    using System.Collections.Generic;
#if EF6Provider
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

    #endregion

    /// <summary>
    /// Implemented by a class that encapsulates a data service provider's metadata representation of a member on a type.
    /// </summary>
    internal interface IProviderMember
    {
        /// <summary>
        /// BuiltInTypeKind for the member's type.
        /// DEVNOTE (sparra): This currently has a dependency on the Entity Framework's enum, but this dependency should be
        /// removed in subsequent refactorings of this class that expand the usage beyond the ObjectContextServiceProvider.
        /// </summary>
        BuiltInTypeKind EdmTypeKind { get; }

        /// <summary>
        /// Name of the member without its namespace.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True if this member is a key on it's declaring type, otherwise false.
        /// </summary>
        bool IsKey { get; }

        /// <summary>
        /// EDM name for the member's type.
        /// </summary>
        string EdmTypeName { get; }

        /// <summary>
        /// MimeType for the member.
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Returns the entity type of the items in the collection if this member is a collection type, otherwise null.
        /// DEVNOTE (sparra): This currently has a dependency on the Entity Framework's EntityType class, but this dependency
        /// should be removed in subsequent refactorings of this class that expand the usage beyond the ObjectContextServiceProvider.
        /// </summary>
        EntityType CollectionItemType { get; }

        /// <summary>
        /// Return the list of the metadata properties for the member.
        /// </summary>
        IEnumerable<MetadataProperty> MetadataProperties { get; }

        /// <summary>
        /// Return the list of facets for the member.
        /// </summary>
        IEnumerable<Facet> Facets { get; }
    }
}
