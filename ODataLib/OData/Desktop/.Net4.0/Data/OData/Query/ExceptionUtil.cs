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
    /// Helper class for throwing exceptions during URI parsing.
    /// </summary>
    internal static class ExceptionUtil
    {
        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="identifier">segment identifier information for which resource was not found.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static ODataException CreateResourceNotFound(string identifier)
        {
            DebugUtils.CheckNoExternalCallers();

            // 404: Not Found
            return ResourceNotFoundError(Strings.RequestUriProcessor_ResourceNotFound(identifier));
        }

        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="errorMessage">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static ODataException ResourceNotFoundError(string errorMessage)
        {
            DebugUtils.CheckNoExternalCallers();

            // 404: Not Found
            return new ODataUnrecognizedPathException(errorMessage);
        }

        /// <summary>Creates a new exception to indicate a syntax error.</summary>
        /// <returns>A new exception to indicate a syntax error.</returns>
        internal static ODataException CreateSyntaxError()
        {
            DebugUtils.CheckNoExternalCallers();
            return CreateBadRequestError(Strings.RequestUriProcessor_SyntaxError);
        }

        /// <summary>
        /// Creates a new exception to indicate BadRequest error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a bad request error.</returns>
        internal static ODataException CreateBadRequestError(string message)
        {
            DebugUtils.CheckNoExternalCallers();

            // 400 - Bad Request
            return new ODataException(message);
        }

        /// <summary>Checks the specific value for syntax validity.</summary>
        /// <param name="valid">Whether syntax is valid.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void ThrowSyntaxErrorIfNotValid(bool valid)
        {
            DebugUtils.CheckNoExternalCallers();
            if (!valid)
            {
                throw CreateSyntaxError();
            }
        }

        /// <summary>Checks the specifid value for syntax validity.</summary>
        /// <param name="resourceExists">Whether syntax is valid.</param>
        /// <param name="identifier">segment indentifier for which the resource was null.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void ThrowIfResourceDoesNotExist(bool resourceExists, string identifier)
        {
            DebugUtils.CheckNoExternalCallers();
            if (!resourceExists)
            {
                throw CreateResourceNotFound(identifier);
            }
        }
    }
}
