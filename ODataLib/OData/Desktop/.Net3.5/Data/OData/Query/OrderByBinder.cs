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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Class to handle the binding of orderby tokens.
    /// </summary>
    internal sealed class OrderByBinder
    {
        /// <summary>
        /// Method to use to visit the token tree and bind the tokens recursively.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Creates an OrderByBinder
        /// </summary>
        /// <param name="bindMethod">Method to use to visit the token tree and bind the tokens recursively.</param>
        internal OrderByBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Processes the order-by tokens of a entityCollection (if any).
        /// </summary>
        /// <param name="state">State to use for binding.</param>
        /// <param name="orderByTokens">The order-by tokens to bind.</param>
        /// <returns>An OrderByClause representing the orderby statements expressed in the tokens.</returns>
        internal OrderByClause BindOrderBy(BindingState state, IEnumerable<OrderByToken> orderByTokens)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(state, "state");
            ExceptionUtils.CheckArgumentNotNull(orderByTokens, "orderByTokens");

            OrderByClause orderByClause = null;

            // Go through the orderby tokens starting from the last one
            foreach (OrderByToken orderByToken in orderByTokens.Reverse())
            {
                orderByClause = this.ProcessSingleOrderBy(state, orderByClause, orderByToken);
            }

            return orderByClause;
        }

        /// <summary>
        /// Processes the specified order-by token.
        /// </summary>
        /// <param name="state">State to use for binding.</param>
        /// <param name="thenBy"> The next OrderBy node, or null if there is no orderby after this.</param>
        /// <param name="orderByToken">The order-by token to bind.</param>
        /// <returns>Returns the combined entityCollection including the ordering.</returns>
        private OrderByClause ProcessSingleOrderBy(BindingState state, OrderByClause thenBy, OrderByToken orderByToken)
        {
            ExceptionUtils.CheckArgumentNotNull(state, "state");
            ExceptionUtils.CheckArgumentNotNull(orderByToken, "orderByToken");

            QueryNode expressionNode = this.bindMethod(orderByToken.Expression);

            // TODO: shall we really restrict order-by expressions to primitive types?
            SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
            if (expressionResultNode == null ||
                (expressionResultNode.TypeReference != null && !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind()))
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
            }

            OrderByClause orderByNode = new OrderByClause(
                thenBy, 
                expressionResultNode,
                orderByToken.Direction,
                state.ImplicitRangeVariable);
            
            return orderByNode;
        }
    }
}
