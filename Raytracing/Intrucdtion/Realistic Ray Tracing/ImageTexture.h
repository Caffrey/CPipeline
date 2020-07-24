#pragma once
#include "Texture.h"

class Image;
class ImageTexture : public Texture
{
	ImageTexture(char* file_name);
	virtual Color value(const Vector2&, const Vector3&) const ;

	Image* image;
};

