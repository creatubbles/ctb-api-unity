//
//  NewCreationData.cs
//  Creatubbles API Client Unity SDK
//
//  Copyright (c) 2017 Creatubbles Pte. Ltd.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
using System;

namespace Creatubbles.Api.Data
{
    /// <summary>
    /// New creation data.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#creation-details and https://stateoftheart.creatubbles.com/api/#create-creation.
    /// </remarks>
	public class NewCreationData
	{
        /// <summary>
        /// The data to be uploaded with new creation.
        /// </summary>
        /// <remarks>
        /// Is mutually exclusive with <see cref="NewCreationData.url"/>.
        /// </remarks>
        public readonly byte[] data;

        /// <summary>
        /// Absolute URL of the file to be uploaded with new creation.
        /// </summary>
        /// <remarks>
        /// Is mutually exclusive with <see cref="NewCreationData.data"/>.
        /// </remarks>
        public readonly string url;

        /// <summary>
        /// Extension of the file passed as <see cref="NewCreationData.data"/> or <see cref="NewCreationData.url"/>. 
        /// </summary>
        public readonly UploadExtension uploadExtension;

        /// <summary>
        /// The creation identifier.
        /// </summary>
		public string creationId;

        /// <summary>
        /// The creation name.
        /// </summary>
		public string name;

        /// <summary>
        /// User’s “reflection” / “comment” about the creation.
        /// </summary>
		public string reflectionText;

        /// <summary>
        /// Link to video on creation process, has to be a valid URL.
        /// </summary>
		public string reflectionVideoUrl;

        /// <summary>
        /// The gallery identifier.
        /// </summary>
		public string galleryId;

        /// <summary>
        /// The creators' identifiers.
        /// </summary>
		public string[] creatorIds;

        /// <summary>
        /// The creation year.
        /// </summary>
        /// /// <remarks>
        /// Can be <c>null</c>
        /// </remarks>
        public int? creationYear;

        /// <summary>
        /// The creation month.
        /// </summary>
        /// <remarks>
        /// Can be <c>null</c>
        /// </remarks>
        public int? creationMonth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.NewCreationData"/> class.
        /// </summary>
        /// <param name="data">Data representing file to be uploaded with the new creation.</param>
        /// <param name="extension">Extension of the file to be uploaded with new creation.</param>
        public NewCreationData(byte[] data, UploadExtension extension)
        {
            this.data = data;
            this.uploadExtension = extension;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.NewCreationData"/> class.
        /// </summary>
        /// <param name="url">URL of the file to be uploaded with the new creation..</param>
        /// <param name="extension">Extension of the file to be uploaded with new creation.</param>
        public NewCreationData(string url, UploadExtension extension)
        {
            this.url = url;
            this.uploadExtension = extension;
        }
	}
}
