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

namespace System.Data.Services
{
    #region Namespaces

    using System;
    using System.ServiceModel.Web;

    #endregion Namespaces

    /// <summary>
    /// This structure supports the .NET Framework infrastructure and is 
    /// not intended to be used directly from your code.
    /// </summary>
    /// <internal>
    /// Provides a host for services of type DataService.
    /// </internal>
    [CLSCompliant(false)]
    public class DataServiceHost : WebServiceHost
    {
        /// <summary>Instantiates <see cref="T:System.Data.Services.DataServiceHost" /> for WCF Data Services.</summary>
        /// <param name="serviceType">Identifies the WCF Data Services to the host.</param>
        /// <param name="baseAddresses">The URI of the host.</param>
        public DataServiceHost(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
