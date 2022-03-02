using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml.Linq;

namespace WCFSelfHost
{
	public class LoyaltyManagementSynxisEndpointBehavior : IEndpointBehavior
	{
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) 
		{
			// This binding has been added here because of the internal overrides 
			bindingParameters.Add(new SynxisLoyaltyMessageEncodingBindingElement(MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None), System.Text.Encoding.UTF8));
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.OperationSelector = new GetOperationFromHtngHeaderAction();
		}

		public void Validate(ServiceEndpoint endpoint) { }
	}

	public class GetOperationFromHtngHeaderAction : IDispatchOperationSelector
	{
		public string SelectOperation(ref Message message)
		{
			return GetActionFromHtngHeader(message);
		}

		private readonly XNamespace _htngHeaderNamespace = "http://htng.org/1.1/Header/";

		private string GetActionFromHtngHeader(Message message)
		{
			var headers = message.Headers;

			var htngHeaderIndex = headers.FindHeader(nameof(HTNGHeader), _htngHeaderNamespace.NamespaceName);

			if (htngHeaderIndex < 0)
			{
				return null;
			}

			var readerHeaders = headers.GetReaderAtHeader(htngHeaderIndex).ReadOuterXml();

			var xDoc = XDocument.Parse(readerHeaders);

			var actionHeader = xDoc.Descendants(_htngHeaderNamespace + nameof(HTNGHeader.action)).FirstOrDefault();

			return actionHeader?.Value;
		}
	}

	public class LoyaltyManagementSynxisMessageEncoderFactory : MessageEncoderFactory
	{
		private readonly MessageEncoder _defaultMessageEncoder;
		private readonly MessageEncoder _internalMessageEncoder;

		public LoyaltyManagementSynxisMessageEncoderFactory(MessageEncoderFactory defaultEncoderFactory)
		{
			_defaultMessageEncoder = defaultEncoderFactory.Encoder;
			_internalMessageEncoder = CreateSessionEncoder();
		}

		public override MessageEncoder Encoder => _internalMessageEncoder;

		public override MessageVersion MessageVersion => _internalMessageEncoder.MessageVersion;

		public override MessageEncoder CreateSessionEncoder()
		{
			return new LoyaltyManagementSynxisMessageEncoder(_defaultMessageEncoder);
		}
	}

	public class LoyaltyManagementSynxisMessageEncoder : MessageEncoder
	{
		private readonly MessageEncoder _defaultMessageEncoder;

		public LoyaltyManagementSynxisMessageEncoder(MessageEncoder messageEncoder)
		{
			_defaultMessageEncoder = messageEncoder;
		}

		public override string ContentType => _defaultMessageEncoder.ContentType;

		public override string MediaType => _defaultMessageEncoder.MediaType;

		public override MessageVersion MessageVersion => _defaultMessageEncoder.MessageVersion;

		public override bool IsContentTypeSupported(string contentType)
		{
			// This flag will allow the content-type application/xml and text/xml in Soap1.2 besides the application/soap+xml
			var valid = contentType.Contains("text/xml") || contentType.Contains("application/xml") || _defaultMessageEncoder.IsContentTypeSupported(contentType);

			if (!valid)
				throw new ArgumentException($"{nameof(LoyaltyManagementSynxisMessageEncoder)}:Content-type sent {contentType} is not valid.", nameof(contentType));

			return valid;
		}

		public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
		{
			return _defaultMessageEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
		}

		public override Message ReadMessage(System.ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			return _defaultMessageEncoder.ReadMessage(buffer, bufferManager, contentType);
		}

		public override void WriteMessage(Message message, Stream stream)
		{
			_defaultMessageEncoder.WriteMessage(message, stream);
		}

		public override System.ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			return _defaultMessageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
		}
	}

	public sealed class SynxisLoyaltyMessageEncodingBindingElement : MessageEncodingBindingElement
	{
		private readonly MessageEncodingBindingElement _defaultMessageEncodingBindingElement;

		public SynxisLoyaltyMessageEncodingBindingElement(MessageVersion messageVersion, Encoding encoding)
		{
			_defaultMessageEncodingBindingElement = new TextMessageEncodingBindingElement(messageVersion, encoding);
		}

		public SynxisLoyaltyMessageEncodingBindingElement(MessageEncodingBindingElement bindingElement)
		{
			_defaultMessageEncodingBindingElement = bindingElement;
		}

		public override MessageVersion MessageVersion
		{
			get => _defaultMessageEncodingBindingElement.MessageVersion;
			set => _defaultMessageEncodingBindingElement.MessageVersion = value;
		}

		public override BindingElement Clone()
		{
			return new SynxisLoyaltyMessageEncodingBindingElement(_defaultMessageEncodingBindingElement);
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			var defaultEncoderFactory = _defaultMessageEncodingBindingElement.CreateMessageEncoderFactory();
			return new LoyaltyManagementSynxisMessageEncoderFactory(defaultEncoderFactory);
		}

		public override T GetProperty<T>(BindingContext context)
		{
			return _defaultMessageEncodingBindingElement.GetProperty<T>(context);
		}
	}

}
