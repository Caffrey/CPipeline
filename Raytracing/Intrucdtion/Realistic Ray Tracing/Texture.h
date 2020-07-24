#pragma once


#include "Vector.h"
#include "Color.h"

class Texture
{
public:
	virtual Color value(const Vector2&, const Vector3&) const = 0;

};




