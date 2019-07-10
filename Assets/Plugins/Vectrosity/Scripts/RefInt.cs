// An integer that's passed by reference; used so VectorManager can update object numbers easily in VisibilityControl
namespace Vectrosity {
public class RefInt {
	public int i;
	public RefInt (int value) {
		i = value;
	}
}
}