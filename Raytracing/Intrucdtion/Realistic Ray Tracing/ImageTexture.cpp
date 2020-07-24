#include "ImageTexture.h"
#include "Image.h"
#include "Common.h"

ImageTexture::ImageTexture(char* file_name)
{
	image = new	Image();
	image->readPPM(file_name);
}

Color ImageTexture::value(const Vector2& uv, const Vector3& p) const
{
	RFLOAT u = uv.x() - int(uv.x());
	RFLOAT v = uv.y() - int(uv.y());

	u *= (image->width() - 3);
	v *= (image->height() - 3);

	int iu = (int)u;
	int iv = (int)v;

	RFLOAT tu = u - iu;
	RFLOAT tv = v - iv;

	Color c = image->GetPixel(iu, iv) * (1 - tu) * (1 - tv) +
		image->GetPixel(iu + 1, iv) * tu * (1 - tv) +
		image->GetPixel(iu, iv + 1) * (1 - tu) * tv +
		image->GetPixel(iu + 1, iv + 1) * tu * tv;
}

