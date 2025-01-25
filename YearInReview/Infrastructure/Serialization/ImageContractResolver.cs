using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YearInReview.Infrastructure.Serialization;

public class ImageContractResolver : DefaultContractResolver
{
	private readonly Base64ImageConverter _base64ImageConverter;

	public ImageContractResolver(Base64ImageConverter base64ImageConverter)
	{
		_base64ImageConverter = base64ImageConverter;
	}

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		var prop = base.CreateProperty(member, memberSerialization);
		var att = prop.AttributeProvider.GetAttributes(true).OfType<ImageAttribute>().FirstOrDefault();
		if (att == null)
		{
			return prop;
		}

		prop.Converter = _base64ImageConverter;
		return prop;
	}
}