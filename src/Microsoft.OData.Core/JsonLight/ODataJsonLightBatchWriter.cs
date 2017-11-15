﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class for writing OData batch messages of MIME application/json type.
    /// </summary>
    internal sealed class ODataJsonLightBatchWriter : ODataBatchWriter
    {
        #region JsonPropertyNames
        /// <summary>
        /// Camel-case property name for request Id in Json batch.
        /// </summary>
        private const string PropertyId = "id";

        /// <summary>
        /// Property name for request atomic group association in Json batch.
        /// </summary>
        private const string PropertyAtomicityGroup = "atomicityGroup";

        /// <summary>
        /// Property name for request HTTP headers in Json batch.
        /// </summary>
        private const string PropertyHeaders = "headers";

        /// <summary>
        /// Property name for request body in Json batch.
        /// </summary>
        private const string PropertyBody = "body";
        #endregion JsonPropertyNames

        #region RequestJsonPropertyNames
        /// <summary>
        /// Property name for top-level requests array in Json batch request.
        /// </summary>
        private const string PropertyRequests = "requests";

        /// <summary>
        /// Property name for request HTTP method in Json batch request.
        /// </summary>
        private const string PropertyMethod = "method";

        /// <summary>
        /// Property name for request URL in Json batch request.
        /// </summary>
        private const string PropertyUrl = "url";
        #endregion RequestJsonPropertyNames

        #region ResponseJsonPropertyNames
        /// <summary>
        /// Property name for top-level responses array in Json batch response.
        /// </summary>
        private const string PropertyResponses = "responses";

        /// <summary>
        /// Property name for response status in Json batch response.
        /// </summary>
        private const string PropertyStatus = "status";
        #endregion ResponseJsonPropertyNames

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// The auto-generated Guid for AtomicityGroup of the Json item. Should be null for Json item
        /// that doesn't belong to atomic group.
        /// </summary>
        private string atomicityGroupId = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightBatchWriter(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext)
        {
            this.jsonWriter = this.JsonLightOutputContext.JsonWriter;
        }

        /// <summary>
        /// Gets the writer's output context as the real runtime type.
        /// </summary>
        private ODataJsonLightOutputContext JsonLightOutputContext
        {
            get { return this.OutputContext as ODataJsonLightOutputContext; }
        }

        /// <summary>
        /// The message for the operation that is currently written; or null if no operation is written right now.
        /// </summary>
        private ODataBatchOperationMessage CurrentOperationMessage
        {
            get
            {
                Debug.Assert(this.CurrentOperationRequestMessage == null || this.CurrentOperationResponseMessage == null,
                    "Only request or response message can be set, not both.");
                if (this.CurrentOperationRequestMessage != null)
                {
                    Debug.Assert(!this.JsonLightOutputContext.WritingResponse, "Request message can only be set when writing request.");
                    return this.CurrentOperationRequestMessage.OperationMessage;
                }
                else if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.JsonLightOutputContext.WritingResponse, "Response message can only be set when writing response.");
                    return this.CurrentOperationResponseMessage.OperationMessage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public override void BatchOperationContentStreamRequested()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Flush the async buffered stream to the underlying message stream (if there's any)
            this.JsonLightOutputContext.FlushBuffers();

            // Set the corresponding state but we need to keep the Json batch writer around.
            this.SetState(BatchWriterState.OperationStreamRequested);
        }

#if PORTABLELIB
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the operation;
        /// null if no such action exists.
        /// </returns>
        public override Task BatchOperationContentStreamRequestedAsync()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Asynchronously flush the async buffered stream to the underlying message stream (if there's any);
            // then dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            return this.JsonLightOutputContext.FlushBuffersAsync()
                .FollowOnSuccessWith(task => this.SetState(BatchWriterState.OperationStreamRequested));
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        public override void BatchOperationContentStreamDisposed()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");

            this.SetState(BatchWriterState.OperationStreamDisposed);
            this.CurrentOperationRequestMessage = null;
            this.CurrentOperationResponseMessage = null;

            EnsurePreceedingMessageIsClosed();
        }

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        public override void OnInStreamError()
        {
            this.JsonLightOutputContext.VerifyNotDisposed();
            this.SetState(BatchWriterState.Error);
            this.JsonLightOutputContext.JsonWriter.Flush();

            // The OData protocol spec did not defined the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that WCF DS Server does serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.JsonLightOutputContext.Flush();
        }

#if PORTABLELIB
        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.JsonLightOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// Starts a new batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteStartBatchImplementation()
        {
            WriteBatchEnvelope();
            this.SetState(BatchWriterState.BatchStarted);
        }

        /// <summary>
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndBatchImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // Close the messages array
            jsonWriter.EndArrayScope();

            // Close the top level scope
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        protected override void WriteStartChangesetImplementation()
        {
            Debug.Assert(this.atomicityGroupId == null, "this.atomicityGroupId == null");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangesetStarted);
            this.atomicityGroupId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndChangesetImplementation()
        {
            Debug.Assert(this.atomicityGroupId != null, "this.atomicityGroupId != null");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangesetCompleted);
            this.atomicityGroupId = null;
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method,
        Uri uri, string contentId, BatchPayloadUriOption payloadUriOption)
        {
            // write pending message data (headers, request line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // For json batch request, content Id is required for single request or request within atomicityGroup.
            if (contentId == null)
            {
                contentId = Guid.NewGuid().ToString();
            }

            // create the new request operation
            this.CurrentOperationRequestMessage = BuildOperationRequestMessage(
                this.JsonLightOutputContext.GetOutputStream(), method, uri, contentId);

            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            this.jsonWriter.WriteName(PropertyId);
            this.jsonWriter.WriteValue(contentId);

            if (this.atomicityGroupId != null)
            {
                this.jsonWriter.WriteName(PropertyAtomicityGroup);
                this.jsonWriter.WriteValue(this.atomicityGroupId);
            }

            this.jsonWriter.WriteName(PropertyMethod);
            this.jsonWriter.WriteValue(method);

            this.jsonWriter.WriteName(PropertyUrl);
            this.jsonWriter.WriteValue(UriUtils.UriToString(uri));

            return this.CurrentOperationRequestMessage;
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch
        /// response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet header.</param>
        /// <returns>The message that can be used to rite the response operation.</returns>
        protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId)
        {
            this.WritePendingMessageData(true);

            // Url resolver: In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            //
            // ContentId: could be null from public API common for both formats, so we don't enforce non-null value for Json format
            // for sake of backward compatibility. For Json Batch response message, normally caller should use a non-null value
            // available from the request; and we generate a new one when a null value is passed in.
            this.CurrentOperationResponseMessage = BuildOperationResponseMessage(
                this.JsonLightOutputContext.GetOutputStream(),
                contentId ?? Guid.NewGuid().ToString());
            this.SetState(BatchWriterState.OperationCreated);

            // Start the Json object for the response
            this.WriteStartBoundaryForOperation();

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Verifies that the writer is not disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.JsonLightOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Writes the start boundary for an operation. This is Json start object.
        /// </summary>
        private void WriteStartBoundaryForOperation()
        {
            // Start the individual message object
            this.jsonWriter.StartObjectScope();
        }

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        private void StartBatchOperationContent()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.JsonLightOutputContext.JsonWriter != null, "Must have a Json writer!");

            // write the pending headers (if any)
            this.WritePendingMessageData(false);

            this.jsonWriter.WriteRawValue(string.Format(CultureInfo.InvariantCulture,
                "{0} \"{1}\" {2}",
                JsonConstants.ArrayElementSeparator,
                PropertyBody,
                JsonConstants.NameValueSeparator));

            // flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            this.JsonLightOutputContext.JsonWriter.Flush();
        }

        /// <summary>
        /// Write any pending data for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        private void WritePendingMessageData(bool reportMessageCompleted)
        {
            if (this.CurrentOperationMessage != null)
            {
                Debug.Assert(this.JsonLightOutputContext.JsonWriter != null,
                    "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationRequestMessage != null)
                {
                    WritePendingRequestMessageData();
                }
                else
                {
                    WritePendingResponseMessageData();
                }

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.PartHeaderProcessingCompleted();
                    this.CurrentOperationRequestMessage = null;
                    this.CurrentOperationResponseMessage = null;

                    EnsurePreceedingMessageIsClosed();
                }
            }
        }

        /// <summary>
        /// Close preceding message Json object if any.
        /// </summary>
        private void EnsurePreceedingMessageIsClosed()
        {
            // There shouldn't be any pending message object.
            Debug.Assert(this.CurrentOperationMessage == null, "this.CurrentOperationMessage == null");
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes the json format batch envelope.
        /// Always sets the isBatchEvelopeWritten flag to true before return.
        /// </summary>
        private void WriteBatchEnvelope()
        {
            // Start the top level scope
            this.jsonWriter.StartObjectScope();

            // Start the requests / responses property
            this.jsonWriter.WriteName(this.JsonLightOutputContext.WritingResponse ?  PropertyResponses : PropertyRequests);
            this.jsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Writing pending data for the current request message.
        /// </summary>
        private void WritePendingRequestMessageData()
        {
            Debug.Assert(this.CurrentOperationRequestMessage != null, "this.CurrentOperationRequestMessage != null");

            // headers property.
            this.jsonWriter.WriteName(PropertyHeaders);
            this.jsonWriter.StartObjectScope();
            IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationRequestMessage.Headers;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> headerPair in headers)
                {
                    this.jsonWriter.WriteName(headerPair.Key);
                    this.jsonWriter.WriteValue(headerPair.Value);
                }
            }

            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writing pending data for the current response message.
        /// </summary>
        private void WritePendingResponseMessageData()
        {
            Debug.Assert(this.JsonLightOutputContext.WritingResponse, "If the response message is available we must be writing response.");
            Debug.Assert(this.CurrentOperationResponseMessage != null, "this.CurrentOperationResponseMessage != null");

            // id property.
            this.jsonWriter.WriteName(PropertyId);
            this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.ContentId);

            // atomicityGroup property.
            if (this.atomicityGroupId != null)
            {
                this.jsonWriter.WriteName(PropertyAtomicityGroup);
                this.jsonWriter.WriteValue(this.atomicityGroupId);
            }

            // response status property.
            this.jsonWriter.WriteName(PropertyStatus);
            this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.StatusCode);

            // headers property.
            this.jsonWriter.WriteName(PropertyHeaders);
            this.jsonWriter.StartObjectScope();
            IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> headerPair in headers)
                {
                    this.jsonWriter.WriteName(headerPair.Key);
                    this.jsonWriter.WriteValue(headerPair.Value);
                }
            }

            this.jsonWriter.EndObjectScope();
        }
    }
}