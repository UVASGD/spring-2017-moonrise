using System.Xml.Linq;

namespace Completed{
	public interface SerialOb {
		XElement serialize();
		bool deserialize(XElement serial);
	}
}