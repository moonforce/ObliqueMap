using UnityEngine;

namespace Vectrosity {

public class CapInfo {
	public EndCap capType;
	public Texture texture;
	public float ratio1;
	public float ratio2;
	public float offset1;
	public float offset2;
	public float scale1;
	public float scale2;
	public float[] uvHeights;
	
	public CapInfo (EndCap capType, Texture texture, float ratio1, float ratio2, float offset1, float offset2, float scale1, float scale2, float[] uvHeights) {
		this.capType = capType;
		this.texture = texture;
		this.ratio1 = ratio1;
		this.ratio2 = ratio2;
		this.offset1 = offset1;
		this.offset2 = offset2;
		this.scale1 = scale1;
		this.scale2 = scale2;
		this.uvHeights = uvHeights;
	}
}

}