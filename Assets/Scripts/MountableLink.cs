using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountableLink
{
	public Chain Chain;
	public int LinkIndex;

	public void Deconstruct(out Chain chain, out int linkIndex)
	{
		chain = Chain;
		linkIndex = LinkIndex;
	}
}
