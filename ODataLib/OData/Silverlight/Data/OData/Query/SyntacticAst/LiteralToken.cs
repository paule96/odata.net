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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using Microsoft.Data.OData.Query.SemanticAst;

    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a literal value.
    /// </summary>
    internal sealed class LiteralToken : QueryToken
    {
        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        private readonly string originalText;

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Create a new LiteralToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        public LiteralToken(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// Create a new LiteralToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        internal LiteralToken(object value, string originalText)
            : this(value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.originalText = originalText;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Literal; }
        }

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        internal string OriginalText
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers(); 
                return this.originalText;
            }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
