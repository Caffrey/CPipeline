#pragma once
#include "Texture.h"
class SimpleTexture : public Texture
{
public:
	SimpleTexture(Color c) { color = c; }
	virtual Color value(const Vector2&, const Vector3&) const {
		return color;
	}
	Color color;
};

