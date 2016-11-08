//
//  NewCreationData.cs
//  CreatubblesApiClient
//
//  Copyright (c) 2016 Creatubbles Pte. Ltd.
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

namespace Creatubbles.Api
{
    [Serializable]
	public class NewCreationData
	{
        public const int InvalidCreationYear = -1;
        public const int InvalidCreationMonth = -1;

		public UploadExtension uploadExtension;

		// TODO - data
		//	public var data: NSData?
		// TODO - image
		//	public var image: UIImage?
		public string url;
		public string creationId;

		public string name;
		public string reflectionText;
		public string reflectionVideoUrl;
		public string galleryId;
		public string[] creatorIds;
        public int creationYear = InvalidCreationYear;
        public int creationMonth = InvalidCreationMonth;

		public CreationDataType dataType;

		public NewCreationData(string url, UploadExtension uploadExtension)
		{
			this.url = url;
			this.dataType = CreationDataType.Url;
			this.uploadExtension = uploadExtension;
		}

        public bool HasCreationYear
        {
            get { return creationYear != InvalidCreationYear; }
        }

        public bool HasCreationMonth
        {
            get { return creationMonth != InvalidCreationMonth; }
        }
		//
		//		public init(data: NSData, uploadExtension: UploadExtension)
		//	{
		//		self.data = data
		//			self.dataType = .Data
		//			self.uploadExtension = uploadExtension
		//	}
		//		public init(image: UIImage, uploadExtension: UploadExtension)
		//	{
		//		self.image = image
		//			self.dataType = .Image
		//			self.uploadExtension = uploadExtension
		//	}
		//
		// TODO - remove? probably Realm connected
		//		init(creationDataEntity: NewCreationDataEntity, url: NSURL)
		//	{
		//		self.creationIdentifier = creationDataEntity.creationIdentifier
		//			self.name = creationDataEntity.name
		//			self.reflectionText = creationDataEntity.reflectionText
		//			self.reflectionVideoUrl = creationDataEntity.reflectionVideoUrl
		//			self.galleryId = creationDataEntity.galleryId
		//			self.creatorIds = Array<String>()
		//			self.uploadExtension = creationDataEntity.uploadExtension
		//
		//			for creatorId in creationDataEntity.creatorIds
		//			{
		//				self.creatorIds!.append(creatorId.creatorIdString!)
		//			}
		//				self.creationMonth = creationDataEntity.creationMonth.value
		//				self.creationYear = creationDataEntity.creationYear.value
		//				self.dataType = creationDataEntity.dataType
		//
		//				if(dataType.rawValue == 0)
		//				{
		//					self.image = UIImage(contentsOfFile: url.path!)
		//				}
		//				else if(dataType.rawValue == 1)
		//				{
		//					self.url = url
		//				}
		//				else if(dataType.rawValue == 2)
		//				{
		//					self.data = NSData(contentsOfFile: "\(url)")
		//				}
		//				}
	}
}